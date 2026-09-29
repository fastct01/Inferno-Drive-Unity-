using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Supabase.Gotrue;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp / Assembly-CSharp__2.cpp: every
// SupabaseGameManager_* function plus the async state machines
//   InitializeOnlineFeaturesAsync d__72, SignInAsGuestOnlineAsync d__75, SignInWithEmailAsync d__76,
//   RegisterWithEmailAsync d__77, SignOutAsync d__78, DeleteAccountAsync d__79, StartAppleSignInAsync d__80,
//   StartGoogleSignInAsync d__81, HandleDeepLink d__82, CompleteNativeAppleSignInAsync d__87,
//   FinishAuthenticatedSignInAsync d__90, SyncProfileAfterCaseClosedAsync d__94,
//   SyncProfileAfterEconomyChangeAsync d__95, SyncGaragePurchaseAsync d__96, RequireOnlineEconomyAsync d__97,
//   RefreshAndSyncWhenOnlineAsync d__103, ExecuteProfileSyncAsync d__105, RefreshSessionCoreAsync d__109,
//   DownloadCloudProfileAsync d__112, FetchCloudProfileAsync d__113 (+DisplayClass113_0),
//   SynchronizePendingChangesAsync d__114, LegacyMergeAsync d__115 (+DisplayClass115_0),
//   EnsureOnlineClientAsync d__127.
// IGotrueClient slots decoded from global-metadata.dat (meta.py type IGotrueClient`2):
//   2 CurrentSession, 3 CurrentUser, 9 GetSessionFromUrl, 10 RefreshSession, 15 SetSession,
//   18 SignIn(email,password), 22 SignIn(provider,options), 23 SignInWithIdToken, 24 SignInAnonymously,
//   29 SignUp(email,password,options), 31 SignOut, 32 Update.
// IPostgrestTable slots: 12 Get, 36 Where, 43 Upsert(model).
// InfernoGarage_IsUnlocked / InfernoGarage_SetUnlocked are the extern "C" bridge functions of
// InfernoGarage.inc; they are exposed as ShadowRunPrototype.InfernoGarageIsUnlocked/SetUnlocked here.
// /* [user request 2026-09-21: cars] */ The bridge now answers for the six non-cloud cars of the
// InfernoCars catalogue (PlayerPrefs InfernoDrive.Vehicle.<id>.Unlocked); the four shipped ids
// (sportcar = Car1, flying_car = Car2, infal_model = Car3, car3 = Car4) keep the cloud columns below.
namespace ShadowRun.Services
{
	public sealed class SupabaseGameManager : MonoBehaviour
	{
		[Serializable]
		private sealed class JwtPayload
		{
			public long exp;
		}

		private const string AuthChoiceKey = "shadowRunAuthChoiceMade";

		private const string LegacyPendingGuestKey = "shadowRunPendingGuest";

		private const string CloudDirtyKey = "shadowRunCloudDirty";

		private const string ProfileIdKey = "shadowRunProfileId";

		private const string CrystalsKey = "shadowRunCrystals";

		private const string Car1Key = "shadowRunCar1Unlocked";

		private const string Car2Key = "shadowRunCar2Unlocked";

		private const string Car3Key = "shadowRunCar3Unlocked";

		private const string Car4Key = "shadowRunCar4Unlocked";

		private const string RememberedGuestKey = "shadowRunRememberedGuest";

		/* [user request 2026-09-27: cannot buy cars] Profile id of a guest whose session died server-side.
		   An anonymous user cannot be signed into again, so the replacement guest gets a new id; this key lets
		   that one profile reset carry the player's crystals and cars over (ResetCachedProfileIfSessionChanged). */
		private const string StrandedGuestProfileKey = "shadowRunStrandedGuestProfile";

		private const string RememberedEmailKey = "shadowRunRememberedEmail";

		private const string PendingCrystalDeltaKey = "shadowRunPendingCrystalDelta";

		private const string InFlightCrystalDeltaKey = "shadowRunInFlightCrystalDelta";

		private const string InFlightOperationIdKey = "shadowRunInFlightOperationId";

		private const string SyncSchemaVersionKey = "shadowRunSyncSchemaVersion";

		private const int SyncSchemaVersion = 1;

		private const int TokenRefreshSkewSeconds = 120;

		private const string OAuthRedirectUrl = "shadowrun://auth-callback";

		private const string OAuthCallbackScheme = "shadowrun";

		private static SupabaseGameManager instance;

		private static bool syncInProgress;

		private static TaskCompletionSource<bool> syncCompletion;

		private static readonly object SessionRefreshLock = new object();

		private static Task<bool> sessionRefreshTask;

		private static string pendingAppleNonce;

		private static string lastOAuthCallback;

		private static bool oauthCompletionInProgress;

		private static Task onlineRecoveryTask;

		public static PlayerProfile CurrentPlayer { get; private set; } = CreateDefaultProfile();

		public static bool HasCompletedFirstChoice => PlayerPrefs.GetInt(AuthChoiceKey, 0) == 1;

		public static bool IsCloudReady => SupabaseBootstrap.IsReady;

		public static bool IsAuthenticated => HasUsableSession(SupabaseBootstrap.Client?.Auth?.CurrentSession) && !string.IsNullOrWhiteSpace(SupabaseBootstrap.Client?.Auth?.CurrentUser?.Id);

		public static bool HasRememberedAccount
		{
			get
			{
				if (IsAuthenticated)
				{
					return true;
				}
				if (HasCompletedFirstChoice && SupabaseBootstrap.HasPersistedSession)
				{
					if (string.IsNullOrWhiteSpace(CurrentPlayer?.Id))
					{
						return !string.IsNullOrWhiteSpace(SupabaseBootstrap.PersistedUserId);
					}
					return true;
				}
				return false;
			}
		}

		public static string CurrentUserId
		{
			get
			{
				string sessionUserId = SupabaseBootstrap.Client?.Auth?.CurrentUser?.Id;
				if (!string.IsNullOrWhiteSpace(sessionUserId))
				{
					return sessionUserId;
				}
				string persistedUserId = SupabaseBootstrap.PersistedUserId;
				if (!string.IsNullOrWhiteSpace(persistedUserId))
				{
					return persistedUserId;
				}
				return CurrentPlayer?.Id ?? string.Empty;
			}
		}

		public static string CurrentUserEmail => SupabaseBootstrap.Client?.Auth?.CurrentUser?.Email ?? PlayerPrefs.GetString(RememberedEmailKey, string.Empty);

		public static bool IsGuestAccount
		{
			get
			{
				if (SupabaseBootstrap.Client == null)
				{
					return false;
				}
				bool? anonymous = SupabaseBootstrap.Client.Auth?.CurrentUser?.IsAnonymous;
				return anonymous == true;
			}
		}

		public static bool IsRememberedGuestAccount => IsAuthenticated ? IsGuestAccount : PlayerPrefs.GetInt(RememberedGuestKey, 0) == 1;

		public static bool ProfileMatchesSession => !string.IsNullOrWhiteSpace(CurrentUserId) && string.Equals(CurrentPlayer?.Id, CurrentUserId, StringComparison.Ordinal);

		public static string AccountDisplayId => IsRememberedGuestAccount ? "GUEST_ID_" + StableGuestNumber(CurrentUserId) : ShortAccountId(CurrentUserId);

		public static int Crystals => CurrentPlayer?.Crystals ?? 25;

		public static event Action ProfileChanged;

		public static event Action AuthenticationCompleted;

		public static event Action<string> AuthenticationStatusChanged;

#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern void ShadowRun_StartAppleSignIn(string rawNonce);

		[DllImport("__Internal")]
		private static extern void ShadowRun_StartWebAuth(string url, string callbackScheme);
#else
		private static void ShadowRun_StartAppleSignIn(string rawNonce)
		{
			Debug.LogWarning("[Supabase] Apple Sign In is only available in the iOS build.");
		}

		private static void ShadowRun_StartWebAuth(string url, string callbackScheme)
		{
			Application.OpenURL(url);
		}
#endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CreateInstance()
		{
			if (FindAnyObjectByType<SupabaseGameManager>() == null)
			{
				new GameObject("Supabase Game Manager").AddComponent<SupabaseGameManager>();
			}
		}

		private void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}
			instance = this;
			DontDestroyOnLoad(gameObject);
			LoadFromLocalStorage();
			Application.deepLinkActivated += HandleDeepLink;
			RequestOnlineRecovery();
			if (!string.IsNullOrWhiteSpace(Application.absoluteURL))
			{
				HandleDeepLink(Application.absoluteURL);
			}
		}

		private void OnDestroy()
		{
			if (instance != this)
			{
				return;
			}
			Application.deepLinkActivated -= HandleDeepLink;
			instance = null;
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				SaveToLocalStorage();
				SupabaseBootstrap.PersistCurrentSession();
				return;
			}
			RequestOnlineRecovery();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				SupabaseBootstrap.PersistCurrentSession();
				return;
			}
			RequestOnlineRecovery();
		}

		private void OnApplicationQuit()
		{
			SaveToLocalStorage();
			SupabaseBootstrap.PersistCurrentSession();
		}

		private static async Task InitializeOnlineFeaturesAsync()
		{
			await SupabaseBootstrap.InitializeAsync();
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				if (HasCompletedFirstChoice && !SupabaseBootstrap.HasPersistedSession)
				{
					RequireAuthentication("Guest session is missing. Connect to the internet and sign in again.");
				}
				else
				{
					SetStatus("Offline. The saved account will reconnect when internet returns.");
				}
				return;
			}
			if (!SupabaseBootstrap.IsReady)
			{
				SetStatus("Cloud login is temporarily unavailable.");
				return;
			}
			if (!IsAuthenticated)
			{
				await SupabaseBootstrap.RestorePersistedSessionAsync();
			}
			if (IsAuthenticated)
			{
				SaveToLocalStorage();
				await RefreshAndSyncWhenOnlineAsync();
			}
			else if (HasCompletedFirstChoice && (!SupabaseBootstrap.HasPersistedSession || IsPermanentSessionFailure(SupabaseBootstrap.SessionRestoreError)))
			{
				RequireAuthentication("Guest account could not be restored. Sign in again.");
			}
		}

		private static void RequestOnlineRecovery()
		{
			if (onlineRecoveryTask == null || onlineRecoveryTask.IsCompleted)
			{
				onlineRecoveryTask = InitializeOnlineFeaturesAsync();
			}
		}

		public static void PlayAsGuest()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				SetStatus("Internet is required to create a Guest account.");
				return;
			}
			_ = SignInAsGuestOnlineAsync();
		}

		private static async Task SignInAsGuestOnlineAsync()
		{
			try
			{
				await SupabaseBootstrap.InitializeAsync();
				if (!SupabaseBootstrap.IsReady)
				{
					throw new InvalidOperationException("Supabase недоступен");
				}
				if (!IsAuthenticated && SupabaseBootstrap.HasPersistedSession)
				{
					await SupabaseBootstrap.RestorePersistedSessionAsync();
				}
				if (!IsAuthenticated)
				{
					await SupabaseBootstrap.Client.Auth.SignInAnonymously(null);
				}
				if (!IsAuthenticated || !IsGuestAccount)
				{
					throw new InvalidOperationException("Guest account was not created.");
				}
				if (!await FinishAuthenticatedSignInAsync())
				{
					throw new InvalidOperationException("Guest account session or cloud profile was not verified.");
				}
				SetStatus("Guest account connected.");
			}
			catch (Exception ex)
			{
				SetStatus("Could not create Guest account: " + FriendlyError(ex));
			}
		}

		public static async Task<bool> SignInWithEmailAsync(string email, string password)
		{
			if (!ValidateCredentials(email, password) || !await EnsureOnlineClientAsync())
			{
				return false;
			}
			try
			{
				if (IsGuestAccount)
				{
					if (!await RefreshAndSyncWhenOnlineAsync() || HasPendingLocalChanges())
					{
						SetStatus("Guest progress must finish syncing before switching accounts.");
						return false;
					}
				}
				Session session = await SupabaseBootstrap.Client.Auth.SignIn(email.Trim(), password);
				if (!HasUsableSession(session) || !IsAuthenticated)
				{
					throw new InvalidOperationException("The server did not return a usable account session.");
				}
				ClearSyncOutbox();
				if (!await FinishAuthenticatedSignInAsync())
				{
					return false;
				}
				SetStatus("Вход выполнен.");
				return true;
			}
			catch (Exception ex)
			{
				SetStatus("Не удалось войти: " + FriendlyError(ex));
				return false;
			}
		}

		public static async Task<bool> RegisterWithEmailAsync(string email, string password)
		{
			if (!ValidateCredentials(email, password) || !await EnsureOnlineClientAsync())
			{
				return false;
			}
			try
			{
				if (IsGuestAccount)
				{
					string guestUserId = CurrentUserId;
					if (!await RefreshAndSyncWhenOnlineAsync() || HasPendingLocalChanges())
					{
						SetStatus("Guest progress must finish syncing before account upgrade.");
						return false;
					}
					await SupabaseBootstrap.Client.Auth.Update(new UserAttributes
					{
						Email = email.Trim(),
						Password = password
					});
					await EnsureFreshSessionAsync(true);
					if (!string.Equals(guestUserId, CurrentUserId, StringComparison.Ordinal))
					{
						throw new InvalidOperationException("Account upgrade changed the user ID.");
					}
					if (!IsAuthenticated || !await FinishAuthenticatedSignInAsync())
					{
						SetStatus("Account credentials changed, but the session or cloud profile was not verified.");
						return false;
					}
					SetStatus(IsGuestAccount ? "Confirm your email to finish securing this guest account." : "Guest progress secured. Account created.");
					return true;
				}
				string normalizedEmail = email.Trim();
				Session session = await SupabaseBootstrap.Client.Auth.SignUp(normalizedEmail, password, null);
				if (!HasUsableSession(session) || !IsAuthenticated)
				{
					try
					{
						session = await SupabaseBootstrap.Client.Auth.SignIn(normalizedEmail, password);
					}
					catch
					{
						SetStatus("Account created. Confirm your email, then press SIGN IN.");
						return false;
					}
				}
				if (!HasUsableSession(session) || !IsAuthenticated)
				{
					SetStatus("Account created, but no valid session was returned. Press SIGN IN.");
					return false;
				}
				if (!await FinishAuthenticatedSignInAsync())
				{
					return false;
				}
				SetStatus("Аккаунт создан.");
				return true;
			}
			catch (Exception ex)
			{
				SetStatus("Не удалось зарегистрироваться: " + FriendlyError(ex));
				return false;
			}
		}

		public static async Task<bool> SignOutAsync()
		{
			if (!await EnsureOnlineClientAsync())
			{
				return false;
			}
			try
			{
				if (IsAuthenticated)
				{
					if (!await RefreshAndSyncWhenOnlineAsync() || HasPendingLocalChanges())
					{
						SetStatus("Progress has not reached Supabase yet. Sign out was cancelled.");
						return false;
					}
				}
				await SupabaseBootstrap.Client.Auth.SignOut(Constants.SignOutScope.Global);
				SupabaseBootstrap.DestroyPersistedSessionExplicitly();
				ClearLocalAccountState();
				SetStatus("Signed out.");
				FirstLaunchAuthScreen.Show();
				return true;
			}
			catch (Exception ex)
			{
				SetStatus("Could not sign out: " + FriendlyError(ex));
				return false;
			}
		}

		public static async Task<bool> DeleteAccountAsync()
		{
			if (!await EnsureOnlineClientAsync() || !IsAuthenticated)
			{
				SetStatus("Sign in before deleting the account.");
				return false;
			}
			try
			{
				if (!await EnsureFreshSessionAsync(false))
				{
					return false;
				}
				await SupabaseBootstrap.Client.Postgrest.Rpc<object>("delete_own_account", new { });
				try
				{
					await SupabaseBootstrap.Client.Auth.SignOut(Constants.SignOutScope.Global);
				}
				catch
				{
				}
				SupabaseBootstrap.DestroyPersistedSessionExplicitly();
				ClearLocalAccountState();
				SetStatus("Account permanently deleted.");
				FirstLaunchAuthScreen.Show();
				return true;
			}
			catch (Exception ex)
			{
				SetStatus("Could not delete account: " + FriendlyError(ex));
				return false;
			}
		}

		public static async Task<bool> StartAppleSignInAsync()
		{
			if (!await EnsureOnlineClientAsync())
			{
				return false;
			}
			try
			{
				bool blocked = IsGuestAccount;
				if (blocked)
				{
					blocked = !await RefreshAndSyncWhenOnlineAsync() || HasPendingLocalChanges();
				}
				if (blocked)
				{
					SetStatus("Guest progress must finish syncing before Apple sign in.");
					return false;
				}
				pendingAppleNonce = CreateSecureNonce();
				SetStatus("Complete sign in with Apple.");
				ShadowRun_StartAppleSignIn(pendingAppleNonce);
				return true;
			}
			catch (Exception ex)
			{
				SetStatus("Apple Sign In недоступен: " + FriendlyError(ex));
				return false;
			}
		}

		public static async Task<bool> StartGoogleSignInAsync()
		{
			if (!await EnsureOnlineClientAsync())
			{
				return false;
			}
			try
			{
				bool blocked = IsGuestAccount;
				if (blocked)
				{
					blocked = !await RefreshAndSyncWhenOnlineAsync() || HasPendingLocalChanges();
				}
				if (blocked)
				{
					SetStatus("Guest progress must finish syncing before Google sign in.");
					return false;
				}
				SignInOptions options = new SignInOptions
				{
					RedirectTo = OAuthRedirectUrl,
					FlowType = Constants.OAuthFlowType.Implicit
				};
				ProviderAuthState state = await SupabaseBootstrap.Client.Auth.SignIn(Constants.Provider.Google, options);
				if (state?.Uri == null)
				{
					throw new InvalidOperationException("Google did not return an authorization URL.");
				}
				SetStatus("Complete sign in in the Google window.");
				ShadowRun_StartWebAuth(state.Uri.AbsoluteUri, OAuthCallbackScheme);
				return true;
			}
			catch (Exception ex)
			{
				SetStatus("Google Sign In is unavailable: " + FriendlyError(ex));
				return false;
			}
		}

		private static async void HandleDeepLink(string url)
		{
			if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(OAuthRedirectUrl, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			if (oauthCompletionInProgress || string.Equals(lastOAuthCallback, url, StringComparison.Ordinal))
			{
				return;
			}
			oauthCompletionInProgress = true;
			lastOAuthCallback = url;
			try
			{
				if (!await EnsureOnlineClientAsync())
				{
					return;
				}
				string error = ReadOAuthValue(url, "error_description") ?? ReadOAuthValue(url, "error");
				if (!string.IsNullOrWhiteSpace(error))
				{
					throw new InvalidOperationException(Uri.UnescapeDataString(error.Replace('+', ' ')));
				}
				Session session = await SupabaseBootstrap.Client.Auth.GetSessionFromUrl(new Uri(url), true);
				if (!HasUsableSession(session) || !await FinishAuthenticatedSignInAsync())
				{
					return;
				}
				SetStatus("Sign in completed.");
			}
			catch (Exception ex)
			{
				SetStatus("Could not complete sign in: " + FriendlyError(ex));
			}
			finally
			{
				oauthCompletionInProgress = false;
			}
		}

		public void OnNativeOAuthCallback(string url)
		{
			HandleDeepLink(url);
		}

		public void OnNativeAppleCredential(string payload)
		{
			_ = CompleteNativeAppleSignInAsync(payload);
		}

		public void OnNativeAuthCancelled(string message)
		{
			pendingAppleNonce = null;
			SetStatus(string.IsNullOrWhiteSpace(message) ? "Sign in was cancelled." : message);
		}

		public void OnNativeAuthError(string message)
		{
			pendingAppleNonce = null;
			SetStatus("Could not complete sign in: " + (string.IsNullOrWhiteSpace(message) ? "Unknown authentication error." : message));
		}

		private static async Task CompleteNativeAppleSignInAsync(string payload)
		{
			if (oauthCompletionInProgress)
			{
				return;
			}
			oauthCompletionInProgress = true;
			try
			{
				int separator = payload?.IndexOf('\n') ?? -1;
				if (separator <= 0 || separator >= payload.Length - 1)
				{
					throw new InvalidOperationException("Apple returned an incomplete credential.");
				}
				string identityToken = payload.Substring(0, separator);
				string rawNonce = payload.Substring(separator + 1);
				if (string.IsNullOrWhiteSpace(pendingAppleNonce) || !string.Equals(rawNonce, pendingAppleNonce, StringComparison.Ordinal))
				{
					throw new InvalidOperationException("Apple sign in security verification failed.");
				}
				if (!await EnsureOnlineClientAsync())
				{
					return;
				}
				Session session = await SupabaseBootstrap.Client.Auth.SignInWithIdToken(Constants.Provider.Apple, identityToken, null, rawNonce, null);
				if (!HasUsableSession(session) || !await FinishAuthenticatedSignInAsync())
				{
					return;
				}
				SetStatus("Sign in completed.");
			}
			catch (Exception ex)
			{
				SetStatus("Could not complete Apple sign in: " + FriendlyError(ex));
			}
			finally
			{
				pendingAppleNonce = null;
				oauthCompletionInProgress = false;
			}
		}

		private static string CreateSecureNonce()
		{
			byte[] bytes = new byte[32];
			using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
			{
				generator.GetBytes(bytes);
			}
			return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		}

		private static string ReadOAuthValue(string url, string key)
		{
			if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
			{
				return null;
			}
			int start = url.IndexOfAny(new char[2] { '?', '#' });
			if (start < 0 || start >= url.Length - 1)
			{
				return null;
			}
			string[] pairs = url.Substring(start + 1).Replace('#', '&').Split('&');
			for (int i = 0; i < pairs.Length; i++)
			{
				string pair = pairs[i];
				int equals = pair.IndexOf('=');
				string name = equals >= 0 ? pair.Substring(0, equals) : pair;
				if (string.Equals(name, key, StringComparison.OrdinalIgnoreCase))
				{
					return equals >= 0 ? pair.Substring(equals + 1) : string.Empty;
				}
			}
			return null;
		}

		private static async Task<bool> FinishAuthenticatedSignInAsync()
		{
			if (!IsAuthenticated)
			{
				SetStatus("The account session was not established. Please sign in again.");
				return false;
			}
			string authenticatedUserId = SupabaseBootstrap.Client.Auth.CurrentUser.Id;
			ResetCachedProfileIfSessionChanged();
			PlayerPrefs.DeleteKey(LegacyPendingGuestKey);
			if (!SupabaseBootstrap.HasPersistedSession)
			{
				SetStatus("The account session could not be saved. Please sign in again.");
				return false;
			}
			if (!await RefreshAndSyncWhenOnlineAsync() || !IsAuthenticated || !string.Equals(authenticatedUserId, CurrentUserId, StringComparison.Ordinal) || !ProfileMatchesSession)
			{
				SetStatus("Signed in, but the cloud profile could not be verified. Please try again.");
				return false;
			}
			CompleteFirstChoice();
			AuthenticationCompleted?.Invoke();
			Debug.Log(string.Format("[Supabase] Authentication finalized. UserId={0}, Guest={1}", CurrentUserId, IsGuestAccount));
			return true;
		}

		private static bool HasUsableSession(Session session)
		{
			return session != null && !string.IsNullOrWhiteSpace(session.AccessToken) && !string.IsNullOrWhiteSpace(session.RefreshToken) && !string.IsNullOrWhiteSpace(session.User?.Id);
		}

		public static Task LoadPlayerFromCloud()
		{
			return RefreshAndSyncWhenOnlineAsync();
		}

		public static Task RefreshProfileIfOnlineAsync()
		{
			return RefreshAndSyncWhenOnlineAsync();
		}

		public static async Task SyncProfileAfterCaseClosedAsync()
		{
			if (await RefreshAndSyncWhenOnlineAsync())
			{
				Debug.Log("[Supabase] Case reward verified from profiles. UserId=" + CurrentUserId);
			}
		}

		public static async Task SyncProfileAfterEconomyChangeAsync()
		{
			if (await RefreshAndSyncWhenOnlineAsync())
			{
				Debug.Log("[Supabase] Economy change verified from profiles. UserId=" + CurrentUserId);
			}
		}

		public static async Task<bool> SyncGaragePurchaseAsync(string vehicleId)
		{
			await SyncProfileAfterEconomyChangeAsync();
			return IsAuthenticated && !HasPendingLocalChanges() && IsVehicleUnlocked(vehicleId);
		}

		public static async Task<bool> RequireOnlineEconomyAsync()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				SetStatus("Для покупок требуется интернет.");
				return false;
			}
			await SupabaseBootstrap.InitializeAsync();
			if (SupabaseBootstrap.IsReady && !IsAuthenticated)
			{
				await SupabaseBootstrap.RestorePersistedSessionAsync();
			}
			if (!SupabaseBootstrap.IsReady || !IsAuthenticated)
			{
				SetStatus("Для покупок требуется подключённый аккаунт.");
				return false;
			}
			if (!await EnsureFreshSessionAsync(false))
			{
				return false;
			}
			return await RefreshAndSyncWhenOnlineAsync();
		}

		public static void SaveGameData()
		{
			SaveToLocalStorage();
		}

		public static void SetCrystals(int amount)
		{
			int before = CurrentPlayer.Crystals;
			CurrentPlayer.Crystals = Mathf.Max(0, amount);
			int delta = CurrentPlayer.Crystals - before;
			if (delta == 0)
			{
				return;
			}
			PlayerPrefs.SetInt(PendingCrystalDeltaKey, PlayerPrefs.GetInt(PendingCrystalDeltaKey, 0) + delta);
			PlayerPrefs.SetInt(CloudDirtyKey, 1);
			ProfileChanged?.Invoke();
			SaveGameData();
		}

		public static bool IsVehicleUnlocked(string vehicleId)
		{
			switch (vehicleId)
			{
			case "sportcar":
				return CurrentPlayer.Car1;
			case "flying_car":
				return CurrentPlayer.Car2;
			case "infal_model":
				return CurrentPlayer.Car3;
			case "car3":
				return CurrentPlayer.Car4;
			default:
				return ShadowRunPrototype.InfernoGarageIsUnlocked(vehicleId) > 0; /* [InfernoLobby patch] */
			}
		}

		public static void SetVehicleUnlocked(string vehicleId, bool unlocked)
		{
			bool changed;
			switch (vehicleId)
			{
			case "sportcar":
				changed = CurrentPlayer.Car1 != unlocked;
				CurrentPlayer.Car1 = unlocked;
				break;
			case "flying_car":
				changed = CurrentPlayer.Car2 != unlocked;
				CurrentPlayer.Car2 = unlocked;
				break;
			case "infal_model":
				changed = CurrentPlayer.Car3 != unlocked;
				CurrentPlayer.Car3 = unlocked;
				break;
			case "car3":
				changed = CurrentPlayer.Car4 != unlocked;
				CurrentPlayer.Car4 = unlocked;
				break;
			default:
				if (ShadowRunPrototype.InfernoGarageSetUnlocked(vehicleId, unlocked)) /* [InfernoLobby patch] */
				{
					PlayerPrefs.SetInt(CloudDirtyKey, 1);
					ProfileChanged?.Invoke();
					SaveGameData();
				}
				return;
			}
			if (!changed)
			{
				return;
			}
			PlayerPrefs.SetInt(CloudDirtyKey, 1);
			ProfileChanged?.Invoke();
			SaveGameData();
		}

		public static void QueueCaseReward(string vehicleId, int crystalAmount)
		{
			if (!IsAuthenticated || string.IsNullOrWhiteSpace(CurrentUserId) || !ProfileMatchesSession)
			{
				SetStatus("Sign in to an account before opening a case.");
				FirstLaunchAuthScreen.Show();
				return;
			}
			bool changed = false;
			if (!string.IsNullOrEmpty(vehicleId))
			{
				switch (vehicleId)
				{
				case "sportcar":
					changed = !CurrentPlayer.Car1;
					CurrentPlayer.Car1 = true;
					break;
				case "flying_car":
					changed = !CurrentPlayer.Car2;
					CurrentPlayer.Car2 = true;
					break;
				case "infal_model":
					changed = !CurrentPlayer.Car3;
					CurrentPlayer.Car3 = true;
					break;
				case "car3":
					changed = !CurrentPlayer.Car4;
					CurrentPlayer.Car4 = true;
					break;
				default:
					if (ShadowRunPrototype.InfernoGarageSetUnlocked(vehicleId, true)) /* [InfernoLobby patch] */
					{
						changed = true;
						break;
					}
					Debug.LogWarning("[Supabase] Unknown case vehicle id: " + vehicleId);
					return;
				}
			}
			else if (crystalAmount > 0)
			{
				CurrentPlayer.Crystals = Mathf.Max(0, CurrentPlayer.Crystals + crystalAmount);
				PlayerPrefs.SetInt(PendingCrystalDeltaKey, PlayerPrefs.GetInt(PendingCrystalDeltaKey, 0) + crystalAmount);
				changed = true;
			}
			if (!changed)
			{
				return;
			}
			PlayerPrefs.SetInt(CloudDirtyKey, 1);
			SaveToLocalStorage();
			ProfileChanged?.Invoke();
			Debug.Log(string.Format("[Supabase] Case reward queued. UserId={0}, Vehicle={1}, Crystals={2}", CurrentUserId, vehicleId, crystalAmount));
		}

		private static async Task<bool> RefreshAndSyncWhenOnlineAsync()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return false;
			}
			await SupabaseBootstrap.InitializeAsync();
			if (!SupabaseBootstrap.IsReady)
			{
				return false;
			}
			if (!IsAuthenticated)
			{
				await SupabaseBootstrap.RestorePersistedSessionAsync();
			}
			if (!IsAuthenticated)
			{
				if (HasCompletedFirstChoice && (!SupabaseBootstrap.HasPersistedSession || IsPermanentSessionFailure(SupabaseBootstrap.SessionRestoreError)))
				{
					RequireAuthentication("Guest account could not be restored. Sign in again.");
				}
				return false;
			}
			if (!await EnsureFreshSessionAsync(false))
			{
				return false;
			}
			ResetCachedProfileIfSessionChanged();
			if (syncInProgress)
			{
				Task<bool> pending = syncCompletion?.Task;
				bool pendingResult = false;
				if (pending != null)
				{
					pendingResult = await pending;
				}
				if (HasPendingLocalChanges())
				{
					return await RefreshAndSyncWhenOnlineAsync();
				}
				return pendingResult;
			}
			syncInProgress = true;
			syncCompletion = new TaskCompletionSource<bool>();
			bool succeeded = false;
			try
			{
				bool expired = false;
				try
				{
					await ExecuteProfileSyncAsync();
					succeeded = true;
				}
				catch (Exception ex) when (IsExpiredJwt(ex))
				{
					expired = true;
				}
				catch (Exception ex)
				{
					SetStatus("Cloud is unavailable. Account progress will sync later: " + FriendlyError(ex));
				}
				if (expired)
				{
					if (!await EnsureFreshSessionAsync(true))
					{
						return false;
					}
					try
					{
						await ExecuteProfileSyncAsync();
						succeeded = true;
					}
					catch (Exception ex)
					{
						SetStatus("Cloud is temporarily unavailable. Account progress is saved on this device until sync: " + FriendlyError(ex));
					}
				}
				return succeeded;
			}
			finally
			{
				syncInProgress = false;
				syncCompletion.TrySetResult(succeeded);
			}
		}

		private static void ResetCachedProfileIfSessionChanged()
		{
			string sessionUserId = SupabaseBootstrap.Client?.Auth?.CurrentUser?.Id;
			if (string.IsNullOrWhiteSpace(sessionUserId) || CurrentPlayer == null || string.IsNullOrWhiteSpace(CurrentPlayer.Id) || string.Equals(CurrentPlayer.Id, sessionUserId, StringComparison.Ordinal))
			{
				return;
			}
			/* [user request 2026-09-27: cannot buy cars] One mismatch is not an account switch: a guest whose
			   session expired server-side (RequireAuthentication recorded its id) signing in as the replacement
			   guest is the same person with a new anonymous id, so it keeps its crystals and cars. The outbox is
			   still dropped either way — those queued writes name the dead user — and the carried values are
			   marked dirty so they upload into the new row. Every other mismatch still resets, which is what
			   keeps two real accounts from mixing.
			   original: (no carry) CurrentPlayer = CreateDefaultProfile(); CurrentPlayer.Id = sessionUserId; */
			bool carryStrandedGuest = IsGuestAccount && string.Equals(PlayerPrefs.GetString(StrandedGuestProfileKey, string.Empty), CurrentPlayer.Id, StringComparison.Ordinal);
			Debug.LogWarning("[Supabase] Cached profile " + CurrentPlayer.Id + " does not match session " + sessionUserId + (carryStrandedGuest ? ". Carried the stranded guest's progress over." : ". Local outbox was isolated."));
			ClearSyncOutbox();
			if (carryStrandedGuest)
			{
				CurrentPlayer.Id = sessionUserId;
				PlayerPrefs.DeleteKey(StrandedGuestProfileKey);
				PlayerPrefs.SetInt(CloudDirtyKey, 1);
			}
			else
			{
				CurrentPlayer = CreateDefaultProfile();
				CurrentPlayer.Id = sessionUserId;
			}
			SaveToLocalStorage();
			ProfileChanged?.Invoke();
		}

		private static async Task ExecuteProfileSyncAsync()
		{
			if (HasPendingLocalChanges())
			{
				await SynchronizePendingChangesAsync();
			}
			else
			{
				await DownloadCloudProfileAsync();
			}
		}

		private static Task<bool> EnsureFreshSessionAsync(bool forceRefresh = false)
		{
			Session session = SupabaseBootstrap.Client?.Auth?.CurrentSession;
			if (session == null || string.IsNullOrWhiteSpace(session.RefreshToken))
			{
				return Task.FromResult(false);
			}
			if (!forceRefresh && !SessionNeedsRefresh(session))
			{
				return Task.FromResult(true);
			}
			lock (SessionRefreshLock)
			{
				if (sessionRefreshTask == null || sessionRefreshTask.IsCompleted)
				{
					sessionRefreshTask = RefreshSessionCoreAsync();
				}
				return sessionRefreshTask;
			}
		}

		private static bool SessionNeedsRefresh(Session session)
		{
			if (string.IsNullOrWhiteSpace(session.AccessToken))
			{
				return true;
			}
			if (TryReadJwtExpirationUtc(session.AccessToken, out DateTime expiresAtUtc))
			{
				return DateTime.UtcNow >= expiresAtUtc.AddSeconds(-TokenRefreshSkewSeconds);
			}
			if (session.ExpiresIn <= 0)
			{
				return true;
			}
			DateTime expiry = session.ExpiresIn > 100000000L ? DateTimeOffset.FromUnixTimeSeconds(session.ExpiresIn).UtcDateTime : session.CreatedAt.ToUniversalTime().AddSeconds(session.ExpiresIn);
			return DateTime.UtcNow >= expiry.AddSeconds(-TokenRefreshSkewSeconds);
		}

		private static bool TryReadJwtExpirationUtc(string accessToken, out DateTime expiresAtUtc)
		{
			expiresAtUtc = default(DateTime);
			try
			{
				string[] parts = accessToken.Split('.');
				if (parts.Length != 3)
				{
					return false;
				}
				string payload = parts[1].Replace('-', '+').Replace('_', '/');
				switch (payload.Length % 4)
				{
				case 2:
					payload += "==";
					break;
				case 3:
					payload += "=";
					break;
				}
				JwtPayload jwt = JsonUtility.FromJson<JwtPayload>(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
				if (jwt == null || jwt.exp <= 0)
				{
					return false;
				}
				expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(jwt.exp).UtcDateTime;
				return true;
			}
			catch
			{
			}
			return false;
		}

		private static async Task<bool> RefreshSessionCoreAsync()
		{
			try
			{
				string expectedUserId = SupabaseBootstrap.Client.Auth.CurrentUser?.Id;
				Session refreshed;
				using (SupabaseBootstrap.ProtectPersistedSessionDuringRefresh())
				{
					refreshed = await SupabaseBootstrap.Client.Auth.RefreshSession();
				}
				string refreshedUserId = refreshed?.User?.Id ?? SupabaseBootstrap.Client.Auth.CurrentUser?.Id;
				if (refreshed == null || string.IsNullOrWhiteSpace(refreshed.AccessToken) || string.IsNullOrWhiteSpace(refreshed.RefreshToken))
				{
					throw new InvalidOperationException("Supabase не вернул обновлённую сессию.");
				}
				if (!string.IsNullOrWhiteSpace(expectedUserId) && !string.Equals(expectedUserId, refreshedUserId, StringComparison.Ordinal))
				{
					throw new InvalidOperationException("После обновления токена изменился User ID. Синхронизация остановлена.");
				}
				Debug.Log("[Supabase] Токен сессии обновлён. UserId=" + refreshedUserId);
				return true;
			}
			catch (Exception ex)
			{
				if (IsPermanentSessionFailure(ex.ToString()))
				{
					/* [user request 2026-09-27: cannot buy cars] Before telling the player their account is
					   gone, try the token on disk once. GoTrue rotates refresh tokens, so the usual cause of a
					   "Session expired" answer is that another process (the device build, an earlier Editor)
					   spent ours and saved a newer one; the in-memory copy then fails on every purchase while a
					   working token sits in the session file. Only if that also fails is the session really
					   dead, and then the prompt is the honest answer. */
					if (await TryAdoptPersistedSessionAsync())
					{
						return true;
					}
					RequireAuthentication("The saved account session is no longer valid. Sign in again.");
				}
				else
				{
					SetStatus("Could not refresh the account session. We will retry when the connection recovers: " + FriendlyError(ex));
				}
				return false;
			}
		}

		/* [user request 2026-09-27: cannot buy cars] One recovery attempt for a rejected refresh: adopt the
		   session saved on disk (SupabaseBootstrap.AdoptPersistedSessionAsync only does so when it holds a
		   different refresh token for the same user) and, if its access token is stale too, refresh once with
		   the newer token. Returns true only with a usable session in hand, so the caller can carry on as if
		   the refresh had worked. Deliberately no loop: two failures mean the saved token is dead as well. */
		private static async Task<bool> TryAdoptPersistedSessionAsync()
		{
			if (!await SupabaseBootstrap.AdoptPersistedSessionAsync())
			{
				return false;
			}
			Session adopted = SupabaseBootstrap.Client?.Auth?.CurrentSession;
			if (adopted == null || string.IsNullOrWhiteSpace(adopted.AccessToken) || string.IsNullOrWhiteSpace(adopted.RefreshToken))
			{
				return false;
			}
			if (!SessionNeedsRefresh(adopted))
			{
				return true;
			}
			try
			{
				string expectedUserId = SupabaseBootstrap.Client.Auth.CurrentUser?.Id;
				Session refreshed;
				using (SupabaseBootstrap.ProtectPersistedSessionDuringRefresh())
				{
					refreshed = await SupabaseBootstrap.Client.Auth.RefreshSession();
				}
				string refreshedUserId = refreshed?.User?.Id ?? SupabaseBootstrap.Client.Auth.CurrentUser?.Id;
				if (refreshed == null || string.IsNullOrWhiteSpace(refreshed.AccessToken) || string.IsNullOrWhiteSpace(refreshed.RefreshToken))
				{
					return false;
				}
				if (!string.IsNullOrWhiteSpace(expectedUserId) && !string.Equals(expectedUserId, refreshedUserId, StringComparison.Ordinal))
				{
					return false;
				}
				Debug.Log("[Supabase] The saved session refreshed after the in-memory one was rejected. UserId=" + refreshedUserId);
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[Supabase] The saved session could not be refreshed either: " + FriendlyError(ex));
				return false;
			}
		}

		private static bool IsExpiredJwt(Exception exception)
		{
			string text = exception?.ToString() ?? string.Empty;
			return text.IndexOf("PGRST303", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("PGRST301", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("JWT expired", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("token is expired", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static bool HasPendingLocalChanges()
		{
			return PlayerPrefs.GetInt(CloudDirtyKey, 0) == 1 || PlayerPrefs.GetInt(PendingCrystalDeltaKey, 0) != 0 || !string.IsNullOrEmpty(PlayerPrefs.GetString(InFlightOperationIdKey, string.Empty));
		}

		private static async Task DownloadCloudProfileAsync()
		{
			string userId = SupabaseBootstrap.Client.Auth.CurrentUser.Id;
			PlayerProfile cloudProfile = await FetchCloudProfileAsync(userId);
			if (cloudProfile == null)
			{
				CurrentPlayer.Id = userId;
				await SupabaseBootstrap.Client.From<PlayerProfile>().Upsert(CurrentPlayer, null, default(CancellationToken));
				SaveToLocalStorage();
				SetStatus("Облачный профиль создан.");
				return;
			}
			ApplyDownloadedProfile(cloudProfile);
			SetStatus("Профиль загружен из облака.");
		}

		private static async Task<PlayerProfile> FetchCloudProfileAsync(string userId)
		{
			var response = await SupabaseBootstrap.Client.From<PlayerProfile>().Where(profile => profile.Id == userId).Get(default(CancellationToken));
			return response.Models.FirstOrDefault();
		}

		private static async Task SynchronizePendingChangesAsync()
		{
			string operationId = PlayerPrefs.GetString(InFlightOperationIdKey, string.Empty);
			int delta;
			if (string.IsNullOrEmpty(operationId))
			{
				operationId = Guid.NewGuid().ToString("D");
				delta = PlayerPrefs.GetInt(PendingCrystalDeltaKey, 0);
				PlayerPrefs.SetString(InFlightOperationIdKey, operationId);
				PlayerPrefs.SetInt(InFlightCrystalDeltaKey, delta);
				PlayerPrefs.SetInt(PendingCrystalDeltaKey, 0);
				PlayerPrefs.Save();
			}
			else
			{
				delta = PlayerPrefs.GetInt(InFlightCrystalDeltaKey, 0);
			}
			PlayerProfile serverProfile = null;
			bool missingFunction = false;
			try
			{
				serverProfile = await SupabaseBootstrap.Client.Postgrest.Rpc<PlayerProfile>("sync_player_profile", new
				{
					p_operation_id = operationId,
					p_crystal_delta = delta,
					p_car_1 = CurrentPlayer.Car1,
					p_car_2 = CurrentPlayer.Car2,
					p_car_3 = CurrentPlayer.Car3,
					p_car_4 = CurrentPlayer.Car4
				});
			}
			catch (Exception ex) when (IsMissingSyncFunction(ex))
			{
				missingFunction = true;
			}
			if (missingFunction)
			{
				Debug.LogWarning("[Supabase] sync_player_profile не установлена; используется совместимый режим.");
				serverProfile = await LegacyMergeAsync(delta);
			}
			if (serverProfile == null)
			{
				throw new InvalidOperationException("Сервер не вернул профиль после синхронизации.");
			}
			PlayerProfile verified = await FetchCloudProfileAsync(SupabaseBootstrap.Client.Auth.CurrentUser.Id);
			CompleteSynchronizedBatch(verified ?? serverProfile);
			if (HasPendingLocalChanges())
			{
				throw new InvalidOperationException("Supabase did not confirm all queued profile changes; the outbox was kept for the next explicit trigger.");
			}
			SetStatus("Прогресс синхронизирован.");
		}

		private static async Task<PlayerProfile> LegacyMergeAsync(int crystalDelta)
		{
			string userId = SupabaseBootstrap.Client.Auth.CurrentUser.Id;
			var response = await SupabaseBootstrap.Client.From<PlayerProfile>().Where(profile => profile.Id == userId).Get(default(CancellationToken));
			PlayerProfile cloud = response.Models.FirstOrDefault() ?? CreateDefaultProfile();
			cloud.Id = userId;
			cloud.Crystals = Mathf.Max(0, cloud.Crystals + crystalDelta);
			cloud.Car1 |= CurrentPlayer.Car1;
			cloud.Car2 |= CurrentPlayer.Car2;
			cloud.Car3 |= CurrentPlayer.Car3;
			cloud.Car4 |= CurrentPlayer.Car4;
			var upserted = await SupabaseBootstrap.Client.From<PlayerProfile>().Upsert(cloud, null, default(CancellationToken));
			return upserted.Models.FirstOrDefault() ?? cloud;
		}

		private static void CompleteSynchronizedBatch(PlayerProfile serverProfile)
		{
			int pendingDelta = PlayerPrefs.GetInt(PendingCrystalDeltaKey, 0);
			bool car1 = CurrentPlayer.Car1;
			bool car2 = CurrentPlayer.Car2;
			bool car3 = CurrentPlayer.Car3;
			bool car4 = CurrentPlayer.Car4;
			bool unlocksMissing = (car1 && !serverProfile.Car1) || (car2 && !serverProfile.Car2) || (car3 && !serverProfile.Car3) || (car4 && !serverProfile.Car4);
			CurrentPlayer = serverProfile;
			CurrentPlayer.Crystals = Mathf.Max(0, serverProfile.Crystals + pendingDelta);
			CurrentPlayer.Car1 |= car1;
			CurrentPlayer.Car2 |= car2;
			CurrentPlayer.Car3 |= car3;
			CurrentPlayer.Car4 |= car4;
			PlayerPrefs.DeleteKey(InFlightOperationIdKey);
			PlayerPrefs.DeleteKey(InFlightCrystalDeltaKey);
			PlayerPrefs.SetInt(CloudDirtyKey, pendingDelta > 0 || unlocksMissing ? 1 : 0);
			SaveToLocalStorage();
			ProfileChanged?.Invoke();
		}

		private static void ApplyDownloadedProfile(PlayerProfile cloudProfile)
		{
			string sessionUserId = SupabaseBootstrap.Client?.Auth?.CurrentUser?.Id;
			if (string.IsNullOrWhiteSpace(sessionUserId) || cloudProfile == null || !string.Equals(cloudProfile.Id, sessionUserId, StringComparison.Ordinal))
			{
				throw new InvalidOperationException("Cloud profile does not match the authenticated user.");
			}
			CurrentPlayer = cloudProfile;
			PlayerPrefs.SetInt(CloudDirtyKey, 0);
			PlayerPrefs.SetInt(PendingCrystalDeltaKey, 0);
			PlayerPrefs.DeleteKey(InFlightOperationIdKey);
			PlayerPrefs.DeleteKey(InFlightCrystalDeltaKey);
			SaveToLocalStorage();
			ProfileChanged?.Invoke();
			Debug.Log(string.Format("[Supabase] Profile applied. UserId={0}, Cars={1},{2},{3},{4}", CurrentPlayer.Id, CurrentPlayer.Car1, CurrentPlayer.Car2, CurrentPlayer.Car3, CurrentPlayer.Car4));
		}

		private static bool IsMissingSyncFunction(Exception exception)
		{
			string text = exception?.ToString() ?? string.Empty;
			if (text.IndexOf("PGRST202", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
			if (text.IndexOf("sync_player_profile", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return false;
			}
			return text.IndexOf("404", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("not find", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("schema cache", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static void LoadFromLocalStorage()
		{
			CurrentPlayer = new PlayerProfile
			{
				Id = PlayerPrefs.GetString(ProfileIdKey, string.Empty),
				Crystals = PlayerPrefs.GetInt(CrystalsKey, 25),
				Car1 = PlayerPrefs.GetInt(Car1Key, 1) == 1,
				Car2 = PlayerPrefs.GetInt(Car2Key, 0) == 1,
				Car3 = PlayerPrefs.GetInt(Car3Key, 0) == 1,
				Car4 = PlayerPrefs.GetInt(Car4Key, 0) == 1
			};
			if (PlayerPrefs.GetInt(SyncSchemaVersionKey, 0) < SyncSchemaVersion)
			{
				PlayerPrefs.SetInt(CloudDirtyKey, 0);
				PlayerPrefs.SetInt(PendingCrystalDeltaKey, 0);
				PlayerPrefs.DeleteKey(InFlightOperationIdKey);
				PlayerPrefs.DeleteKey(InFlightCrystalDeltaKey);
				PlayerPrefs.SetInt(SyncSchemaVersionKey, SyncSchemaVersion);
				PlayerPrefs.Save();
			}
			ProfileChanged?.Invoke();
		}

		private static void SaveToLocalStorage()
		{
			if (CurrentPlayer == null)
			{
				CurrentPlayer = CreateDefaultProfile();
			}
			if (IsAuthenticated)
			{
				CurrentPlayer.Id = SupabaseBootstrap.Client.Auth.CurrentUser.Id;
				PlayerPrefs.SetInt(RememberedGuestKey, IsGuestAccount ? 1 : 0);
				PlayerPrefs.SetString(RememberedEmailKey, SupabaseBootstrap.Client.Auth.CurrentUser.Email ?? string.Empty);
			}
			PlayerPrefs.SetString(ProfileIdKey, CurrentPlayer.Id ?? string.Empty);
			PlayerPrefs.SetInt(CrystalsKey, CurrentPlayer.Crystals);
			PlayerPrefs.SetInt(Car1Key, CurrentPlayer.Car1 ? 1 : 0);
			PlayerPrefs.SetInt(Car2Key, CurrentPlayer.Car2 ? 1 : 0);
			PlayerPrefs.SetInt(Car3Key, CurrentPlayer.Car3 ? 1 : 0);
			PlayerPrefs.SetInt(Car4Key, CurrentPlayer.Car4 ? 1 : 0);
			PlayerPrefs.Save();
		}

		private static PlayerProfile CreateDefaultProfile()
		{
			return new PlayerProfile
			{
				Crystals = 25,
				Car1 = true,
				Car2 = false,
				Car3 = false,
				Car4 = false
			};
		}

		private static void ClearSyncOutbox()
		{
			PlayerPrefs.SetInt(CloudDirtyKey, 0);
			PlayerPrefs.SetInt(PendingCrystalDeltaKey, 0);
			PlayerPrefs.DeleteKey(InFlightOperationIdKey);
			PlayerPrefs.DeleteKey(InFlightCrystalDeltaKey);
			PlayerPrefs.Save();
		}

		private static void ClearLocalAccountState()
		{
			ClearSyncOutbox();
			PlayerPrefs.DeleteKey(AuthChoiceKey);
			PlayerPrefs.DeleteKey(LegacyPendingGuestKey);
			PlayerPrefs.DeleteKey(ProfileIdKey);
			PlayerPrefs.DeleteKey(CrystalsKey);
			PlayerPrefs.DeleteKey(Car1Key);
			PlayerPrefs.DeleteKey(Car2Key);
			PlayerPrefs.DeleteKey(Car3Key);
			PlayerPrefs.DeleteKey(Car4Key);
			PlayerPrefs.DeleteKey(RememberedGuestKey);
			PlayerPrefs.DeleteKey(RememberedEmailKey);
			PlayerPrefs.Save();
			CurrentPlayer = CreateDefaultProfile();
			ProfileChanged?.Invoke();
		}

		private static string StableGuestNumber(string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				return "CONNECTING";
			}
			string hex = new string(userId.Where(Uri.IsHexDigit).Take(8).ToArray());
			if (uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint value))
			{
				uint number = value % 100000000u;
				return number.ToString("D8", CultureInfo.InvariantCulture);
			}
			int hash = Math.Abs(userId.GetHashCode());
			return hash.ToString("D8", CultureInfo.InvariantCulture);
		}

		private static string ShortAccountId(string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
			{
				return "ACCOUNT_CONNECTING";
			}
			string compact = userId.Replace("-", string.Empty).ToUpperInvariant();
			return "ID_" + compact.Substring(0, Mathf.Min(10, compact.Length));
		}

		private static bool ValidateCredentials(string email, string password)
		{
			if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
			{
				SetStatus("Введи правильную почту.");
				return false;
			}
			if (string.IsNullOrEmpty(password) || password.Length < 6)
			{
				SetStatus("Пароль должен содержать минимум 6 символов.");
				return false;
			}
			return true;
		}

		private static async Task<bool> EnsureOnlineClientAsync()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				SetStatus("Internet is required to sign in.");
				return false;
			}
			await SupabaseBootstrap.InitializeAsync();
			if (!SupabaseBootstrap.IsReady)
			{
				SetStatus("Supabase временно недоступен.");
				return false;
			}
			return true;
		}

		private static void CompleteFirstChoice()
		{
			PlayerPrefs.SetInt(AuthChoiceKey, 1);
			PlayerPrefs.DeleteKey(LegacyPendingGuestKey);
			PlayerPrefs.Save();
		}

		/* [user request 2026-09-27: cannot buy cars] Every caller reaches here having found the saved session
		   unusable. For a guest that is terminal — there is no credential to sign in with — so the player's next
		   "Play as Guest" creates a different user id, and the profile reset that follows would hand them
		   CreateDefaultProfile() (25 crystals, one car) in place of what they own. Record the stranded id first. */
		private static void RequireAuthentication(string message)
		{
			if (IsRememberedGuestAccount && CurrentPlayer != null && !string.IsNullOrWhiteSpace(CurrentPlayer.Id))
			{
				PlayerPrefs.SetString(StrandedGuestProfileKey, CurrentPlayer.Id);
				PlayerPrefs.Save();
			}
			SetStatus(message);
			FirstLaunchAuthScreen.Show();
		}

		private static bool IsPermanentSessionFailure(string error)
		{
			if (string.IsNullOrWhiteSpace(error))
			{
				return false;
			}
			if (IsTransientNetworkFailure(error))
			{
				return false;
			}
			string lower = error.ToLowerInvariant();
			/* [user request 2026-09-27: cannot buy cars] GoTrue answers a refresh whose session is past its
			   timebox / inactivity limit with "Session expired" (code session_expired), and a rotated token that
			   was already spent with refresh_token_already_used. Neither string was listed here, so both fell
			   through to the transient branch: the game logged "we will retry when the connection recovers" and
			   retried the same dead token on every purchase, forever, without ever asking the player to sign in.
			   original: return lower.Contains("invalid refresh token") || lower.Contains("refresh_token_not_found") || lower.Contains("session_not_found") || lower.Contains("user not found") || lower.Contains("user_not_found") || lower.Contains("invalid_grant"); */
			return lower.Contains("invalid refresh token") || lower.Contains("refresh_token_not_found") || lower.Contains("refresh_token_already_used") || lower.Contains("session_not_found") || lower.Contains("session_expired") || lower.Contains("session expired") || lower.Contains("user not found") || lower.Contains("user_not_found") || lower.Contains("invalid_grant");
		}

		private static bool IsTransientNetworkFailure(string error)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return true;
			}
			if (string.IsNullOrWhiteSpace(error))
			{
				return false;
			}
			string lower = error.ToLowerInvariant();
			return lower.Contains("timeout") || lower.Contains("timed out") || lower.Contains("task was canceled") || lower.Contains("operation was canceled") || lower.Contains("network") || lower.Contains("offline") || lower.Contains("connection") || lower.Contains("dns") || lower.Contains("resolve host") || lower.Contains("name or service not known") || lower.Contains("temporarily unavailable") || lower.Contains("http 502") || lower.Contains("http 503") || lower.Contains("http 504");
		}

		private static void SetStatus(string message)
		{
			AuthenticationStatusChanged?.Invoke(message);
			Debug.Log("[Supabase] " + message);
		}

		private static string FriendlyError(Exception exception)
		{
			if (string.IsNullOrWhiteSpace(exception?.Message))
			{
				return "неизвестная ошибка";
			}
			return exception.Message;
		}
	}
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Gotrue;
using Supabase.Postgrest.Responses;
using UnityEngine;

namespace ShadowRun.Services
{
	public sealed class SupabaseGameManager : MonoBehaviour
	{
		[Serializable]
		private sealed class JwtPayload
		{
			public long exp;
		}

		private sealed class _003C_003Ec__DisplayClass113_0
		{
			public string userId;
		}

		private sealed class _003C_003Ec__DisplayClass115_0
		{
			public string userId;
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CCompleteNativeAppleSignInAsync_003Ed__87 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public string payload;

			private string _003CidentityToken_003E5__2;

			private string _003CrawNonce_003E5__3;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private TaskAwaiter<Session> _003C_003Eu__2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CDeleteAccountAsync_003Ed__79 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private TaskAwaiter<object> _003C_003Eu__2;

			private TaskAwaiter _003C_003Eu__3;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CDownloadCloudProfileAsync_003Ed__112 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			private string _003CuserId_003E5__2;

			private TaskAwaiter<PlayerProfile> _003C_003Eu__1;

			private TaskAwaiter<ModeledResponse<PlayerProfile>> _003C_003Eu__2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CEnsureOnlineClientAsync_003Ed__127 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CExecuteProfileSyncAsync_003Ed__105 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CFetchCloudProfileAsync_003Ed__113 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<PlayerProfile> _003C_003Et__builder;

			public string userId;

			private TaskAwaiter<ModeledResponse<PlayerProfile>> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CFinishAuthenticatedSignInAsync_003Ed__90 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private string _003CauthenticatedUserId_003E5__2;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CHandleDeepLink_003Ed__82 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public string url;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private TaskAwaiter<Session> _003C_003Eu__2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CInitializeOnlineFeaturesAsync_003Ed__72 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			private TaskAwaiter _003C_003Eu__1;

			private TaskAwaiter<bool> _003C_003Eu__2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CLegacyMergeAsync_003Ed__115 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<PlayerProfile> _003C_003Et__builder;

			private _003C_003Ec__DisplayClass115_0 _003C_003E8__1;

			public int crystalDelta;

			private PlayerProfile _003Ccloud_003E5__2;

			private TaskAwaiter<ModeledResponse<PlayerProfile>> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CRefreshAndSyncWhenOnlineAsync_003Ed__103 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private bool _003Csucceeded_003E5__2;

			private TaskAwaiter _003C_003Eu__1;

			private TaskAwaiter<bool> _003C_003Eu__2;

			private int _003C_003E7__wrap2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CRefreshSessionCoreAsync_003Ed__109 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private string _003CexpectedUserId_003E5__2;

			private IDisposable _003C_003E7__wrap2;

			private TaskAwaiter<Session> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CRegisterWithEmailAsync_003Ed__77 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public string email;

			public string password;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private string _003CnormalizedEmail_003E5__2;

			private string _003CguestUserId_003E5__3;

			private TaskAwaiter<User> _003C_003Eu__2;

			private TaskAwaiter<Session> _003C_003Eu__3;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CRequireOnlineEconomyAsync_003Ed__97 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private TaskAwaiter _003C_003Eu__1;

			private TaskAwaiter<bool> _003C_003Eu__2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSignInAsGuestOnlineAsync_003Ed__75 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			private TaskAwaiter _003C_003Eu__1;

			private TaskAwaiter<bool> _003C_003Eu__2;

			private TaskAwaiter<Session> _003C_003Eu__3;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSignInWithEmailAsync_003Ed__76 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public string email;

			public string password;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private TaskAwaiter<Session> _003C_003Eu__2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSignOutAsync_003Ed__78 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private TaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CStartAppleSignInAsync_003Ed__80 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CStartGoogleSignInAsync_003Ed__81 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private TaskAwaiter<ProviderAuthState> _003C_003Eu__2;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSyncGaragePurchaseAsync_003Ed__96 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public string vehicleId;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSyncProfileAfterCaseClosedAsync_003Ed__94 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSyncProfileAfterEconomyChangeAsync_003Ed__95 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSynchronizePendingChangesAsync_003Ed__114 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			private int _003Cdelta_003E5__2;

			private PlayerProfile _003CserverProfile_003E5__3;

			private int _003C_003E7__wrap3;

			private TaskAwaiter<PlayerProfile> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
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

		private static readonly object SessionRefreshLock;

		private static Task<bool> sessionRefreshTask;

		private static string pendingAppleNonce;

		private static string lastOAuthCallback;

		private static bool oauthCompletionInProgress;

		private static Task onlineRecoveryTask;

		private static PlayerProfile CurrentPlayer__BackingField;

		private static Action m_ProfileChanged;

		private static Action m_AuthenticationCompleted;

		private static Action<string> m_AuthenticationStatusChanged;

		public static PlayerProfile CurrentPlayer
		{
			get
			{
				return CurrentPlayer__BackingField;
			}
			private set
			{
				CurrentPlayer__BackingField = value;
			}
		}

		public static bool HasCompletedFirstChoice => false;

		public static bool IsCloudReady => false;

		public static bool IsAuthenticated => false;

		public static bool HasRememberedAccount => false;

		public static string CurrentUserId => null;

		public static string CurrentUserEmail => null;

		public static bool IsGuestAccount => false;

		public static bool IsRememberedGuestAccount => false;

		public static bool ProfileMatchesSession => false;

		public static string AccountDisplayId => null;

		public static int Crystals => 0;

		public static event Action ProfileChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public static event Action AuthenticationCompleted
		{
			add
			{
			}
			remove
			{
			}
		}

		public static event Action<string> AuthenticationStatusChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		[PreserveSig]
		private static extern void ShadowRun_StartAppleSignIn(string rawNonce);

		[PreserveSig]
		private static extern void ShadowRun_StartWebAuth(string url, string callbackScheme);

		private static void CreateInstance()
		{
		}

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		private void OnApplicationPause(bool paused)
		{
		}

		private void OnApplicationFocus(bool hasFocus)
		{
		}

		private void OnApplicationQuit()
		{
		}

		private static Task InitializeOnlineFeaturesAsync()
		{
			return null;
		}

		private static void RequestOnlineRecovery()
		{
		}

		public static void PlayAsGuest()
		{
		}

		private static Task SignInAsGuestOnlineAsync()
		{
			return null;
		}

		public static Task<bool> SignInWithEmailAsync(string email, string password)
		{
			return null;
		}

		public static Task<bool> RegisterWithEmailAsync(string email, string password)
		{
			return null;
		}

		public static Task<bool> SignOutAsync()
		{
			return null;
		}

		public static Task<bool> DeleteAccountAsync()
		{
			return null;
		}

		public static Task<bool> StartAppleSignInAsync()
		{
			return null;
		}

		public static Task<bool> StartGoogleSignInAsync()
		{
			return null;
		}

		private static void HandleDeepLink(string url)
		{
		}

		public void OnNativeOAuthCallback(string url)
		{
		}

		public void OnNativeAppleCredential(string payload)
		{
		}

		public void OnNativeAuthCancelled(string message)
		{
		}

		public void OnNativeAuthError(string message)
		{
		}

		private static Task CompleteNativeAppleSignInAsync(string payload)
		{
			return null;
		}

		private static string CreateSecureNonce()
		{
			return null;
		}

		private static string ReadOAuthValue(string url, string key)
		{
			return null;
		}

		private static Task<bool> FinishAuthenticatedSignInAsync()
		{
			return null;
		}

		private static bool HasUsableSession(Session session)
		{
			return false;
		}

		public static Task LoadPlayerFromCloud()
		{
			return null;
		}

		public static Task RefreshProfileIfOnlineAsync()
		{
			return null;
		}

		public static Task SyncProfileAfterCaseClosedAsync()
		{
			return null;
		}

		public static Task SyncProfileAfterEconomyChangeAsync()
		{
			return null;
		}

		public static Task<bool> SyncGaragePurchaseAsync(string vehicleId)
		{
			return null;
		}

		public static Task<bool> RequireOnlineEconomyAsync()
		{
			return null;
		}

		public static void SaveGameData()
		{
		}

		public static void SetCrystals(int amount)
		{
		}

		public static bool IsVehicleUnlocked(string vehicleId)
		{
			return false;
		}

		public static void SetVehicleUnlocked(string vehicleId, bool unlocked)
		{
		}

		public static void QueueCaseReward(string vehicleId, int crystalAmount)
		{
		}

		private static Task<bool> RefreshAndSyncWhenOnlineAsync()
		{
			return null;
		}

		private static void ResetCachedProfileIfSessionChanged()
		{
		}

		private static Task ExecuteProfileSyncAsync()
		{
			return null;
		}

		private static Task<bool> EnsureFreshSessionAsync(bool forceRefresh = false)
		{
			return null;
		}

		private static bool SessionNeedsRefresh(Session session)
		{
			return false;
		}

		private static bool TryReadJwtExpirationUtc(string accessToken, out DateTime expiresAtUtc)
		{
			expiresAtUtc = default;
			return false;
		}

		private static Task<bool> RefreshSessionCoreAsync()
		{
			return null;
		}

		private static bool IsExpiredJwt(Exception exception)
		{
			return false;
		}

		private static bool HasPendingLocalChanges()
		{
			return false;
		}

		private static Task DownloadCloudProfileAsync()
		{
			return null;
		}

		private static Task<PlayerProfile> FetchCloudProfileAsync(string userId)
		{
			return null;
		}

		private static Task SynchronizePendingChangesAsync()
		{
			return null;
		}

		private static Task<PlayerProfile> LegacyMergeAsync(int crystalDelta)
		{
			return null;
		}

		private static void CompleteSynchronizedBatch(PlayerProfile serverProfile)
		{
		}

		private static void ApplyDownloadedProfile(PlayerProfile cloudProfile)
		{
		}

		private static bool IsMissingSyncFunction(Exception exception)
		{
			return false;
		}

		private static void LoadFromLocalStorage()
		{
		}

		private static void SaveToLocalStorage()
		{
		}

		private static PlayerProfile CreateDefaultProfile()
		{
			return null;
		}

		private static void ClearSyncOutbox()
		{
		}

		private static void ClearLocalAccountState()
		{
		}

		private static string StableGuestNumber(string userId)
		{
			return null;
		}

		private static string ShortAccountId(string userId)
		{
			return null;
		}

		private static bool ValidateCredentials(string email, string password)
		{
			return false;
		}

		private static Task<bool> EnsureOnlineClientAsync()
		{
			return null;
		}

		private static void CompleteFirstChoice()
		{
		}

		private static void RequireAuthentication(string message)
		{
		}

		private static bool IsPermanentSessionFailure(string error)
		{
			return false;
		}

		private static bool IsTransientNetworkFailure(string error)
		{
			return false;
		}

		private static void SetStatus(string message)
		{
		}

		private static string FriendlyError(Exception exception)
		{
			return null;
		}
	}
}

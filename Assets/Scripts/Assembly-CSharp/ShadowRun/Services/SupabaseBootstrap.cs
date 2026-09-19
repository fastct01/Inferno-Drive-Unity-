using System;
using System.Threading.Tasks;
using Supabase;
using Supabase.Gotrue;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   SupabaseBootstrap_get_HasPersistedSession_mA66E047A354BC6F0630A1D27F46303E0E33BC9A7
//   SupabaseBootstrap_get_PersistedUserId_mBC46947554AE1B5D62EA146F170F91747135FD9D
//   SupabaseBootstrap_CreateInstance_mD7BD08A31B5C0EF98036C1B7FE43A0B8F38C70E6
//   SupabaseBootstrap_Awake_m65B7BE5314B177A5F1C9DBA1989FA9E80092BD99
//   SupabaseBootstrap_InitializeAsync_mB9A7FC1702FD0E219A2952CE2A4DB2AEA18A6868
//   U3CInitializeCoreAsyncU3Ed__30_MoveNext_m8AE0005B92F7A0526B71A49A4B8449C2B5EE632D
//   SupabaseBootstrap_RestorePersistedSessionAsync_mF44E240A5D0E69F311C3534A67CA38D91700F2E0
//   SupabaseBootstrap_PersistCurrentSession_mECCA9F1782CB6B52DBCDF74825F4C2571100849C
//   SupabaseBootstrap_DestroyPersistedSessionExplicitly_m5531A985B65F190962602EB1A428496223E5560E
//   SupabaseBootstrap_ProtectPersistedSessionDuringRefresh_mAB9345266E296AC02FA61D0A7DE87304853F773F
//   U3CRestorePersistedSessionCoreAsyncU3Ed__35_MoveNext_mCC739C0F5B3AA470E75D37D67D4DBB4F976B6CD0
//   SupabaseBootstrap_IsConfigured_m9400EA9EB1C66BA355EBA4A9FFB987A4C782036A
//   SupabaseBootstrap_SetError_mC3D34F7D02EF731C9999B2770DBD275C803154B7
namespace ShadowRun.Services
{
	public sealed class SupabaseBootstrap : MonoBehaviour
	{
		[Serializable]
		private sealed class ProjectConfig
		{
			public string project_url;

			public string publishable_key;
		}

		private const string ConfigResourceName = "supabase";

		private const string PlaceholderProjectUrl = "YOUR_PROJECT_REF";

		private const string PlaceholderPublishableKey = "YOUR_SUPABASE_PUBLISHABLE_KEY";

		private static Task initializationTask;

		private static Task<bool> sessionRestoreTask;

		private static SupabaseSessionStore sessionStore;

		public static Supabase.Client Client { get; private set; }

		public static bool IsReady { get; private set; }

		public static string InitializationError { get; private set; }

		public static string SessionRestoreError { get; private set; }

		public static bool HasPersistedSession => sessionStore?.LoadSession() != null;

		public static string PersistedUserId => sessionStore?.LoadSession()?.User?.Id ?? string.Empty;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CreateInstance()
		{
			if (UnityEngine.Object.FindAnyObjectByType<SupabaseBootstrap>() != null)
			{
				return;
			}
			GameObject host = new GameObject("Supabase");
			UnityEngine.Object.DontDestroyOnLoad(host);
			host.AddComponent<SupabaseBootstrap>();
		}

		private void Awake()
		{
			_ = InitializeAsync();
		}

		public static Task InitializeAsync()
		{
			return initializationTask ?? (initializationTask = InitializeCoreAsync());
		}

		private static async Task InitializeCoreAsync()
		{
			if (Client != null)
			{
				return;
			}
			InitializationError = null;
			TextAsset configAsset = Resources.Load<TextAsset>(ConfigResourceName);
			if (configAsset == null)
			{
				SetError("Не найден Assets/Resources/supabase.json.");
				return;
			}
			ProjectConfig config = JsonUtility.FromJson<ProjectConfig>(configAsset.text);
			if (!IsConfigured(config))
			{
				SetError("Укажите project_url и publishable_key в Assets/Resources/supabase.json.");
				return;
			}
			try
			{
				sessionStore = new SupabaseSessionStore();
				SupabaseOptions options = new SupabaseOptions
				{
					AutoConnectRealtime = false,
					AutoRefreshToken = false,
					SessionHandler = sessionStore
				};
				Client = new Supabase.Client(config.project_url.Trim().TrimEnd('/'), config.publishable_key.Trim(), options);
				using (sessionStore.ProtectDuringRestore())
				{
					await Client.InitializeAsync();
				}
				IsReady = true;
				InitializationError = null;
				if (Application.internetReachability != NetworkReachability.NotReachable)
				{
					await RestorePersistedSessionAsync();
				}
				Debug.Log("[Supabase] Клиент инициализирован.");
			}
			catch (Exception ex)
			{
				Client = null;
				initializationTask = null;
				SetError(ex.Message);
			}
		}

		public static Task<bool> RestorePersistedSessionAsync()
		{
			if (Client?.Auth?.CurrentSession != null)
			{
				return Task.FromResult(true);
			}
			if (!IsReady || sessionStore == null || Application.internetReachability == NetworkReachability.NotReachable)
			{
				return Task.FromResult(false);
			}
			if (sessionRestoreTask == null || sessionRestoreTask.IsCompleted)
			{
				sessionRestoreTask = RestorePersistedSessionCoreAsync();
			}
			return sessionRestoreTask;
		}

		public static void PersistCurrentSession()
		{
			Session session = Client?.Auth?.CurrentSession;
			if (session == null || string.IsNullOrWhiteSpace(session.RefreshToken))
			{
				return;
			}
			sessionStore?.SaveSession(session);
		}

		public static void DestroyPersistedSessionExplicitly()
		{
			sessionStore?.DestroySessionExplicitly();
		}

		internal static IDisposable ProtectPersistedSessionDuringRefresh()
		{
			return sessionStore?.ProtectDuringRestore();
		}

		private static async Task<bool> RestorePersistedSessionCoreAsync()
		{
			try
			{
				SessionRestoreError = null;
				Session persisted = sessionStore.LoadSession();
				if (persisted == null || string.IsNullOrWhiteSpace(persisted.AccessToken) || string.IsNullOrWhiteSpace(persisted.RefreshToken))
				{
					return false;
				}
				string expectedUserId = persisted.User?.Id;
				Session restored;
				using (sessionStore.ProtectDuringRestore())
				{
					restored = await Client.Auth.SetSession(persisted.AccessToken, persisted.RefreshToken, false);
				}
				string restoredUserId = restored?.User?.Id ?? Client.Auth.CurrentUser?.Id;
				if (!string.IsNullOrWhiteSpace(expectedUserId) && !string.Equals(expectedUserId, restoredUserId, StringComparison.Ordinal))
				{
					throw new InvalidOperationException("Восстановленная сессия имеет другой User ID.");
				}
				bool hasSession = Client.Auth.CurrentSession != null;
				if (hasSession)
				{
					Debug.Log("[Supabase] Сессия восстановлена. UserId=" + restoredUserId);
				}
				return hasSession;
			}
			catch (Exception ex)
			{
				SessionRestoreError = ex.ToString();
				Debug.LogWarning("[Supabase] Сессия пока не восстановлена: " + ex.Message);
				return false;
			}
		}

		private static bool IsConfigured(ProjectConfig config)
		{
			return config != null && !string.IsNullOrWhiteSpace(config.project_url) && !string.IsNullOrWhiteSpace(config.publishable_key) && !config.project_url.Contains(PlaceholderProjectUrl) && !config.publishable_key.Contains(PlaceholderPublishableKey);
		}

		private static void SetError(string message)
		{
			IsReady = false;
			InitializationError = message;
			Debug.LogError("[Supabase] Ошибка инициализации: " + message);
		}
	}
}

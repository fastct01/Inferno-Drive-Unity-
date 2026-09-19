using System;

namespace Unity.Services.LevelPlay
{
	public class LevelPlay
	{
		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9;

			internal void _003C_002Ecctor_003Eb__1_0(LevelPlayConfiguration configuration)
			{
			}

			internal void _003C_002Ecctor_003Eb__1_1(LevelPlayInitError error)
			{
			}

			internal void _003C_002Ecctor_003Eb__1_2(LevelPlayImpressionData impressionData)
			{
			}
		}

		private static readonly ILevelPlaySdk Sdk;

		public static string PluginVersion;

		private static Action<LevelPlayConfiguration> m_OnInitSuccessReceived;

		private static Action<LevelPlayInitError> m_OnInitFailedReceived;

		private static Action<LevelPlayImpressionData> m_OnImpressionDataReadyReceived;

		public static string UnityVersion => null;

		private static event Action<LevelPlayConfiguration> OnInitSuccessReceived
		{
			add
			{
			}
			remove
			{
			}
		}

		private static event Action<LevelPlayInitError> OnInitFailedReceived
		{
			add
			{
			}
			remove
			{
			}
		}

		private static event Action<LevelPlayImpressionData> OnImpressionDataReadyReceived
		{
			add
			{
			}
			remove
			{
			}
		}

		public static event Action<LevelPlayConfiguration> OnInitSuccess
		{
			add
			{
			}
			remove
			{
			}
		}

		public static event Action<LevelPlayInitError> OnInitFailed
		{
			add
			{
			}
			remove
			{
			}
		}

		public static event Action<LevelPlayImpressionData> OnImpressionDataReady
		{
			add
			{
			}
			remove
			{
			}
		}

		static LevelPlay()
		{
		}

		public static void Init(string appKey, string userId = null)
		{
		}

		public static void SetPauseGame(bool pause)
		{
		}

		public static bool SetDynamicUserId(string dynamicUserId)
		{
			return false;
		}

		public static void ValidateIntegration()
		{
		}

		public static void LaunchTestSuite()
		{
		}

		public static void SetAdaptersDebug(bool enabled)
		{
		}

		public static void SetNetworkData(string networkKey, string networkData)
		{
		}

		public static void SetMetaData(string key, string value)
		{
		}

		public static void SetMetaData(string key, string[] values)
		{
		}

		public static void SetConsent(bool consent)
		{
		}

		public static void SetSegment(LevelPlaySegment segment)
		{
		}
	}
}

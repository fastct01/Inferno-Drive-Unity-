using System;
using System.Collections.Generic;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosLevelPlaySdk : ILevelPlaySdk
	{
		private delegate void InitSuccessCallback(string configuration);

		private delegate void InitFailureCallback(string error);

		private delegate void ImpressionSuccessCallback(string impressionData);

		internal static readonly IosLevelPlaySdk Instance;

		private readonly IIosNativeBridge m_NativeBridge;

		private Action<LevelPlayConfiguration> m_OnInitSuccess;

		private Action<LevelPlayInitError> m_OnInitFailed;

		private Action<LevelPlayImpressionData> m_OnImpressionDataReady;

		public event Action<LevelPlayConfiguration> OnInitSuccess
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayInitError> OnInitFailed
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayImpressionData> OnImpressionDataReady
		{
			add
			{
			}
			remove
			{
			}
		}

		private IosLevelPlaySdk()
		{
		}

		internal IosLevelPlaySdk(IIosNativeBridge bridge)
		{
		}

		public void Initialize(string appKey, string userId)
		{
		}

		public void SetPauseGame(bool pause)
		{
		}

		public bool SetDynamicUserId(string dynamicUserId)
		{
			return false;
		}

		public void ValidateIntegration()
		{
		}

		public void LaunchTestSuite()
		{
		}

		public void SetAdaptersDebug(bool enabled)
		{
		}

		public void SetNetworkData(string networkKey, string networkData)
		{
		}

		public void SetMetaData(string key, string value)
		{
		}

		public void SetMetaData(string key, string[] values)
		{
		}

		public void SetConsent(bool consent)
		{
		}

		public void SetSegment(LevelPlaySegment segment)
		{
		}

		public void SetGDPRConsents(Dictionary<string, bool> networkConsents)
		{
		}

		public void SetCCPA(bool value)
		{
		}

		public void SetCOPPA(bool value)
		{
		}

		private static void OnInitializationSuccess(string configuration)
		{
		}

		private static void OnInitializationFailed(string error)
		{
		}

		private static void OnImpressionSuccess(string impressionData)
		{
		}
	}
}

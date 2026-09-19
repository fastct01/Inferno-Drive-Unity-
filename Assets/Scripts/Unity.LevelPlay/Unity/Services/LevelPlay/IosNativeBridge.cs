using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal class IosNativeBridge : IIosNativeBridge
	{
		public void LPMInitialize(string appKey, string userId, Action<string> initSuccessCallback, Action<string> initFailureCallback, Action<string> impressionSuccessCallback)
		{
		}

		public void SetPluginData(string pluginType, string pluginVersion, string pluginFrameworkVersion)
		{
		}

		public void LPMSetPauseGame(bool pause)
		{
		}

		public bool LPMSetDynamicUserId(string dynamicUserId)
		{
			return false;
		}

		public void LPMValidateIntegration()
		{
		}

		public void LPMLaunchTestSuite()
		{
		}

		public void LPMSetAdaptersDebug(bool enabled)
		{
		}

		public void LPMSetNetworkData(string networkKey, string networkData)
		{
		}

		public void LPMSetMetaData(string key, string value)
		{
		}

		public void LPMSetMetaDataWithValues(string key, string[] values)
		{
		}

		public void LPMSetConsent(bool consent)
		{
		}

		public void LPMSetSegment(string json)
		{
		}

		public void LPMSetGDPRConsents(string json)
		{
		}

		public void LPMSetCCPA(bool value)
		{
		}

		public void LPMSetCOPPA(bool value)
		{
		}

		[PreserveSig]
		private static extern void LPMInitializeNative(string appKey, string userId, Action<string> initSuccessCallback, Action<string> initFailureCallback, Action<string> impressionSuccessCallback);

		[PreserveSig]
		private static extern void setPluginDataNative(string pluginType, string pluginVersion, string pluginFrameworkVersion);

		[PreserveSig]
		private static extern void LPMSetPauseGameNative(bool pause);

		[PreserveSig]
		private static extern bool LPMSetDynamicUserIdNative(string dynamicUserId);

		[PreserveSig]
		private static extern void LPMValidateIntegrationNative();

		[PreserveSig]
		private static extern void LPMLaunchTestSuiteNative();

		[PreserveSig]
		private static extern void LPMSetAdaptersDebugNative(bool enabled);

		[PreserveSig]
		private static extern void LPMSetNetworkDataNative(string networkKey, string networkData);

		[PreserveSig]
		private static extern void LPMSetMetaDataNative(string key, string value);

		[PreserveSig]
		private static extern void LPMSetMetaDataWithValuesNative(string key, string[] values);

		[PreserveSig]
		private static extern void LPMSetConsentNative(bool consent);

		[PreserveSig]
		private static extern void LPMSetSegmentNative(string json);

		[PreserveSig]
		private static extern void LPMSetGDPRConsentsNative(string json);

		[PreserveSig]
		private static extern void LPMSetCCPANative(bool value);

		[PreserveSig]
		private static extern void LPMSetCOPPANative(bool value);
	}
}

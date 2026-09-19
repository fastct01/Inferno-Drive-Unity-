using System;

namespace Unity.Services.LevelPlay
{
	internal interface IIosNativeBridge
	{
		void LPMInitialize(string appKey, string userId, Action<string> initSuccessCallback, Action<string> initFailureCallback, Action<string> impressionSuccessCallback);

		void SetPluginData(string pluginType, string pluginVersion, string pluginFrameworkVersion);

		void LPMSetPauseGame(bool pause);

		bool LPMSetDynamicUserId(string dynamicUserId);

		void LPMValidateIntegration();

		void LPMLaunchTestSuite();

		void LPMSetAdaptersDebug(bool enabled);

		void LPMSetNetworkData(string networkKey, string networkData);

		void LPMSetMetaData(string key, string value);

		void LPMSetMetaDataWithValues(string key, string[] values);

		void LPMSetConsent(bool consent);

		void LPMSetSegment(string json);

		void LPMSetGDPRConsents(string json);

		void LPMSetCCPA(bool value);

		void LPMSetCOPPA(bool value);
	}
}

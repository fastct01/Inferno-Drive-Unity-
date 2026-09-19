using UnityEngine;

public class LevelPlayMediationSettings : ScriptableObject
{
	public static readonly string s_LevelPlaySettingsAssetPath;

	public bool EnableIronsourceSDKInitAPI;

	public string AndroidAppKey;

	public string IOSAppKey;

	public bool DeclareAD_IDPermission;

	public bool EnableAdapterDebug;

	public bool EnableIntegrationHelper;
}

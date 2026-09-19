using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

// Build entry points for the remake (tools/remake/build.sh):
//   Unity -batchmode -projectPath . -executeMethod InfernoBuild.ConfigurePlayerSettings -quit
//   Unity -batchmode -projectPath . -buildTarget iOS     -executeMethod InfernoBuild.BuildIOS     -quit
//   Unity -batchmode -projectPath . -buildTarget Android -executeMethod InfernoBuild.BuildAndroid -quit
// The values mirror the shipped iOS export (car точно финал: Info.plist, project.pbxproj, Podfile):
// bundle com.baalattackgroup.infernodrive, "Inferno Drive" 0.1.0 (1), iOS 15.0, IL2CPP, portrait only,
// URL scheme shadowrun, the magenta helmet racer icon (Icon-Store-1024.png).
public static class InfernoBuild
{
	private const string BundleId = "com.baalattackgroup.infernodrive";
	private const string IconPath = "Assets/AppIcon/Icon-Store-1024.png";

	public static void ConfigurePlayerSettings()
	{
		PlayerSettings.companyName = "BaalAttackGroup";
		PlayerSettings.productName = "Inferno Drive";
		PlayerSettings.bundleVersion = "0.1.0";
		PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, BundleId);
		PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, BundleId);
		PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
		PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
		PlayerSettings.iOS.buildNumber = "1";
		PlayerSettings.iOS.targetOSVersionString = "15.0";
		PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
		PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
		PlayerSettings.iOS.iOSUrlSchemes = new[] { "shadowrun" };
		PlayerSettings.iOS.hideHomeButton = false;
		PlayerSettings.statusBarHidden = true;
		PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
		PlayerSettings.allowedAutorotateToPortrait = true;
		PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
		PlayerSettings.allowedAutorotateToLandscapeLeft = false;
		PlayerSettings.allowedAutorotateToLandscapeRight = false;
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
		PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
		PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new[] { UnityEngine.Rendering.GraphicsDeviceType.Metal });
		PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
		var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
		if (icon != null)
		{
			// the legacy default icon (used by every platform whose module is not installed yet)
			PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { icon }, IconKind.Any);
			foreach (var target in new[] { NamedBuildTarget.iOS, NamedBuildTarget.Android })
			{
				try
				{
					foreach (var kind in PlayerSettings.GetSupportedIconKinds(target))
					{
						var icons = PlayerSettings.GetPlatformIcons(target, kind);
						foreach (var slot in icons) slot.SetTextures(Enumerable.Repeat(icon, slot.maxLayerCount).ToArray());
						PlayerSettings.SetPlatformIcons(target, kind, icons);
					}
				}
				catch (Exception e) { Debug.Log("[InfernoBuild] icons for " + target.TargetName + ": " + e.Message); }
			}
		}
		else Debug.LogWarning("[InfernoBuild] app icon missing at " + IconPath);
		AssetDatabase.SaveAssets();
		Debug.Log("[InfernoBuild] player settings configured");
	}

	private static string[] Scenes()
	{
		return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
	}

	public static void BuildIOS()
	{
		ConfigurePlayerSettings();
		string output = Path.GetFullPath("Builds/iOS");
		Directory.CreateDirectory(output);
		var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
		{
			scenes = Scenes(),
			locationPathName = output,
			target = BuildTarget.iOS,
			options = BuildOptions.None,
		});
		Debug.Log("[InfernoBuild] iOS " + report.summary.result + " -> " + output + " (" + report.summary.totalErrors + " errors)");
		if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) EditorApplication.Exit(1);
	}

	public static void BuildAndroid()
	{
		ConfigurePlayerSettings();
		string output = Path.GetFullPath("Builds/Android/InfernoDrive.apk");
		Directory.CreateDirectory(Path.GetDirectoryName(output));
		var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
		{
			scenes = Scenes(),
			locationPathName = output,
			target = BuildTarget.Android,
			options = BuildOptions.None,
		});
		Debug.Log("[InfernoBuild] Android " + report.summary.result + " -> " + output + " (" + report.summary.totalErrors + " errors)");
		if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) EditorApplication.Exit(1);
	}
}

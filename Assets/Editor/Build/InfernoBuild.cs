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

	// EDM4U (RevenueCat / LevelPlay dependency XMLs) patches mainTemplate.gradle only when its resolver
	// runs; in batch mode that is an explicit call (GvhProjectSettings.xml enables template patching so
	// Unity's own Gradle + JDK fetch the artifacts instead of EDM4U's bundled Gradle 5.1.1).
	private static void ResolveAndroidDependencies()
	{
		Type resolver = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("GooglePlayServices.PlayServicesResolver")).FirstOrDefault(t => t != null);
		if (resolver == null) { Debug.LogWarning("[InfernoBuild] EDM4U resolver not found"); return; }
		var method = resolver.GetMethod("MenuForceResolve", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
		if (method == null) { Debug.LogWarning("[InfernoBuild] PlayServicesResolver.MenuForceResolve not found"); return; }
		method.Invoke(null, null);
		Debug.Log("[InfernoBuild] Android dependencies resolved");
	}

	public static void BuildAndroid()
	{
		ConfigurePlayerSettings();
		ResolveAndroidDependencies();
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

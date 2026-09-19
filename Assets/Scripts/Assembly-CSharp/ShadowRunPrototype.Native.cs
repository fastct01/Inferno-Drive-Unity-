using System.Runtime.InteropServices;

// Native iOS plugin entry points (Libraries/Plugins/iOS/ShadowRunNativeAuth.mm in the shipped export).
// The original C# declared these with [DllImport("__Internal")]; Cpp2IL only preserved [PreserveSig].
// Outside an iOS player the symbols do not exist, so the editor/desktop builds get no-op fallbacks
// (see docs/remake-assumptions.md).
public sealed partial class ShadowRunPrototype
{
#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void ShadowRun_PlayImpactHaptic(int strength);

	[DllImport("__Internal")]
	private static extern void ShadowRun_ShowAccountConfirmation(int action, string unityGameObjectName);

	[DllImport("__Internal")]
	private static extern void ShadowRun_ConfigureGameAudioSession();

	[DllImport("__Internal")]
	private static extern int ShadowRun_IsOtherAudioPlaying();
#else
	private static void ShadowRun_PlayImpactHaptic(int strength) { }

	private static void ShadowRun_ShowAccountConfirmation(int action, string unityGameObjectName) { }

	private static void ShadowRun_ConfigureGameAudioSession() { }

	private static int ShadowRun_IsOtherAudioPlaying() { return 0; }
#endif
}

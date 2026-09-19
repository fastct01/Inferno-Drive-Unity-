using System.Runtime.InteropServices;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   InAppLegalBrowser_Open_mC59BD9F107C7CBED2BC5336D032C1CF69581AB9C
// Native: Libraries/Plugins/iOS/ShadowRunNativeAuth.mm (ShadowRun_OpenInAppBrowser).
namespace ShadowRun.Services
{
	public static class InAppLegalBrowser
	{
		public const string PrivacyPolicyUrl = "https://www.mett.space/privacy";

		public const string TermsOfServiceUrl = "https://www.mett.space/terms";

#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern void ShadowRun_OpenInAppBrowser(string url);
#else
		private static void ShadowRun_OpenInAppBrowser(string url)
		{
			Application.OpenURL(url);
		}
#endif

		public static void Open(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return;
			}
			ShadowRun_OpenInAppBrowser(url);
		}
	}
}

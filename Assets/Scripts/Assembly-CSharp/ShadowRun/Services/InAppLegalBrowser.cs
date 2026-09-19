using System.Runtime.InteropServices;

namespace ShadowRun.Services
{
	public static class InAppLegalBrowser
	{
		public const string PrivacyPolicyUrl = "https://www.mett.space/privacy";

		public const string TermsOfServiceUrl = "https://www.mett.space/terms";

		[PreserveSig]
		private static extern void ShadowRun_OpenInAppBrowser(string url);

		public static void Open(string url)
		{
		}
	}
}

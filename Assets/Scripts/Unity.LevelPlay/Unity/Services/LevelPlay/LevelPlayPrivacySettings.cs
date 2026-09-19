using System.Collections.Generic;

namespace Unity.Services.LevelPlay
{
	public static class LevelPlayPrivacySettings
	{
		private static ILevelPlaySdk Sdk => null;

		public static void SetGDPRConsent(bool consent)
		{
		}

		public static void SetGDPRConsents(Dictionary<string, bool> networkConsents)
		{
		}

		public static void SetCCPA(bool value)
		{
		}

		public static void SetCOPPA(bool value)
		{
		}
	}
}

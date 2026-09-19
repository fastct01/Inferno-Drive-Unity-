using System;

namespace Unity.Services.LevelPlay
{
	internal static class LevelPlaySdkProvider
	{
		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9;

			public static Func<ILevelPlaySdk> _003C_003E9__6_0;

			internal ILevelPlaySdk _003CReset_003Eb__6_0()
			{
				return null;
			}
		}

		private static Func<ILevelPlaySdk> Factory__BackingField;

		internal static Func<ILevelPlaySdk> Factory
		{
			get
			{
				return Factory__BackingField;
			}
			set
			{
				Factory__BackingField = value;
			}
		}

		static LevelPlaySdkProvider()
		{
		}

		internal static ILevelPlaySdk Get()
		{
			return null;
		}

		internal static void Reset()
		{
		}
	}
}

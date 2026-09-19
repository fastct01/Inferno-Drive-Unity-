namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayAdSize
	{
		private IPlatformLevelPlayAdSize m_PlatformLevelPlayAdSize;

		public static LevelPlayAdSize BANNER;

		public static LevelPlayAdSize LARGE;

		public static LevelPlayAdSize MEDIUM_RECTANGLE;

		public static LevelPlayAdSize LEADERBOARD;

		public string Description => null;

		public int Width => 0;

		public int Height => 0;

		internal IPlatformLevelPlayAdSize GetPlatformLevelPlayAdSize()
		{
			return null;
		}

		internal LevelPlayAdSize()
		{
		}

		internal LevelPlayAdSize(IPlatformLevelPlayAdSize adSize)
		{
		}

		private LevelPlayAdSize(PlatformLevelPlayAdSizeType adSizeType)
		{
		}

		private LevelPlayAdSize(int width, int height)
		{
		}

		public static LevelPlayAdSize CreateCustomBannerSize(int width, int height)
		{
			return null;
		}

		public static LevelPlayAdSize CreateAdaptiveAdSize(int customWidth = -1)
		{
			return null;
		}

		public override string ToString()
		{
			return null;
		}
	}
}

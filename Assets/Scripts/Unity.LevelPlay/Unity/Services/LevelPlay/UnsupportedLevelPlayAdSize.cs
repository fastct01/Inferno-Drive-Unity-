namespace Unity.Services.LevelPlay
{
	internal sealed class UnsupportedLevelPlayAdSize : IPlatformLevelPlayAdSize
	{
		public int Width => 0;

		public int Height => 0;

		public PlatformLevelPlayAdSizeType AdSizeType => default;

		internal UnsupportedLevelPlayAdSize()
		{
		}
	}
}

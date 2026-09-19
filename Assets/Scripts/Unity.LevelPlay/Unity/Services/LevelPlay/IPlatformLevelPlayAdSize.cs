namespace Unity.Services.LevelPlay
{
	internal interface IPlatformLevelPlayAdSize
	{
		int Width { get; }

		int Height { get; }

		PlatformLevelPlayAdSizeType AdSizeType { get; }
	}
}

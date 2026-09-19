using UnityEngine;

namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayBannerPosition
	{
		private enum Presets
		{
			TopLeft = 0,
			TopCenter = 1,
			TopRight = 2,
			CenterLeft = 3,
			Center = 4,
			CenterRight = 5,
			BottomLeft = 6,
			BottomCenter = 7,
			BottomRight = 8,
			Custom = 9
		}

		internal readonly Vector2 Position;

		public readonly string Description;

		public static readonly LevelPlayBannerPosition TopLeft;

		public static readonly LevelPlayBannerPosition TopCenter;

		public static readonly LevelPlayBannerPosition TopRight;

		public static readonly LevelPlayBannerPosition CenterLeft;

		public static readonly LevelPlayBannerPosition Center;

		public static readonly LevelPlayBannerPosition CenterRight;

		public static readonly LevelPlayBannerPosition BottomLeft;

		public static readonly LevelPlayBannerPosition BottomCenter;

		public static readonly LevelPlayBannerPosition BottomRight;

		private LevelPlayBannerPosition(Presets presets, Vector2 position = default(Vector2))
		{
		}

		public LevelPlayBannerPosition(Vector2 position)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}
}

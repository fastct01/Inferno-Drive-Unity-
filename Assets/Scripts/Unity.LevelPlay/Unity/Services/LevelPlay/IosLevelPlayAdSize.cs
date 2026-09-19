using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosLevelPlayAdSize : IosNativeObject, IPlatformLevelPlayAdSize
	{
		private PlatformLevelPlayAdSizeType m_PlatformLevelPlayAdSizeType;

		public int Width => 0;

		public int Height => 0;

		public PlatformLevelPlayAdSizeType AdSizeType => default;

		internal IosLevelPlayAdSize(PlatformLevelPlayAdSizeType adSizeType)
			: base(usesCallbacks: false)
		{
		}

		internal IosLevelPlayAdSize(int width, int height)
			: base(usesCallbacks: false)
		{
		}

		private IosLevelPlayAdSize(PlatformLevelPlayAdSizeType adSizeType, IntPtr nativePtr)
			: base(usesCallbacks: false)
		{
		}

		internal static IosLevelPlayAdSize CreateAdaptiveAdSize()
		{
			return null;
		}

		internal static IosLevelPlayAdSize CreateAdaptiveAdSize(int width)
		{
			return null;
		}

		[PreserveSig]
		private static extern IntPtr LPMCreateAdSizeWithType(PlatformLevelPlayAdSizeType adSizeType, int width, int height);

		[PreserveSig]
		private static extern IntPtr LPMCreateAdaptiveAdSize();

		[PreserveSig]
		private static extern IntPtr LPMCreateAdaptiveAdSizeWithWidth(int width);

		[PreserveSig]
		private static extern int LPMGetAdSizeWidth(IntPtr adSize);

		[PreserveSig]
		private static extern int LPMGetAdSizeHeight(IntPtr adSize);
	}
}

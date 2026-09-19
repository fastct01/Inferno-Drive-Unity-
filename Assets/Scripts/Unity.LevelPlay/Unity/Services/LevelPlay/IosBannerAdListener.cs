using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosBannerAdListener : IosNativeObject
	{
		private delegate void DidLoadAdWithAdInfo(IntPtr bannerPtr, string adInfoJson);

		private delegate void DidFailToLoadAdWithAdUnitId(IntPtr bannerPtr, string errorPtr);

		private delegate void DidClickWithAdInfo(IntPtr bannerPtr, string adInfoJson);

		private delegate void DidDisplayWithAdInfo(IntPtr bannerPtr, string adInfoJson);

		private delegate void DidFailToDisplayWithAdInfo(IntPtr bannerPtr, string adInfoJson, string errorPtr);

		private delegate void DidExpandAdWithAdInfo(IntPtr bannerPtr, string adInfoJson);

		private delegate void DidCollapseAdWithAdInfo(IntPtr bannerPtr, string adInfoJson);

		private delegate void DidLeaveAppWithAdInfo(IntPtr bannerPtr, string adInfoJson);

		private readonly DidLoadAdWithAdInfo _kLoadSuccessCallback;

		private readonly DidFailToLoadAdWithAdUnitId _kFailedToLoad;

		private readonly DidClickWithAdInfo _kClicked;

		private readonly DidDisplayWithAdInfo _kDisplayed;

		private readonly DidFailToDisplayWithAdInfo _kDisplayFailed;

		private readonly DidExpandAdWithAdInfo _kExpanded;

		private readonly DidCollapseAdWithAdInfo _kCollapsed;

		private readonly DidLeaveAppWithAdInfo _kLeftApp;

		private IosBannerAd _bannerAd;

		public IosBannerAdListener(IosBannerAd bannerAd)
			: base(usesCallbacks: false)
		{
		}

		public override void Dispose()
		{
		}

		[PreserveSig]
		private static extern IntPtr LPMBannerAdViewDelegateCreate(IntPtr bannerAdPtr, DidLoadAdWithAdInfo loadSuccess, DidFailToLoadAdWithAdUnitId loadFailed, DidClickWithAdInfo clicked, DidDisplayWithAdInfo displayed, DidFailToDisplayWithAdInfo displayFailed, DidExpandAdWithAdInfo expanded, DidCollapseAdWithAdInfo collapsed, DidLeaveAppWithAdInfo leftApp);

		[PreserveSig]
		private static extern void LPMBannerAdViewDelegateDestroy(IntPtr delegatePtr);

		private static void LoadSuccess(IntPtr ptr, string adInfoJson)
		{
		}

		private static void LoadFailed(IntPtr ptr, string errorPtr)
		{
		}

		private static void Clicked(IntPtr ptr, string adInfoJson)
		{
		}

		private static void Displayed(IntPtr ptr, string adInfoJson)
		{
		}

		private static void DisplayFailed(IntPtr ptr, string adInfoJson, string errorPtr)
		{
		}

		private static void Expanded(IntPtr ptr, string adInfoJson)
		{
		}

		private static void Collapsed(IntPtr ptr, string adInfoJson)
		{
		}

		private static void LeftApplication(IntPtr ptr, string adInfoJson)
		{
		}
	}
}

using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosRewardedAdListener : IosNativeObject
	{
		private delegate void DidLoadAdWithAdInfo(IntPtr rewardedAd, string adInfoJson);

		private delegate void DidFailToLoadAdWithAdUnitId(IntPtr rewardedAd, string errorPtr);

		private delegate void DidDisplayWithAdInfo(IntPtr rewardedAd, string adInfoJson);

		private delegate void DidFailToDisplayWithAdInfo(IntPtr rewardedAd, string adInfoJson, string errorPtr);

		private delegate void DidAdRewardedWithAdInfo(IntPtr rewardedAd, string adInfoJson, string rewardName, int rewardAmount);

		private delegate void DidClickWithAdInfo(IntPtr rewardedAd, string adInfoJson);

		private delegate void DidCloseWithAdInfo(IntPtr rewardedAd, string adInfoJson);

		private delegate void DidChangeAdInfo(IntPtr rewardedAd, string adInfoJson);

		private readonly DidLoadAdWithAdInfo k_LoadSuccessCallback;

		private readonly DidFailToLoadAdWithAdUnitId k_FailedToLoadCallback;

		private readonly DidDisplayWithAdInfo k_DisplayedCallback;

		private readonly DidFailToDisplayWithAdInfo k_DisplayFailedCallback;

		private readonly DidAdRewardedWithAdInfo k_RewardedCallback;

		private readonly DidClickWithAdInfo k_ClickedCallback;

		private readonly DidCloseWithAdInfo k_ClosedCallback;

		private readonly DidChangeAdInfo k_AdInfoChangedCallback;

		private IosRewardedAd _mRewardedAd;

		internal IosRewardedAdListener(IosRewardedAd rewardedAd)
			: base(usesCallbacks: false)
		{
		}

		[PreserveSig]
		private static extern IntPtr LPMRewardedAdDelegateCreate(IntPtr rewardedAd, DidLoadAdWithAdInfo loadSuccess, DidFailToLoadAdWithAdUnitId loadFailed, DidDisplayWithAdInfo displayed, DidFailToDisplayWithAdInfo displayFailed, DidAdRewardedWithAdInfo rewarded, DidClickWithAdInfo clicked, DidCloseWithAdInfo closed, DidChangeAdInfo adInfoChanged);

		[PreserveSig]
		private static extern void LPMRewardedAdDelegateDestroy(IntPtr delegatePtr);

		private static void Loaded(IntPtr ptr, string adInfoJson)
		{
		}

		private static void LoadFailed(IntPtr ptr, string errorPtr)
		{
		}

		private static void Displayed(IntPtr ptr, string adInfoJson)
		{
		}

		private static void DisplayFailed(IntPtr ptr, string adInfoJson, string errorPtr)
		{
		}

		private static void Rewarded(IntPtr ptr, string adInfoJson, string rewardName, int rewardAmount)
		{
		}

		private static void Clicked(IntPtr ptr, string adInfoJson)
		{
		}

		private static void Closed(IntPtr ptr, string adInfoJson)
		{
		}

		private static void AdInfoChanged(IntPtr ptr, string adInfoJson)
		{
		}
	}
}

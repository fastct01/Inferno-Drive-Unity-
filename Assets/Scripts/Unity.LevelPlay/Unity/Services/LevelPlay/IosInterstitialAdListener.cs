using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosInterstitialAdListener : IosNativeObject
	{
		private delegate void DidLoadAdWithAdInfo(IntPtr interstitialAd, string adInfoJson);

		private delegate void DidFailToLoadAdWithAdUnitId(IntPtr interstitialAd, string errorPtr);

		private delegate void DidDisplayWithAdInfo(IntPtr interstitialAd, string adInfoJson);

		private delegate void DidFailToDisplayWithAdInfo(IntPtr interstitialAd, string adInfoJson, string errorPtr);

		private delegate void DidClickWithAdInfo(IntPtr interstitialAd, string adInfoJson);

		private delegate void DidCloseWithAdInfo(IntPtr interstitialAd, string adInfoJson);

		private delegate void DidChangeAdInfo(IntPtr interstitialAd, string adInfoJson);

		private readonly DidLoadAdWithAdInfo k_LoadSuccessCallback;

		private readonly DidFailToLoadAdWithAdUnitId k_FailedToLoadCallback;

		private readonly DidDisplayWithAdInfo k_DisplayedCallback;

		private readonly DidFailToDisplayWithAdInfo k_DisplayFailedCallback;

		private readonly DidClickWithAdInfo k_ClickedCallback;

		private readonly DidCloseWithAdInfo k_ClosedCallback;

		private readonly DidChangeAdInfo k_AdInfoChangedCallback;

		private IosInterstitialAd m_InterstitialAd;

		internal IosInterstitialAdListener(IosInterstitialAd interstitialAd)
			: base(usesCallbacks: false)
		{
		}

		[PreserveSig]
		private static extern IntPtr LPMInterstitialAdDelegateCreate(IntPtr interstitialAd, DidLoadAdWithAdInfo loadSuccess, DidFailToLoadAdWithAdUnitId loadFailed, DidDisplayWithAdInfo displayed, DidFailToDisplayWithAdInfo displayFailed, DidClickWithAdInfo clicked, DidCloseWithAdInfo closed, DidChangeAdInfo adInfoChanged);

		[PreserveSig]
		private static extern void LPMInterstitialAdDelegateDestroy(IntPtr delegatePtr);

		private static void Loaded(IntPtr ptr, string adInfoJson)
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

		private static void Closed(IntPtr ptr, string adInfoJson)
		{
		}

		private static void AdInfoChanged(IntPtr ptr, string adInfoJson)
		{
		}
	}
}

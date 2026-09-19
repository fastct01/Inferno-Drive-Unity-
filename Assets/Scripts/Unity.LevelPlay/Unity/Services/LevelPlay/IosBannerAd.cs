using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosBannerAd : IosNativeObject, IPlatformBannerAd, IDisposable, IIosImpressionDataReceiver
	{
		internal class Config : IPlatformBannerAd.IConfig
		{
			internal class Builder : IPlatformBannerAd.IConfigBuilder
			{
				private LevelPlayAdSize _adSize;

				private LevelPlayBannerPosition _position;

				private string _placementName;

				private bool _displayOnLoad;

				private readonly IntPtr m_Builder;

				public void SetBidFloor(double bidFloor)
				{
				}

				public void SetSize(LevelPlayAdSize size)
				{
				}

				public void SetPosition(LevelPlayBannerPosition position)
				{
				}

				public void SetPlacementName(string placementName)
				{
				}

				public void SetDisplayOnLoad(bool displayOnLoad)
				{
				}

				public void SetRespectSafeArea(bool respectSafeArea)
				{
				}

				public IPlatformBannerAd.IConfig Build()
				{
					return null;
				}

				[PreserveSig]
				private static extern IntPtr BannerAdCreateConfigBuilder();

				[PreserveSig]
				private static extern IntPtr BannerAdConfigBuilderSetBidFloor(IntPtr builder, double bidFloor);

				[PreserveSig]
				private static extern IntPtr BannerAdConfigBuilderSetSize(IntPtr builder, IntPtr size);

				[PreserveSig]
				private static extern IntPtr BannerAdConfigBuilderSetPlacementName(IntPtr builder, string placementName);

				[PreserveSig]
				private static extern IntPtr BannerAdConfigBuilderBuild(IntPtr builder);
			}

			private readonly LevelPlayAdSize AdSize__BackingField;

			private readonly LevelPlayBannerPosition Position__BackingField;

			private readonly string PlacementName__BackingField;

			private readonly bool DisplayOnLoad__BackingField;

			private readonly IntPtr IosConfig__BackingField;

			internal LevelPlayAdSize AdSize => AdSize__BackingField;

			internal LevelPlayBannerPosition Position => Position__BackingField;

			internal string PlacementName => PlacementName__BackingField;

			internal bool DisplayOnLoad => DisplayOnLoad__BackingField;

			internal IntPtr IosConfig => IosConfig__BackingField;

			private Config(LevelPlayAdSize adSize, LevelPlayBannerPosition position, string placementName, bool displayOnLoad, IntPtr iosConfig)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass54_0
		{
			public IosBannerAd _003C_003E4__this;

			public LevelPlayAdInfo adInfo;

			internal void _003CInvokeLoadedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass55_0
		{
			public IosBannerAd _003C_003E4__this;

			public LevelPlayAdError error;

			internal void _003CInvokeFailedLoadEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass56_0
		{
			public IosBannerAd _003C_003E4__this;

			public LevelPlayAdInfo adInfo;

			internal void _003CInvokeClickedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass57_0
		{
			public IosBannerAd _003C_003E4__this;

			public LevelPlayAdInfo adInfo;

			internal void _003CInvokeDisplayedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass58_0
		{
			public IosBannerAd _003C_003E4__this;

			public LevelPlayAdInfo adInfo;

			public LevelPlayAdError error;

			internal void _003CInvokeFailedDisplayEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass59_0
		{
			public IosBannerAd _003C_003E4__this;

			public LevelPlayAdInfo adInfo;

			internal void _003CInvokeExpandedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass60_0
		{
			public IosBannerAd _003C_003E4__this;

			public LevelPlayAdInfo adInfo;

			internal void _003CInvokeCollapsedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass61_0
		{
			public IosBannerAd _003C_003E4__this;

			public LevelPlayAdInfo adInfo;

			internal void _003CInvokeLeftApplicationEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass62_0
		{
			public IosBannerAd _003C_003E4__this;

			public string impressionDataJson;

			internal void _003CInvokeImpressionDataReadyEvent_003Eb__0(object state)
			{
			}
		}

		private Action<LevelPlayAdInfo> m_OnAdLoaded;

		private Action<LevelPlayAdError> m_OnAdLoadFailed;

		private Action<LevelPlayAdInfo> m_OnAdClicked;

		private Action<LevelPlayAdInfo> m_OnAdDisplayed;

		private Action<LevelPlayAdInfo, LevelPlayAdError> m_OnAdDisplayFailed;

		private Action<LevelPlayAdInfo> m_OnAdExpanded;

		private Action<LevelPlayAdInfo> m_OnAdCollapsed;

		private Action<LevelPlayAdInfo> m_OnAdLeftApplication;

		private Action<LevelPlayImpressionData> m_OnAdImpressionDataReady;

		private readonly string AdUnitId__BackingField;

		private readonly LevelPlayAdSize AdSize__BackingField;

		private readonly string PlacementName__BackingField;

		private readonly LevelPlayBannerPosition Position__BackingField;

		private readonly bool DisplayOnLoad__BackingField;

		private IosBannerAdListener _mBannerAdListener;

		private IosImpressionDataDelegateListener _mImpressionListener;

		public string AdUnitId => AdUnitId__BackingField;

		public LevelPlayAdSize AdSize => AdSize__BackingField;

		public string PlacementName => PlacementName__BackingField;

		public LevelPlayBannerPosition Position => Position__BackingField;

		private bool DisplayOnLoad => DisplayOnLoad__BackingField;

		public string AdId => null;

		public event Action<LevelPlayAdInfo> OnAdLoaded
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdError> OnAdLoadFailed
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdInfo> OnAdClicked
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdInfo> OnAdDisplayed
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdInfo, LevelPlayAdError> OnAdDisplayFailed
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdInfo> OnAdExpanded
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdInfo> OnAdCollapsed
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdInfo> OnAdLeftApplication
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayImpressionData> OnAdImpressionDataReady
		{
			add
			{
			}
			remove
			{
			}
		}

		internal IosBannerAd(string adUnitId, Config config)
			: base(usesCallbacks: false)
		{
		}

		public void PauseAutoRefresh()
		{
		}

		public void ResumeAutoRefresh()
		{
		}

		public void LoadAd()
		{
		}

		public void DestroyAd()
		{
		}

		public void SetPosition()
		{
		}

		public void ShowAd()
		{
		}

		public void HideAd()
		{
		}

		internal void InvokeLoadedEvent(LevelPlayAdInfo adInfo)
		{
		}

		internal void InvokeFailedLoadEvent(LevelPlayAdError error)
		{
		}

		internal void InvokeClickedEvent(LevelPlayAdInfo adInfo)
		{
		}

		internal void InvokeDisplayedEvent(LevelPlayAdInfo adInfo)
		{
		}

		internal void InvokeFailedDisplayEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
		{
		}

		internal void InvokeExpandedEvent(LevelPlayAdInfo adInfo)
		{
		}

		internal void InvokeCollapsedEvent(LevelPlayAdInfo adInfo)
		{
		}

		internal void InvokeLeftApplicationEvent(LevelPlayAdInfo adInfo)
		{
		}

		public void InvokeImpressionDataReadyEvent(string impressionDataJson)
		{
		}

		private string GetAdId()
		{
			return null;
		}

		[PreserveSig]
		private static extern IntPtr BannerAdCreate(string adUnitId, IntPtr config, IntPtr adSize);

		[PreserveSig]
		private static extern void BannerAdSetDelegate(IntPtr bannerAdView, IntPtr bannerAdListener);

		[PreserveSig]
		private static extern void BannerAdSetImpressionDataDelegate(IntPtr bannerAdView, IntPtr impressionDataDelegate);

		[PreserveSig]
		private static extern void BannerAdLoad(IntPtr bannerAdView);

		[PreserveSig]
		private static extern void BannerAdDestroy(IntPtr bannerAdView);

		[PreserveSig]
		private static extern void BannerAdSetPosition(IntPtr bannerAdView, string position, float x, float y);

		[PreserveSig]
		private static extern void BannerAdViewShow(IntPtr bannerAdView);

		[PreserveSig]
		private static extern void BannerAdViewHide(IntPtr bannerAdView);

		[PreserveSig]
		private static extern void BannerAdPauseAutoRefresh(IntPtr bannerAdView);

		[PreserveSig]
		private static extern void BannerAdResumeAutoRefresh(IntPtr bannerAdView);

		[PreserveSig]
		private static extern string BannerAdId(IntPtr bannerAdView);
	}
}

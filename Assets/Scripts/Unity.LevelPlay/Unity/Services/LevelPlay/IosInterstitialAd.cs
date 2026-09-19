using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosInterstitialAd : IosNativeObject, IPlatformInterstitialAd, IDisposable, IIosImpressionDataReceiver
	{
		internal class Config : IPlatformInterstitialAd.IConfig
		{
			internal class Builder : IPlatformInterstitialAd.IConfigBuilder
			{
				private readonly IntPtr m_Builder;

				public void SetBidFloor(double bidFloor)
				{
				}

				public IPlatformInterstitialAd.IConfig Build()
				{
					return null;
				}

				[PreserveSig]
				private static extern IntPtr InterstitialAdCreateConfigBuilder();

				[PreserveSig]
				private static extern IntPtr InterstitialConfigBuilderSetBidFloor(IntPtr builder, double bidFloor);

				[PreserveSig]
				private static extern IntPtr InterstitialConfigBuilderBuild(IntPtr builder);
			}

			private readonly IntPtr IosConfig__BackingField;

			internal IntPtr IosConfig => IosConfig__BackingField;

			private Config(IntPtr iosConfig)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass37_0
		{
			public IosInterstitialAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeLoadedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass38_0
		{
			public IosInterstitialAd _003C_003E4__this;

			public string error;

			internal void _003CInvokeFailedLoadEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass39_0
		{
			public IosInterstitialAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeClickedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass40_0
		{
			public IosInterstitialAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeDisplayedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass41_0
		{
			public IosInterstitialAd _003C_003E4__this;

			public string adInfo;

			public string error;

			internal void _003CInvokeFailedDisplayEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass42_0
		{
			public IosInterstitialAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeClosedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass43_0
		{
			public IosInterstitialAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeOnAdInfoChangedEvent_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass44_0
		{
			public IosInterstitialAd _003C_003E4__this;

			public string impressionDataJson;

			internal void _003CInvokeImpressionDataReadyEvent_003Eb__0(object state)
			{
			}
		}

		private Action<LevelPlayAdInfo> m_OnAdLoaded;

		private Action<LevelPlayAdError> m_OnAdLoadFailed;

		private Action<LevelPlayAdInfo> m_OnAdDisplayed;

		private Action<LevelPlayAdInfo, LevelPlayAdError> m_OnAdDisplayFailed;

		private Action<LevelPlayAdInfo> m_OnAdClicked;

		private Action<LevelPlayAdInfo> m_OnAdClosed;

		private Action<LevelPlayAdInfo> m_OnAdInfoChanged;

		private Action<LevelPlayImpressionData> m_OnAdImpressionDataReady;

		private readonly string AdUnitId__BackingField;

		private IosInterstitialAdListener m_InterstitialListener;

		private IosImpressionDataDelegateListener m_ImpressionListener;

		public string AdUnitId => AdUnitId__BackingField;

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

		public event Action<LevelPlayAdInfo> OnAdClicked
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdInfo> OnAdClosed
		{
			add
			{
			}
			remove
			{
			}
		}

		public event Action<LevelPlayAdInfo> OnAdInfoChanged
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

		public IosInterstitialAd(string adUnitId, Config config)
			: base(usesCallbacks: false)
		{
		}

		public void LoadAd()
		{
		}

		public void ShowAd(string placementName)
		{
		}

		public bool IsAdReady()
		{
			return false;
		}

		public static bool IsPlacementCapped(string placementName)
		{
			return false;
		}

		public override void Dispose()
		{
		}

		internal void InvokeLoadedEvent(string adInfo)
		{
		}

		internal void InvokeFailedLoadEvent(string error)
		{
		}

		internal void InvokeClickedEvent(string adInfo)
		{
		}

		internal void InvokeDisplayedEvent(string adInfo)
		{
		}

		internal void InvokeFailedDisplayEvent(string adInfo, string error)
		{
		}

		internal void InvokeClosedEvent(string adInfo)
		{
		}

		internal void InvokeOnAdInfoChangedEvent(string adInfo)
		{
		}

		public void InvokeImpressionDataReadyEvent(string impressionDataJson)
		{
		}

		private string GetAdId()
		{
			return null;
		}

		~IosInterstitialAd()
		{
		}

		[PreserveSig]
		private static extern IntPtr InterstitialAdCreate(string adUnitId, IntPtr config);

		[PreserveSig]
		private static extern void InterstitialAdSetDelegate(IntPtr interstitialAd, IntPtr interstitialAdListener);

		[PreserveSig]
		private static extern void InterstitialAdSetImpressionDataDelegate(IntPtr interstitialAd, IntPtr impressionDataDelegate);

		[PreserveSig]
		private static extern void InterstitialAdLoadAd(IntPtr interstitialAd);

		[PreserveSig]
		private static extern void InterstitialAdShowAd(IntPtr interstitialAd, string placementName);

		[PreserveSig]
		private static extern bool InterstitialAdIsAdReady(IntPtr interstitialAd);

		[PreserveSig]
		private static extern bool InterstitialAdIsPlacementCapped(string placementName);

		[PreserveSig]
		private static extern string InterstitialAdId(IntPtr interstitialAd);
	}
}

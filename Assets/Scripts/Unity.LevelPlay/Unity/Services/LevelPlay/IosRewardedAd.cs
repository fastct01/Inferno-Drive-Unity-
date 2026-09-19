using System;
using System.Runtime.InteropServices;

namespace Unity.Services.LevelPlay
{
	internal sealed class IosRewardedAd : IosNativeObject, IPlatformRewardedAd, IDisposable, IIosImpressionDataReceiver
	{
		internal class Config : IPlatformRewardedAd.IConfig
		{
			internal class Builder : IPlatformRewardedAd.IConfigBuilder
			{
				private readonly IntPtr m_Builder;

				public void SetBidFloor(double bidFloor)
				{
				}

				public IPlatformRewardedAd.IConfig Build()
				{
					return null;
				}

				[PreserveSig]
				private static extern IntPtr RewardedAdCreateConfigBuilder();

				[PreserveSig]
				private static extern IntPtr RewardedConfigBuilderSetBidFloor(IntPtr builder, double bidFloor);

				[PreserveSig]
				private static extern IntPtr RewardedConfigBuilderBuild(IntPtr builder);
			}

			private readonly IntPtr IosConfig__BackingField;

			internal IntPtr IosConfig => IosConfig__BackingField;

			private Config(IntPtr iosConfig)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass41_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeLoadedEvent_003Eb__0(object _)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass42_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string error;

			internal void _003CInvokeFailedLoadEvent_003Eb__0(object _)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass43_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeDisplayedEvent_003Eb__0(object _)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass44_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string adInfo;

			public string error;

			internal void _003CInvokeFailedDisplayEvent_003Eb__0(object _)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass45_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string adInfo;

			public string rewardName;

			public int rewardAmount;

			internal void _003CInvokeRewardedEvent_003Eb__0(object _)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass46_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeClickedEvent_003Eb__0(object _)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass47_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeClosedEvent_003Eb__0(object _)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass48_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string adInfo;

			internal void _003CInvokeOnAdInfoChangedEvent_003Eb__0(object _)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass49_0
		{
			public IosRewardedAd _003C_003E4__this;

			public string impressionDataJson;

			internal void _003CInvokeImpressionDataReadyEvent_003Eb__0(object _)
			{
			}
		}

		private Action<LevelPlayAdInfo> m_OnAdLoaded;

		private Action<LevelPlayAdError> m_OnAdLoadFailed;

		private Action<LevelPlayAdInfo> m_OnAdDisplayed;

		private Action<LevelPlayAdInfo, LevelPlayAdError> m_OnAdDisplayFailed;

		private Action<LevelPlayAdInfo, LevelPlayReward> m_OnAdRewarded;

		private Action<LevelPlayAdInfo> m_OnAdClicked;

		private Action<LevelPlayAdInfo> m_OnAdClosed;

		private Action<LevelPlayAdInfo> m_OnAdInfoChanged;

		private Action<LevelPlayImpressionData> m_OnAdImpressionDataReady;

		private readonly string AdUnitId__BackingField;

		private IosRewardedAdListener m_RewardedAdListener;

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

		public event Action<LevelPlayAdInfo, LevelPlayReward> OnAdRewarded
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

		public IosRewardedAd(string adUnitId, Config config)
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

		public LevelPlayReward GetReward(string placement)
		{
			return null;
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

		internal void InvokeDisplayedEvent(string adInfo)
		{
		}

		internal void InvokeFailedDisplayEvent(string adInfo, string error)
		{
		}

		internal void InvokeRewardedEvent(string adInfo, string rewardName, int rewardAmount)
		{
		}

		internal void InvokeClickedEvent(string adInfo)
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

		~IosRewardedAd()
		{
		}

		[PreserveSig]
		private static extern IntPtr RewardedAdCreate(string adUnitId, IntPtr config);

		[PreserveSig]
		private static extern void RewardedAdSetDelegate(IntPtr rewardedAd, IntPtr rewardedAdListener);

		[PreserveSig]
		private static extern void RewardedAdSetImpressionDataDelegate(IntPtr rewardedAd, IntPtr impressionDataDelegate);

		[PreserveSig]
		private static extern void RewardedAdLoadAd(IntPtr rewardedAd);

		[PreserveSig]
		private static extern void RewardedAdShowAd(IntPtr rewardedAd, string placementName);

		[PreserveSig]
		private static extern bool RewardedAdIsAdReady(IntPtr rewardedAd);

		[PreserveSig]
		private static extern bool RewardedAdIsPlacementCapped(string placementName);

		[PreserveSig]
		private static extern string RewardedAdId(IntPtr rewardedAd);

		[PreserveSig]
		private static extern IntPtr RewardedAdGetReward(IntPtr rewardedAd, string placement);

		[PreserveSig]
		private static extern string RewardedAdRewardGetName(IntPtr reward);

		[PreserveSig]
		private static extern int RewardedAdRewardGetAmount(IntPtr reward);
	}
}

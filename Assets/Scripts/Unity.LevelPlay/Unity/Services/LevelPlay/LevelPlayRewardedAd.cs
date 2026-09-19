using System;

namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayRewardedAd : ILevelPlayRewardedAd, IDisposable
	{
		public class Config
		{
			public class Builder
			{
				private readonly IPlatformRewardedAd.IConfigBuilder m_Builder;

				public Builder SetBidFloor(double bidFloor)
				{
					return null;
				}

				public Config Build()
				{
					return null;
				}
			}

			private readonly IPlatformRewardedAd.IConfig PlatformConfig__BackingField;

			internal IPlatformRewardedAd.IConfig PlatformConfig => PlatformConfig__BackingField;

			private Config(IPlatformRewardedAd.IConfig platformConfig)
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

		private readonly IPlatformRewardedAd m_RewardedAd;

		public string AdUnitId => null;

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

		public LevelPlayRewardedAd(string adUnitId, Config config = null)
		{
		}

		private void SetupEvents()
		{
		}

		internal LevelPlayRewardedAd(IPlatformRewardedAd platformRewardedAd)
		{
		}

		public void LoadAd()
		{
		}

		public void ShowAd(string placementName = null)
		{
		}

		public void DestroyAd()
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

		public void Dispose()
		{
		}

		public string GetAdId()
		{
			return null;
		}

		public LevelPlayReward GetReward(string placement = null)
		{
			return null;
		}

		private void _003CSetupEvents_003Eb__31_0(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__31_1(LevelPlayAdError error)
		{
		}

		private void _003CSetupEvents_003Eb__31_2(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__31_3(LevelPlayAdInfo info, LevelPlayAdError error)
		{
		}

		private void _003CSetupEvents_003Eb__31_4(LevelPlayAdInfo info, LevelPlayReward reward)
		{
		}

		private void _003CSetupEvents_003Eb__31_5(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__31_6(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__31_7(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__31_8(LevelPlayImpressionData data)
		{
		}
	}
}

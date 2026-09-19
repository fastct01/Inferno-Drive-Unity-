using System;

namespace Unity.Services.LevelPlay
{
	internal sealed class UnsupportedRewardedAd : IPlatformRewardedAd, IDisposable
	{
		internal class Config : IPlatformRewardedAd.IConfig
		{
			internal class Builder : IPlatformRewardedAd.IConfigBuilder
			{
				public void SetBidFloor(double bidFloor)
				{
				}

				public IPlatformRewardedAd.IConfig Build()
				{
					return null;
				}
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

		private readonly string AdId__BackingField;

		private readonly string AdUnitId__BackingField;

		public string AdId => AdId__BackingField;

		public string AdUnitId => AdUnitId__BackingField;

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

		public void Dispose()
		{
		}

		public UnsupportedRewardedAd(string adUnitId)
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

		public LevelPlayReward GetReward(string placement)
		{
			return null;
		}
	}
}

using System;

namespace Unity.Services.LevelPlay
{
	internal sealed class UnsupportedInterstitialAd : IPlatformInterstitialAd, IDisposable
	{
		internal class Config : IPlatformInterstitialAd.IConfig
		{
			internal class Builder : IPlatformInterstitialAd.IConfigBuilder
			{
				public void SetBidFloor(double bidFloor)
				{
				}

				public IPlatformInterstitialAd.IConfig Build()
				{
					return null;
				}
			}
		}

		private Action<LevelPlayAdInfo> m_OnAdLoaded;

		private Action<LevelPlayAdError> m_OnAdLoadFailed;

		private Action<LevelPlayAdInfo> m_OnAdDisplayed;

		private Action<LevelPlayAdInfo> m_OnAdClosed;

		private Action<LevelPlayAdInfo> m_OnAdClicked;

		private Action<LevelPlayAdInfo, LevelPlayAdError> m_OnAdDisplayFailed;

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

		public event Action<LevelPlayAdInfo> OnAdClosed
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

		public event Action<LevelPlayAdInfo, LevelPlayAdError> OnAdDisplayFailed
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

		public UnsupportedInterstitialAd(string adUnitId)
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
	}
}

using System;

namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayInterstitialAd : ILevelPlayInterstitialAd, IDisposable
	{
		public class Config
		{
			public class Builder
			{
				private readonly IPlatformInterstitialAd.IConfigBuilder m_Builder;

				public Builder SetBidFloor(double bidFloor)
				{
					return null;
				}

				public Config Build()
				{
					return null;
				}
			}

			private readonly IPlatformInterstitialAd.IConfig PlatformConfig__BackingField;

			internal IPlatformInterstitialAd.IConfig PlatformConfig => PlatformConfig__BackingField;

			private Config(IPlatformInterstitialAd.IConfig platformConfig)
			{
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

		private readonly IPlatformInterstitialAd m_InterstitialAd;

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

		public LevelPlayInterstitialAd(string adUnitId, Config config = null)
		{
		}

		private void SetupEvents()
		{
		}

		internal LevelPlayInterstitialAd(IPlatformInterstitialAd platformInterstitialAd)
		{
		}

		public void LoadAd()
		{
		}

		public void DestroyAd()
		{
		}

		public void ShowAd(string placementName = null)
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

		private void _003CSetupEvents_003Eb__28_0(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__28_1(LevelPlayAdError error)
		{
		}

		private void _003CSetupEvents_003Eb__28_2(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__28_3(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__28_4(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__28_5(LevelPlayAdInfo info, LevelPlayAdError error)
		{
		}

		private void _003CSetupEvents_003Eb__28_6(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupEvents_003Eb__28_7(LevelPlayImpressionData data)
		{
		}
	}
}

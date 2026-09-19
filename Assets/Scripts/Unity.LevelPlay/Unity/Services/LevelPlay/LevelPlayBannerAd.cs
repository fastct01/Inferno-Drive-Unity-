using System;

namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayBannerAd : ILevelPlayBannerAd, IDisposable
	{
		public class Config
		{
			public class Builder
			{
				private readonly IPlatformBannerAd.IConfigBuilder m_Builder;

				public Builder SetBidFloor(double bidFloor)
				{
					return null;
				}

				public Builder SetSize(LevelPlayAdSize size)
				{
					return null;
				}

				public Builder SetPosition(LevelPlayBannerPosition position)
				{
					return null;
				}

				public Builder SetPlacementName(string placementName)
				{
					return null;
				}

				public Builder SetDisplayOnLoad(bool displayOnLoad)
				{
					return null;
				}

				public Builder SetRespectSafeArea(bool respectSafeArea)
				{
					return null;
				}

				public Config Build()
				{
					return null;
				}
			}

			private readonly IPlatformBannerAd.IConfig PlatformConfig__BackingField;

			internal IPlatformBannerAd.IConfig PlatformConfig => PlatformConfig__BackingField;

			private Config(IPlatformBannerAd.IConfig platformConfig)
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

		private bool m_autoRefresh;

		private readonly IPlatformBannerAd m_BannerAd;

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

		public LevelPlayBannerAd(string adUnitId, Config config = null)
		{
		}

		internal LevelPlayBannerAd(IPlatformBannerAd platformBannerAd)
		{
		}

		public void LoadAd()
		{
		}

		public void DestroyAd()
		{
		}

		public void ShowAd()
		{
		}

		public void HideAd()
		{
		}

		public string GetAdId()
		{
			return null;
		}

		public string GetAdUnitId()
		{
			return null;
		}

		public LevelPlayAdSize GetAdSize()
		{
			return null;
		}

		public LevelPlayBannerPosition GetPosition()
		{
			return null;
		}

		public string GetPlacementName()
		{
			return null;
		}

		public void PauseAutoRefresh()
		{
		}

		public void ResumeAutoRefresh()
		{
		}

		private void SetupCallbacks()
		{
		}

		public void Dispose()
		{
		}

		private void _003CSetupCallbacks_003Eb__42_0(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupCallbacks_003Eb__42_1(LevelPlayAdError error)
		{
		}

		private void _003CSetupCallbacks_003Eb__42_2(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupCallbacks_003Eb__42_3(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupCallbacks_003Eb__42_4(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupCallbacks_003Eb__42_5(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupCallbacks_003Eb__42_6(LevelPlayAdInfo info)
		{
		}

		private void _003CSetupCallbacks_003Eb__42_7(LevelPlayAdInfo info, LevelPlayAdError error)
		{
		}

		private void _003CSetupCallbacks_003Eb__42_8(LevelPlayImpressionData data)
		{
		}
	}
}

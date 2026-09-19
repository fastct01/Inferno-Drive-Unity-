using System;

namespace Unity.Services.LevelPlay
{
	internal sealed class UnsupportedBannerAd : IPlatformBannerAd, IDisposable
	{
		internal class Config : IPlatformBannerAd.IConfig
		{
			internal class Builder : IPlatformBannerAd.IConfigBuilder
			{
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

		private readonly LevelPlayBannerPosition Position__BackingField;

		private readonly string AdId__BackingField;

		private readonly string AdUnitId__BackingField;

		private readonly LevelPlayAdSize AdSize__BackingField;

		private readonly LevelPlayAdSize Size__BackingField;

		private readonly string PlacementName__BackingField;

		public LevelPlayBannerPosition Position => Position__BackingField;

		public string AdId => AdId__BackingField;

		public string AdUnitId => AdUnitId__BackingField;

		public LevelPlayAdSize AdSize => AdSize__BackingField;

		public LevelPlayAdSize Size => Size__BackingField;

		public string PlacementName => PlacementName__BackingField;

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

		internal UnsupportedBannerAd(string adUnitId, LevelPlayAdSize size, LevelPlayBannerPosition position, string placementId)
		{
		}

		internal UnsupportedBannerAd(string adUnitId, Config config)
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

		public void PauseAutoRefresh()
		{
		}

		public void ResumeAutoRefresh()
		{
		}

		public void SetAutoRefresh(bool flag)
		{
		}

		public void Dispose()
		{
		}
	}
}

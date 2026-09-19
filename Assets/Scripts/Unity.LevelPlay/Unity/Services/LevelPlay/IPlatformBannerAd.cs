using System;

namespace Unity.Services.LevelPlay
{
	internal interface IPlatformBannerAd : IDisposable
	{
		internal interface IConfig
		{
		}

		internal interface IConfigBuilder
		{
			void SetBidFloor(double bidFloor);

			void SetSize(LevelPlayAdSize size);

			void SetPosition(LevelPlayBannerPosition position);

			void SetPlacementName(string placementName);

			void SetDisplayOnLoad(bool displayOnLoad);

			void SetRespectSafeArea(bool respectSafeArea);

			IConfig Build();
		}

		string AdId { get; }

		string AdUnitId { get; }

		LevelPlayAdSize AdSize { get; }

		string PlacementName { get; }

		LevelPlayBannerPosition Position { get; }

		event Action<LevelPlayAdInfo> OnAdLoaded;

		event Action<LevelPlayAdError> OnAdLoadFailed;

		event Action<LevelPlayAdInfo> OnAdClicked;

		event Action<LevelPlayAdInfo> OnAdDisplayed;

		event Action<LevelPlayAdInfo, LevelPlayAdError> OnAdDisplayFailed;

		event Action<LevelPlayAdInfo> OnAdExpanded;

		event Action<LevelPlayAdInfo> OnAdCollapsed;

		event Action<LevelPlayAdInfo> OnAdLeftApplication;

		event Action<LevelPlayImpressionData> OnAdImpressionDataReady;

		void LoadAd();

		void DestroyAd();

		void ShowAd();

		void HideAd();

		void PauseAutoRefresh();

		void ResumeAutoRefresh();
	}
}

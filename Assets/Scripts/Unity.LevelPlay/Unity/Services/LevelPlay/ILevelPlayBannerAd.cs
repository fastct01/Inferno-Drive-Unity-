using System;

namespace Unity.Services.LevelPlay
{
	public interface ILevelPlayBannerAd : IDisposable
	{
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

		string GetAdId();

		string GetAdUnitId();

		LevelPlayAdSize GetAdSize();

		LevelPlayBannerPosition GetPosition();

		string GetPlacementName();

		void PauseAutoRefresh();

		void ResumeAutoRefresh();
	}
}

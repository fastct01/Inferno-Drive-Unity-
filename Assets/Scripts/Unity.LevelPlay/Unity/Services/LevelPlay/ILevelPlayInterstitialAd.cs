using System;

namespace Unity.Services.LevelPlay
{
	public interface ILevelPlayInterstitialAd : IDisposable
	{
		string AdUnitId { get; }

		event Action<LevelPlayAdInfo> OnAdLoaded;

		event Action<LevelPlayAdError> OnAdLoadFailed;

		event Action<LevelPlayAdInfo> OnAdDisplayed;

		event Action<LevelPlayAdInfo> OnAdClosed;

		event Action<LevelPlayAdInfo> OnAdClicked;

		event Action<LevelPlayAdInfo, LevelPlayAdError> OnAdDisplayFailed;

		event Action<LevelPlayAdInfo> OnAdInfoChanged;

		event Action<LevelPlayImpressionData> OnAdImpressionDataReady;

		string GetAdId();

		void LoadAd();

		void ShowAd(string placementName = null);

		void DestroyAd();

		bool IsAdReady();
	}
}

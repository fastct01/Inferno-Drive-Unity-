using System;

namespace Unity.Services.LevelPlay
{
	public interface ILevelPlayRewardedAd : IDisposable
	{
		string AdUnitId { get; }

		event Action<LevelPlayAdInfo> OnAdLoaded;

		event Action<LevelPlayAdError> OnAdLoadFailed;

		event Action<LevelPlayAdInfo> OnAdDisplayed;

		event Action<LevelPlayAdInfo, LevelPlayAdError> OnAdDisplayFailed;

		event Action<LevelPlayAdInfo, LevelPlayReward> OnAdRewarded;

		event Action<LevelPlayAdInfo> OnAdClicked;

		event Action<LevelPlayAdInfo> OnAdClosed;

		event Action<LevelPlayAdInfo> OnAdInfoChanged;

		event Action<LevelPlayImpressionData> OnAdImpressionDataReady;

		string GetAdId();

		void LoadAd();

		void ShowAd(string placementName = null);

		void DestroyAd();

		bool IsAdReady();

		LevelPlayReward GetReward(string placement);
	}
}

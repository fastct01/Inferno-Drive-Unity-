using System;

namespace Unity.Services.LevelPlay
{
	internal interface IPlatformInterstitialAd : IDisposable
	{
		internal interface IConfig
		{
		}

		internal interface IConfigBuilder
		{
			void SetBidFloor(double bidFloor);

			IConfig Build();
		}

		string AdId { get; }

		string AdUnitId { get; }

		event Action<LevelPlayAdInfo> OnAdLoaded;

		event Action<LevelPlayAdError> OnAdLoadFailed;

		event Action<LevelPlayAdInfo> OnAdDisplayed;

		event Action<LevelPlayAdInfo> OnAdClosed;

		event Action<LevelPlayAdInfo> OnAdClicked;

		event Action<LevelPlayAdInfo, LevelPlayAdError> OnAdDisplayFailed;

		event Action<LevelPlayAdInfo> OnAdInfoChanged;

		event Action<LevelPlayImpressionData> OnAdImpressionDataReady;

		void LoadAd();

		void ShowAd(string placementName);

		bool IsAdReady();
	}
}

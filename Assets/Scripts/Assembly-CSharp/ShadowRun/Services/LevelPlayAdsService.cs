using System;
using System.Collections;
using Unity.Services.LevelPlay;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   LevelPlayAdsService_get_IsCaseRewardedReady_m2F7E89B244F85B81009B5DC391B8F35DB836FB0E
//   LevelPlayAdsService_Bootstrap_m3FF79B4A6EC2AC557B6FFA2FE25E404093202829 (RuntimeInitializeOnLoads.json: loadTypes 1 = BeforeSceneLoad)
//   LevelPlayAdsService_Awake_mB7E5C4651BC03DC7A6C12904EB3B1B73F37EF238
//   LevelPlayAdsService_OnDestroy_m8E4E2DFAC6BAAD5EBB102B2FAC3A3539FE0949AC
//   LevelPlayAdsService_ShowCaseRewarded_m2D600E43F2D583B76B97604C598D35C0B9806723
//   LevelPlayAdsService_OnInitializationSucceeded_m9918C9A1BB7C63CCB1E90DBFDDDCBE7CB304B4CE
//   LevelPlayAdsService_OnInitializationFailed_mDA29BDFF041313F3BB58F52EB2588D32802693C1
//   LevelPlayAdsService_OnRewardedLoaded_m5F372B30F792DBAE85850C66E2FBF2321297C8C8
//   LevelPlayAdsService_OnRewardedLoadFailed_m952E992BF81AA9BF0E752A5B2F2F923273E60489
//   LevelPlayAdsService_OnRewardedDisplayed_m6FB20FC30E08BCFC62D24B620D923B08F5DB8EB9
//   LevelPlayAdsService_OnRewardedDisplayFailed_m8FE2CE815DD4CC6E9B84A8DA38FCE56E665BD40C
//   LevelPlayAdsService_OnRewarded_mFE929C99E38C504BE3E757C48DFF8B4A73DB1626
//   LevelPlayAdsService_OnRewardedClosed_mA7000837D8775F7637EBC150C274A96A7F98F067
//   U3CClearSkippedRequestU3Ed__25_MoveNext_mE957546EE16B492F3694B2491D317A327DFF3156
//   LevelPlayAdsService_FailPending_mFE223FAF54B24ECD8000FEEDBC05E13E8571CDEA
//   LevelPlayAdsService_ReleaseRewardedAd_m63CAA2C1EA624075874A4210D278DE981F35A8CB
namespace ShadowRun.Services
{
	public sealed class LevelPlayAdsService : MonoBehaviour
	{
		private const string IosAppKey = "274fcc13d";

		private const string CaseRewardedAdUnitId = "qzjtnnabo7bsrs59";

		private LevelPlayRewardedAd caseRewardedAd;

		private Action pendingReward;

		private Action<string> pendingFailure;

		private int requestSerial;

		private int activeRequestSerial;

		public static LevelPlayAdsService Instance { get; private set; }

		public bool IsCaseRewardedReady => caseRewardedAd != null && caseRewardedAd.IsAdReady() && pendingReward == null;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			if (Instance != null)
			{
				return;
			}
			GameObject host = new GameObject("LevelPlay Ads Service");
			DontDestroyOnLoad(host);
			host.AddComponent<LevelPlayAdsService>();
		}

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;
			DontDestroyOnLoad(gameObject);
			LevelPlay.OnInitSuccess += OnInitializationSucceeded;
			LevelPlay.OnInitFailed += OnInitializationFailed;
			LevelPlay.Init(IosAppKey, null);
		}

		private void OnDestroy()
		{
			if (Instance != this)
			{
				return;
			}
			LevelPlay.OnInitSuccess -= OnInitializationSucceeded;
			LevelPlay.OnInitFailed -= OnInitializationFailed;
			ReleaseRewardedAd();
			Instance = null;
		}

		public void ShowCaseRewarded(Action onRewarded, Action<string> onUnavailable)
		{
			if (onRewarded == null)
			{
				return;
			}
			if (pendingReward != null)
			{
				onUnavailable?.Invoke("Ad is already opening.");
				return;
			}
			if (caseRewardedAd == null || !caseRewardedAd.IsAdReady())
			{
				caseRewardedAd?.LoadAd();
				onUnavailable?.Invoke("Rewarded ad is still loading.");
				return;
			}
			requestSerial++;
			activeRequestSerial = requestSerial;
			pendingReward = onRewarded;
			pendingFailure = onUnavailable;
			caseRewardedAd.ShowAd(null);
		}

		private void OnInitializationSucceeded(LevelPlayConfiguration configuration)
		{
			if (caseRewardedAd != null)
			{
				return;
			}
			caseRewardedAd = new LevelPlayRewardedAd(CaseRewardedAdUnitId, null);
			caseRewardedAd.OnAdLoaded += OnRewardedLoaded;
			caseRewardedAd.OnAdLoadFailed += OnRewardedLoadFailed;
			caseRewardedAd.OnAdDisplayed += OnRewardedDisplayed;
			caseRewardedAd.OnAdDisplayFailed += OnRewardedDisplayFailed;
			caseRewardedAd.OnAdRewarded += OnRewarded;
			caseRewardedAd.OnAdClosed += OnRewardedClosed;
			caseRewardedAd.LoadAd();
		}

		private void OnInitializationFailed(LevelPlayInitError error)
		{
			Debug.LogWarning("LevelPlay initialization failed: " + error?.ToString());
			FailPending("Ads initialization failed.");
		}

		private void OnRewardedLoaded(LevelPlayAdInfo adInfo)
		{
			Debug.Log("Case rewarded ad loaded.");
		}

		private void OnRewardedLoadFailed(LevelPlayAdError error)
		{
			Debug.LogWarning("Case rewarded ad failed to load: " + error?.ToString());
			FailPending("Rewarded ad is unavailable.");
		}

		private void OnRewardedDisplayed(LevelPlayAdInfo adInfo)
		{
			AudioListener.pause = true;
		}

		private void OnRewardedDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
		{
			AudioListener.pause = false;
			Debug.LogWarning("Case rewarded ad failed to display: " + error?.ToString());
			FailPending("Rewarded ad could not be shown.");
			caseRewardedAd?.LoadAd();
		}

		private void OnRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
		{
			Action callback = pendingReward;
			pendingReward = null;
			pendingFailure = null;
			callback?.Invoke();
		}

		private void OnRewardedClosed(LevelPlayAdInfo adInfo)
		{
			AudioListener.pause = false;
			int closedRequest = activeRequestSerial;
			StartCoroutine(ClearSkippedRequest(closedRequest));
			caseRewardedAd?.LoadAd();
		}

		private IEnumerator ClearSkippedRequest(int closedRequest)
		{
			yield return new WaitForSecondsRealtime(2f);
			if (closedRequest == activeRequestSerial && pendingReward != null)
			{
				FailPending("Watch the full ad to open the case.");
			}
		}

		private void FailPending(string message)
		{
			Action<string> failure = pendingFailure;
			pendingReward = null;
			pendingFailure = null;
			failure?.Invoke(message);
		}

		private void ReleaseRewardedAd()
		{
			if (caseRewardedAd == null)
			{
				return;
			}
			caseRewardedAd.OnAdLoaded -= OnRewardedLoaded;
			caseRewardedAd.OnAdLoadFailed -= OnRewardedLoadFailed;
			caseRewardedAd.OnAdDisplayed -= OnRewardedDisplayed;
			caseRewardedAd.OnAdDisplayFailed -= OnRewardedDisplayFailed;
			caseRewardedAd.OnAdRewarded -= OnRewarded;
			caseRewardedAd.OnAdClosed -= OnRewardedClosed;
			caseRewardedAd = null;
		}
	}
}

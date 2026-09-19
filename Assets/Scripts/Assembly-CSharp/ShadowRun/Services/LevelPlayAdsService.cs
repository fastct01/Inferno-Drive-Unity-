using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace ShadowRun.Services
{
	public sealed class LevelPlayAdsService : MonoBehaviour
	{
		private sealed class _003CClearSkippedRequest_003Ed__25 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public int closedRequest;

			public LevelPlayAdsService _003C_003E4__this;

			object IEnumerator<object>.Current => null;

			object IEnumerator.Current => null;

			public _003CClearSkippedRequest_003Ed__25(int _003C_003E1__state)
			{
			}

			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				return false;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			void IEnumerator.Reset()
			{
			}
		}

		private const string IosAppKey = "274fcc13d";

		private const string CaseRewardedAdUnitId = "qzjtnnabo7bsrs59";

		private static LevelPlayAdsService Instance__BackingField;

		private LevelPlayRewardedAd caseRewardedAd;

		private Action pendingReward;

		private Action<string> pendingFailure;

		private int requestSerial;

		private int activeRequestSerial;

		public static LevelPlayAdsService Instance
		{
			get
			{
				return Instance__BackingField;
			}
			private set
			{
				Instance__BackingField = value;
			}
		}

		public bool IsCaseRewardedReady => false;

		private static void Bootstrap()
		{
		}

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		public void ShowCaseRewarded(Action onRewarded, Action<string> onUnavailable)
		{
		}

		private void OnInitializationSucceeded(LevelPlayConfiguration configuration)
		{
		}

		private void OnInitializationFailed(LevelPlayInitError error)
		{
		}

		private void OnRewardedLoaded(LevelPlayAdInfo adInfo)
		{
		}

		private void OnRewardedLoadFailed(LevelPlayAdError error)
		{
		}

		private void OnRewardedDisplayed(LevelPlayAdInfo adInfo)
		{
		}

		private void OnRewardedDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
		{
		}

		private void OnRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
		{
		}

		private void OnRewardedClosed(LevelPlayAdInfo adInfo)
		{
		}

		private IEnumerator ClearSkippedRequest(int closedRequest)
		{
			return null;
		}

		private void FailPending(string message)
		{
		}

		private void ReleaseRewardedAd()
		{
		}
	}
}

using UnityEngine;

namespace Unity.Services.LevelPlay
{
	internal sealed class UnityRewardedAdListener : AndroidJavaProxy, IUnityRewardedAdListener
	{
		private sealed class _003C_003Ec__DisplayClass10_0
		{
			public UnityRewardedAdListener _003C_003E4__this;

			public string adInfo;

			internal void _003ConAdInfoChanged_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass3_0
		{
			public UnityRewardedAdListener _003C_003E4__this;

			public string adInfo;

			internal void _003ConAdLoaded_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass4_0
		{
			public UnityRewardedAdListener _003C_003E4__this;

			public string error;

			internal void _003ConAdLoadFailed_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass5_0
		{
			public UnityRewardedAdListener _003C_003E4__this;

			public string adInfo;

			internal void _003ConAdDisplayed_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass6_0
		{
			public UnityRewardedAdListener _003C_003E4__this;

			public string error;

			public string adInfo;

			internal void _003ConAdDisplayFailed_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass7_0
		{
			public UnityRewardedAdListener _003C_003E4__this;

			public string adInfo;

			public string rewardName;

			public int rewardAmount;

			internal void _003ConAdRewarded_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass8_0
		{
			public UnityRewardedAdListener _003C_003E4__this;

			public string adInfo;

			internal void _003ConAdClicked_003Eb__0(object state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass9_0
		{
			public UnityRewardedAdListener _003C_003E4__this;

			public string adInfo;

			internal void _003ConAdClosed_003Eb__0(object state)
			{
			}
		}

		private const string k_AndroidRewardedAdListenerName = "com.ironsource.unity.androidbridge.IUnityRewardedAdListener";

		private readonly IUnityRewardedAdListener m_UnityListener;

		public UnityRewardedAdListener(IUnityRewardedAdListener listener)
			: base((string)null)
		{
		}

		public void onAdLoaded(string adInfo)
		{
		}

		public void onAdLoadFailed(string error)
		{
		}

		public void onAdDisplayed(string adInfo)
		{
		}

		public void onAdDisplayFailed(string error, string adInfo)
		{
		}

		public void onAdRewarded(string adInfo, string rewardName, int rewardAmount)
		{
		}

		public void onAdClicked(string adInfo)
		{
		}

		public void onAdClosed(string adInfo)
		{
		}

		public void onAdInfoChanged(string adInfo)
		{
		}
	}
}

namespace RevenueCat
{
	public class AdTracker
	{
		public class MediatorName
		{
			private readonly string Value__BackingField;

			public static readonly MediatorName AdMob;

			public static readonly MediatorName AppLovin;

			public string Value => Value__BackingField;

			public MediatorName(string value)
			{
			}

			public override bool Equals(object obj)
			{
				return false;
			}

			public override int GetHashCode()
			{
				return 0;
			}

			public static bool operator ==(MediatorName a, MediatorName b)
			{
				return false;
			}

			public static bool operator !=(MediatorName a, MediatorName b)
			{
				return false;
			}
		}

		public class Format
		{
			private readonly string Value__BackingField;

			public static readonly Format Banner;

			public static readonly Format Interstitial;

			public static readonly Format Rewarded;

			public static readonly Format RewardedInterstitial;

			public static readonly Format Native;

			public static readonly Format AppOpen;

			public static readonly Format Other;

			public string Value => Value__BackingField;

			public Format(string value)
			{
			}

			public override bool Equals(object obj)
			{
				return false;
			}

			public override int GetHashCode()
			{
				return 0;
			}

			public static bool operator ==(Format a, Format b)
			{
				return false;
			}

			public static bool operator !=(Format a, Format b)
			{
				return false;
			}
		}

		public class Precision
		{
			private readonly string Value__BackingField;

			public static readonly Precision Exact;

			public static readonly Precision PublisherDefined;

			public static readonly Precision Estimated;

			public static readonly Precision Unknown;

			public string Value => Value__BackingField;

			public Precision(string value)
			{
			}

			public override bool Equals(object obj)
			{
				return false;
			}

			public override int GetHashCode()
			{
				return 0;
			}

			public static bool operator ==(Precision a, Precision b)
			{
				return false;
			}

			public static bool operator !=(Precision a, Precision b)
			{
				return false;
			}
		}

		private readonly IPurchasesWrapper _wrapper;

		internal AdTracker(IPurchasesWrapper wrapper)
		{
		}

		public void TrackAdDisplayed(AdDisplayedData data)
		{
		}

		public void TrackAdOpened(AdOpenedData data)
		{
		}

		public void TrackAdRevenue(AdRevenueData data)
		{
		}

		public void TrackAdLoaded(AdLoadedData data)
		{
		}

		public void TrackAdFailedToLoad(AdFailedToLoadData data)
		{
		}
	}
}

namespace RevenueCat
{
	public class AdRevenueData
	{
		private readonly AdTracker.MediatorName MediatorName__BackingField;

		private readonly AdTracker.Format AdFormat__BackingField;

		private readonly string AdUnitId__BackingField;

		private readonly string ImpressionId__BackingField;

		private readonly long RevenueMicros__BackingField;

		private readonly string Currency__BackingField;

		private readonly AdTracker.Precision Precision__BackingField;

		private readonly string NetworkName__BackingField;

		private readonly string Placement__BackingField;

		public AdTracker.MediatorName MediatorName => MediatorName__BackingField;

		public AdTracker.Format AdFormat => AdFormat__BackingField;

		public string AdUnitId => AdUnitId__BackingField;

		public string ImpressionId => ImpressionId__BackingField;

		public long RevenueMicros => RevenueMicros__BackingField;

		public string Currency => Currency__BackingField;

		public AdTracker.Precision Precision => Precision__BackingField;

		public string NetworkName => NetworkName__BackingField;

		public string Placement => Placement__BackingField;

		public AdRevenueData(AdTracker.MediatorName mediatorName, AdTracker.Format adFormat, string adUnitId, string impressionId, long revenueMicros, string currency, AdTracker.Precision precision, string networkName = null, string placement = null)
		{
		}

		public override string ToString()
		{
			return null;
		}

		public string ToJsonString()
		{
			return null;
		}
	}
}

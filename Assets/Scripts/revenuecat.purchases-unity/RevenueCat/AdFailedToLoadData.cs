namespace RevenueCat
{
	public class AdFailedToLoadData
	{
		private readonly AdTracker.MediatorName MediatorName__BackingField;

		private readonly AdTracker.Format AdFormat__BackingField;

		private readonly string AdUnitId__BackingField;

		private readonly string Placement__BackingField;

		private readonly int? MediatorErrorCode__BackingField;

		public AdTracker.MediatorName MediatorName => MediatorName__BackingField;

		public AdTracker.Format AdFormat => AdFormat__BackingField;

		public string AdUnitId => AdUnitId__BackingField;

		public string Placement => Placement__BackingField;

		public int? MediatorErrorCode => MediatorErrorCode__BackingField;

		public AdFailedToLoadData(AdTracker.MediatorName mediatorName, AdTracker.Format adFormat, string adUnitId, string placement = null, int? mediatorErrorCode = null)
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

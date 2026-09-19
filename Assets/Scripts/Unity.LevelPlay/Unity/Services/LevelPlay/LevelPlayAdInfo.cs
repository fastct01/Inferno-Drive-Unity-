namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayAdInfo
	{
		private const string AdIdKey = "adId";

		private const string AdUnitIdKey = "adUnitId";

		private const string AdUnitNameKey = "adUnitName";

		private const string AdSizeKey = "adSize";

		private const string AdFormatKey = "adFormat";

		private const string PlacementNameKey = "placementName";

		private const string AuctionIdKey = "auctionId";

		private const string CountryKey = "country";

		private const string AbKey = "ab";

		private const string SegmentNameKey = "segmentName";

		private const string AdNetworkKey = "adNetwork";

		private const string InstanceNameKey = "instanceName";

		private const string InstanceIdKey = "instanceId";

		private const string RevenueKey = "revenue";

		private const string PrecisionKey = "precision";

		private const string EncryptedCpmKey = "encryptedCPM";

		private const string AdSizeDescriptionKey = "description";

		private const string AdSizeWidthKey = "width";

		private const string AdSizeHeightKey = "height";

		public readonly string AdId;

		public readonly string AdUnitId;

		public readonly string AdUnitName;

		public readonly LevelPlayAdSize AdSize;

		public readonly string AdFormat;

		public readonly string PlacementName;

		public readonly string AuctionId;

		public readonly string CreativeId;

		public readonly string Country;

		public readonly string Ab;

		public readonly string SegmentName;

		public readonly string AdNetwork;

		public readonly string InstanceName;

		public readonly string InstanceId;

		public readonly double? Revenue;

		public readonly string Precision;

		public readonly string EncryptedCPM;

		internal LevelPlayAdInfo(string json)
		{
		}

		private static LevelPlayAdSize GetAdSize(string adSizeJson)
		{
			return null;
		}

		private static LevelPlayAdSize GetAdSize(string description, int width = 0, int height = 0)
		{
			return null;
		}

		public override string ToString()
		{
			return null;
		}
	}
}

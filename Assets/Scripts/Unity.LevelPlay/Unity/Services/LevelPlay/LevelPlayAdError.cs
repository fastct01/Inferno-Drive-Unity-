namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayAdError
	{
		private readonly int ErrorCode__BackingField;

		private readonly string ErrorMessage__BackingField;

		private readonly string AdUnitId__BackingField;

		private readonly string AdId__BackingField;

		public int ErrorCode => ErrorCode__BackingField;

		public string ErrorMessage => ErrorMessage__BackingField;

		public string AdUnitId => AdUnitId__BackingField;

		public string AdId => AdId__BackingField;

		internal LevelPlayAdError(string json)
		{
		}

		internal LevelPlayAdError(string adUnitId, int errorCode, string errorMessage, string adId)
		{
		}

		internal LevelPlayAdError(string adUnitId, int errorCode, string errorMessage)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}
}

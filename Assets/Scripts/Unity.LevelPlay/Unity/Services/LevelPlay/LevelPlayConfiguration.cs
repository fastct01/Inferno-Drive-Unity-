namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayConfiguration
	{
		private const string k_IsAdQualityEnabled = "isAdQualityEnabled";

		private readonly bool IsAdQualityEnabled__BackingField;

		public bool IsAdQualityEnabled => IsAdQualityEnabled__BackingField;

		internal LevelPlayConfiguration(string json)
		{
		}
	}
}

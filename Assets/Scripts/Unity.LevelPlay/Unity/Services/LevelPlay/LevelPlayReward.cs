namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayReward
	{
		internal static readonly LevelPlayReward Default;

		private readonly string Name__BackingField;

		private readonly int Amount__BackingField;

		public string Name => Name__BackingField;

		public int Amount => Amount__BackingField;

		internal LevelPlayReward(string name, int amount)
		{
		}
	}
}

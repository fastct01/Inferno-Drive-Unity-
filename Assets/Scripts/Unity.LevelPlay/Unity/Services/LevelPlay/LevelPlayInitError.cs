namespace Unity.Services.LevelPlay
{
	public sealed class LevelPlayInitError
	{
		private readonly int ErrorCode__BackingField;

		private readonly string ErrorMessage__BackingField;

		public int ErrorCode => ErrorCode__BackingField;

		public string ErrorMessage => ErrorMessage__BackingField;

		internal LevelPlayInitError(string json)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}
}

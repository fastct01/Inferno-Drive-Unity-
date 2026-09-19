namespace Supabase.Functions.Exceptions
{
	public static class FailureHint
	{
		public enum Reason
		{
			Unknown = 0,
			NotAuthorized = 1,
			Internal = 2
		}

		public static Reason DetectReason(FunctionsException ex)
		{
			return default;
		}
	}
}

namespace Supabase.Postgrest.Exceptions
{
	public static class FailureHint
	{
		public enum Reason
		{
			Unknown = 0,
			NotAuthorized = 1,
			ForeignKeyViolation = 2,
			UniquenessViolation = 3,
			ServerError = 4,
			UndefinedTable = 5,
			UndefinedFunction = 6,
			InvalidArgument = 7
		}

		public static Reason DetectReason(PostgrestException pgex)
		{
			return default;
		}
	}
}

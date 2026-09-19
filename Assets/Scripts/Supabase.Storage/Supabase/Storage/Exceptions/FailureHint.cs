namespace Supabase.Storage.Exceptions
{
	public static class FailureHint
	{
		public enum Reason
		{
			Unknown = 0,
			NotAuthorized = 1,
			Internal = 2,
			NotFound = 3,
			AlreadyExists = 4,
			InvalidInput = 5
		}

		public static Reason DetectReason(SupabaseStorageException storageException)
		{
			return default;
		}
	}
}

namespace Supabase.Gotrue.Mfa
{
	public class MfaChallengeResponse
	{
		private string Id__BackingField;

		private long ExpiresAt__BackingField;

		public string Id
		{
			get
			{
				return Id__BackingField;
			}
			set
			{
				Id__BackingField = value;
			}
		}

		public long ExpiresAt
		{
			get
			{
				return ExpiresAt__BackingField;
			}
			set
			{
				ExpiresAt__BackingField = value;
			}
		}
	}
}

namespace Supabase.Gotrue.Responses
{
	public class GenerateLinkResponse : User
	{
		private string ActionLink__BackingField;

		private string EmailOtp__BackingField;

		private string HashedToken__BackingField;

		private string RedirectTo__BackingField;

		private string VerificationType__BackingField;

		public string ActionLink
		{
			get
			{
				return ActionLink__BackingField;
			}
			set
			{
				ActionLink__BackingField = value;
			}
		}

		public string EmailOtp
		{
			get
			{
				return EmailOtp__BackingField;
			}
			set
			{
				EmailOtp__BackingField = value;
			}
		}

		public string HashedToken
		{
			get
			{
				return HashedToken__BackingField;
			}
			set
			{
				HashedToken__BackingField = value;
			}
		}

		public string RedirectTo
		{
			get
			{
				return RedirectTo__BackingField;
			}
			set
			{
				RedirectTo__BackingField = value;
			}
		}

		public string VerificationType
		{
			get
			{
				return VerificationType__BackingField;
			}
			set
			{
				VerificationType__BackingField = value;
			}
		}
	}
}

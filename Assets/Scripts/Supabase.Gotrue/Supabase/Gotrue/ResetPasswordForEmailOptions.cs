namespace Supabase.Gotrue
{
	public class ResetPasswordForEmailOptions
	{
		private string Email__BackingField;

		private Constants.OAuthFlowType FlowType__BackingField;

		private string RedirectTo__BackingField;

		private string CaptchaToken__BackingField;

		private string PKCEVerifier__BackingField;

		public string Email
		{
			get
			{
				return Email__BackingField;
			}
			private set
			{
				Email__BackingField = value;
			}
		}

		public Constants.OAuthFlowType FlowType
		{
			get
			{
				return FlowType__BackingField;
			}
			set
			{
				FlowType__BackingField = value;
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

		public string CaptchaToken
		{
			get
			{
				return CaptchaToken__BackingField;
			}
			set
			{
				CaptchaToken__BackingField = value;
			}
		}

		public string PKCEVerifier
		{
			get
			{
				return PKCEVerifier__BackingField;
			}
			set
			{
				PKCEVerifier__BackingField = value;
			}
		}

		public ResetPasswordForEmailOptions(string email)
		{
		}
	}
}

namespace Supabase.Gotrue
{
	public class SignInWithPasswordlessEmailOptions : SignInWithPasswordlessOptions
	{
		private string Email__BackingField;

		private string EmailRedirectTo__BackingField;

		private Constants.OAuthFlowType FlowType__BackingField;

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

		public string EmailRedirectTo
		{
			get
			{
				return EmailRedirectTo__BackingField;
			}
			set
			{
				EmailRedirectTo__BackingField = value;
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

		public SignInWithPasswordlessEmailOptions(string email)
		{
		}
	}
}

namespace Supabase.Gotrue
{
	public class SignInWithSSOOptions : SignInOptions
	{
		private string CaptchaToken__BackingField;

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
	}
}

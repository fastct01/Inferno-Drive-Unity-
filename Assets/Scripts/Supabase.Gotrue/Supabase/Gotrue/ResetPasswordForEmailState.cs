namespace Supabase.Gotrue
{
	public class ResetPasswordForEmailState
	{
		private string PKCEVerifier__BackingField;

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
	}
}

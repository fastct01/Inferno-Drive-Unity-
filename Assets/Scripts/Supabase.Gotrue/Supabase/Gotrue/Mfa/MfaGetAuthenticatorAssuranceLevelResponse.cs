namespace Supabase.Gotrue.Mfa
{
	public class MfaGetAuthenticatorAssuranceLevelResponse
	{
		private AuthenticatorAssuranceLevel? CurrentLevel__BackingField;

		private AuthenticatorAssuranceLevel? NextLevel__BackingField;

		private AmrEntry[] CurrentAuthenticationMethods__BackingField;

		public AuthenticatorAssuranceLevel? CurrentLevel
		{
			get
			{
				return CurrentLevel__BackingField;
			}
			set
			{
				CurrentLevel__BackingField = value;
			}
		}

		public AuthenticatorAssuranceLevel? NextLevel
		{
			get
			{
				return NextLevel__BackingField;
			}
			set
			{
				NextLevel__BackingField = value;
			}
		}

		public AmrEntry[] CurrentAuthenticationMethods
		{
			get
			{
				return CurrentAuthenticationMethods__BackingField;
			}
			set
			{
				CurrentAuthenticationMethods__BackingField = value;
			}
		}
	}
}

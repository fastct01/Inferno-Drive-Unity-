namespace Supabase.Gotrue.Mfa
{
	public class MfaEnrollParams
	{
		private string FactorType__BackingField;

		private string Issuer__BackingField;

		private string FriendlyName__BackingField;

		public string FactorType
		{
			get
			{
				return FactorType__BackingField;
			}
			set
			{
				FactorType__BackingField = value;
			}
		}

		public string Issuer
		{
			get
			{
				return Issuer__BackingField;
			}
			set
			{
				Issuer__BackingField = value;
			}
		}

		public string FriendlyName
		{
			get
			{
				return FriendlyName__BackingField;
			}
			set
			{
				FriendlyName__BackingField = value;
			}
		}
	}
}

using System.Collections.Generic;

namespace Supabase.Gotrue.Mfa
{
	public class MfaListFactorsResponse
	{
		private List<Factor> All__BackingField;

		private List<Factor> Totp__BackingField;

		public List<Factor> All
		{
			get
			{
				return All__BackingField;
			}
			set
			{
				All__BackingField = value;
			}
		}

		public List<Factor> Totp
		{
			get
			{
				return Totp__BackingField;
			}
			set
			{
				Totp__BackingField = value;
			}
		}
	}
}

using System.Collections.Generic;

namespace Supabase.Gotrue.Mfa
{
	public class MfaAdminListFactorsResponse
	{
		private List<Factor> Factors__BackingField;

		public List<Factor> Factors
		{
			get
			{
				return Factors__BackingField;
			}
			set
			{
				Factors__BackingField = value;
			}
		}
	}
}

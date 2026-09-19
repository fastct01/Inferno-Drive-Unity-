using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class SignUpOptions : SignInOptions
	{
		private Dictionary<string, object> Data__BackingField;

		public Dictionary<string, object> Data
		{
			get
			{
				return Data__BackingField;
			}
			set
			{
				Data__BackingField = value;
			}
		}
	}
}

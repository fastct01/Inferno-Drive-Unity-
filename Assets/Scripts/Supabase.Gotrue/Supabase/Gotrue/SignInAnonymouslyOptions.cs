using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class SignInAnonymouslyOptions
	{
		private Dictionary<string, object> Data__BackingField;

		private string CaptchaToken__BackingField;

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

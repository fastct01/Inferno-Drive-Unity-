using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class SignInWithPasswordlessOptions
	{
		public Dictionary<string, object> Data;

		private string CaptchaToken__BackingField;

		private bool ShouldCreateUser__BackingField;

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

		public bool ShouldCreateUser
		{
			get
			{
				return ShouldCreateUser__BackingField;
			}
			set
			{
				ShouldCreateUser__BackingField = value;
			}
		}
	}
}

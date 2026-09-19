using System;

namespace Supabase.Gotrue
{
	public class ProviderAuthState
	{
		private Uri Uri__BackingField;

		private string PKCEVerifier__BackingField;

		public Uri Uri
		{
			get
			{
				return Uri__BackingField;
			}
			set
			{
				Uri__BackingField = value;
			}
		}

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

		public ProviderAuthState(Uri uri)
		{
		}
	}
}

using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class UserAttributes
	{
		private string Email__BackingField;

		private string EmailChangeToken__BackingField;

		private string Nonce__BackingField;

		private string Password__BackingField;

		private string Phone__BackingField;

		private Dictionary<string, object> Data__BackingField;

		public string Email
		{
			get
			{
				return Email__BackingField;
			}
			set
			{
				Email__BackingField = value;
			}
		}

		public string EmailChangeToken
		{
			get
			{
				return EmailChangeToken__BackingField;
			}
			set
			{
				EmailChangeToken__BackingField = value;
			}
		}

		public string Nonce
		{
			get
			{
				return Nonce__BackingField;
			}
			set
			{
				Nonce__BackingField = value;
			}
		}

		public string Password
		{
			get
			{
				return Password__BackingField;
			}
			set
			{
				Password__BackingField = value;
			}
		}

		public string Phone
		{
			get
			{
				return Phone__BackingField;
			}
			set
			{
				Phone__BackingField = value;
			}
		}

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

using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class GenerateLinkOptions
	{
		public enum LinkType
		{
			SignUp = 0,
			Invite = 1,
			MagicLink = 2,
			Recovery = 3,
			EmailChangeCurrent = 4,
			EmailChangeNew = 5
		}

		private readonly string Type__BackingField;

		private readonly string Email__BackingField;

		private string Password__BackingField;

		private string NewEmail__BackingField;

		private Dictionary<string, object> Data__BackingField;

		private string RedirectTo__BackingField;

		public string Type => Type__BackingField;

		public string Email => Email__BackingField;

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

		public string NewEmail
		{
			get
			{
				return NewEmail__BackingField;
			}
			set
			{
				NewEmail__BackingField = value;
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

		public string RedirectTo
		{
			get
			{
				return RedirectTo__BackingField;
			}
			set
			{
				RedirectTo__BackingField = value;
			}
		}

		public GenerateLinkOptions(LinkType linkType, string email)
		{
		}
	}
}

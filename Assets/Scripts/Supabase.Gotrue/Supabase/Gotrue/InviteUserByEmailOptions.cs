using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class InviteUserByEmailOptions
	{
		private string RedirectTo__BackingField;

		private Dictionary<string, object> Data__BackingField;

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

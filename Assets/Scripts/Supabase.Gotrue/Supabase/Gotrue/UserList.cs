using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class UserList<TUser>
	{
		private string Aud__BackingField;

		private List<TUser> Users__BackingField;

		public string Aud
		{
			get
			{
				return Aud__BackingField;
			}
			set
			{
				Aud__BackingField = value;
			}
		}

		public List<TUser> Users
		{
			get
			{
				return Users__BackingField;
			}
			set
			{
				Users__BackingField = value;
			}
		}
	}
}

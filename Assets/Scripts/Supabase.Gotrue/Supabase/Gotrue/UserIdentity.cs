using System;
using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class UserIdentity
	{
		private string Id__BackingField;

		private string UserId__BackingField;

		private Dictionary<string, object> IdentityData__BackingField;

		private string IdentityId__BackingField;

		private string Provider__BackingField;

		private DateTime CreatedAt__BackingField;

		private DateTime LastSignInAt__BackingField;

		private DateTime? UpdatedAt__BackingField;

		public string Id
		{
			get
			{
				return Id__BackingField;
			}
			set
			{
				Id__BackingField = value;
			}
		}

		public string UserId
		{
			get
			{
				return UserId__BackingField;
			}
			set
			{
				UserId__BackingField = value;
			}
		}

		public Dictionary<string, object> IdentityData
		{
			get
			{
				return IdentityData__BackingField;
			}
			set
			{
				IdentityData__BackingField = value;
			}
		}

		public string IdentityId
		{
			get
			{
				return IdentityId__BackingField;
			}
			set
			{
				IdentityId__BackingField = value;
			}
		}

		public string Provider
		{
			get
			{
				return Provider__BackingField;
			}
			set
			{
				Provider__BackingField = value;
			}
		}

		public DateTime CreatedAt
		{
			get
			{
				return CreatedAt__BackingField;
			}
			set
			{
				CreatedAt__BackingField = value;
			}
		}

		public DateTime LastSignInAt
		{
			get
			{
				return LastSignInAt__BackingField;
			}
			set
			{
				LastSignInAt__BackingField = value;
			}
		}

		public DateTime? UpdatedAt
		{
			get
			{
				return UpdatedAt__BackingField;
			}
			set
			{
				UpdatedAt__BackingField = value;
			}
		}
	}
}

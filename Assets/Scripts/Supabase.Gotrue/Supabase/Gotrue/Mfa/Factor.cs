using System;

namespace Supabase.Gotrue.Mfa
{
	public class Factor
	{
		private string Id__BackingField;

		private string FriendlyName__BackingField;

		private string FactorType__BackingField;

		private string Status__BackingField;

		private DateTime CreatedAt__BackingField;

		private DateTime UpdatedAt__BackingField;

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

		public string FriendlyName
		{
			get
			{
				return FriendlyName__BackingField;
			}
			set
			{
				FriendlyName__BackingField = value;
			}
		}

		public string FactorType
		{
			get
			{
				return FactorType__BackingField;
			}
			set
			{
				FactorType__BackingField = value;
			}
		}

		public string Status
		{
			get
			{
				return Status__BackingField;
			}
			set
			{
				Status__BackingField = value;
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

		public DateTime UpdatedAt
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

using System;
using System.Collections.Generic;

namespace Supabase.Storage
{
	public class Bucket
	{
		private string Id__BackingField;

		private string Name__BackingField;

		private string Owner__BackingField;

		private DateTime? CreatedAt__BackingField;

		private DateTime? UpdatedAt__BackingField;

		private bool Public__BackingField;

		private string FileSizeLimit__BackingField;

		private List<string> AllowedMimes__BackingField;

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

		public string Name
		{
			get
			{
				return Name__BackingField;
			}
			set
			{
				Name__BackingField = value;
			}
		}

		public string Owner
		{
			get
			{
				return Owner__BackingField;
			}
			set
			{
				Owner__BackingField = value;
			}
		}

		public DateTime? CreatedAt
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

		public bool Public
		{
			get
			{
				return Public__BackingField;
			}
			set
			{
				Public__BackingField = value;
			}
		}

		public string FileSizeLimit
		{
			get
			{
				return FileSizeLimit__BackingField;
			}
			set
			{
				FileSizeLimit__BackingField = value;
			}
		}

		public List<string> AllowedMimes
		{
			get
			{
				return AllowedMimes__BackingField;
			}
			set
			{
				AllowedMimes__BackingField = value;
			}
		}
	}
}

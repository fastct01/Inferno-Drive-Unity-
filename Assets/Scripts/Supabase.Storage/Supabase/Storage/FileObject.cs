using System;
using System.Collections.Generic;

namespace Supabase.Storage
{
	public class FileObject
	{
		private string Name__BackingField;

		private string BucketId__BackingField;

		private string Owner__BackingField;

		private string Id__BackingField;

		private DateTime? UpdatedAt__BackingField;

		private DateTime? CreatedAt__BackingField;

		private DateTime? LastAccessedAt__BackingField;

		public Dictionary<string, object> MetaData;

		private Bucket Buckets__BackingField;

		public bool IsFolder => false;

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

		public string BucketId
		{
			get
			{
				return BucketId__BackingField;
			}
			set
			{
				BucketId__BackingField = value;
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

		public DateTime? LastAccessedAt
		{
			get
			{
				return LastAccessedAt__BackingField;
			}
			set
			{
				LastAccessedAt__BackingField = value;
			}
		}

		public Bucket Buckets
		{
			get
			{
				return Buckets__BackingField;
			}
			set
			{
				Buckets__BackingField = value;
			}
		}
	}
}

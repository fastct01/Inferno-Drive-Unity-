using System;
using System.Collections.Generic;

namespace Supabase.Postgrest.Models
{
	public class CachedModel<TModel> where TModel : new()
	{
		private List<TModel> Models__BackingField;

		private DateTime CachedAt__BackingField;

		public List<TModel> Models
		{
			get
			{
				return Models__BackingField;
			}
			set
			{
				Models__BackingField = value;
			}
		}

		public DateTime CachedAt
		{
			get
			{
				return CachedAt__BackingField;
			}
			set
			{
				CachedAt__BackingField = value;
			}
		}
	}
}

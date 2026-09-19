using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Responses;

namespace Supabase.Postgrest.Models
{
	public abstract class BaseModel
	{
		private string BaseUrl__BackingField;

		private ClientOptions RequestClientOptions__BackingField;

		private Func<Dictionary<string, string>> GetHeaders__BackingField;

		public string BaseUrl
		{
			get
			{
				return BaseUrl__BackingField;
			}
			set
			{
				BaseUrl__BackingField = value;
			}
		}

		public ClientOptions RequestClientOptions
		{
			get
			{
				return RequestClientOptions__BackingField;
			}
			set
			{
				RequestClientOptions__BackingField = value;
			}
		}

		internal Func<Dictionary<string, string>> GetHeaders
		{
			get
			{
				return GetHeaders__BackingField;
			}
			set
			{
				GetHeaders__BackingField = value;
			}
		}

		public string TableName => null;

		public Dictionary<PrimaryKeyAttribute, object> PrimaryKey => null;

		public Task<ModeledResponse<T>> Update<T>(CancellationToken cancellationToken = default(CancellationToken)) where T : new()
		{
			return null;
		}

		public virtual Task Delete<T>(CancellationToken cancellationToken = default(CancellationToken)) where T : new()
		{
			return null;
		}
	}
}

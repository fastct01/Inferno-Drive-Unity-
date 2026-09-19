using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Supabase.Postgrest.Responses
{
	public class ModeledResponse<T> : BaseResponse where T : new()
	{
		private readonly List<T> Models__BackingField;

		public T Model => default;

		public List<T> Models => Models__BackingField;

		public ModeledResponse(BaseResponse baseResponse, JsonSerializerSettings serializerSettings, Func<Dictionary<string, string>> getHeaders = null, bool shouldParse = true)
			: base(null, null, null)
		{
		}
	}
}

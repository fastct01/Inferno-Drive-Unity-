using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Supabase.Postgrest
{
	public delegate void OnRequestPreparedEventHandler(object sender, ClientOptions clientOptions, HttpMethod method, string url, JsonSerializerSettings serializerSettings, object data = null, Dictionary<string, string> headers = null);
}

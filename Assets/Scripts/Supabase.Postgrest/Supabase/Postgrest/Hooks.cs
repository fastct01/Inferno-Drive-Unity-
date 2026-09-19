using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Supabase.Postgrest
{
	internal class Hooks
	{
		private static Hooks _instance__BackingField;

		private readonly List<OnRequestPreparedEventHandler> _requestPreparedEventHandlers;

		private static Hooks _instance
		{
			get
			{
				return _instance__BackingField;
			}
			set
			{
				_instance__BackingField = value;
			}
		}

		public static Hooks Instance => null;

		private Hooks()
		{
		}

		public void AddRequestPreparedHandler(OnRequestPreparedEventHandler handler)
		{
		}

		public void RemoveRequestPreparedHandler(OnRequestPreparedEventHandler handler)
		{
		}

		public void ClearRequestPreparedHandlers()
		{
		}

		public void NotifyOnRequestPreparedHandlers(object sender, ClientOptions clientOptions, HttpMethod method, string url, JsonSerializerSettings serializerSettings, object data = null, Dictionary<string, string> headers = null)
		{
		}
	}
}

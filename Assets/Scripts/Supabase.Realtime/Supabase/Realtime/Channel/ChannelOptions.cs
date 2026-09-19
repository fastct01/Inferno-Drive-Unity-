using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Supabase.Realtime.Channel
{
	public class ChannelOptions
	{
		private Func<string> RetrieveAccessToken__BackingField;

		private Dictionary<string, string> Parameters__BackingField;

		private readonly ClientOptions ClientOptions__BackingField;

		private readonly JsonSerializerSettings SerializerSettings__BackingField;

		public Func<string> RetrieveAccessToken
		{
			get
			{
				return RetrieveAccessToken__BackingField;
			}
			private set
			{
				RetrieveAccessToken__BackingField = value;
			}
		}

		public Dictionary<string, string> Parameters
		{
			get
			{
				return Parameters__BackingField;
			}
			set
			{
				Parameters__BackingField = value;
			}
		}

		public ClientOptions ClientOptions => ClientOptions__BackingField;

		public JsonSerializerSettings SerializerSettings => SerializerSettings__BackingField;

		public ChannelOptions(ClientOptions clientOptions, Func<string> retrieveAccessToken, JsonSerializerSettings serializerSettings)
		{
		}
	}
}

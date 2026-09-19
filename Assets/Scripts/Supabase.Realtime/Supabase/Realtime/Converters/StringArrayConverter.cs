using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Supabase.Realtime.Converters
{
	public class StringArrayConverter : JsonConverter
	{
		public override bool CanRead => false;

		public override bool CanWrite => false;

		public override bool CanConvert(Type objectType)
		{
			return false;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
		}

		internal static List<string> Parse(string value)
		{
			return null;
		}
	}
}

using System;
using Newtonsoft.Json;

namespace Supabase.Postgrest.Converters
{
	public class IntArrayConverter : JsonConverter
	{
		public override bool CanRead => false;

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
	}
}

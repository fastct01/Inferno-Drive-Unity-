using System;
using Newtonsoft.Json;

namespace Supabase.Postgrest.Converters
{
	internal class RangeConverter : JsonConverter
	{
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

		public static IntRange ParseIntRange(string value)
		{
			return null;
		}
	}
}

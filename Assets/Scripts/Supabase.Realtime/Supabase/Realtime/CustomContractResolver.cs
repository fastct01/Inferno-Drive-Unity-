using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Supabase.Postgrest.Attributes;

namespace Supabase.Realtime
{
	internal class CustomContractResolver : DefaultContractResolver
	{
		private sealed class _003C_003Ec__DisplayClass0_0
		{
			public PrimaryKeyAttribute primaryKeyAtt;

			internal bool _003CCreateProperty_003Eb__0(object _)
			{
				return false;
			}
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			return null;
		}
	}
}

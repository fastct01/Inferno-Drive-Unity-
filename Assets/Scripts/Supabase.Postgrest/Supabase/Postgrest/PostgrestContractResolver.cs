using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Supabase.Postgrest.Attributes;

namespace Supabase.Postgrest
{
	public class PostgrestContractResolver : DefaultContractResolver
	{
		private sealed class _003C_003Ec__DisplayClass13_0
		{
			public PrimaryKeyAttribute primaryKeyAttribute;

			public PostgrestContractResolver _003C_003E4__this;

			internal bool _003CCreateProperty_003Eb__0(object instance)
			{
				return false;
			}
		}

		private bool IsUpdate__BackingField;

		private bool IsInsert__BackingField;

		private bool IsUpsert__BackingField;

		private bool IsUpdate
		{
			get
			{
				return IsUpdate__BackingField;
			}
			set
			{
				IsUpdate__BackingField = value;
			}
		}

		private bool IsInsert
		{
			get
			{
				return IsInsert__BackingField;
			}
			set
			{
				IsInsert__BackingField = value;
			}
		}

		private bool IsUpsert
		{
			get
			{
				return IsUpsert__BackingField;
			}
			set
			{
				IsUpsert__BackingField = value;
			}
		}

		public void SetState(bool isInsert = false, bool isUpdate = false, bool isUpsert = false)
		{
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			return null;
		}
	}
}

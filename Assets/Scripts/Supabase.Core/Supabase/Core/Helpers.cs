using System;
using Supabase.Core.Attributes;

namespace Supabase.Core
{
	public static class Helpers
	{
		public static T GetPropertyValue<T>(object obj, string propName)
		{
			return default;
		}

		public static T GetCustomAttribute<T>(object obj)
		{
			return default;
		}

		public static T GetCustomAttribute<T>(Type type)
		{
			return default;
		}

		public static MapToAttribute GetMappedToAttr(Enum obj)
		{
			return null;
		}
	}
}

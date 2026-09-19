using System.Collections.Generic;

namespace Supabase.Extensions
{
	internal static class DictionaryExtensions
	{
		public static T MergeLeft<T, K, V>(T me, IDictionary<K, V>[] others) where T : new()
		{
			return default;
		}
	}
}

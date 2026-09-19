using System;
using System.Collections.Generic;

namespace RevenueCat
{
	internal static class Utilities
	{
		[Serializable]
		private sealed class _003C_003Ec__3<T>
		{
			public static readonly _003C_003Ec__3<T> _003C_003E9;

			public static Func<KeyValuePair<string, T>, string> _003C_003E9__3_0;

			internal string _003CDictToString_003Eb__3_0(KeyValuePair<string, T> kvp)
			{
				return null;
			}
		}

		[Serializable]
		private sealed class _003C_003Ec__4<T>
		{
			public static readonly _003C_003Ec__4<T> _003C_003E9;

			public static Func<T, string> _003C_003E9__4_0;

			internal string _003CListToString_003Eb__4_0(T arg)
			{
				return null;
			}
		}

		private static readonly DateTime Epoch;

		internal static DateTime FromUnixTimeInMilliseconds(long unixTimeInMilliseconds)
		{
			return default;
		}

		internal static DateTime? FromOptionalUnixTimeInMilliseconds(long unixTimeInMilliseconds)
		{
			return null;
		}

		internal static string DictToString<T>(Dictionary<string, T> dictionary)
		{
			return null;
		}

		internal static string ListToString<T>(List<T> list)
		{
			return null;
		}

		internal static DateTime? FromISO8601(string iso8601)
		{
			return null;
		}
	}
}

using System;
using System.Runtime.CompilerServices;

namespace Supabase.Postgrest
{
	public class IntRange : IEquatable<IntRange>
	{
		private readonly Index Start__BackingField;

		private readonly Index End__BackingField;

		public Index Start => Start__BackingField;

		public Index End => End__BackingField;

		public static IntRange All => null;

		public IntRange(Index start, Index end)
		{
		}

		public override bool Equals(object value)
		{
			return false;
		}

		public bool Equals(IntRange other)
		{
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			return null;
		}

		public static IntRange StartAt(Index start)
		{
			return null;
		}

		public static IntRange EndAt(Index end)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public (int, int) GetOffsetAndLength(int length)
		{
			return default;
		}
	}
}

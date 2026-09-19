using System;
using System.Runtime.CompilerServices;

namespace Supabase.Postgrest
{
	public class Index : IEquatable<Index>
	{
		private readonly int _value;

		public static Index Start => null;

		public static Index End => null;

		public int Value => 0;

		public bool IsFromEnd => false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Index(int value, bool fromEnd = false)
		{
		}

		private Index(int value)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Index FromStart(int value)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Index FromEnd(int value)
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetOffset(int length)
		{
			return 0;
		}

		public override bool Equals(object value)
		{
			return false;
		}

		public bool Equals(Index other)
		{
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public static implicit operator Index(int value)
		{
			return null;
		}

		public override string ToString()
		{
			return null;
		}
	}
}

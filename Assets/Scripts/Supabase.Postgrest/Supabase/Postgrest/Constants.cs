namespace Supabase.Postgrest
{
	public static class Constants
	{
		public enum Operator
		{
			And = 0,
			Or = 1,
			Equals = 2,
			GreaterThan = 3,
			GreaterThanOrEqual = 4,
			LessThan = 5,
			LessThanOrEqual = 6,
			NotEqual = 7,
			Like = 8,
			ILike = 9,
			In = 10,
			Is = 11,
			FTS = 12,
			PLFTS = 13,
			PHFTS = 14,
			WFTS = 15,
			Contains = 16,
			ContainedIn = 17,
			Overlap = 18,
			StrictlyLeft = 19,
			StrictlyRight = 20,
			NotRightOf = 21,
			NotLeftOf = 22,
			Adjacent = 23,
			Not = 24
		}

		public enum Ordering
		{
			Ascending = 0,
			Descending = 1
		}

		public enum NullPosition
		{
			First = 0,
			Last = 1
		}

		public enum CountType
		{
			Exact = 0,
			Planned = 1,
			Estimated = 2
		}
	}
}

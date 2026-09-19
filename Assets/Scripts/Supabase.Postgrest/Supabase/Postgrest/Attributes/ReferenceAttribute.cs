using System;
using System.Collections.Generic;

namespace Supabase.Postgrest.Attributes
{
	public class ReferenceAttribute : Attribute
	{
		public enum JoinType
		{
			Inner = 0,
			Left = 1
		}

		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9;

			public static Func<Type, bool> _003C_003E9__39_0;

			internal bool _003CIsDerivedFromBaseModel_003Eb__39_0(Type t)
			{
				return false;
			}
		}

		private sealed class _003C_003Ec__DisplayClass38_0
		{
			public ReferenceAttribute refAttr;

			internal bool _003CParseRelationships_003Eb__0(ReferenceAttribute r)
			{
				return false;
			}
		}

		private readonly Type Model__BackingField;

		private string ColumnName__BackingField;

		private string ForeignKey__BackingField;

		private readonly string TableName__BackingField;

		private List<string> Columns__BackingField;

		private bool IgnoreOnInsert__BackingField;

		private bool IgnoreOnUpdate__BackingField;

		private readonly bool IncludeInQuery__BackingField;

		private readonly bool UseInnerJoin__BackingField;

		public Type Model => Model__BackingField;

		public string ColumnName
		{
			get
			{
				return ColumnName__BackingField;
			}
			private set
			{
				ColumnName__BackingField = value;
			}
		}

		public string ForeignKey
		{
			get
			{
				return ForeignKey__BackingField;
			}
			private set
			{
				ForeignKey__BackingField = value;
			}
		}

		public string TableName => TableName__BackingField;

		public List<string> Columns
		{
			get
			{
				return Columns__BackingField;
			}
			private set
			{
				Columns__BackingField = value;
			}
		}

		public bool IgnoreOnInsert
		{
			get
			{
				return IgnoreOnInsert__BackingField;
			}
			private set
			{
				IgnoreOnInsert__BackingField = value;
			}
		}

		public bool IgnoreOnUpdate
		{
			get
			{
				return IgnoreOnUpdate__BackingField;
			}
			private set
			{
				IgnoreOnUpdate__BackingField = value;
			}
		}

		public bool IncludeInQuery => IncludeInQuery__BackingField;

		public bool UseInnerJoin => UseInnerJoin__BackingField;

		public ReferenceAttribute(Type model, JoinType joinType, bool includeInQuery = true, string columnName = "", string foreignKey = null)
		{
		}

		public ReferenceAttribute(Type model, bool includeInQuery = true, bool useInnerJoin = true, string columnName = null, string foreignKey = null)
		{
		}

		internal void ParseProperties(List<ReferenceAttribute> seenRefs = null)
		{
		}

		private void ParseColumns(ref List<ReferenceAttribute> seenRefs)
		{
		}

		public override bool Equals(object obj)
		{
			return false;
		}

		private void ParseRelationships(List<ReferenceAttribute> seenRefs)
		{
		}

		private static bool IsDerivedFromBaseModel(Type type)
		{
			return false;
		}
	}
}

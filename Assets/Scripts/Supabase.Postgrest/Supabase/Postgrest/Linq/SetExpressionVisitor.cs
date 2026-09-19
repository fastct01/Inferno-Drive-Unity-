using System;
using System.Linq.Expressions;

namespace Supabase.Postgrest.Linq
{
	internal class SetExpressionVisitor : ExpressionVisitor
	{
		private string Column__BackingField;

		private Type ExpectedType__BackingField;

		private object Value__BackingField;

		public string Column
		{
			get
			{
				return Column__BackingField;
			}
			private set
			{
				Column__BackingField = value;
			}
		}

		public Type ExpectedType
		{
			get
			{
				return ExpectedType__BackingField;
			}
			private set
			{
				ExpectedType__BackingField = value;
			}
		}

		public object Value
		{
			get
			{
				return Value__BackingField;
			}
			private set
			{
				Value__BackingField = value;
			}
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			return null;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			return null;
		}

		protected override Expression VisitNew(NewExpression node)
		{
			return null;
		}

		private void HandleKeyValuePair(NewExpression node)
		{
		}

		private string GetColumnFromMemberExpression(MemberExpression node)
		{
			return null;
		}
	}
}

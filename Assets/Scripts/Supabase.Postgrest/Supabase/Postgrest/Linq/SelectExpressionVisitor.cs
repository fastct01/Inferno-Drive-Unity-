using System.Collections.Generic;
using System.Linq.Expressions;

namespace Supabase.Postgrest.Linq
{
	internal class SelectExpressionVisitor : ExpressionVisitor
	{
		private readonly List<string> Columns__BackingField;

		public List<string> Columns => Columns__BackingField;

		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			return null;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			return null;
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			return null;
		}

		private string GetColumnFromMemberExpression(MemberExpression node)
		{
			return null;
		}
	}
}

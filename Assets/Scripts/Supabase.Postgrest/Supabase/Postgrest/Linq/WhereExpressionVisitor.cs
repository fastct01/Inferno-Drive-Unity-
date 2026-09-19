using System.Collections.Generic;
using System.Linq.Expressions;

namespace Supabase.Postgrest.Linq
{
	internal class WhereExpressionVisitor : ExpressionVisitor
	{
		private QueryFilter Filter__BackingField;

		public QueryFilter Filter
		{
			get
			{
				return Filter__BackingField;
			}
			private set
			{
				Filter__BackingField = value;
			}
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			return null;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			return null;
		}

		private void HandleConstantExpression(string column, Constants.Operator op, ConstantExpression constantExpression)
		{
		}

		private void HandleMemberExpression(string column, Constants.Operator op, MemberExpression memberExpression)
		{
		}

		private void HandleUnaryExpression(string column, Constants.Operator op, UnaryExpression unaryExpression)
		{
		}

		private void HandleNewExpression(string column, Constants.Operator op, NewExpression newExpression)
		{
		}

		private string GetColumnFromMemberExpression(MemberExpression node)
		{
			return null;
		}

		private object GetMemberExpressionValue(MemberExpression member)
		{
			return null;
		}

		private Constants.Operator GetMappedOperator(Expression node)
		{
			return default;
		}

		private List<object> GetArgumentValues(MethodCallExpression methodCall)
		{
			return null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Supabase.Postgrest.Interfaces;

namespace Supabase.Postgrest
{
	public class QueryFilter<TModel, TCriterion> : IPostgrestQueryFilter
	{
		private readonly object Criteria__BackingField;

		private readonly Constants.Operator Op__BackingField;

		private readonly string Property__BackingField;

		public object Criteria => Criteria__BackingField;

		public Constants.Operator Op => Op__BackingField;

		public string Property => Property__BackingField;

		public QueryFilter(Expression<Func<TModel, object>> predicate, Constants.Operator op, TCriterion criterion)
		{
		}
	}
	public class QueryFilter : IPostgrestQueryFilter
	{
		public const string NullVal = "null";

		private string Property__BackingField;

		private Constants.Operator Op__BackingField;

		private object Criteria__BackingField;

		public string Property
		{
			get
			{
				return Property__BackingField;
			}
			private set
			{
				Property__BackingField = value;
			}
		}

		public Constants.Operator Op
		{
			get
			{
				return Op__BackingField;
			}
			private set
			{
				Op__BackingField = value;
			}
		}

		public object Criteria
		{
			get
			{
				return Criteria__BackingField;
			}
			private set
			{
				Criteria__BackingField = value;
			}
		}

		public QueryFilter(string property, Constants.Operator op, object criteria)
		{
		}

		public QueryFilter(string property, Constants.Operator op, FullTextSearchConfig fullTextSearchConfig)
		{
		}

		public QueryFilter(string property, Constants.Operator op, IntRange range)
		{
		}

		public QueryFilter(Constants.Operator op, List<IPostgrestQueryFilter> filters)
		{
		}

		public QueryFilter(Constants.Operator op, IPostgrestQueryFilter filter)
		{
		}
	}
}

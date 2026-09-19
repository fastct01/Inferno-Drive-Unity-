using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Postgrest.Responses;

namespace Supabase.Postgrest.Interfaces
{
	public interface IPostgrestTable<TModel> : IGettableHeaders where TModel : new()
	{
		string BaseUrl { get; }

		string TableName { get; }

		string GenerateUrl();

		IPostgrestTable<TModel> And(List<IPostgrestQueryFilter> filters);

		void Clear();

		IPostgrestTable<TModel> Columns(string[] columns);

		IPostgrestTable<TModel> Columns(Expression<Func<TModel, object[]>> predicate);

		Task<int> Count(Constants.CountType type, CancellationToken cancellationToken = default(CancellationToken));

		Task Delete(QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ModeledResponse<TModel>> Delete(TModel model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

		IPostgrestTable<TModel> Filter<TCriterion>(string columnName, Constants.Operator op, TCriterion criterion);

		IPostgrestTable<TModel> Filter<TCriterion>(Expression<Func<TModel, object>> predicate, Constants.Operator op, TCriterion criterion);

		Task<ModeledResponse<TModel>> Get(CancellationToken cancellationToken = default(CancellationToken));

		Task<ModeledResponse<TModel>> Insert(ICollection<TModel> models, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ModeledResponse<TModel>> Insert(TModel model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

		IPostgrestTable<TModel> Limit(int limit, string foreignTableName = null);

		IPostgrestTable<TModel> Match(Dictionary<string, string> query);

		IPostgrestTable<TModel> Match(TModel model);

		IPostgrestTable<TModel> Not(IPostgrestQueryFilter filter);

		IPostgrestTable<TModel> Not(string columnName, Constants.Operator op, Dictionary<string, object> criteria);

		IPostgrestTable<TModel> Not(Expression<Func<TModel, object>> predicate, Constants.Operator op, Dictionary<string, object> criteria);

		IPostgrestTable<TModel> Not<TCriterion>(string columnName, Constants.Operator op, List<TCriterion> criteria);

		IPostgrestTable<TModel> Not<TCriterion>(Expression<Func<TModel, object>> predicate, Constants.Operator op, List<TCriterion> criteria);

		IPostgrestTable<TModel> Not<TCriterion>(string columnName, Constants.Operator op, TCriterion criterion);

		IPostgrestTable<TModel> Not<TCriterion>(Expression<Func<TModel, object>> predicate, Constants.Operator op, TCriterion criterion);

		IPostgrestTable<TModel> Offset(int offset, string foreignTableName = null);

		IPostgrestTable<TModel> OnConflict(string columnName);

		IPostgrestTable<TModel> OnConflict(Expression<Func<TModel, object>> predicate);

		IPostgrestTable<TModel> Or(List<IPostgrestQueryFilter> filters);

		IPostgrestTable<TModel> Order(string column, Constants.Ordering ordering, Constants.NullPosition nullPosition = Constants.NullPosition.First);

		IPostgrestTable<TModel> Order(Expression<Func<TModel, object>> predicate, Constants.Ordering ordering, Constants.NullPosition nullPosition = Constants.NullPosition.First);

		IPostgrestTable<TModel> Order(string foreignTable, string column, Constants.Ordering ordering, Constants.NullPosition nullPosition = Constants.NullPosition.First);

		IPostgrestTable<TModel> Range(int from);

		IPostgrestTable<TModel> Range(int from, int to);

		IPostgrestTable<TModel> Select(string columnQuery);

		IPostgrestTable<TModel> Select(Expression<Func<TModel, object[]>> predicate);

		IPostgrestTable<TModel> Where(Expression<Func<TModel, bool>> predicate);

		Task<TModel> Single(CancellationToken cancellationToken = default(CancellationToken));

		IPostgrestTable<TModel> Set(Expression<Func<TModel, object>> keySelector, object value);

		IPostgrestTable<TModel> Set(Expression<Func<TModel, KeyValuePair<object, object>>> keyValuePairExpression);

		Task<ModeledResponse<TModel>> Update(QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ModeledResponse<TModel>> Update(TModel model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ModeledResponse<TModel>> Upsert(ICollection<TModel> model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ModeledResponse<TModel>> Upsert(TModel model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}

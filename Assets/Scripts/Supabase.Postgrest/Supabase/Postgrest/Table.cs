using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Core.Interfaces;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Responses;

namespace Supabase.Postgrest
{
	public class Table<TModel> : IPostgrestTable<TModel>, IGettableHeaders where TModel : new()
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CCount_003Ed__64 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<int> _003C_003Et__builder;

			public Table<TModel> _003C_003E4__this;

			public Constants.CountType type;

			public CancellationToken cancellationToken;

			private TaskAwaiter<BaseResponse> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSingle_003Ed__65 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<TModel> _003C_003Et__builder;

			public Table<TModel> _003C_003E4__this;

			public CancellationToken cancellationToken;

			private TaskAwaiter<ModeledResponse<TModel>> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		private readonly string BaseUrl__BackingField;

		private readonly string TableName__BackingField;

		private Func<Dictionary<string, string>> GetHeaders__BackingField;

		private readonly ClientOptions _options;

		private readonly JsonSerializerSettings _serializerSettings;

		private HttpMethod _method;

		private string _columnQuery;

		private readonly List<IPostgrestQueryFilter> _filters;

		private readonly List<QueryOrderer> _orderers;

		private readonly List<string> _columns;

		private readonly Dictionary<object, object> _setData;

		private readonly List<ReferenceAttribute> _references;

		private int _rangeFrom;

		private int _rangeTo;

		private int _limit;

		private string _limitForeignKey;

		private int _offset;

		private string _offsetForeignKey;

		private string _onConflict;

		public string BaseUrl => BaseUrl__BackingField;

		public string TableName => TableName__BackingField;

		public Func<Dictionary<string, string>> GetHeaders
		{
			get
			{
				return GetHeaders__BackingField;
			}
			set
			{
				GetHeaders__BackingField = value;
			}
		}

		public Table(string baseUrl, JsonSerializerSettings serializerSettings, ClientOptions options = null)
		{
		}

		public IPostgrestTable<TModel> Filter<TCriterion>(Expression<Func<TModel, object>> predicate, Constants.Operator op, TCriterion criterion)
		{
			return null;
		}

		public IPostgrestTable<TModel> Filter<TCriterion>(string columnName, Constants.Operator op, TCriterion criterion)
		{
			return null;
		}

		public IPostgrestTable<TModel> Not(IPostgrestQueryFilter filter)
		{
			return null;
		}

		public IPostgrestTable<TModel> Not<TCriterion>(string columnName, Constants.Operator op, TCriterion criterion)
		{
			return null;
		}

		public IPostgrestTable<TModel> Not<TCriterion>(Expression<Func<TModel, object>> predicate, Constants.Operator op, TCriterion criterion)
		{
			return null;
		}

		public IPostgrestTable<TModel> Not<TCriterion>(string columnName, Constants.Operator op, List<TCriterion> criteria)
		{
			return null;
		}

		public IPostgrestTable<TModel> Not<TCriterion>(Expression<Func<TModel, object>> predicate, Constants.Operator op, List<TCriterion> criteria)
		{
			return null;
		}

		public IPostgrestTable<TModel> Not(string columnName, Constants.Operator op, Dictionary<string, object> criteria)
		{
			return null;
		}

		public IPostgrestTable<TModel> Not(Expression<Func<TModel, object>> predicate, Constants.Operator op, Dictionary<string, object> criteria)
		{
			return null;
		}

		public IPostgrestTable<TModel> And(List<IPostgrestQueryFilter> filters)
		{
			return null;
		}

		public IPostgrestTable<TModel> Or(List<IPostgrestQueryFilter> filters)
		{
			return null;
		}

		public IPostgrestTable<TModel> Match(TModel model)
		{
			return null;
		}

		public IPostgrestTable<TModel> Match(Dictionary<string, string> query)
		{
			return null;
		}

		public IPostgrestTable<TModel> Order(Expression<Func<TModel, object>> predicate, Constants.Ordering ordering, Constants.NullPosition nullPosition = Constants.NullPosition.First)
		{
			return null;
		}

		public IPostgrestTable<TModel> Order(string column, Constants.Ordering ordering, Constants.NullPosition nullPosition = Constants.NullPosition.First)
		{
			return null;
		}

		public IPostgrestTable<TModel> Order(string foreignTable, string column, Constants.Ordering ordering, Constants.NullPosition nullPosition = Constants.NullPosition.First)
		{
			return null;
		}

		public IPostgrestTable<TModel> Range(int from)
		{
			return null;
		}

		public IPostgrestTable<TModel> Range(int from, int to)
		{
			return null;
		}

		public IPostgrestTable<TModel> Select(string columnQuery)
		{
			return null;
		}

		public IPostgrestTable<TModel> Select(Expression<Func<TModel, object[]>> predicate)
		{
			return null;
		}

		public IPostgrestTable<TModel> Where(Expression<Func<TModel, bool>> predicate)
		{
			return null;
		}

		public IPostgrestTable<TModel> Limit(int limit, string foreignTableName = null)
		{
			return null;
		}

		public IPostgrestTable<TModel> OnConflict(string columnName)
		{
			return null;
		}

		public IPostgrestTable<TModel> OnConflict(Expression<Func<TModel, object>> predicate)
		{
			return null;
		}

		public IPostgrestTable<TModel> Columns(string[] columns)
		{
			return null;
		}

		public IPostgrestTable<TModel> Columns(Expression<Func<TModel, object[]>> predicate)
		{
			return null;
		}

		public IPostgrestTable<TModel> Offset(int offset, string foreignTableName = null)
		{
			return null;
		}

		public Task<ModeledResponse<TModel>> Insert(TModel model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task<ModeledResponse<TModel>> Insert(ICollection<TModel> models, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task<ModeledResponse<TModel>> Upsert(TModel model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task<ModeledResponse<TModel>> Upsert(ICollection<TModel> model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public IPostgrestTable<TModel> Set(Expression<Func<TModel, object>> keySelector, object value)
		{
			return null;
		}

		public IPostgrestTable<TModel> Set(Expression<Func<TModel, KeyValuePair<object, object>>> keyValuePairExpression)
		{
			return null;
		}

		public Task<ModeledResponse<TModel>> Update(QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task<ModeledResponse<TModel>> Update(TModel model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task Delete(QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task<ModeledResponse<TModel>> Delete(TModel model, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task<int> Count(Constants.CountType type, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task<TModel> Single(CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public Task<ModeledResponse<TModel>> Get(CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public string GenerateUrl()
		{
			return null;
		}

		private object PrepareRequestData(object data, bool isInsert = false, bool isUpdate = false, bool isUpsert = false)
		{
			return null;
		}

		internal KeyValuePair<string, string> PrepareFilter(IPostgrestQueryFilter filter)
		{
			return default;
		}

		public void Clear()
		{
		}

		private Task<ModeledResponse<TModel>> PerformInsert(object data, QueryOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		private Task<BaseResponse> Send(HttpMethod method, object data, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken), bool isInsert = false, bool isUpdate = false, bool isUpsert = false)
		{
			return null;
		}

		private Task<ModeledResponse<TU>> Send<TU>(HttpMethod method, object data, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken), bool isInsert = false, bool isUpdate = false, bool isUpsert = false) where TU : new()
		{
			return null;
		}

		private static string FindTableName(object obj = null)
		{
			return null;
		}
	}
}

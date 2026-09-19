using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase.Functions.Interfaces;
using Supabase.Gotrue;
using Supabase.Postgrest;
using Supabase.Postgrest.Responses;
using Supabase.Storage;
using Supabase.Storage.Interfaces;

namespace Supabase
{
	public static class StatelessClient
	{
		private sealed class _003C_003Ec__DisplayClass3_0
		{
			public Dictionary<string, string> headers;

			internal Dictionary<string, string> _003CFunctions_003Eb__0()
			{
				return null;
			}
		}

		public static Supabase.Gotrue.ClientOptions GetAuthOptions(string supabaseUrl, string supabaseKey = null, SupabaseOptions options = null)
		{
			return null;
		}

		public static Supabase.Postgrest.ClientOptions GetRestOptions(string supabaseKey = null, SupabaseOptions options = null)
		{
			return null;
		}

		public static IStorageClient<Bucket, FileObject> Storage(string supabaseUrl, string supabaseKey = null, SupabaseOptions options = null)
		{
			return null;
		}

		public static IFunctionsClient Functions(string supabaseUrl, string supabaseKey, SupabaseOptions options = null)
		{
			return null;
		}

		public static SupabaseTable<T> From<T>(string supabaseUrl, string supabaseKey, SupabaseOptions options = null) where T : new()
		{
			return null;
		}

		public static Task<BaseResponse> Rpc(string supabaseUrl, string supabaseKey, string procedureName, Dictionary<string, object> parameters, SupabaseOptions options = null)
		{
			return null;
		}

		internal static Dictionary<string, string> GetAuthHeaders(string supabaseKey, SupabaseOptions options)
		{
			return null;
		}
	}
}

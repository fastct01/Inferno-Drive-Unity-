using System.Net.Http;

namespace Supabase.Postgrest.Responses
{
	public class BaseResponse
	{
		private HttpResponseMessage ResponseMessage__BackingField;

		private string Content__BackingField;

		private ClientOptions ClientOptions__BackingField;

		public HttpResponseMessage ResponseMessage
		{
			get
			{
				return ResponseMessage__BackingField;
			}
			set
			{
				ResponseMessage__BackingField = value;
			}
		}

		public string Content
		{
			get
			{
				return Content__BackingField;
			}
			set
			{
				Content__BackingField = value;
			}
		}

		public ClientOptions ClientOptions
		{
			get
			{
				return ClientOptions__BackingField;
			}
			set
			{
				ClientOptions__BackingField = value;
			}
		}

		public BaseResponse(ClientOptions clientOptions, HttpResponseMessage responseMessage, string content)
		{
		}
	}
}

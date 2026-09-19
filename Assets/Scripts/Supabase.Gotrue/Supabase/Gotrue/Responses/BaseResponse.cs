using System.Net.Http;

namespace Supabase.Gotrue.Responses
{
	public class BaseResponse
	{
		private HttpResponseMessage ResponseMessage__BackingField;

		private string Content__BackingField;

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
	}
}

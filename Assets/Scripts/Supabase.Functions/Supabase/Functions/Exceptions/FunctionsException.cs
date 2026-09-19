using System;
using System.Net.Http;

namespace Supabase.Functions.Exceptions
{
	public class FunctionsException : Exception
	{
		private HttpResponseMessage Response__BackingField;

		private string Content__BackingField;

		private int StatusCode__BackingField;

		private FailureHint.Reason Reason__BackingField;

		public HttpResponseMessage Response
		{
			get
			{
				return Response__BackingField;
			}
			internal set
			{
				Response__BackingField = value;
			}
		}

		public string Content
		{
			get
			{
				return Content__BackingField;
			}
			internal set
			{
				Content__BackingField = value;
			}
		}

		public int StatusCode
		{
			get
			{
				return StatusCode__BackingField;
			}
			internal set
			{
				StatusCode__BackingField = value;
			}
		}

		public FailureHint.Reason Reason
		{
			get
			{
				return Reason__BackingField;
			}
			internal set
			{
				Reason__BackingField = value;
			}
		}

		public FunctionsException(string message)
		{
		}

		public FunctionsException(string message, Exception innerException)
		{
		}

		public void AddReason()
		{
		}
	}
}

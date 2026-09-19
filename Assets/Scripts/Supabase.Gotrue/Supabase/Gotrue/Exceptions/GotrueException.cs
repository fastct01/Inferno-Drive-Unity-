using System;
using System.Net.Http;

namespace Supabase.Gotrue.Exceptions
{
	public class GotrueException : Exception
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
			private set
			{
				Reason__BackingField = value;
			}
		}

		public GotrueException(string message)
		{
		}

		public GotrueException(string message, Exception innerException)
		{
		}

		public GotrueException(string message, FailureHint.Reason reason)
		{
		}

		public GotrueException(string message, FailureHint.Reason reason, Exception innerException)
		{
		}

		public void AddReason()
		{
		}
	}
}

namespace Supabase.Storage
{
	public class ErrorResponse
	{
		private int StatusCode__BackingField;

		private string Message__BackingField;

		public int StatusCode
		{
			get
			{
				return StatusCode__BackingField;
			}
			set
			{
				StatusCode__BackingField = value;
			}
		}

		public string Message
		{
			get
			{
				return Message__BackingField;
			}
			set
			{
				Message__BackingField = value;
			}
		}
	}
}

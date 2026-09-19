using System;
using System.Collections.Generic;

namespace Supabase.Realtime.Socket
{
	public class SocketResponsePayload<T> : SocketResponsePayload where T : class
	{
		private T Record__BackingField;

		private T OldRecord__BackingField;

		public new T Record
		{
			get
			{
				return Record__BackingField;
			}
			set
			{
				Record__BackingField = value;
			}
		}

		public new T OldRecord
		{
			get
			{
				return OldRecord__BackingField;
			}
			set
			{
				OldRecord__BackingField = value;
			}
		}
	}
	public class SocketResponsePayload
	{
		private List<object> Columns__BackingField;

		private DateTimeOffset? CommitTimestamp__BackingField;

		private object Record__BackingField;

		private object OldRecord__BackingField;

		private string Schema__BackingField;

		private string Table__BackingField;

		private string _type__BackingField;

		private string Status__BackingField;

		private object Response__BackingField;

		private List<string> Errors__BackingField;

		private object Joins__BackingField;

		private object Leaves__BackingField;

		private string Channel__BackingField;

		private string Extension__BackingField;

		private string Message__BackingField;

		public List<object> Columns
		{
			get
			{
				return Columns__BackingField;
			}
			set
			{
				Columns__BackingField = value;
			}
		}

		public DateTimeOffset? CommitTimestamp
		{
			get
			{
				return CommitTimestamp__BackingField;
			}
			set
			{
				CommitTimestamp__BackingField = value;
			}
		}

		public object Record
		{
			get
			{
				return Record__BackingField;
			}
			set
			{
				Record__BackingField = value;
			}
		}

		public object OldRecord
		{
			get
			{
				return OldRecord__BackingField;
			}
			set
			{
				OldRecord__BackingField = value;
			}
		}

		public string Schema
		{
			get
			{
				return Schema__BackingField;
			}
			set
			{
				Schema__BackingField = value;
			}
		}

		public string Table
		{
			get
			{
				return Table__BackingField;
			}
			set
			{
				Table__BackingField = value;
			}
		}

		public string _type
		{
			get
			{
				return _type__BackingField;
			}
			set
			{
				_type__BackingField = value;
			}
		}

		public Constants.EventType Type => default;

		public string Status
		{
			get
			{
				return Status__BackingField;
			}
			set
			{
				Status__BackingField = value;
			}
		}

		public object Response
		{
			get
			{
				return Response__BackingField;
			}
			set
			{
				Response__BackingField = value;
			}
		}

		public List<string> Errors
		{
			get
			{
				return Errors__BackingField;
			}
			set
			{
				Errors__BackingField = value;
			}
		}

		public object Joins
		{
			get
			{
				return Joins__BackingField;
			}
			set
			{
				Joins__BackingField = value;
			}
		}

		public object Leaves
		{
			get
			{
				return Leaves__BackingField;
			}
			set
			{
				Leaves__BackingField = value;
			}
		}

		public string Channel
		{
			get
			{
				return Channel__BackingField;
			}
			set
			{
				Channel__BackingField = value;
			}
		}

		public string Extension
		{
			get
			{
				return Extension__BackingField;
			}
			set
			{
				Extension__BackingField = value;
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

using System;

namespace Supabase.Core.Attributes
{
	public class MapToAttribute : Attribute
	{
		private string Mapping__BackingField;

		private string Formatter__BackingField;

		public string Mapping
		{
			get
			{
				return Mapping__BackingField;
			}
			set
			{
				Mapping__BackingField = value;
			}
		}

		public string Formatter
		{
			get
			{
				return Formatter__BackingField;
			}
			set
			{
				Formatter__BackingField = value;
			}
		}

		public MapToAttribute(string mapping, string formatter = null)
		{
		}
	}
}

namespace Supabase.Storage
{
	public class TransformOptions
	{
		public enum ResizeType
		{
			Cover = 0,
			Contain = 1,
			Fill = 2
		}

		private int? Width__BackingField;

		private int? Height__BackingField;

		private ResizeType Resize__BackingField;

		private int Quality__BackingField;

		private string Format__BackingField;

		public int? Width
		{
			get
			{
				return Width__BackingField;
			}
			set
			{
				Width__BackingField = value;
			}
		}

		public int? Height
		{
			get
			{
				return Height__BackingField;
			}
			set
			{
				Height__BackingField = value;
			}
		}

		public ResizeType Resize
		{
			get
			{
				return Resize__BackingField;
			}
			set
			{
				Resize__BackingField = value;
			}
		}

		public int Quality
		{
			get
			{
				return Quality__BackingField;
			}
			set
			{
				Quality__BackingField = value;
			}
		}

		public string Format
		{
			get
			{
				return Format__BackingField;
			}
			set
			{
				Format__BackingField = value;
			}
		}
	}
}

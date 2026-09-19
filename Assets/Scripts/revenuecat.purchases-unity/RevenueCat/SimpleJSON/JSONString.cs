using System.Text;

namespace RevenueCat.SimpleJSON
{
	public class JSONString : JSONNode
	{
		private string m_Data;

		public override JSONNodeType Tag => default;

		public override bool IsString => false;

		public override string Value
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public override Enumerator GetEnumerator()
		{
			return default;
		}

		public JSONString(string aData)
		{
		}

		public override JSONNode Clone()
		{
			return null;
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
		}

		public override bool Equals(object obj)
		{
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}

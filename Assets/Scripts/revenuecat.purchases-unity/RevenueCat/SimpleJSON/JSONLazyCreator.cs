using System.Text;

namespace RevenueCat.SimpleJSON
{
	internal class JSONLazyCreator : JSONNode
	{
		private JSONNode m_Node;

		private string m_Key;

		public override JSONNodeType Tag => default;

		// C# has no syntax for parameterized property 'Item'.
		public override JSONNode get_Item(int aIndex)
		{
			return null;
		}

		public override void set_Item(int aIndex, JSONNode value)
		{
		}

		// C# has no syntax for parameterized property 'Item'.
		public override JSONNode get_Item(string aKey)
		{
			return null;
		}

		public override void set_Item(string aKey, JSONNode value)
		{
		}

		public override int AsInt
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public override float AsFloat
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public override double AsDouble
		{
			get
			{
				return 0.0;
			}
			set
			{
			}
		}

		public override long AsLong
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public override bool AsBool
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public override JSONArray AsArray => null;

		public override JSONObject AsObject => null;

		public override Enumerator GetEnumerator()
		{
			return default;
		}

		public JSONLazyCreator(JSONNode aNode)
		{
		}

		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
		}

		private T Set<T>(T aVal)
		{
			return default;
		}

		public override void Add(JSONNode aItem)
		{
		}

		public override void Add(string aKey, JSONNode aItem)
		{
		}

		public static bool operator ==(JSONLazyCreator a, object b)
		{
			return false;
		}

		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return false;
		}

		public override bool Equals(object obj)
		{
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
		}
	}
}

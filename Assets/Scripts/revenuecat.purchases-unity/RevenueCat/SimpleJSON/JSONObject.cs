using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RevenueCat.SimpleJSON
{
	public class JSONObject : JSONNode
	{
		private sealed class _003C_003Ec__DisplayClass21_0
		{
			public JSONNode aNode;

			internal bool _003CRemove_003Eb__0(KeyValuePair<string, JSONNode> k)
			{
				return false;
			}
		}

		private sealed class _003Cget_Children_003Ed__26 : IEnumerable<JSONNode>, IEnumerable, IEnumerator<JSONNode>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private JSONNode _003C_003E2__current;

			private int _003C_003El__initialThreadId;

			public JSONObject _003C_003E4__this;

			private Dictionary<string, JSONNode>.Enumerator _003C_003E7__wrap1;

			JSONNode IEnumerator<JSONNode>.Current => null;

			object IEnumerator.Current => null;

			public _003Cget_Children_003Ed__26(int _003C_003E1__state)
			{
			}

			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				return false;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			private void _003C_003Em__Finally1()
			{
			}

			void IEnumerator.Reset()
			{
			}

			IEnumerator<JSONNode> IEnumerable<JSONNode>.GetEnumerator()
			{
				return null;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return null;
			}
		}

		private Dictionary<string, JSONNode> m_Dict;

		private bool inline;

		public override bool Inline
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public override JSONNodeType Tag => default;

		public override bool IsObject => false;

		// C# has no syntax for parameterized property 'Item'.
		public override JSONNode get_Item(string aKey)
		{
			return null;
		}

		public override void set_Item(string aKey, JSONNode value)
		{
		}

		// C# has no syntax for parameterized property 'Item'.
		public override JSONNode get_Item(int aIndex)
		{
			return null;
		}

		public override void set_Item(int aIndex, JSONNode value)
		{
		}

		public override int Count => 0;

		public override IEnumerable<JSONNode> Children => null;

		public override Enumerator GetEnumerator()
		{
			return default;
		}

		public override void Add(string aKey, JSONNode aItem)
		{
		}

		public override JSONNode Remove(string aKey)
		{
			return null;
		}

		public override JSONNode Remove(int aIndex)
		{
			return null;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			return null;
		}

		public override JSONNode Clone()
		{
			return null;
		}

		public override bool HasKey(string aKey)
		{
			return false;
		}

		public override JSONNode GetValueOrDefault(string aKey, JSONNode aDefault)
		{
			return null;
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
		}
	}
}

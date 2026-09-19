using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RevenueCat.SimpleJSON
{
	public class JSONArray : JSONNode
	{
		private sealed class _003Cget_Children_003Ed__23 : IEnumerable<JSONNode>, IEnumerable, IEnumerator<JSONNode>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private JSONNode _003C_003E2__current;

			private int _003C_003El__initialThreadId;

			public JSONArray _003C_003E4__this;

			private List<JSONNode>.Enumerator _003C_003E7__wrap1;

			JSONNode IEnumerator<JSONNode>.Current => null;

			object IEnumerator.Current => null;

			public _003Cget_Children_003Ed__23(int _003C_003E1__state)
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

		private List<JSONNode> m_List;

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

		public override bool IsArray => false;

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

		public override int Count => 0;

		public override IEnumerable<JSONNode> Children => null;

		public override Enumerator GetEnumerator()
		{
			return default;
		}

		public override void Add(string aKey, JSONNode aItem)
		{
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

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
		}
	}
}

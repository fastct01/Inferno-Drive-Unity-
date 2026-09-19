using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RevenueCat.SimpleJSON
{
	public abstract class JSONNode
	{
		public struct Enumerator
		{
			private enum Type
			{
				None = 0,
				Array = 1,
				Object = 2
			}

			private Type type;

			private Dictionary<string, JSONNode>.Enumerator m_Object;

			private List<JSONNode>.Enumerator m_Array;

			public bool IsValid => false;

			public KeyValuePair<string, JSONNode> Current => default;

			public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				type = default;
				m_Object = default;
				m_Array = default;
			}

			public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				type = default;
				m_Object = default;
				m_Array = default;
			}

			public bool MoveNext()
			{
				return false;
			}
		}

		public struct ValueEnumerator
		{
			private Enumerator m_Enumerator;

			public JSONNode Current => null;

			public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				m_Enumerator = default;
			}

			public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				m_Enumerator = default;
			}

			public ValueEnumerator(Enumerator aEnumerator)
			{
				m_Enumerator = default;
			}

			public bool MoveNext()
			{
				return false;
			}

			public ValueEnumerator GetEnumerator()
			{
				return default;
			}
		}

		public struct KeyEnumerator
		{
			private Enumerator m_Enumerator;

			public string Current => null;

			public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				m_Enumerator = default;
			}

			public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				m_Enumerator = default;
			}

			public KeyEnumerator(Enumerator aEnumerator)
			{
				m_Enumerator = default;
			}

			public bool MoveNext()
			{
				return false;
			}

			public KeyEnumerator GetEnumerator()
			{
				return default;
			}
		}

		public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerator, IDisposable, IEnumerable<KeyValuePair<string, JSONNode>>, IEnumerable
		{
			private JSONNode m_Node;

			private Enumerator m_Enumerator;

			public KeyValuePair<string, JSONNode> Current => default;

			object IEnumerator.Current => null;

			internal LinqEnumerator(JSONNode aNode)
			{
			}

			public bool MoveNext()
			{
				return false;
			}

			public void Dispose()
			{
			}

			public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
			{
				return null;
			}

			public void Reset()
			{
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return null;
			}
		}

		private sealed class _003Cget_Children_003Ed__42 : IEnumerable<JSONNode>, IEnumerable, IEnumerator<JSONNode>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private JSONNode _003C_003E2__current;

			private int _003C_003El__initialThreadId;

			JSONNode IEnumerator<JSONNode>.Current => null;

			object IEnumerator.Current => null;

			public _003Cget_Children_003Ed__42(int _003C_003E1__state)
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

		private sealed class _003Cget_DeepChildren_003Ed__44 : IEnumerable<JSONNode>, IEnumerable, IEnumerator<JSONNode>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private JSONNode _003C_003E2__current;

			private int _003C_003El__initialThreadId;

			public JSONNode _003C_003E4__this;

			private IEnumerator<JSONNode> _003C_003E7__wrap1;

			private IEnumerator<JSONNode> _003C_003E7__wrap2;

			JSONNode IEnumerator<JSONNode>.Current => null;

			object IEnumerator.Current => null;

			public _003Cget_DeepChildren_003Ed__44(int _003C_003E1__state)
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

			private void _003C_003Em__Finally2()
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

		public static bool forceASCII;

		public static bool longAsString;

		public static bool allowLineComments;

		private static StringBuilder m_EscapeBuilder;

		public abstract JSONNodeType Tag { get; }

		// C# has no syntax for parameterized property 'Item'.
		public virtual JSONNode get_Item(int aIndex)
		{
			return null;
		}

		public virtual void set_Item(int aIndex, JSONNode value)
		{
		}

		// C# has no syntax for parameterized property 'Item'.
		public virtual JSONNode get_Item(string aKey)
		{
			return null;
		}

		public virtual void set_Item(string aKey, JSONNode value)
		{
		}

		public virtual string Value
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual int Count => 0;

		public virtual bool IsNumber => false;

		public virtual bool IsString => false;

		public virtual bool IsBoolean => false;

		public virtual bool IsNull => false;

		public virtual bool IsArray => false;

		public virtual bool IsObject => false;

		public virtual bool Inline
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual IEnumerable<JSONNode> Children => null;

		public IEnumerable<JSONNode> DeepChildren => null;

		public IEnumerable<KeyValuePair<string, JSONNode>> Linq => null;

		public KeyEnumerator Keys => default;

		public ValueEnumerator Values => default;

		public virtual double AsDouble
		{
			get
			{
				return 0.0;
			}
			set
			{
			}
		}

		public virtual int AsInt
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public virtual float AsFloat
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public virtual bool AsBool
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual long AsLong
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public virtual JSONArray AsArray => null;

		public virtual JSONObject AsObject => null;

		internal static StringBuilder EscapeBuilder => null;

		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		public virtual void Add(JSONNode aItem)
		{
		}

		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return null;
		}

		public virtual JSONNode Clone()
		{
			return null;
		}

		public virtual bool HasKey(string aKey)
		{
			return false;
		}

		public virtual JSONNode GetValueOrDefault(string aKey, JSONNode aDefault)
		{
			return null;
		}

		public override string ToString()
		{
			return null;
		}

		public virtual string ToString(int aIndent)
		{
			return null;
		}

		internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

		public abstract Enumerator GetEnumerator();

		public static implicit operator JSONNode(string s)
		{
			return null;
		}

		public static implicit operator string(JSONNode d)
		{
			return null;
		}

		public static implicit operator JSONNode(double n)
		{
			return null;
		}

		public static implicit operator double(JSONNode d)
		{
			return 0.0;
		}

		public static implicit operator JSONNode(float n)
		{
			return null;
		}

		public static implicit operator float(JSONNode d)
		{
			return 0f;
		}

		public static implicit operator JSONNode(int n)
		{
			return null;
		}

		public static implicit operator int(JSONNode d)
		{
			return 0;
		}

		public static implicit operator JSONNode(long n)
		{
			return null;
		}

		public static implicit operator long(JSONNode d)
		{
			return 0L;
		}

		public static implicit operator JSONNode(bool b)
		{
			return null;
		}

		public static implicit operator bool(JSONNode d)
		{
			return false;
		}

		public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue)
		{
			return null;
		}

		public static bool operator ==(JSONNode a, object b)
		{
			return false;
		}

		public static bool operator !=(JSONNode a, object b)
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

		internal static string Escape(string aText)
		{
			return null;
		}

		private static JSONNode ParseElement(string token, bool quoted)
		{
			return null;
		}

		public static JSONNode Parse(string aJSON)
		{
			return null;
		}
	}
}

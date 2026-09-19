using System;
using System.Collections;
using System.Collections.Generic;

namespace Supabase.Postgrest.Extensions
{
	internal static class TypeExtensions
	{
		private sealed class _003CGetInheritanceHierarchy_003Ed__0 : IEnumerable<Type>, IEnumerable, IEnumerator<Type>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private Type _003C_003E2__current;

			private int _003C_003El__initialThreadId;

			private Type type;

			public Type _003C_003E3__type;

			private Type _003Ccurrent_003E5__2;

			Type IEnumerator<Type>.Current => null;

			object IEnumerator.Current => null;

			public _003CGetInheritanceHierarchy_003Ed__0(int _003C_003E1__state)
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

			IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
			{
				return null;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return null;
			}
		}

		internal static IEnumerable<Type> GetInheritanceHierarchy(Type type)
		{
			return null;
		}
	}
}

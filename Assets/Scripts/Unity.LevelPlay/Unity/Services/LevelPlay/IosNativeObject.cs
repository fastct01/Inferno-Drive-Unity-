using System;
using System.Collections.Concurrent;

namespace Unity.Services.LevelPlay
{
	internal abstract class IosNativeObject : IDisposable
	{
		private static ConcurrentDictionary<IntPtr, IosNativeObject> s_Objects;

		private IntPtr m_NativePtr;

		private readonly bool m_UsesCallbacks;

		public IntPtr NativePtr
		{
			get
			{
				return (IntPtr)0;
			}
			protected set
			{
			}
		}

		protected IosNativeObject(bool usesCallbacks)
		{
		}

		protected static T Get<T>(IntPtr ptr)
		{
			return default;
		}

		public virtual void Dispose()
		{
		}

		protected bool CheckDisposedAndLogError(string message)
		{
			return false;
		}
	}
}

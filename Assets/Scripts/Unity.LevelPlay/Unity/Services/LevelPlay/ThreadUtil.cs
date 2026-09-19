using System.Threading;

namespace Unity.Services.LevelPlay
{
	internal static class ThreadUtil
	{
		internal static SynchronizationContext UnitySynchronizationContext;

		private static void Init()
		{
		}

		public static void Post(SendOrPostCallback d, object state = null)
		{
		}

		public static void Send(SendOrPostCallback d, object state = null)
		{
		}
	}
}

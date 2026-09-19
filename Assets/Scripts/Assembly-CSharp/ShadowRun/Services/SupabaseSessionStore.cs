using System;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace ShadowRun.Services
{
	internal sealed class SupabaseSessionStore : IGotrueSessionPersistence<Session>
	{
		private sealed class RestoreProtection : IDisposable
		{
			private SupabaseSessionStore owner;

			public RestoreProtection(SupabaseSessionStore owner)
			{
			}

			public void Dispose()
			{
			}
		}

		private readonly string sessionPath;

		private readonly string temporaryPath;

		private readonly string backupPath;

		private readonly object fileLock;

		private int restoreProtectionCount;

		public void SaveSession(Session session)
		{
		}

		public Session LoadSession()
		{
			return null;
		}

		public void DestroySession()
		{
		}

		public void DestroySessionExplicitly()
		{
		}

		private void DestroySessionFiles()
		{
		}

		public IDisposable ProtectDuringRestore()
		{
			return null;
		}

		private static Session TryReadSession(string path)
		{
			return null;
		}
	}
}

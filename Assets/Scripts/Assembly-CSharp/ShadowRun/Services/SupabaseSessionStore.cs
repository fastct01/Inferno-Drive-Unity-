using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__2.cpp:
//   SupabaseSessionStore__ctor_mF0C0102E5A99CAB9AA920E1741E603370143868F
//   RestoreProtection__ctor_m7BF543210FEBA24B11C7F16E00E74D00C84B9B01 / RestoreProtection_Dispose_mD5C663A9C7A823EC9009C270B33745BD2480E207
//   SupabaseSessionStore_SaveSession_m829B339EB53B8D01C743245872541A0504C009FA
//   SupabaseSessionStore_LoadSession_mB5CF8B0B88384B415AEA2044F3D64EF8C822AF42
//   SupabaseSessionStore_DestroySession_m47982AC4F37F449E3EA4D94A6B42C2C7AC999E32
//   SupabaseSessionStore_DestroySessionExplicitly_m726BF498D13D8076DCBCFA5B37860014C63B9725
//   SupabaseSessionStore_DestroySessionFiles_mDDABCA224B55209B0367D15FDCD71A9B27AB7E9C
//   SupabaseSessionStore_ProtectDuringRestore_mBCE2CA14A7F412EA709C9C4DCC248C31AC734E4C
//   SupabaseSessionStore_TryReadSession_m5B2C8FFB7FE1B70C1F4631ED32AE78F0BAEA23B7
namespace ShadowRun.Services
{
	internal sealed class SupabaseSessionStore : IGotrueSessionPersistence<Session>
	{
		private sealed class RestoreProtection : IDisposable
		{
			private SupabaseSessionStore owner;

			public RestoreProtection(SupabaseSessionStore owner)
			{
				this.owner = owner;
			}

			public void Dispose()
			{
				SupabaseSessionStore store = Interlocked.Exchange(ref owner, null);
				if (store != null)
				{
					Interlocked.Decrement(ref store.restoreProtectionCount);
				}
			}
		}

		private readonly string sessionPath;

		private readonly string temporaryPath;

		private readonly string backupPath;

		private readonly object fileLock = new object();

		private int restoreProtectionCount;

		public SupabaseSessionStore()
		{
			sessionPath = Path.Combine(Application.persistentDataPath, "supabase-session.json");
			temporaryPath = sessionPath + ".tmp";
			backupPath = sessionPath + ".backup";
		}

		public void SaveSession(Session session)
		{
			if (session == null)
			{
				Debug.LogWarning("[Supabase] Ignored an empty transient session save.");
				return;
			}
			try
			{
				lock (fileLock)
				{
					string json = JsonConvert.SerializeObject(session);
					File.WriteAllText(temporaryPath, json);
					if (File.Exists(sessionPath))
					{
						try
						{
							File.Replace(temporaryPath, sessionPath, null);
						}
						catch (Exception ex) when (ex is PlatformNotSupportedException || ex is IOException)
						{
							File.Copy(temporaryPath, sessionPath, true);
							File.Delete(temporaryPath);
						}
					}
					else
					{
						File.Move(temporaryPath, sessionPath);
					}
					File.Copy(sessionPath, backupPath, true);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[Supabase] Не удалось сохранить сессию: " + ex.Message);
			}
		}

		public Session LoadSession()
		{
			try
			{
				lock (fileLock)
				{
					Session session = TryReadSession(sessionPath);
					if (session != null)
					{
						return session;
					}
					session = TryReadSession(backupPath);
					if (session != null)
					{
						File.Copy(backupPath, sessionPath, true);
						Debug.LogWarning("[Supabase] Основной файл сессии восстановлен из резервной копии.");
					}
					return session;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[Supabase] Не удалось восстановить сессию: " + ex.Message);
			}
			return null;
		}

		public void DestroySession()
		{
			if (Volatile.Read(ref restoreProtectionCount) > 0)
			{
				Debug.Log("[Supabase] Preserved persisted session during token restore.");
				return;
			}
			DestroySessionFiles();
		}

		public void DestroySessionExplicitly()
		{
			DestroySessionFiles();
		}

		private void DestroySessionFiles()
		{
			try
			{
				lock (fileLock)
				{
					if (File.Exists(sessionPath))
					{
						File.Delete(sessionPath);
					}
					if (File.Exists(temporaryPath))
					{
						File.Delete(temporaryPath);
					}
					if (File.Exists(backupPath))
					{
						File.Delete(backupPath);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[Supabase] Не удалось удалить сессию: " + ex.Message);
			}
		}

		public IDisposable ProtectDuringRestore()
		{
			Interlocked.Increment(ref restoreProtectionCount);
			return new RestoreProtection(this);
		}

		private static Session TryReadSession(string path)
		{
			if (!File.Exists(path))
			{
				return null;
			}
			try
			{
				Session session = JsonConvert.DeserializeObject<Session>(File.ReadAllText(path));
				return session != null && !string.IsNullOrWhiteSpace(session.RefreshToken) ? session : null;
			}
			catch
			{
			}
			return null;
		}
	}
}

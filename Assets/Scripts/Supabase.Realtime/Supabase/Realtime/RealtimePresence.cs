using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Realtime.Channel;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.Presence;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime
{
	public class RealtimePresence<TPresenceModel> : IRealtimePresence
	{
		private sealed class _003C_003Ec__DisplayClass19_0
		{
			public TaskCompletionSource<Push> tcs;

			public Push push;

			internal void _003CTrack_003Eg__Handler_007C0(IRealtimePush<RealtimeChannel, SocketResponse> chanel, SocketResponse response)
			{
			}

			internal void _003CTrack_003Eb__1(object sender, EventArgs args)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass20_0
		{
			public TaskCompletionSource<Push> tcs;

			public Push push;

			internal void _003CUntrack_003Eg__Handler_007C0(IRealtimePush<RealtimeChannel, SocketResponse> chanel, SocketResponse response)
			{
			}

			internal void _003CUntrack_003Eb__1(object sender, EventArgs args)
			{
			}
		}

		private Dictionary<string, List<TPresenceModel>> LastState__BackingField;

		private readonly Dictionary<string, List<TPresenceModel>> CurrentState__BackingField;

		private PresenceOptions _options;

		private SocketResponse _currentResponse;

		private readonly RealtimeChannel _channel;

		private readonly JsonSerializerSettings _serializerSettings;

		private readonly Dictionary<IRealtimePresence.EventType, List<IRealtimePresence.PresenceEventHandler>> _presenceEventListeners;

		public Dictionary<string, List<TPresenceModel>> LastState
		{
			get
			{
				return LastState__BackingField;
			}
			private set
			{
				LastState__BackingField = value;
			}
		}

		public Dictionary<string, List<TPresenceModel>> CurrentState => CurrentState__BackingField;

		public RealtimePresence(RealtimeChannel channel, PresenceOptions options, JsonSerializerSettings serializerSettings)
		{
		}

		public void AddPresenceEventHandler(IRealtimePresence.EventType eventType, IRealtimePresence.PresenceEventHandler handler)
		{
		}

		public void RemovePresenceEventHandlers(IRealtimePresence.EventType eventType, IRealtimePresence.PresenceEventHandler handler)
		{
		}

		public void ClearPresenceEventHandlers(IRealtimePresence.EventType? eventType = null)
		{
		}

		private void NotifyPresenceEventHandlers(IRealtimePresence.EventType eventType)
		{
		}

		public void TriggerSync(SocketResponse response)
		{
		}

		public void TriggerDiff(SocketResponse response)
		{
		}

		public Task<Push> Track(object payload, int timeoutMs = 10000)
		{
			return null;
		}

		public Task<Push> Untrack()
		{
			return null;
		}

		private void SetState()
		{
		}
	}
}

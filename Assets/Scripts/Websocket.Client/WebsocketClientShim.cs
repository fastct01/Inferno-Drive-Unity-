// Minimal compile-time shim for the Websocket.Client types the Supabase.Realtime
// stub references. The recovered project keeps SDK code as empty stubs; the real
// SDK (NuGet Websocket.Client / Supabase) replaces this when networking is
// restored.
using System;

namespace Websocket.Client
{
    public enum DisconnectionType { Exit, Lost, NoMessageReceived, Error, ByUser, ByServer }
    public enum ReconnectionType { Initial, Lost, NoMessageReceived, Error, ByUser, ByServer }

    public class DisconnectionInfo
    {
        public DisconnectionType Type;
        public int? CloseStatus;
        public string CloseStatusDescription;
        public string SubProtocol;
        public Exception Exception;
        public bool CancelReconnection;
        public bool CancelClosing;
    }

    public class ReconnectionInfo
    {
        public ReconnectionType Type;
    }

    public class ResponseMessage
    {
        public string Text;
        public byte[] Binary;
    }

    public class WebsocketClient : IDisposable
    {
        public WebsocketClient(Uri url) { }
        public void Dispose() { }
    }
}

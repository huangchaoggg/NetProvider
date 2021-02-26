using System;
using WebSocket4Net;

namespace NetProvider.Sock.Net.Inter
{
    public interface IWebSocketClient : ISocketClientBase
    {
        Uri Uri { get; set; }
        WebSocket WebSocket { get; }
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}

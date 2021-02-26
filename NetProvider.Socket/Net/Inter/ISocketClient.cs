using NetProvider.Sock.EventArgs;

using System;
using System.Net;

namespace NetProvider.Sock.Net.Inter
{
    public interface ISocketClient : ISocketClientBase
    {
        IPEndPoint IPEndPoint { get; set; }
        event EventHandler<SendMessageArgs> SendMessageEvent;
    }
}
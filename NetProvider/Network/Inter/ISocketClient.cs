using NetProvider.EventArgs;
using System;
using System.Net;
using System.Text;

namespace NetProvider.Network.Inter
{
    public interface ISocketClient: ISocketClientBase
    {
        IPEndPoint IPEndPoint { get; set; }
        event EventHandler<SendMessageArgs> SendMessageEvent;
    }
}
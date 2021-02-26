using NetProvider.Core.Channels;
using NetProvider.Sock.Net.Inter;

namespace NetProvider.Sock.Factory
{
    internal interface ISocketServiceChannel: IServiceChannel
    {
        ISocketClient SocketClient { get; }
    }
}

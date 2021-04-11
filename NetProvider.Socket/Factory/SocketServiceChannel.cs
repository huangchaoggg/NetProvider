
using NetProvider.Core;
using NetProvider.Core.Channels;
using NetProvider.Core.Extension;
using NetProvider.Sock.Net;
using NetProvider.Sock.Net.Inter;

using System.Threading.Tasks;

namespace NetProvider.Sock.Factory
{
    public class SocketServiceChannel: ISocketServiceChannel
    {
        public ISocketClient SocketClient { get; private set; }
        public SocketServiceChannel(string ip, int port)
        {
            this.SocketClient = new SocketClient(ip, port);
        }
        public void StartClient()
        {
            SocketClient.StartClientAndReceive();
        }
        public void Close()
        {
            SocketClient.Close();
        }

        public object Invok(Parameters parameters)
        {
            string value = parameters.ParametersInfo[0].ToJsonString();
            SocketClient.SendMessage(value);
            return 0;
        }

        public Task<object> InvokAsync(Parameters parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}

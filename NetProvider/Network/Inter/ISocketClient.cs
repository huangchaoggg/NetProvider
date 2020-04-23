using NetProvider.EventArgs;
using System;
using System.Net;
using System.Text;

namespace NetProvider.Network.Inter
{
    public interface ISocketClient
    {
        IPEndPoint IPEndPoint { get; set; }

        event EventHandler<ProviderException> ExceptionEvent;
        event EventHandler<ReceiveMessageArgs> ReceiveMessageEvent;
        event EventHandler<SendMessageArgs> SendMessageEvent;

        void Close();
        void EndReceiveMessage();
        void ReceiveMessage();
        void SendMessage(byte[] buffer);
        void SendMessage(string strMsg);
        void SendMessage(string strMsg, Encoding encoding);
        void StartClient();
    }
}
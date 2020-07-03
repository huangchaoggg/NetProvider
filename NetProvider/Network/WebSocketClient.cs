using NetProvider.EventArgs;
using NetProvider.Network.Inter;
using System;
using System.Text;
using WebSocket4Net;

namespace NetProvider.Network
{
    public class WebSocketClient : IWebSocketClient
    {
        public WebSocketClient() { }
        public WebSocketClient(Uri uri)
        {
            this.Uri = uri;
        }
        public Uri Uri { get; set; }
        public WebSocket WebSocket { get; private set; }
        /// <summary>
        /// 请求重试次数
        /// </summary>
        public byte ReconnectionNumber { get; set; } = 4;
        public event EventHandler<System.EventArgs> ConnectEvent;
        public event EventHandler<ProviderException> ExceptionEvent;
        public event EventHandler<ReceiveMessageArgs> ReceiveMessageEvent;
        public event EventHandler<SendMessageArgs> SendMessageEvent;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public void Close()
        {
            WebSocket?.Close();
        }

        public void SendMessage(byte[] buffer)
        {
            WebSocket?.Send(buffer, 0, buffer.Length);
        }

        public void SendMessage(string strMsg)
        {
            WebSocket.Send(strMsg);
        }

        public void SendMessage(string strMsg, Encoding encoding)
        {
            SendMessage(encoding.GetBytes(strMsg));
        }
        private bool Reconnection(int index = 0)
        {
            if (index >= ReconnectionNumber)
            {
                return false;
            }
            if (WebSocket == null ||
                WebSocket.State == WebSocketState.Closed ||
                WebSocket.State == WebSocketState.Closing)
            {
                WebSocket = new WebSocket(Uri.AbsoluteUri);
                WebSocket.DataReceived += WebSocket_DataReceived;
                WebSocket.MessageReceived += WebSocket_MessageReceived;
                WebSocket.Opened += WebSocket_Opened;
                //WebSocket.ReceiveBufferSize = 4096;
                WebSocket.Error += WebSocket_Error;
                //WebSocket.AutoSendPingInterval = 20;
            }
            try
            {
                WebSocket?.Open();
                return true;

            }
            catch (Exception e)
            {
                Close();
                return Reconnection(index++);
            }
        }

        private void WebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            if (WebSocket.State == WebSocketState.Closed || WebSocket.State == WebSocketState.Closing)
            {
                if (!Reconnection())
                {
                    ExceptionEvent?.Invoke(this, new ProviderException("websocket重连失败", e.Exception));
                }
            }
        }

        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        private void WebSocket_Opened(object sender, System.EventArgs e)
        {
            ConnectEvent?.Invoke(this, new System.EventArgs());
        }

        private void WebSocket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            ReceiveMessageEvent?.Invoke(this, new ReceiveMessageArgs(e.Data));
        }

        public void StartClientAndReceive()
        {
            if (!Reconnection())
            {
                ExceptionEvent?.Invoke(this, new ProviderException("websocket连接不成功"));
            }
        }
    }
}

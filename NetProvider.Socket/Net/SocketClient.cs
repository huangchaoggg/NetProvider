using NetProvider.Core;
using NetProvider.Sock.EventArgs;
using NetProvider.Sock.Net.Inter;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetProvider.Sock.Net
{
    public class SocketClient : ISocketClient
    {
        public SocketClient(string ipString, int port) :
            this(new IPEndPoint(IPAddress.Parse(ipString), port))
        {
        }

        public SocketClient(IPEndPoint iPEndPoint)
        {
            this.IPEndPoint = iPEndPoint;
            ExceptionEvent += SocketClient_ExceptionEvent;
        }
        private byte number = 0;
        private void SocketClient_ExceptionEvent(object sender, ProviderException e)
        {
            if (e.InnerException is SocketException)
            {
                Reconnection();
                number++;
            }
        }

        public IPEndPoint IPEndPoint { get; set; }
        public byte ReconnectionNumber { get; set; } = 3;
        #region 内部属性
        private Socket socket;
        private bool isReceive = true;
        public bool IsReceive
        {
            get => isReceive;
            set
            {
                isReceive = value;
                if (value)
                    ReceiveMessage();
            }
        }
        private bool IsSocketConnected()
        {
            bool part1 = socket.Poll(2000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 短线重连
        /// </summary>
        /// <param name="number"></param>
        private void Reconnection()
        {
            if (this.number < this.ReconnectionNumber)
            {
                StartClientAndReceive();
            }
            else
            {
                this.number = 0;
            }
        }
        #endregion
        /// <summary>
        /// 开始连接
        /// </summary>
        public void StartClientAndReceive()
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.BeginConnect(IPEndPoint, asyncResult =>
                {
                    try
                    {
                        if (asyncResult.IsCompleted)
                        {
                            socket.EndConnect(asyncResult);
                            isReceive = true;
                            ConnectEvent?.Invoke(this, new System.EventArgs());
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionEvent?.Invoke(this, new ProviderException(e.Message, e));
                    }

                }, socket);
            }
            catch (Exception e)
            {
                ExceptionEvent?.Invoke(this, new ProviderException(e.Message, e));
            }
        }
        public void SendMessage(byte[] buffer)
        {
            try
            {
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        if (asyncResult.IsCompleted)
                        {
                            int sendLength = socket.EndSend(asyncResult);
                            SendMessageEvent?.Invoke(this, new SendMessageArgs(buffer, sendLength));
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionEvent?.Invoke(this, new ProviderException(e.Message, e));
                    }
                }, socket);
            }
            catch (Exception e)
            {
                ExceptionEvent?.Invoke(this, new ProviderException(e.Message, e));
            }
        }
        public void SendMessage(string strMsg, Encoding encoding)
        {
            SendMessage(encoding.GetBytes(strMsg));
        }
        public void SendMessage(string strMsg)
        {
            SendMessage(strMsg, Encoding.UTF8);
        }

        private void ReceiveMessage()
        {
            try
            {

                byte[] buffer = new byte[1024 * 1024];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        if (asyncResult.IsCompleted)
                        {
                            int length = socket.EndReceive(asyncResult);
                            byte[] recBytes = new byte[length];
                            Array.Copy(buffer, 0, recBytes, 0, length);
                            if (IsReceive && IsSocketConnected())
                            {
                                ReceiveMessage();
                            }
                            if (length > 0)
                            {
                                ReceiveMessageEvent?.Invoke(this, new ReceiveMessageArgs(recBytes));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionEvent?.Invoke(this, new ProviderException(e.Message, e));
                    }
                }, socket);
            }
            catch (Exception e)
            {
                ExceptionEvent?.Invoke(this, new ProviderException(e.Message, e));
            }
        }
        public void Close()
        {
            try
            {
                isReceive = false;
                socket.Disconnect(true);
            }
            catch (Exception e)
            {
                ExceptionEvent?.Invoke(this, new ProviderException(e.Message, e));
            }
        }
        #region 事件处理

        public event EventHandler<System.EventArgs> ConnectEvent;
        public event EventHandler<ProviderException> ExceptionEvent;
        public event EventHandler<SendMessageArgs> SendMessageEvent;
        public event EventHandler<ReceiveMessageArgs> ReceiveMessageEvent;
        #endregion
    }
}

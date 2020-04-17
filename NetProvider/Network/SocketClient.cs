using NetProvider.EventArgs;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetProvider.Network
{
    public class SocketClient
    {
        public SocketClient(string ipString, int port):this(new IPEndPoint(IPAddress.Parse(ipString), port)) 
        {
        }

        public SocketClient(IPEndPoint iPEndPoint)
        {
            this.IPEndPoint = iPEndPoint;
        }
        public IPEndPoint IPEndPoint { get;set; }
        #region 内部属性
        private Socket socket;
        private bool isReceive=true;
        private bool IsSocketConnected()
        {
            bool part1 = socket.Poll(2000,SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        #endregion
        public void StartClient()
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
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionEvent?.Invoke(this, new ProviderException(e.Message));
                    }

                }, socket);
            }
            catch(Exception e)
            {
                ExceptionEvent?.Invoke(this, new ProviderException(e.Message));
            }
        }
        public void SendMessage(byte[] buffer)
        {
            try
            {
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, asyncResult => {
                    try
                    {
                        if (asyncResult.IsCompleted)
                        {
                            int sendLength= socket.EndSend(asyncResult);
                            SendMessageEvent?.Invoke(this, new SendMessageArgs(buffer, sendLength));
                        }
                    }catch(Exception e)
                    {
                        ExceptionEvent?.Invoke(this, new ProviderException(e.Message));
                    }
                }, socket);
            }catch(Exception e)
            {
                ExceptionEvent?.Invoke(this, new ProviderException(e.Message));
            }
        }
        public void SendMessage(string strMsg,Encoding encoding)
        {
            SendMessage(encoding.GetBytes(strMsg));
        }
        public void SendMessage(string strMsg)
        {
            SendMessage(strMsg,Encoding.UTF8);
        }

        public void ReceiveMessage()
        {
            try
            {
                isReceive = true;
                byte[] buffer = new byte[1024 * 1024 * 2];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, asyncResult => {
                    try
                    {
                        if (asyncResult.IsCompleted)
                        {
                            int length= socket.EndReceive(asyncResult);
                            byte[] recBytes = new byte[length];
                            Array.Copy(buffer,0, recBytes,0,length);
                            if (isReceive&&IsSocketConnected())
                            {
                                ReceiveMessage();
                            }
                            if (length > 0)
                            {
                                ReceiveMessageEvent?.Invoke(this, new ReceiveMessageArgs(recBytes));
                            }
                        }
                    }catch(Exception e)
                    {
                        ExceptionEvent?.Invoke(this, new ProviderException(e.Message));
                    }
                }, socket);
            }
            catch(Exception e)
            {
                ExceptionEvent?.Invoke(this, new ProviderException(e.Message));
            }
        }
        public void EndReceiveMessage()
        {
            isReceive = false;
        }

        public void Close()
        {
            try
            {
                EndReceiveMessage();
                socket.Disconnect(true);
            }catch(Exception e)
            {
                ExceptionEvent?.Invoke(this, new ProviderException(e.Message));
            }
        }
        #region 事件处理

        public event EventHandler<ProviderException> ExceptionEvent;
        public event EventHandler<SendMessageArgs> SendMessageEvent;
        public event EventHandler<ReceiveMessageArgs> ReceiveMessageEvent;
        #endregion
    }
}

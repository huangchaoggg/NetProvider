﻿using NetProvider.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetProvider.Network.Inter
{
    public interface ISocketClientBase
    {
        byte ReconnectionNumber { get; set; }
        event EventHandler<System.EventArgs> ConnectEvent;
        event EventHandler<ProviderException> ExceptionEvent;
        event EventHandler<ReceiveMessageArgs> ReceiveMessageEvent;

        void Close();
        void SendMessage(byte[] buffer);
        void SendMessage(string strMsg);
        void SendMessage(string strMsg, Encoding encoding);
        void StartClientAndReceive();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace NetProvider.EventArgs
{
    public sealed class SendMessageArgs
    {
        public SendMessageArgs(byte[] sendBuffer, int sendLength)
        {
            SendBuffer = sendBuffer;
            SendLength = sendLength;
        }

        public byte[] SendBuffer { get; private set; }
        public int SendLength { get; private set; }
    }
}

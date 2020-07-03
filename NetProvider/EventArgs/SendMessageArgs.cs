namespace NetProvider.EventArgs
{
    public sealed class SendMessageArgs
    {
        public SendMessageArgs(byte[] sendBuffer, int sendLength)
        {
            SendBuffer = sendBuffer;
            SendLength = sendLength;
        }
        public SendMessageArgs(string sendMsg)
        {
            SendMsg = sendMsg;
        }
        public string SendMsg { get; set; }
        public byte[] SendBuffer { get; private set; }
        public int SendLength { get; private set; }
    }
}

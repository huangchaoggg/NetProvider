using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NetProvider.EventArgs
{
    public sealed class ReceiveMessageArgs
    {
        public ReceiveMessageArgs(byte[] buffer)
        {
            Buffer = buffer;
        }

        public byte[] Buffer { get; private set; }
        public override string ToString()
        {
            return ToString(Encoding.UTF8);
        }
        public string ToString(Encoding encoding)
        {
            if(Buffer!=null&&Buffer.Length>0)
                return encoding.GetString(Buffer);
            return string.Empty;
        }
        public T ToObject<T>(Encoding encoding) where T:class
        {
            string str = ToString(encoding);
            if (string.IsNullOrEmpty(str))
                return null;
            return JsonConvert.DeserializeObject<T>(str);
        }
        public T ToObject<T>() where T : class
        {
            string str = ToString();
            if (string.IsNullOrEmpty(str))
                return null;
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}

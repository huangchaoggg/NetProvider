using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProvider
{
    public class ProviderException:Exception
    {
        public ProviderException(string mesg) : base(mesg) { }
        public ProviderException(string mesg,Exception innerException) : base(mesg, innerException) { }
    }
    public class MessageException : Exception
    {
        public MessageException(string mesg) : base(mesg) { }
        public MessageException(string mesg, Exception innerException) : base(mesg, innerException) { }
    }
}

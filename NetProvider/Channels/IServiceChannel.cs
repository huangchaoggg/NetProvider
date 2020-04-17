using System;
using System.Collections.Generic;
using System.Text;

namespace NetProvider.Channels
{
    public interface IServiceChannel
    {
        object Invok(Parameters parameters);
    }
    public interface ISocketServiceChannel:IServiceChannel
    {
        void StartClient();
        void Close();
    }
}

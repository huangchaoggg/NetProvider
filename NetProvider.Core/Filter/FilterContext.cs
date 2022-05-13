using NetProvider.Core.Channels;

using System;

namespace NetProvider.Core.Filter
{
    public class FilterContext
    {
        public Type RetType { get; private set; }
        public Parameters Parameters { get; private set; }
        public IServiceChannel ServiceChannel { get; private set; }

        public FilterContext(Type retType, Parameters parameters, IServiceChannel serviceChannel)
        {
            RetType = retType;
            Parameters = parameters;
            ServiceChannel = serviceChannel;
        }
    }
}

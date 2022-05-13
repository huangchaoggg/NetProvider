using NetProvider.Core.Channels;

using System;

namespace NetProvider.Core.Filter
{
    public class MessageFilterContext : FilterContext
    {
        public object Value { get; set; }
        public MessageFilterContext(Type retType, Parameters parameters, 
            IServiceChannel serviceChannel):base(retType,parameters,serviceChannel)
        {
        }
    }
    public class MessageFilterContext<T>: MessageFilterContext
    {
        new public T Value { get; set; }
        public MessageFilterContext(Type retType, Parameters parameters,
            IServiceChannel serviceChannel) : base(retType, parameters, serviceChannel)
        {
        }
    }
}

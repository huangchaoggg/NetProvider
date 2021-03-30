using NetProvider.Core.Channels;

using System;
using System.Collections.Generic;

namespace NetProvider.Core.Filter
{
    public class MessageFilterContext : FilterContext<IMessageFilter,MessageFilterContext>
    {
        public object RawValue { get; private set; }

        public object NewValue { get; private set; }
        public MessageFilterContext(Type retType, Parameters parameters, 
            IServiceChannel serviceChannel,
            LinkedListNode<IMessageFilter> filterNode):base(retType,parameters,serviceChannel, filterNode)
        {
        }
        public override object Invoke(FilterContext<IMessageFilter,MessageFilterContext> context, object value)
        {
            if (context != null) 
                RawValue = ((MessageFilterContext)context).RawValue;
            else
                RawValue = value;
            NewValue = value;
            return filter_node.Value.Filter(this);

        }
        protected override FilterContext<IMessageFilter,MessageFilterContext> CreateNextContext()
        {
            if (filter_node.Next == null) return null;
            var context = new MessageFilterContext(RetType, Parameters, ServiceChannel, filter_node.Next);
            return context;
        }
    }
}

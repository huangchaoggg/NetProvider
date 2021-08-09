using NetProvider.Core.Channels;

using System;
using System.Collections.Generic;

namespace NetProvider.Core.Filter
{
    public class MessageFilterContext : FilterContext<IMessageFilter,MessageFilterContext>
    {
        public object Value { get; internal set; }
        [Obsolete]
        public object RawValue { get; private set; }
        [Obsolete]
        public object NewValue { get; private set; }
        public MessageFilterContext(Type retType, Parameters parameters, 
            IServiceChannel serviceChannel,
            LinkedListNode<IMessageFilter> filterNode):base(retType,parameters,serviceChannel, filterNode)
        {
        }
        [Obsolete]
        public override object Invoke(FilterContext<IMessageFilter,MessageFilterContext> context, object value)
        {
            if (context != null)
            {
                Value = ((MessageFilterContext)context).Value;
                RawValue = Value;
            }
            else
            {
                Value = value;
                RawValue = value;
            }
            NewValue = value;
            return filter_node.Value.Filter(this);

        }
        protected override FilterContext<IMessageFilter,MessageFilterContext> CreateNextContext()
        {
            if (filter_node.Next == null) return null;
            var context = new MessageFilterContext(RetType, Parameters, ServiceChannel, filter_node.Next);
            return context;
        }

        public override object Next(object value)
        {
            MessageFilterContext fc = NextContext as MessageFilterContext;
            if (fc != null)
            {
                fc.Value = value;
                return fc.filter_node.Value.Filter(fc);
            }
            return value;
        }
    }
}

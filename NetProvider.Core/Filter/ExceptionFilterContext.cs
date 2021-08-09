
using NetProvider.Core.Channels;

using System;
using System.Collections.Generic;

namespace NetProvider.Core.Filter
{
    public class ExceptionFilterContext : FilterContext<IExceptionFilter,ExceptionFilterContext>
    {
        public bool ExceptionHandled { get; set; }

        public Exception Exception { get; internal set; }

        public ExceptionFilterContext(Type retType, Parameters parameters,
            IServiceChannel serviceChannel,
            LinkedListNode<IExceptionFilter> filterNode) : base(retType, parameters, serviceChannel, filterNode)
        {
        }
        public override object Invoke(FilterContext<IExceptionFilter, ExceptionFilterContext> context, object value)
        {
            if (context != null)
                ExceptionHandled = ((ExceptionFilterContext)context).ExceptionHandled;
            else
                ExceptionHandled = false;
            Exception = value as Exception;
            return filter_node.Value.Filter(this);
        }
        protected override FilterContext<IExceptionFilter, ExceptionFilterContext> CreateNextContext()
        {
            if (filter_node.Next == null) return null;
            var context= new ExceptionFilterContext(RetType, Parameters, ServiceChannel, filter_node.Next);
            context.ExceptionHandled = ExceptionHandled;
            return context;
        }
        public override object Next(object value)
        {
            ExceptionFilterContext fc = NextContext as ExceptionFilterContext;
            if (fc != null)
            {
                fc.Exception = value as Exception;
                return fc.filter_node.Value.Filter(fc);
            }
            return value;
        }
    }
}

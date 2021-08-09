using NetProvider.Core.Channels;

using System;
using System.Collections.Generic;

namespace NetProvider.Core.Filter
{
    public class FilterManagement
    {
        private LinkedList<IExceptionFilter> ExceptionFilters { get; } = new LinkedList<IExceptionFilter>();
        private LinkedList<IMessageFilter> MessageFilters { get; } = new LinkedList<IMessageFilter>();
        public object CallMessageFilter(object value, Type retType, Parameters parameters, IServiceChannel serviceChannel)
        {

            if (MessageFilters.Count == 0)
                return value;
            var message = new MessageFilterContext(retType, parameters, serviceChannel, MessageFilters.First);
            message.Value = value;
            return message.filter_node.Value.Filter(message);
        }
        public void AddMessageFilter(IMessageFilter filter)
        {
            if (!MessageFilters.Contains(filter))
            {
                MessageFilters.AddLast(filter);
            }
            else
            {
                MessageFilters.Find(filter).Value=filter;
            }
        }
        public void AddExceptionFilter(IExceptionFilter filter)
        {
            if (!ExceptionFilters.Contains(filter))
            {
                ExceptionFilters.AddLast(filter);
            }
            else
            {
                ExceptionFilters.Find(filter).Value = filter;
            }
        }
        public object CallExceptionFilter<Excp>(Excp value, 
            Type retType,
            Parameters parameters,
            IServiceChannel serviceChannel) where Excp: Exception
        {
            if (ExceptionFilters.Count == 0)
                 throw new ProviderException(value.GetBaseException().Message);
            var context= new ExceptionFilterContext(retType, parameters, serviceChannel, ExceptionFilters.First);
            context.Exception = value;
            var obj= context.Next(context);
            if (!context.ExceptionHandled)
            {
                throw new ProviderException(value.GetBaseException().Message);
            }
            return obj;
        }
    }
}

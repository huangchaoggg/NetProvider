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
            LinkedListNode<IMessageFilter> filterNode = MessageFilters.First;
            var message = new MessageFilterContext(retType, parameters, serviceChannel);
            while (filterNode != null) {
                var filter = filterNode.Value;
                message.Value = value;
                filter.Filter(message);
                filterNode = filterNode.Next;
            }
            return message.Value;
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
        public void CallExceptionFilter<Excp>(Excp value, 
            Type retType,
            Parameters parameters,
            IServiceChannel serviceChannel) where Excp: Exception
        {
            if (ExceptionFilters.Count == 0)
                 throw new ProviderException(value.GetBaseException().Message);

            LinkedListNode<IExceptionFilter> filterNode = ExceptionFilters.First;
            var context= new ExceptionFilterContext(retType, parameters, serviceChannel);
            context.Exception = value;
            context.ExceptionHandled = false;
            while (filterNode != null)
            {
                var filter = filterNode.Value;
                filter.Filter(context);
                filterNode = filterNode.Next;
            }
            if (!context.ExceptionHandled)
            {
                throw new ProviderException(value.GetBaseException().Message);
            }
        }
    }
}

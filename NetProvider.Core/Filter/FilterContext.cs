using NetProvider.Core.Channels;

using System;
using System.Collections.Generic;

namespace NetProvider.Core.Filter
{
    public abstract class FilterContext<T,V> where T:IFilter<V> where V: FilterContext<T, V>
    {
        protected internal LinkedListNode<T> filter_node;
        public Type RetType { get; private set; }
        public Parameters Parameters { get; private set; }
        public IServiceChannel ServiceChannel { get; private set; }

        public FilterContext<T,V> NextContext { get; private set; }
        public FilterContext(Type retType, Parameters parameters, IServiceChannel serviceChannel, LinkedListNode<T> filterNode)
        {
            RetType = retType;
            Parameters = parameters;
            ServiceChannel = serviceChannel;
            filter_node = filterNode;
            NextContext = CreateNextContext();
        }
        protected abstract FilterContext<T,V> CreateNextContext();

        [Obsolete("请使用 Next 方法")]
        public abstract object Invoke(FilterContext<T,V> context, object value);

        public abstract object Next(object value);
    }
}

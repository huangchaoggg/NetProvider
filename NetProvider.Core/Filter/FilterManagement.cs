using NetProvider.Core.Channels;

using System;
using System.Collections.Generic;

namespace NetProvider.Core.Filter
{
    public static class FilterManagement
    {
        public static object Filter(LinkedListNode<IFilter> Filters, object value, Type retType, Parameters parameters, IServiceChannel serviceChannel)
        {

            if (Filters == null)
                return value;

            object v = Filters.Value.Filter(value, retType, parameters, serviceChannel);
            return Filter(Filters.Next, v, retType, parameters, serviceChannel);

        }
    }
}

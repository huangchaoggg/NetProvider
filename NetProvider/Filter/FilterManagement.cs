using NetProvider.Channels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetProvider.Filter
{
    internal static class FilterManagement
    {
        internal static object Filter(List<IFilter> Filters, object value, Type retType, Parameters parameters, ServiceChannel serviceChannel, IList<IFilter> usFilter = null)
        {
            if (usFilter == null)
            {
                usFilter = new List<IFilter>();
            }
            IFilter filter = Filters.FirstOrDefault(s => s.IsFilter(value) && usFilter != null && !usFilter.Contains(s));
            if (filter == null)
            {
                return value;
            }
            else
            {
                object v = filter.Filter(value, retType, parameters, serviceChannel);
                usFilter.Add(filter);
                return Filter(Filters, v, retType, parameters, serviceChannel, usFilter);
            }
        }
    }
}

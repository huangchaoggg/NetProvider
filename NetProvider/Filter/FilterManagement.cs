using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using NetProvider.Channels;

namespace NetProvider.Filter
{
    public static class FilterManagement
    {
        public static List<IFilter> Filters { get; } = new List<IFilter>();

        internal static object Filter(object value,Type retType, Parameters parameters, ServiceChannel serviceChannel, IList<IFilter> usFilter=null)
        {
            if (usFilter == null)
            {
                usFilter = new List<IFilter>();
            }
            IFilter filter = Filters.FirstOrDefault(s => s.IsFilter(value)&& usFilter!=null&&!usFilter.Contains(s));
            if (filter == null)
            {
                return value;
            }
            else
            {
                object v = filter.Filter(value,retType,parameters, serviceChannel);
                usFilter.Add(filter);
                return Filter(v, retType,parameters, serviceChannel, usFilter);
            }
        }
    }
}

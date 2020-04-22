using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace NetProvider.Filter
{
    public static class FilterManagement
    {
        public static List<IFilter> Filters { get; } = new List<IFilter>();

        internal static object Filter(object value,Type retType,IList<IFilter> usFilter=null)
        {
            if (usFilter == null)
            {
                usFilter = new List<IFilter>();
            }
            //return Filter(value,value.GetType(),retType);
            IFilter filter = Filters.FirstOrDefault(s => s.IsFilter(value.GetType())&& usFilter!=null&&!usFilter.Contains(s));
            if (filter == null)
            {
                return value;
            }
            else
            {
                object v = filter.Filter(value, retType);
                usFilter.Add(filter);
                return Filter(v, retType,usFilter);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NetProvider.Filter
{
    public interface IFilter
    {
        bool IsFilter(Type sourceType);
        object Filter(object value,Type retType);
    }

}

using NetProvider.Channels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NetProvider.Filter
{
    public interface IFilter
    {
        bool IsFilter(object value);
        object Filter(object value,Type retType, Parameters parameters,ServiceChannel serviceChannel);
    }

}

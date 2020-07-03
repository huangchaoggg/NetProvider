using NetProvider.Channels;
using System;

namespace NetProvider.Filter
{
    public interface IFilter
    {
        bool IsFilter(object value);
        object Filter(object value, Type retType, Parameters parameters, ServiceChannel serviceChannel);
    }

}

using NetProvider.Core.Channels;

using System;

namespace NetProvider.Core.Filter
{
    public interface IFilter
    {
        bool IsFilter(object value);
        object Filter(object value, Type retType, Parameters parameters, IServiceChannel serviceChannel);
    }

}

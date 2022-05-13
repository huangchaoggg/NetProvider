
using NetProvider.Core.Channels;

using System;

namespace NetProvider.Core.Filter
{
    public class ExceptionFilterContext : FilterContext
    {
        public bool ExceptionHandled { get; set; }

        public Exception Exception { get; internal set; }

        public ExceptionFilterContext(Type retType, Parameters parameters,
            IServiceChannel serviceChannel) : base(retType, parameters, serviceChannel)
        {
        }
    }
}

using System;
using System.Threading.Tasks;

namespace NetProvider.Core.Channels
{
    public interface IServiceChannel
    {
        //[Obsolete]
        //object Invok(Parameters parameters);
        //[Obsolete]
        //Task<object> InvokAsync(Parameters parameters);
        T Invok<T>(Parameters parameters) where T:class;
        Task<T> InvokAsync<T>(Parameters parameters) where T:class;
    }
}

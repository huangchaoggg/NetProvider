using System.Threading.Tasks;

namespace NetProvider.Core.Channels
{
    public interface IServiceChannel
    {
        object Invok(Parameters parameters);
        Task<object> InvokAsync(Parameters parameters);
    }
}

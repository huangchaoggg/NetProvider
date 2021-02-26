
using NetProvider.Core.Channels;
using NetProvider.Network;
using NetProvider.Network.Inter;

namespace NetProvider.Factory
{
    public interface IWebApiServiceChannel : IServiceChannel
    {
        string Uri { get; }
        /// <summary>
        /// web访问器
        /// </summary>
        IHttpWebNetwork HttpWebNetwork { get; set; }
        HttpClientSetting ClientSetting { get; }
    }
}

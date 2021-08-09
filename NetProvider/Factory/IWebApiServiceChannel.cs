
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
        /// <summary>
        /// 客户端配置
        /// </summary>
        HttpClientSetting ClientSetting { get; }

        /// <summary>
        /// 设置请求中header参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetHeader(string key, string value);
    }
}

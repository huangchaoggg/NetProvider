using NetProvider.Core;

using System;

namespace NetProvider.Network
{
    /// <summary>
    /// 主机和路由配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class |AttributeTargets.Interface)]
    public class HostAttribute:RouteAttribute
    {
        /// <summary>
        /// 主机路由配置
        /// </summary>
        /// <param name="uri">主机地址</param>
        /// <param name="clientSettingType">http配置 HttpClientSetting 类型</param>
        public HostAttribute(string uri=default,Type clientSettingType = default)
        {
            Uri = uri;
            if (clientSettingType == null || clientSettingType == default)
            {
                HttpClientSetting = HttpClientSetting.DefaultSetting;
            }
            else
            {
                if (clientSettingType == typeof(HttpClientSetting) || clientSettingType.IsAssignableFrom(typeof(HttpClientSetting)))
                    Activator.CreateInstance(clientSettingType);
                else
                    throw new ProviderException("clientSettingType 必须为 HttpClientSetting 类型或它的子类");
            }
        }
        /// <summary>
        /// 客户端网络配置
        /// </summary>
        public HttpClientSetting HttpClientSetting { get; set; }
    }
}

using Microsoft.Extensions.DependencyInjection;

using NetProvider;
using NetProvider.Factory;
using NetProvider.Network;

using System;
using System.Linq;
using System.Reflection;

namespace VitalSignsVisualizationDigitalSystem.Server.OpenAPIs
{
    /// <summary>
    /// OpenApi扩展类
    /// </summary>
    public static class OpenApiExtend
    {
        /// <summary>
        /// OpenApi中间件注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type">要代理的类型查找起点</param>
        /// <param name="baseUri">默认地址</param>
        /// <returns></returns>
        public static IServiceCollection AddOpenApi(this IServiceCollection services, Type type,string baseUri=default)
        {
            Type[] types = Assembly.GetAssembly(type).GetTypes()
                .Where(s=>s.IsInterface&&s.GetCustomAttribute<HostAttribute>()!=null)?.ToArray();
            if (types == null || types.Length == 0) return services;
            foreach (Type t in types)
            {
                services.AddScoped(t,f=> 
                {
                    ServiceChannel channel= ApiServiceCreater.CreateObject(t) as ServiceChannel;
                    if (!string.IsNullOrWhiteSpace(baseUri) && string.IsNullOrWhiteSpace(channel.Uri))
                    {
                        channel.Uri = baseUri;

                    }
                    return channel;
                });
            }
            return services;
        }
    }
}

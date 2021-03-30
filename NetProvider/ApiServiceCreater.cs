using NetProvider.Factory;
using NetProvider.Network;

using System;

namespace NetProvider
{
    /// <summary>
    /// api实例对象工厂
    /// </summary>
    public static class ApiServiceCreater
    {
        static ApiFactory factory = new ApiFactory();
        /// <summary>
        /// 创建api实例对象
        /// </summary>
        /// <typeparam name="T">需要代理的接口</typeparam>
        /// <param name="baseUri"></param>
        /// <param name="clientSetting"></param>
        /// <returns></returns>
        public static T CreateObject<T>(string baseUri,HttpClientSetting clientSetting=null) where T:class
        {
            var objType= factory.CreateChannel<T>();
            object ob = Activator.CreateInstance(objType, baseUri, clientSetting==null?HttpClientSetting.DefaultSetting:clientSetting);
            return ob as T;
        }
    }
}

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
        [Obsolete]
        public static T CreateObject<T>(string baseUri,HttpClientSetting clientSetting=null) where T:class
        {
            var objType= factory.CreateChannel<T>();
            object ob = Activator.CreateInstance(objType, baseUri, clientSetting==null?HttpClientSetting.DefaultSetting:clientSetting);
            return ob as T;
        }
        /// <summary>
        /// 创建api实例对象
        /// </summary>
        /// <typeparam name="T">要代理的接口</typeparam>
        /// <returns></returns>
        public static T CreateObject<T>() where T : class
        {
            var objType = factory.CreateChannel<T>();
            object ob = Activator.CreateInstance(objType,typeof(T));
            return ob as T;
        }
        /// <summary>
        /// 创建api实例对象
        /// </summary>
        /// <param name="type">要代理的接口</param>
        /// <returns></returns>
        public static object CreateObject(Type type)
        {
            var objType = factory.CreateChannel(type);
            object ob = Activator.CreateInstance(objType,type);
            return ob;
        }
    }
}

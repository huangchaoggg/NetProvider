using NetProvider.Network;

using System;

namespace NetProvider.Factory
{
    public class ChannelFactory<T> where T : class
    {
        private static ApiFactory factory=new ApiFactory();
        private string uri;
        private HttpClientSetting httpClientSetting = HttpClientSetting.DefaultSetting;
        public ChannelFactory(string uri)
        {
            this.uri = uri;
            this.Channel = Create();
        }
        /// <summary>
        /// 代理对象实例
        /// </summary>
        public T Channel { get; protected set; }
        public ChannelFactory(string uri, HttpClientSetting setting) : this(uri)
        {
            httpClientSetting = setting;
        }
        /// <summary>
        /// 为对象创建服务通道
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T Create()
        {
            Type objType = factory.CreateChannel<T>();
            object ob = Activator.CreateInstance(objType, uri, httpClientSetting);
            return ob as T;
            //Type t = typeof(T);
            //if (t.IsInterface)
            //{
            //    MethodInfo[] infos = t.GetMethods();
            //    //运行并创建类的新实例
            //    //指定名称，访问模式
            //    TypeBuilder typeBuilder = moduleBuilder.DefineType(t.Name.TrimStart('I') + "Impl", 
            //        TypeAttributes.Public |
            //        TypeAttributes.Class |
            //        TypeAttributes.AutoClass |
            //        TypeAttributes.AnsiClass |
            //        TypeAttributes.BeforeFieldInit |
            //        TypeAttributes.AutoLayout
            //        ,typeof(ServiceChannel));
            //    typeBuilder.AddInterfaceImplementation(t);
            //    //typeBuilder.AddInterfaceImplementation(typeof(IWebApiServiceChannel));
            //    CreateKittyClassStructure(typeBuilder, typeof(ServiceChannel), typeof(string), typeof(HttpClientSetting));
            //    DynamicMethod<ServiceChannel,T>(infos, typeBuilder, t);
            //    Type rt = typeBuilder.CreateTypeInfo().AsType();
            //    Object ob = Activator.CreateInstance(rt, uri, httpClientSetting);
            //    return ob as T;
            //}
            //else
            //{
            //    return Activator.CreateInstance<T>();
            //}
        }
    }
}

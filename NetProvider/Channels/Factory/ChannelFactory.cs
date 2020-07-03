using NetProvider.Network;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NetProvider.Channels
{
    public class ChannelFactory<T> : FactoryBase<T> where T : class
    {
        private string uri;
        private HttpClientSetting clientDefaultSetting= HttpClientSetting.DefaultSetting;
        public ChannelFactory(string uri)
        {
            this.uri = uri;
            base.Channel = CreateChannel();
        }
        public ChannelFactory(string uri, HttpClientSetting setting):this(uri)
        {
            clientDefaultSetting = setting;
        }
        protected private override T CreateChannel()
        {
            Type t = typeof(T);
            if (t.IsInterface)
            {
                MethodInfo[] infos = t.GetMethods();
                //运行并创建类的新实例
                //指定名称，访问模式
                TypeBuilder typeBuilder = moduleBuilder.DefineType(t.Name + "Impl", TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout
                    , typeof(ServiceChannel));
                typeBuilder.AddInterfaceImplementation(t);
                CreateKittyClassStructure(typeBuilder,typeof(ServiceChannel),typeof(string),typeof(HttpClientSetting));
                DynamicMethod(infos, typeBuilder, t);
                Type rt = typeBuilder.CreateTypeInfo().AsType();
                Object ob = Activator.CreateInstance(rt, uri, clientDefaultSetting);
                return ob as T;
            }
            else
            {
                return Activator.CreateInstance<T>();
            }
        }
    }
}

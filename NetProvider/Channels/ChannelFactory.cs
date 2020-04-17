using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace NetProvider.Channels
{
    public class ChannelFactory<T>:FactoryBase<T> where T : class
    {
        private string uri;
        public ChannelFactory(string uri)
        {
            this.uri = uri;
            base.Channel= CreateChannel();
        }
        protected private override T CreateChannel()
        {
            Type t = typeof(T);
            if (t.IsInterface)
            {
                MethodInfo[] infos = t.GetMethods();
                //运行并创建类的新实例
                TypeBuilder typeBuilder = null;

                //指定名称，访问模式


                typeBuilder = moduleBuilder.DefineType(t.Name + "Impl", TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout
                    , typeof(ServiceChannel));
                typeBuilder.AddInterfaceImplementation(t);
                CreateKittyClassStructure(typeBuilder);
                DynamicMethod(infos, typeBuilder, t);
                Type rt = typeBuilder.CreateTypeInfo().AsType();
                Object ob = Activator.CreateInstance(rt, uri);
                return ob as T;
            }
            else
            {
                return Activator.CreateInstance<T>();
            }
        }
    }
}

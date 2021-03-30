using NetProvider.Core.Channels;
using NetProvider.Network;

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NetProvider.Factory
{
    public class ApiFactory : FactoryBase
    {
        public override Type CreateChannel<T>()
        {
            Type t = typeof(T);
            if (t.IsInterface)
            {
                Type retType;
                string className = t.Name.TrimStart('I') + "Impl";
                retType= moduleBuilder.GetType(className);
                if (retType != null) return retType;

                MethodInfo[] infos = t.GetMethods();
                //运行并创建类的新实例
                //指定名称，访问模式
                TypeBuilder typeBuilder = moduleBuilder.DefineType(className,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout
                    , typeof(ServiceChannel));
                typeBuilder.AddInterfaceImplementation(t);
                CreateKittyClassStructure(typeBuilder, typeof(ServiceChannel), typeof(string), typeof(HttpClientSetting));
                DynamicMethod<ServiceChannel, T>(infos, typeBuilder, t);
                retType = typeBuilder.CreateTypeInfo().AsType();
                return retType;
            }
            else
            {
                return typeof(T);
            }
        }
    }
}

using NetProvider.Core;
using NetProvider.Core.Channels;
using NetProvider.Network;

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NetProvider.Factory
{
    public class ApiFactory : FactoryBase
    {
        /// <summary>
        /// 创建通道
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override Type CreateChannel<T>()
        {
            Type t = typeof(T);
            return CreateChannel(t);
            //if (t.IsInterface)
            //{
            //    Type retType;
            //    string className = t.Name.TrimStart('I') + "Impl";
            //    retType= moduleBuilder.GetType(className);
            //    if (retType != null) return retType;

            //    MethodInfo[] infos = t.GetMethods();
            //    //运行并创建类的新实例
            //    //指定名称，访问模式
            //    TypeBuilder typeBuilder = moduleBuilder.DefineType(className,
            //        TypeAttributes.Public |
            //        TypeAttributes.Class |
            //        TypeAttributes.AutoClass |
            //        TypeAttributes.AnsiClass |
            //        TypeAttributes.BeforeFieldInit |
            //        TypeAttributes.AutoLayout
            //        , typeof(ServiceChannel));
            //    typeBuilder.AddInterfaceImplementation(t);
            //    CreateKittyClassStructure(typeBuilder, typeof(ServiceChannel), typeof(string), typeof(HttpClientSetting));
            //    DynamicMethod<ServiceChannel>(infos, typeBuilder, t);
            //    retType = typeBuilder.CreateTypeInfo().AsType();
            //    return retType;
            //}
            //else
            //{
            //    return typeof(T);
            //}
        }
        /// <summary>
        /// 创建通道
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override Type CreateChannel(Type type)
        {
            Type t = type;
            if (t.IsInterface)
            {
                Type retType;
                string className = t.Name.TrimStart('I') + "Impl";
                retType = moduleBuilder.GetType(className);
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
                CreateKittyClassStructure(typeBuilder, typeof(ServiceChannel), typeof(Type));//创建带type构造函数
                CreateKittyClassStructure(typeBuilder, typeof(ServiceChannel), typeof(string), typeof(HttpClientSetting));//创建带参数构造函数
                //DynamicMethod<ServiceChannel, T>(infos, typeBuilder, t);
                DynamicMethod<ServiceChannel>(infos.Where(s=>s.GetCustomAttribute<RequestAttribute>()!=null).ToArray(), typeBuilder, t);
                retType = typeBuilder.CreateTypeInfo().AsType();
                return retType;
            }
            else
            {
                throw new ProviderException("输入类型不是一个接口类型");
            }
        }
    }
}

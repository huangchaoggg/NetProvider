using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace NetProvider.Core.Channels
{
    public abstract class FactoryBase<T>
    {
        //定义和表示动态程序集的模块
        protected static ModuleBuilder moduleBuilder = null;
        //定义表示动态程序集
        protected static AssemblyBuilder assemblyBuilder = null;
        static FactoryBase()
        {
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Channels"), AssemblyBuilderAccess.RunAndCollect);//AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Channels"), AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("Channels");
        }
        /// <summary>
        /// 代理对象实例
        /// </summary>
        public T Channel { get; protected set; }
        /// <summary>
        /// 函数动态处理
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="typeBuilder"></param>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        protected virtual void DynamicMethod<Serrvice>(MethodInfo[] infos, TypeBuilder typeBuilder, Type type) where Serrvice : class, IServiceChannel
        {
            foreach (MethodInfo info in infos)
            {
                MethodOverride<Serrvice>(info, typeBuilder, type);
            }
        }
        /// <summary>
        /// 重写或实现接口/虚方法
        /// </summary>
        /// <param name="webNetwork"></param>
        /// <param name="info"></param>
        /// <param name="iL"></param>
        /// <param name="type"></param>
        ///<typeparam name="T"></typeparam>
        private void MethodOverride<Serrvice>(MethodInfo info, TypeBuilder typeBuilder, Type type) where Serrvice: class,IServiceChannel
        {
            Type[] ParameterTypes = info.GetParameters().Select(s => s.ParameterType).ToArray();
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(info.Name, MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.UnmanagedExport |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot |
                MethodAttributes.Final, info.ReturnType, ParameterTypes);
            ILGenerator iL = methodBuilder.GetILGenerator();//生成中间语言指令
            MethodInfo methodInfo = typeof(Serrvice).GetMethod("Invok", BindingFlags.Public | BindingFlags.Instance);

            LocalBuilder lisLb = iL.DeclareLocal(typeof(object[]));
            LocalBuilder parameters = iL.DeclareLocal(typeof(Parameters));
            ConstructorInfo constructorInfo = parameters.LocalType.GetConstructor(new Type[] {
                    typeof(string),typeof(string),typeof(object[]) });
            iL.Emit(OpCodes.Nop);
            int leth = info.GetParameters().Length;
            if (leth > 0)
            {
                iL.Emit(OpCodes.Ldc_I4, leth);
                iL.Emit(OpCodes.Newarr, typeof(object));
                iL.Emit(OpCodes.Dup);
                for (int i = 0; i < leth; i++)
                {
                    //iL.Emit(OpCodes.Ldloc, lisLb);
                    iL.Emit(OpCodes.Ldc_I4, i);
                    iL.Emit(OpCodes.Ldarg, i + 1);
                    iL.Emit(OpCodes.Box, ParameterTypes[i]);
                    iL.Emit(OpCodes.Stelem_Ref);
                    if (leth - 1 != i)
                        iL.Emit(OpCodes.Dup);
                }
                iL.Emit(OpCodes.Stloc, lisLb.LocalIndex);
            }
            iL.Emit(OpCodes.Ldstr, typeof(T).Name);
            iL.Emit(OpCodes.Ldstr, info.Name);
            iL.Emit(OpCodes.Ldloc, lisLb.LocalIndex);
            iL.Emit(OpCodes.Newobj, constructorInfo);
            iL.Emit(OpCodes.Stloc, parameters);

            iL.Emit(OpCodes.Ldarg_0);
            iL.Emit(OpCodes.Ldloc, parameters);
            iL.EmitCall(OpCodes.Call, methodInfo, new Type[] { typeof(Parameters) });

            iL.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, type.GetMethod(info.Name));
        }
        /// <summary>
        /// 创建构造器
        /// </summary>
        /// <param name="typeBuilder">类型构造器</param>
        /// <param name="objType">继承的对象</param>
        /// <param name="types">参数类型</param>
        protected void CreateKittyClassStructure(TypeBuilder typeBuilder, Type objType, params Type[] types)
        {
            ConstructorInfo objCtor = objType.GetConstructor(types);
            if (objCtor == null) throw new ProviderException("未找到构造函数");
            var constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public, CallingConventions.Standard, types);
            ILGenerator ilOfCtor = constructorBuilder.GetILGenerator();

            ilOfCtor.Emit(OpCodes.Ldarg_0);
            for (int i = 1; i <= types.Length; i++)
            {
                ilOfCtor.Emit(OpCodes.Ldarg, i);
                //ilOfCtor.Emit(OpCodes.Ldarg_1);
            }
            ilOfCtor.Emit(OpCodes.Call, objCtor);
            ilOfCtor.Emit(OpCodes.Nop);
            ilOfCtor.Emit(OpCodes.Ret);

            // ---- define properties ----
        }
        protected abstract T CreateChannel();
    }
}

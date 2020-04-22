using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace NetProvider.Channels
{
    public abstract class FactoryBase<T>
    {
        //定义和表示动态程序集的模块
        protected private static ModuleBuilder moduleBuilder = null;
        //定义表示动态程序集
        protected private static AssemblyBuilder assemblyBuilder = null;
        static FactoryBase()
        {
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Channels"), AssemblyBuilderAccess.RunAndCollect);//AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Channels"), AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("Channels");
        }
        public T Channel { get; protected private set; }
        /// <summary>
        /// 函数动态处理
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="typeBuilder"></param>
        /// <param name="type"></param>
        protected private virtual void DynamicMethod(MethodInfo[] infos, TypeBuilder typeBuilder, Type type)
        {
            foreach (MethodInfo info in infos)
            {
                MethodOverride(info, typeBuilder, type);
            }
        }
        /// <summary>
        /// 重写或实现接口/虚方法
        /// </summary>
        /// <param name="webNetwork"></param>
        /// <param name="info"></param>
        /// <param name="iL"></param>
        protected private void MethodOverride(MethodInfo info, TypeBuilder typeBuilder, Type type)
        {
            Type[] ParameterTypes = info.GetParameters().Select(s => s.ParameterType).ToArray();
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(info.Name, MethodAttributes.Private |
                MethodAttributes.Virtual |
                MethodAttributes.UnmanagedExport |
                MethodAttributes.HideBySig |
                MethodAttributes.NewSlot |
                MethodAttributes.Final, info.ReturnType, ParameterTypes);
            ILGenerator iL = methodBuilder.GetILGenerator();//生成中间语言指令
            MethodInfo methodInfo = typeof(ServiceChannel).GetMethod("Invok", BindingFlags.Public | BindingFlags.Instance);

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
        /// 创建无参构造器
        /// </summary>
        /// <param name="typeBuilder"></param>
        protected private void CreateKittyClassStructure(TypeBuilder typeBuilder)
        {

            Type objType = typeof(ServiceChannel);
            ConstructorInfo objCtor = objType.GetConstructor(new Type[1] { typeof(string) });

            Type[] constructorArgs = { typeof(string) };
            var constructorBuilder = typeBuilder.DefineConstructor(
               MethodAttributes.Public, CallingConventions.Standard, constructorArgs);
            ILGenerator ilOfCtor = constructorBuilder.GetILGenerator();

            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Ldarg_1);
            ilOfCtor.Emit(OpCodes.Call, objCtor);
            ilOfCtor.Emit(OpCodes.Nop);
            ilOfCtor.Emit(OpCodes.Ret);

            // ---- define properties ----
        }
        protected private abstract T CreateChannel();
    }
}

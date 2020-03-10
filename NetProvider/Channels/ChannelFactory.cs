using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace NetProvider.Channels
{
    public class ChannelFactory<T> where T : class
    {
        //定义和表示动态程序集的模块
        private ModuleBuilder moduleBuilder = null;
        //定义表示动态程序集
        private AssemblyBuilder assemblyBuilder = null;
        public ChannelFactory(string uri)
        {
            this.Channel= CreateChannel(uri);
        }
        public T Channel { get; private set; }
        private T CreateChannel(string uri)
        {

            Type t = typeof(T);
            if (t.IsInterface)
            {
                MethodInfo[] infos = t.GetMethods();
                //运行并创建类的新实例
                TypeBuilder typeBuilder = null;
                
                //指定名称，访问模式

                assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Channels"), AssemblyBuilderAccess.RunAndCollect);//AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Channels"), AssemblyBuilderAccess.Run);
                moduleBuilder = assemblyBuilder.DefineDynamicModule("Channels");
                typeBuilder = moduleBuilder.DefineType(t.Name + "Impl", TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout
                    , typeof(ServiceChannel));
                typeBuilder.AddInterfaceImplementation(t);
                CreateKittyClassStructure(typeBuilder);
                Create(infos, typeBuilder, t);
                Type rt = typeBuilder.CreateTypeInfo().AsType();
                Object ob = Activator.CreateInstance(rt,uri);
                return ob as T;
            }
            else
            {
                return Activator.CreateInstance<T>();
            }
        }
        /// <summary>
        /// 重写或实现接口/虚方法
        /// </summary>
        /// <param name="webNetwork"></param>
        /// <param name="info"></param>
        /// <param name="iL"></param>
        private void Create(MethodInfo[] infos, TypeBuilder typeBuilder, Type type)
        {
            foreach (MethodInfo info in infos)
            {
                Type[] ParameterTypes = info.GetParameters().Select(s => s.ParameterType).ToArray();
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(info.Name, MethodAttributes.Public |
                    MethodAttributes.Virtual |
                    MethodAttributes.HideBySig |
                    MethodAttributes.NewSlot |
                    MethodAttributes.Final, info.ReturnType, ParameterTypes);
                ILGenerator iL = methodBuilder.GetILGenerator();//生成中间语言指令
                MethodInfo methodInfo = typeof(ServiceChannel).GetMethod("Invok", BindingFlags.Public | BindingFlags.Instance);
                Task t= Task.Factory.StartNew(() =>
                {
                    
                });
                LocalBuilder lisLb = iL.DeclareLocal(typeof(object[]));
                iL.Emit(OpCodes.Nop);
                int leth = info.GetParameters().Length;
                iL.Emit(OpCodes.Ldc_I4, leth);
                iL.Emit(OpCodes.Newarr, typeof(object));
                iL.Emit(OpCodes.Stloc, lisLb);
                for (int i = 0; i < leth; i++)
                {
                    iL.Emit(OpCodes.Ldloc, lisLb);
                    iL.Emit(OpCodes.Ldc_I4, i);
                    iL.Emit(OpCodes.Ldarg, i + 1);
                    iL.Emit(OpCodes.Stelem_Ref);
                }
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldstr, typeof(T).Name);
                iL.Emit(OpCodes.Ldstr, info.Name);
                iL.Emit(OpCodes.Ldloc, lisLb);
                iL.EmitCall(OpCodes.Call, methodInfo, new Type[] { typeof(string), typeof(string), typeof(object[]) });

                iL.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(methodBuilder, type.GetMethod(info.Name));
            }
        }
        /// <summary>
        /// 创建无参构造器
        /// </summary>
        /// <param name="typeBuilder"></param>
        private static void CreateKittyClassStructure(TypeBuilder typeBuilder)
        {

            Type objType = typeof(ServiceChannel);
            ConstructorInfo objCtor = objType.GetConstructor(new Type[1] {typeof(string) });

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

    }
}

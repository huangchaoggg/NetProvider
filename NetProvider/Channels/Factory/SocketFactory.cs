using NetProvider.EventArgs;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace NetProvider.Channels
{
    public class SocketFactory<T, C, ReceiveType> : FactoryBase<T> where T : class where C : IReceiveMessage<ReceiveType>, new() where ReceiveType : class
    {
        private string _ip;
        private int _port;
        public C ReceiveClass { get; private set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public SocketFactory(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
        }
        private protected override T CreateChannel()
        {
            T cs = Create();
            SocketServiceChannel socket = cs as SocketServiceChannel;
            if (socket == null)
            {
                throw new ProviderException("创建Socket代理失败");
            }
            ReceiveClass = new C();
            socket.SocketClient.ReceiveMessageEvent += SocketClient_ReceiveMessageEvent;
            socket.SocketClient.ReceiveMessageEvent += ReceiveMessageEvent;
            socket.SocketClient.SendMessageEvent += SendMessageEvent;
            socket.SocketClient.ExceptionEvent += ExceptionEvent;
            return cs;
        }

        private void SocketClient_ReceiveMessageEvent(object sender, ReceiveMessageArgs e)
        {
            ReceiveClass.Receives(e.ToObject<ReceiveType>(Encoding));
        }

        private T Create()
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
                    , typeof(SocketServiceChannel));
                typeBuilder.AddInterfaceImplementation(t);
                CreateKittyClassStructure(typeBuilder, typeof(SocketServiceChannel), typeof(string));
                DynamicMethod(infos, typeBuilder, t);
                Type rt = typeBuilder.CreateTypeInfo().AsType();
                Object ob = Activator.CreateInstance(rt, _ip, _port);
                return ob as T;
            }
            else
            {
                return Activator.CreateInstance<T>();
            }
        }
        public event EventHandler<ProviderException> ExceptionEvent;
        public event EventHandler<SendMessageArgs> SendMessageEvent;
        public event EventHandler<ReceiveMessageArgs> ReceiveMessageEvent;
    }
}

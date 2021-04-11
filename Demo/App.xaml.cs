using Autofac;

using Demo.Service;

using NetProvider;
using NetProvider.Network;

using System.Windows;

namespace Demo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        ContainerBuilder buider = new ContainerBuilder();
        public static IContainer Container;
        public App()
        {
            HttpClientSetting.DefaultSetting.SetHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36 Edg/89.0.774.75");
            //var service = ApiServiceCreater.CreateObject<IMicrosoftService>("https://docs.microsoft.com");
            //buider.RegisterInstance(service);
            buider.Register(new System.Func<IComponentContext, IMicrosoftService>((e) => 
            {
                //return new MicrosoftService("https://docs.microsoft.com/");
                return ApiServiceCreater.CreateObject<IMicrosoftService>("https://docs.microsoft.com/");
            }));
            Container =buider.Build();
        }
    }
}

using Autofac;

using Demo.Service;

using NetProvider;

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
            var service = ApiServiceCreater.CreateObject<IMicrosoftService>("https://docs.microsoft.com");
            buider.RegisterInstance(service);
            Container=buider.Build();
        }
    }
}

using System.Threading.Tasks;
using System.Windows;

using Autofac;

using Demo.Service;

using NetProvider.Core.Extension;

namespace Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        IMicrosoftService service = App.Container.Resolve<IMicrosoftService>();
        //IMicrosoftService service = new MicrosoftService("https://docs.microsoft.com");
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string rd= service.GetJson();
            Dispatcher.Invoke(() =>
            {
                TextBox1.Text = rd;
                Meta mt = rd.ToObject<Meta>();
                treeView.ItemsSource = null;
                treeView.ItemsSource = mt.items;
            });
            //Task<string> rd = service.GetJson();
            //rd.ContinueWith((rr) =>
            //{
            //    Dispatcher.Invoke(() =>
            //    {
            //        TextBox1.Text = rr.Result;
            //        Meta mt = rr.Result.ToObject<Meta>();
            //        treeView.ItemsSource = null;
            //        treeView.ItemsSource = mt.items;
            //    });
            //});
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Meta mt = await service.GetMeta();
            //treeView.ItemsSource = null;
            //treeView.ItemsSource = mt.items;
            Task<Meta> mt = service.GetMeta();
            mt.ContinueWith((rt) =>
            {
                Dispatcher.Invoke(() =>
                {
                    treeView.ItemsSource = null;
                    treeView.ItemsSource = rt.Result.items;
                });

            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            TextBox1.Text = string.Empty;
            treeView.ItemsSource = null;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Task t= service.Tutorials();
            t.ContinueWith((s)=>{
                Dispatcher.Invoke(() =>
                {
                    TextBox1.Text = "发送成功";
                });
            });
           
        }
    }
}

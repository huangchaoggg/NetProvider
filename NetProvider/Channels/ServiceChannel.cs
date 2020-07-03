using NetProvider.Filter;
using NetProvider.Network;
using NetProvider.Network.Inter;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetProvider.Channels
{
    /// <summary>
    /// 定义Web服务通道
    /// </summary>
    //[AOPAttribute]
    public abstract class ServiceChannel : IServiceChannel
    {
        public string Uri { get; private set; }
        /// <summary>
        /// web访问器
        /// </summary>
        public IHttpWebNetwork HttpWebNetwork { get; set; }
        public HttpClientSetting ClientSetting { get; }
        public ServiceChannel(string uri, HttpClientSetting setting)
        {
            this.Uri = uri;
            this.ClientSetting = setting;
            HttpWebNetwork = new HttpWebNetwork(setting);
        }

        public object Invok(Parameters parameters)
        {
            Type t = this.GetType();
            MethodInfo info = t.GetInterface(parameters.InterfaceName).GetMethod(parameters.MethodName);
            Attribute[] attributes = Attribute.GetCustomAttributes(info);
            return RunMethod(attributes, info.ReturnType, info.GetParameters(), parameters);

        }
        private async Task<object> RunMethod(Attribute[] attributes, Type retType, ParameterInfo[] parameterInfos, Parameters parameters)
        {
            RequestAttribute ra = attributes.FirstOrDefault(s => s is RequestAttribute) as RequestAttribute;
            if (ra == null)
            {
                throw new MessageException("特性不存在");
            }

            HttpResponseMessage rd = await Request(ra, parameterInfos, parameters.ParametersInfo);
            if (rd.IsSuccessStatusCode)
            {
                string str = await rd.Content.ReadAsStringAsync();
                if (retType.IsGenericType && retType.BaseType == typeof(Task))
                {
                    Type t = retType.GetGenericArguments()[0];
                    return FilterManagement.Filter(ClientSetting.Filters, str, t, parameters, this);
                }

                return FilterManagement.Filter(ClientSetting.Filters,str, retType, parameters, this);
            }
            throw new MessageException("请求错误");
        }
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ra"></param>
        /// <param name="parameters">参数属性</param>
        /// <param name="objs">参数</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> Request(RequestAttribute ra, ParameterInfo[] parameters, params object[] objs)
        {
            HttpResponseMessage rd = null;
            string uri = HttpWebHelper.PathCombine(Uri, ra.Uri);
            switch (ra.RequestType)
            {
                case RequestType.Get:
                    rd = await GetRequest(uri, parameters, objs);
                    break;
                default:
                    rd = await PostRequest(uri, objs);
                    break;
            }
            return rd;
        }
        /// <summary>
        /// get请求处理
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetRequest(string uri, ParameterInfo[] parameters, params object[] objs)
        {
            HttpResponseMessage rd;
            if (objs == null || objs.Length == 0)
            {
                rd = await HttpWebNetwork.GetRequest(uri);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (ParameterInfo p in parameters)
                {
                    if (objs[p.Position] != null)
                        sb.Append($" {p.Name}={objs[p.Position].ToString()}&");
                }
                string ss = sb.ToString().TrimEnd('&').Trim();
                string url = $"{uri}?{ss}";

                rd = await HttpWebNetwork.GetRequest(System.Uri.EscapeUriString(url));
            }
            return rd;
        }
        /// <summary>
        /// Post请求处理
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> PostRequest(string uri, params object[] objs)
        {
            HttpResponseMessage rd;
            if (objs == null || objs.Length == 0)
            {
                rd = await HttpWebNetwork.PostRequest(uri);
            }
            else
            {
                object obj = objs[0];
                if (obj is Stream)
                {
                    rd = await HttpWebNetwork.PostRequest(uri, (FileStream)obj);
                }
                else
                {
                    string body = obj.ToJsonString();
                    rd = await HttpWebNetwork.PostRequest(uri, body);
                }
            }
            return rd;
        }
    }
    /// <summary>
    /// 定义web上传下载服务通道
    /// </summary>
    public abstract class FileServiceChannel : IServiceChannel
    {
        public object Invok(Parameters parameters)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 定义Socket服务通道
    /// </summary>
    public abstract class SocketServiceChannel : ISocketServiceChannel
    {
        public SocketClient SocketClient { get; private set; }
        public SocketServiceChannel(string ip, int port)
        {
            this.SocketClient = new SocketClient(ip, port);
        }
        public void StartClient()
        {
            SocketClient.StartClientAndReceive();
        }
        public void Close()
        {
            SocketClient.Close();
        }

        public object Invok(Parameters parameters)
        {
            string value = parameters.ParametersInfo[0].ToJsonString();
            SocketClient.SendMessage(value);
            return 0;
        }
    }
    //public interface Aaa
    //{


    //    [RequestAttribute(RequestType.Post, "http://test-his-api.ccuol.net/user/login")]
    //    object AAa(object cc);
    //    object BBcd(object cc);

    //    object Bbbc();
    //}
    //public class Test
    //{
    //    public int test1()
    //    {
    //        return 11;
    //    }
    //    public void test2(string a)
    //    {

    //    }
    //    public void test3(params string[] aa)
    //    {

    //    }
    //    public void test4(int[] sb,params string[] aa)
    //    {

    //    }
    //}
    //public class BBb : Test,Aaa
    //{
    //    public object AAa(object cc)
    //    {
    //        return null;
    //    }

    //    public void abc()
    //    {
    //        int aa = 0;
    //        aa=test1();
    //        test2("ss");
    //        test3("sssd", "sss");
    //        int[] a = { 1, 2, 3 };
    //        test4(a, "sds", "sdsd");
    //    }

    //    public object Bbbc()
    //    {
    //        return null;
    //    }

    //    public object BBcd(object cc)
    //    {
    //        return null;
    //    }
    //}
}

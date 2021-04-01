using NetProvider.Core;
using NetProvider.Core.Channels;
using NetProvider.Core.Extension;
using NetProvider.Core.Filter;
using NetProvider.Network;
using NetProvider.Network.Inter;

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetProvider.Factory
{
    /// <summary>
    /// 定义Web服务通道
    /// </summary>
    //[AOPAttribute]
    public abstract class ServiceChannel : IWebApiServiceChannel
    {
        private FilterManagement filterManagement;
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
            filterManagement = this.ClientSetting.FilterManagement;
            HttpWebNetwork = new HttpWebNetwork(setting);
        }

        public object Invok(Parameters parameters)
        {
            Type t = this.GetType();
            MethodInfo info = t.GetInterface(parameters.InterfaceName).GetMethod(parameters.MethodName);
            Attribute[] attributes = Attribute.GetCustomAttributes(info);
            var value = RunMethod(attributes, info.ReturnType, info.GetParameters(), parameters);
            value.ContinueWith((result) =>
            {
                if (result.Exception != null)
                    filterManagement.CallExceptionFilter(result.Exception, info.ReturnType, parameters, this);
            });
            if (info.ReturnType.IsTask())
            {
                return value;
            }
            else
            {
                return value.GetAwaiter().GetResult();
            }
        }
        private async Task<object> RunMethod(Attribute[] attributes, Type retType, ParameterInfo[] parameterInfos, Parameters parameters)
        {
            RequestAttribute ra = attributes.FirstOrDefault(s => s is RequestAttribute) as RequestAttribute;
            if (ra == null)
            {
                FileTransferAttribute sa = attributes.FirstOrDefault(s => s is FileTransferAttribute) as FileTransferAttribute;
                if (sa == null)
                {
                    throw new MessageException("特性不存在");
                }
                return SendStream(sa, parameters.ParametersInfo);
            }

            HttpResponseMessage rd = await Request(ra, parameterInfos, parameters.ParametersInfo);
            if (rd.IsSuccessStatusCode)
            {
                string str = await rd.Content.ReadAsStringAsync();
                if (retType.IsTask())
                {
                    var args= retType.GenericTypeArguments;
                    if (args.Length > 0)
                    {
                        return filterManagement.CallMessageFilter(str, args[0], parameters, this);
                    }
                    return Task.CompletedTask;
                }
                else
                {
                    if (retType == typeof(void))
                    {
                        return Task.CompletedTask;
                    }
                    return filterManagement.CallMessageFilter(str, retType, parameters, this);
                }
            }
             throw new MessageException(rd.ToString());
        }
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ra"></param>
        /// <param name="parameters">参数属性</param>
        /// <param name="objs">参数</param>
        /// <returns></returns>
        private Task<HttpResponseMessage> Request(RequestAttribute ra, ParameterInfo[] parameters, params object[] objs)
        {
            Task<HttpResponseMessage> rd = null;
            string uri = HttpWebHelper.PathCombine(Uri, ra.Uri);
            switch (ra.RequestType)
            {
                case RequestType.Get:
                    rd = GetRequest(uri, parameters, objs);
                    break;
                default:
                    rd = PostRequest(uri, objs);
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
        private Task<HttpResponseMessage> GetRequest(string uri, ParameterInfo[] parameters, params object[] objs)
        {
            Task<HttpResponseMessage> rd;
            if (objs == null || objs.Length == 0)
            {
                rd = HttpWebNetwork.GetRequest(uri);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (ParameterInfo p in parameters)
                {
                    if (objs[p.Position] != null)
                        sb.Append($" {p.Name}={objs[p.Position]}&");
                }
                string ss = sb.ToString().TrimEnd('&').Trim();
                string url = $"{uri}?{ss}";

                rd = HttpWebNetwork.GetRequest(System.Uri.EscapeUriString(url));
            }
            return rd;
        }
        /// <summary>
        /// Post请求处理
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        private Task<HttpResponseMessage> PostRequest(string uri, params object[] objs)
        {
            Task<HttpResponseMessage> rd;
            if (objs == null || objs.Length == 0)
            {
                rd = HttpWebNetwork.PostRequest(uri);
            }
            else
            {
                object obj = objs[0];
                if (obj is Stream)
                {
                    rd = HttpWebNetwork.PostRequest(uri, (FileStream)obj);
                }
                else
                {
                    string body = obj.ToJsonString();
                    rd = HttpWebNetwork.PostRequest(uri, body);
                }
            }
            return rd;
        }

        private Task<HttpResponseMessage> SendStream(FileTransferAttribute sa,params object[] objs)
        {
            string uri = HttpWebHelper.PathCombine(Uri, sa.Uri);
            return HttpWebNetwork.SendStream(uri,sa.ContentName,sa.ContentType, objs);
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

using NetProvider.Core;
using NetProvider.Core.Channels;
using NetProvider.Core.Extension;
using NetProvider.Core.Filter;
using NetProvider.Network;
using NetProvider.Network.Inter;

using Newtonsoft.Json;

using System;
using System.ComponentModel;
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
            Task<object> obj = new Task<object>(() =>
            {
                return InvokAsync(parameters).Result;
            });
            obj.Start();
            return obj.Result;


        }
        public Task<object> InvokAsync(Parameters parameters)
        {
            Type t = this.GetType();
            MethodInfo info = t.GetInterface(parameters.InterfaceName).GetMethod(parameters.MethodName);
            Attribute[] attributes = Attribute.GetCustomAttributes(info);
            Type retType = info.ReturnType;
            if (info.ReturnType.IsTask())
            {
                var args = info.ReturnType.GenericTypeArguments;
                if (args.Length > 0)
                {
                    retType = args[0];
                }
            }
            return RunMethod(attributes, retType, info.GetParameters(), parameters)
            .ContinueWith((result) =>
            {
                object ret;
                if (result.Exception != null)
                    ret = filterManagement.CallExceptionFilter(result.Exception, retType, parameters, this);
                else
                {
                    ret = result.Result;
                }
                return ret;
            });
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
                object value;
               
                if (retType == typeof(HttpResponseMessage))
                {
                    value= rd;
                }
                else if (retType == typeof(Stream))
                {
                    value= await rd.Content.ReadAsStreamAsync();
                }
                else if (retType == typeof(byte[]))
                {
                    value= await rd.Content.ReadAsByteArrayAsync();
                }
                else if (retType == typeof(void))
                {
                    value= Task.CompletedTask;
                }
                else
                {
                    value= await rd.Content.ReadAsStringAsync();
                }

                var retValue = filterManagement.CallMessageFilter(value, retType, parameters, this);
                if(retValue is string v)
                {
                    if (retType == typeof(string))
                    {
                        
                    }
                    else if (retType.IsClass&&!retType.IsEnum&&!retType.IsValueType)
                    {
                        retValue = v.ToObject(retType);
                    }
                }
                return retValue;
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

        private Task<HttpResponseMessage> SendStream(FileTransferAttribute sa, params object[] objs)
        {
            string uri = HttpWebHelper.PathCombine(Uri, sa.Uri);
            return HttpWebNetwork.SendStream(uri, sa.ContentName, sa.ContentType, objs);
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

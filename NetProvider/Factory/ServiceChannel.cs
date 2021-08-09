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
        public string Uri { get; set; }
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
        public ServiceChannel(Type type)
        {
            if (type == null)
                throw new ProviderException("实例化失败,实例化参数为空");
            if (type.IsInterface)
            {
               HostAttribute att= type.GetCustomAttribute<HostAttribute>();
                if (att == null)
                    throw new ProviderException("HostAttribute 未设置");
                this.Uri = att.Uri;
                this.ClientSetting = att.HttpClientSetting;
                this.filterManagement=this.ClientSetting.FilterManagement;
                this.HttpWebNetwork = new HttpWebNetwork(ClientSetting);
            }
        }
        //public object Invok(Parameters parameters)
        //{
        //    Task<object> obj = new Task<object>(() =>
        //    {
        //        return InvokAsync(parameters).Result;
        //    });
        //    obj.Start();
        //    return obj.Result;


        //}
        //public Task<object> InvokAsync(Parameters parameters)
        //{
        //    Type t = this.GetType();
        //    MethodInfo info = t.GetInterface(parameters.InterfaceName).GetMethod(parameters.MethodName);
        //    Attribute[] attributes = Attribute.GetCustomAttributes(info);
        //    Type retType = info.ReturnType;
        //    if (info.ReturnType.IsTask())
        //    {
        //        var args = info.ReturnType.GenericTypeArguments;
        //        if (args.Length > 0)
        //        {
        //            retType = args[0];
        //        }
        //    }
        //    return RunMethod<object>(attributes, retType, info.GetParameters(), parameters)
        //    .ContinueWith((result) =>
        //    {
        //        object ret;
        //        if (result.Exception != null)
        //        {
        //            try
        //            {
        //                ret = filterManagement.CallExceptionFilter(result.Exception, retType, parameters, this);

        //            }catch(ProviderException e)
        //            {
        //                ret = null;
        //                return Task.FromException(e);
        //            }
        //        }
        //        else
        //        {
        //            ret = result.Result;
        //        }
        //        return ret;
        //    });
        //}
        public T Invok<T>(Parameters parameters) where T : class
        {
            Task<T> obj = new Task<T>(() =>
            {
                return InvokAsync<T>(parameters).Result;
            });
            obj.Start();
            try
            {
                return obj.Result;
                
            }catch(Exception e)
            {
                throw e.GetBaseException();
            }
        }

        public Task<T> InvokAsync<T>(Parameters parameters) where T:class
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
            return RunMethod<T>(attributes, retType, info.GetParameters(), parameters)
            .ContinueWith((result) =>
            {
                T ret;
                if (result.Exception != null)
                {
                    try
                    {
                        ret = (T)filterManagement.CallExceptionFilter(result.Exception, retType, parameters, this);

                    }
                    catch (ProviderException e)
                    {
                        throw e;
                    }
                }
                else
                {
                    ret = result.Result;
                }
                return ret;
            });
        }
        private async Task<T> RunMethod<T>(Attribute[] attributes, Type retType, ParameterInfo[] parameterInfos, Parameters parameters) where T : class
        {
            RequestAttribute ra = attributes.FirstOrDefault(s => s is RequestAttribute) as RequestAttribute;
            if (ra == null)
            {
                FileTransferAttribute sa = attributes.FirstOrDefault(s => s is FileTransferAttribute) as FileTransferAttribute;
                if (sa == null)
                {
                    throw new MessageException("特性不存在");
                }
                await SendStream(sa, parameters.ParametersInfo);
                return null;
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
                }else if (retValue is Newtonsoft.Json.Linq.JToken o)
                {
                    retValue = o.ToObject(retType);
                }
                return (T)retValue;
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
                        sb.Append($"{p.Name}={objs[p.Position]}&");
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

        public void SetHeader(string key, string value)
        {
            ClientSetting.SetHeader(key, value);
        }
    }
    //public interface Aaa
    //{


    //    [RequestAttribute(RequestType.Post, "http://test-his-api.ccuol.net/user/login")]
    //    object AAa(object cc);
    //    object BBcd(object cc);

    //    object Bbbc();
    //}
    //[Host("http://www.baidu.com",typeof(HttpClientSetting))]
    public class Test : ServiceChannel
    {
        public Test(string uri, HttpClientSetting setting) : base(uri, setting) { }
        public async Task<T> test1<T>() where T:class
        {
            return await base.InvokAsync<T>(null);
        }
        public void test2<T>() where T:class
        {
            T t= test3<T>("ss","dd");
        }
        public T test3<T>(params string[] aa) where T:class
        {
            return null;
        }
        public void test4(int[] sb, params string[] aa)
        {
            test5(sb, aa);
        }
        public object test5(int[] sb, params string[] aa)
        {
            return null;
        }
    }
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

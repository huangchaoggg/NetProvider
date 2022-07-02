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
               HostAttribute att= GetHostAttribute(type);
                if (att == null)
                    throw new ProviderException("HostAttribute 未设置");
                this.Uri = att.Uri;
                this.ClientSetting = att.HttpClientSetting;
                this.filterManagement=this.ClientSetting.FilterManagement;
                this.HttpWebNetwork = new HttpWebNetwork(ClientSetting);
            }
        }
        /// <summary>
        /// 循环获取本级或父级的特性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private HostAttribute GetHostAttribute(Type type)
        {
            HostAttribute att = type.GetCustomAttribute<HostAttribute>();
            if (att != null)
            {
                return att;
            }
            var baseInterfaces= type.GetInterfaces();
            if (baseInterfaces==null||baseInterfaces.Count() == 0)
                return null;
            foreach(var inter in baseInterfaces)
            {
                att= GetHostAttribute(inter);
                if (att != null)
                    return att;
            }
            return null;
        }
        public T Invok<T>(Parameters parameters) where T : class
        {
            return InvokAsync<T>(parameters).Result;
        }

        public async Task<T> InvokAsync<T>(Parameters parameters) where T : class
        {
            var result=await InvokAsync(parameters);
            if(result is T)
            {
                return (T)result;
            }
            return default(T);
        }
        private Task<object> InvokAsync(Parameters parameters)
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
                if (result.Exception != null)
                {
                    ExceptionFilterContext context= filterManagement.CallExceptionFilter(result.Exception, retType, parameters, this);
                    if (!context.ExceptionHandled)
                        throw context.Exception;
                }
                else
                {
                    return result.Result;
                }
                return null;
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
                case RequestType.Delete:
                    rd = ClientSetting.Client.DeleteAsync(uri);
                    break;
                case RequestType.Put:
                    rd= HttpWebNetwork.PutRequest(uri, objs?[0]?.ToJsonString());
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
            if(objs[0] is FileTransferData)
            {
                return HttpWebNetwork.SendStream(uri, objs);
            }
            else
                return HttpWebNetwork.SendStream(uri, sa.ContentName, sa.ContentType, objs);
        }
        /// <summary>
        /// 设置请求头
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetHeader(string key, string value)
        {
            ClientSetting.SetHeader(key, value);
        }
    }
}

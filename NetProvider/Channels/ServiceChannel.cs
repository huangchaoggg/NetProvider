using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NetProvider.Network;
using NetProvider.Network.Inter;

namespace NetProvider.Channels
{
    /// <summary>
    /// 定义Web服务通道
    /// </summary>
    //[AOPAttribute]
    public abstract class ServiceChannel
    {
        public string Uri { get; private set; }
        /// <summary>
        /// web访问器
        /// </summary>
        public IHttpWebNetwork HttpWebNetwork { get; set; } = new HttpWebNetwork();
        public ServiceChannel(string uri)
        {
            this.Uri = uri;
        }
        
        public async Task<object> Invok(string InterfaceName,string methodName,params object[] objs)
        {
            Type t = this.GetType();
            MethodInfo info= t.GetInterface(InterfaceName).GetMethod(methodName);
            Attribute[] attributes = Attribute.GetCustomAttributes(info);
            return await RunMethod(attributes,info.ReturnType,info.GetParameters(),objs);
        }
        public async Task<object> RunMethod(Attribute[] attributes, Type retType, ParameterInfo[] parameters, params object[] objs)
        {
            RequestAttribute ra = attributes.FirstOrDefault(s => s is RequestAttribute) as RequestAttribute;// Attribute.GetCustomAttribute(call.MethodBase, typeof(RequestAttribute)) as RequestAttribute;
            if (ra == null)
            {
                throw new MessageException("特性不存在");
            }
            Task<HttpResponseMessage> rd = Request(ra, parameters, objs);
            if (retType == typeof(Task<HttpResponseMessage>))
            {
                return rd;
            }
            return await rd.ContinueWith(async(vv) =>
            {
                var result= vv.Result;
                var str=await result.Content.ReadAsStringAsync();
                if (retType.IsGenericType)
                {
                    Type t= retType.GenericTypeArguments[0];
                    return str.ToObject(t);
                }
                return str;
            });
        }
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="ra"></param>
        /// <param name="parameters">参数属性</param>
        /// <param name="objs">参数</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Request(RequestAttribute ra, ParameterInfo[] parameters, params object[] objs)
        {
            HttpResponseMessage rd = null;
            string uri = HttpWebHelper.PathCombine(Uri, ra.Uri);
            switch (ra.RequestType)
            {
                case RequestType.Get:
                    rd =await GetRequest(uri, parameters, objs);
                    break;
                default:
                    rd =await OtherRequest(uri, objs);
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
                rd =await HttpWebNetwork.GetRequest(uri);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (ParameterInfo p in parameters)
                {
                    sb.Append($" {p.Name}={objs[p.Position].ToString()}&");
                }
                string ss = sb.ToString().TrimEnd('&');
                rd =await HttpWebNetwork.GetRequest($"{uri}?{ss}");
            }
            return rd;
        }
        /// <summary>
        /// 其他请求处理
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> OtherRequest(string uri, params object[] objs)
        {
            HttpResponseMessage rd;
            if (objs == null || objs.Length == 0)
            {
                rd = await HttpWebNetwork.PostRequest(uri);
            }
            else
            {
                string body = objs[0].ToJsonString();
                rd =await HttpWebNetwork.PostRequest(uri, body);
            }
            return rd;
        }
    }
    /// <summary>
    /// 定义web上传下载服务通道
    /// </summary>
    public abstract class FileServiceChannel : ServiceChannel
    { 
        public FileServiceChannel(string uri) : base(uri) { }
    }
    /// <summary>
    /// 定义Socket服务通道
    /// </summary>
    public abstract class SocketServiceChannel : ServiceChannel
    {
        public SocketServiceChannel(string uri) : base(uri) { }
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

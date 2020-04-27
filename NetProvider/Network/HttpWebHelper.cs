using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;

namespace NetProvider.Network
{
    /// <summary>
    /// Htttp帮助类
    /// </summary>
    public static class HttpWebHelper
    {
        static HttpWebHelper()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.UseCookies = true;
            handler.AllowAutoRedirect = true;
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            handler.CookieContainer = CookieContainer;
            HttpClientCurrent = new HttpClient(handler);
            HttpClientCurrent.Timeout = new TimeSpan(0, 0, 30);
        }
        public static HttpClient HttpClientCurrent { get;private set; }
        public static CookieContainer CookieContainer { get; private set; } = new CookieContainer();
        public static Dictionary<string,string> ContentHeaders { get; private set; } = new Dictionary<string, string>();
        /// <summary>
        /// 将多个URL路径合并成一个
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string PathCombine(params string[] strs)
        {
            StringBuilder sb =new StringBuilder();
            foreach (string s in strs)
            {
                string ns=s.TrimEnd('/', '\\');
                string[] paths= ns.Split('\\', '/');
                sb.Append(string.Join("/", paths));
            }
            return sb.ToString().TrimEnd('/');
        }
       
    }
}

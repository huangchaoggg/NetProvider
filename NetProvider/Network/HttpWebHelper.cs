using System;
using System.Net;
using System.Text;
using System.Linq;

namespace NetProvider.Network
{
    /// <summary>
    /// Htttp帮助类
    /// </summary>
    public static class HttpWebHelper
    {
        public static CookieContainer CookieContainer { get; private set; } = new CookieContainer();
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
                string[] paths= s.Split('\\', '/');
                sb.Append(string.Join("/", paths));
                sb.Append("/");
            }
            return sb.ToString().TrimEnd('/');
        }
    }
}

using System.Text;

namespace NetProvider.Network
{
    /// <summary>
    /// Htttp帮助类
    /// </summary>
    public static class HttpWebHelper
    {
        public static string PathCombine(params string[] strs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in strs)
            {
                string ns = s.TrimEnd('/', '\\');
                string[] paths = ns.Split('\\', '/');
                sb.Append(string.Join("/", paths));
            }
            return sb.ToString().TrimEnd('/');
        }

    }
}

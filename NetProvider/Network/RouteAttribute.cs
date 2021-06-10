using System;

namespace NetProvider.Network
{
    /// <summary>
    /// 主机
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface|AttributeTargets.Method)]
    public class RouteAttribute:Attribute
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// 包含主机地址的标签
        /// </summary>
        /// <param name="uri"></param>
        public RouteAttribute(string uri=null)
        {
            this.Uri = uri;
        }
    }
}

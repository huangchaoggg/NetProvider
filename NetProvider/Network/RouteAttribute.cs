using System;

namespace NetProvider.Network
{
    /// <summary>
    /// 路由
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface|AttributeTargets.Method)]
    public abstract class RouteAttribute:Attribute
    {
        /// <summary>
        /// 路由地址
        /// </summary>
        public string Uri { get; set; }

        ///// <summary>
        ///// 包含主机地址的标签
        ///// </summary>
        ///// <param name="uri"></param>
        //public RouteAttribute(string uri)
        //{
        //    this.Uri = uri;
        //}
    }
}

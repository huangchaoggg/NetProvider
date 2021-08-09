using System;

namespace NetProvider.Network
{
    /// <summary>
    /// 路由路径
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestAttribute : RouteAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public RequestType RequestType { get;private set; }

        /// <summary>
        /// 方法属性
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="uri"></param>
        public RequestAttribute(RequestType requestType, string uri)
        {
            Uri = uri;
            RequestType = requestType;
        }
    }
}

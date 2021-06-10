using System;

namespace NetProvider.Network
{
    /// <summary>
    /// 
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
        public RequestAttribute(RequestType requestType, string uri):base(uri)
        {
            RequestType = requestType;
        }
    }
}

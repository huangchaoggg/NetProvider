using System;

namespace NetProvider.Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestAttribute : Attribute
    {
        public RequestType RequestType { get; set; }

        public string Uri { get; set; }

        /// <summary>
        /// 方法属性
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="uri"></param>
        public RequestAttribute(RequestType requestType, string uri)
        {
            RequestType = requestType;
            Uri = uri;
        }
    }
}

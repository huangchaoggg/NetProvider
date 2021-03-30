using System;

namespace NetProvider.Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class StreamAttribute:Attribute
    {
        public StreamAttribute(string uri, string contentName="file", string contentType= "application/octet-stream")
        {
            Uri = uri;
            ContentName = contentName;
            ContentType = contentType;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Uri { get; private set; }

        public string ContentName { get; private set; }
        public string ContentType { get; internal set; }
    }
}

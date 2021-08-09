using System;

namespace NetProvider.Network
{
    /// <summary>
    /// 定义文件上传
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class FileTransferAttribute: RouteAttribute
    {
        public FileTransferAttribute(string uri, string contentName="file", string contentType= "application/octet-stream")
        {
            Uri = uri;
            ContentName = contentName;
            ContentType = contentType;
        }
        public string ContentName { get; private set; }

        public string ContentType { get; internal set; }
    }
}

namespace NetProvider
{
    public enum RequestType
    {
        /// <summary>
        /// 向特定的资源发出请求
        /// </summary>
        Get = 0,
        /// <summary>
        /// 向指定资源提交数据进行处理请求（例如提交表单或者上传文件）。数据被包含在请求体中。POST请求可能会导致新的资源的创建和/或已有资源的修改
        /// </summary>
        Post = 1,
        /// <summary>
        /// 向指定资源位置上传其最新内容
        /// </summary>
        Put = 2,
        /// <summary>
        /// 回显服务器收到的请求，主要用于测试或诊断
        /// </summary>
        Trace = 3,
        /// <summary>
        /// 请求服务器删除Request-URI所标识的资源。
        /// </summary>
        Delete = 4,
        /// <summary>
        /// 返回服务器针对特定资源所支持的HTTP请求方法。也可以利用向Web服务器发送’*’的请求来测试服务器的功能性。
        /// </summary>
        Options = 5,
        /// <summary>
        /// 向服务器索要与GET请求相一致的响应，只不过响应体将不会被返回。这一方法可以在不必传输整个响应内容的情况下，就可以获取包含在响应消息头中的元信息
        /// </summary>
        Head = 6,
        /// <summary>
        /// HTTP/1.1协议中预留给能够将连接改为管道方式的代理服务器。
        /// </summary>
        Connect = 7
    }
}
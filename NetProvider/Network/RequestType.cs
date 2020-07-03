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
        /// 请求服务器删除Request-URI所标识的资源。
        /// </summary>
        Delete = 4,
    }
}
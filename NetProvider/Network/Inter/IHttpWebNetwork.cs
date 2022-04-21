using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetProvider.Network.Inter
{
    public interface IHttpWebNetwork : IWebNetwork
    {
        /// <summary>
        /// Http头
        /// </summary>
        //HttpRequestHeaders Headers { get; set; }
        /// <summary>
        /// 请求Get数据
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> GetRequest(string uri);


        Task<HttpResponseMessage> PostRequest(string uri);


        Task<HttpResponseMessage> PostRequest(string uri, string body);
        /// <summary>
        /// 文件流直传
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [Obsolete("应使用SendStream")]
        Task<HttpResponseMessage> PostRequest(string uri, FileStream body);
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="contentName"></param>
        /// <param name="contentType"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendStream(string uri,string contentName,string contentType, params object[] objs);
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendStream(string uri, params object[] objs);
        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="type"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> HttpRequest(string uri, RequestType type, string body);
        Task<HttpResponseMessage> PutRequest(string uri, string v);
    }
}

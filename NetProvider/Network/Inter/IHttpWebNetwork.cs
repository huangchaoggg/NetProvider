using System.Net.Http;
using System.Threading.Tasks;

namespace NetProvider.Network.Inter
{
    public interface IHttpWebNetwork:IWebNetwork
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


        Task<HttpResponseMessage> PostRequest(string uri,string body);

        Task<HttpResponseMessage> HttpRequest(string uri, RequestType type, string body);
    }
}

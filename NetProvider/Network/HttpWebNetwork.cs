using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NetProvider.Network.Inter;
using System.Linq;

namespace NetProvider.Network
{
    /// <summary>
    /// http请求
    /// </summary>
    public class HttpWebNetwork : IHttpWebNetwork
    {
        
        private static HttpClient httpClient= HttpWebHelper.HttpClientCurrent;
        public async Task<HttpResponseMessage> GetRequest(string uri)
        {
            return await httpClient.GetAsync(uri);
            //return await HttpRequest(uri, RequestType.Get, null);
        }

        public async Task<HttpResponseMessage> PostRequest(string uri)
        {
            HttpContent content = new StringContent("");
            SetContentHeader(content.Headers);
            return await httpClient.PostAsync(uri,content);
        }

        public async Task<HttpResponseMessage> PostRequest(string uri, string body)
        {
            HttpContent content = new StringContent(body);
            SetContentHeader(content.Headers);
            return await httpClient.PostAsync(uri, content);
        }
        private void SetContentHeader(HttpContentHeaders headers)
        {
            foreach (var header in HttpWebHelper.ContentHeaders)
            {
                try
                {
                    headers.Remove(header.Key);
                    headers.Add(header.Key, header.Value);
                }
                catch (Exception e)
                {
                    throw new MessageException("Herder参数设置错误");
                }
            }
        }
        public virtual async Task<HttpResponseMessage> HttpRequest(string uri, RequestType type, string body) {

            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(type.ToString()), uri);
            HttpContent content=null;
            if (type != RequestType.Get)
            {
                if (string.IsNullOrWhiteSpace(body))
                {
                    throw new MessageException("缺少Body参数");
                }
                content = new StringContent(body);
            }
            SetContentHeader(content.Headers);
            message.Content = content;
            return await httpClient.SendAsync(message);
        }
    }
}

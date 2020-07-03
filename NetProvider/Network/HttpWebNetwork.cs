using NetProvider.Network.Inter;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NetProvider.Network
{
    /// <summary>
    /// http请求
    /// </summary>
    public class HttpWebNetwork : IHttpWebNetwork
    {
        private HttpClientSetting clientSetting;
        public HttpWebNetwork(HttpClientSetting clientSetting)
        {
            this.clientSetting = clientSetting;
            httpClient = clientSetting.Client;
        }
        private HttpClient httpClient =null;
        public async Task<HttpResponseMessage> GetRequest(string uri)
        {
            return await HttpRequest(uri,RequestType.Get,null);
        }

        public async Task<HttpResponseMessage> PostRequest(string uri)
        {
            HttpContent content = new StringContent("");
            SetContentHeader(content.Headers);
            return await httpClient.PostAsync(uri, content);
        }

        public async Task<HttpResponseMessage> PostRequest(string uri, string body)
        {
            HttpContent content = new StringContent(body);
            SetContentHeader(content.Headers);
            return await httpClient.PostAsync(uri, content);
        }
        private void SetContentHeader(HttpHeaders headers)
        {
            foreach (var header in clientSetting.DefaultContentHeaders)
            {
                if (headers.Contains(header.Key))
                    headers.Remove(header.Key);
                headers.Add(header.Key, header.Value);
            }
        }
        public virtual async Task<HttpResponseMessage> HttpRequest(string uri, RequestType type, string body)
        {

            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(type.ToString()), uri);
            HttpContent content = null;
            if (type != RequestType.Get)
            {
                if (string.IsNullOrWhiteSpace(body))
                {
                    throw new MessageException("缺少Body参数");
                }
                content = new StringContent(body);
                SetContentHeader(content.Headers);
                message.Content = content;
            }
            return await httpClient.SendAsync(message);
        }
        /// <summary>
        /// 文件上载
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostRequest(string uri, FileStream body)
        {
            byte[] bytes = new byte[body.Length];
            body.Read(bytes, 0, bytes.Length);
            string boundary = string.Format("----------------------------{0}", DateTime.Now.Ticks.ToString("x"));
            MultipartFormDataContent content = new MultipartFormDataContent(boundary);
            var sc = new ByteArrayContent(bytes);
            sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(sc, "files", Path.GetFileName(body.Name));
            var resp = await httpClient.PostAsync(uri, content);
            return resp;
        }
    }
}

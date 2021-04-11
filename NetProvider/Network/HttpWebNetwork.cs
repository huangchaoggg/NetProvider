using NetProvider.Core;
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
        }
        public Task<HttpResponseMessage> GetRequest(string uri)
        {
            try
            {
                return HttpRequest(uri, RequestType.Get, null);

            }catch(TaskCanceledException e)
            {
                throw new ProviderException(e.Message);
            }
        }

        public Task<HttpResponseMessage> PostRequest(string uri)
        {
            HttpContent content = new StringContent("");
            SetContentHeader(content.Headers);
            return clientSetting.Client.PostAsync(uri, content);
        }

        public Task<HttpResponseMessage> PostRequest(string uri, string body)
        {
            HttpContent content = new StringContent(body);
            SetContentHeader(content.Headers);
            return clientSetting.Client.PostAsync(uri, content);
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
        public virtual Task<HttpResponseMessage> HttpRequest(string uri, RequestType type, string body)
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
            return clientSetting.Client.SendAsync(message);
        }
        /// <summary>
        /// 文件上载
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [Obsolete]
        public Task<HttpResponseMessage> PostRequest(string uri, FileStream body)
        {
            byte[] bytes = new byte[body.Length];
            body.Read(bytes, 0, bytes.Length);
            string boundary = string.Format("----------------------------{0}", DateTime.Now.Ticks.ToString("x"));
            MultipartFormDataContent content = new MultipartFormDataContent(boundary);
            var sc = new ByteArrayContent(bytes);
            sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(sc, "files", Path.GetFileName(body.Name));
            var resp = clientSetting.Client.PostAsync(uri, content);
            return resp;
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="contentName"></param>
        /// <param name="contentType"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> SendStream(string uri, string contentName, string contentType, params object[] objs)
        {
            if(objs==null) throw new ProviderException("数据不能为空");

            string boundary = string.Format("----------------------------{0}", DateTime.Now.Ticks.ToString("x"));
            MultipartFormDataContent content = new MultipartFormDataContent(boundary);

            foreach (object v in objs)
            {
                FileStream fs;
                if (v is string)
                {
                    fs= File.OpenRead(v as string);
                }
                else if(v is FileStream)
                {
                    fs = v as FileStream;
                }
                else
                {
                    throw new ProviderException("仅支持以'string'文件路径或'FileStream'流的形式");
                }
                var sc = new StreamContent(fs);
                sc.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(sc, contentName, Path.GetFileName(fs.Name));
            }            
            var resp = clientSetting.Client.PostAsync(uri, content);
            return resp;
        }
    }
}

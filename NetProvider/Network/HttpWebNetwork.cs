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

namespace NetProvider.Network
{
    /// <summary>
    /// http请求
    /// </summary>
    public class HttpWebNetwork : IHttpWebNetwork
    {
        public HttpWebNetwork()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.UseCookies = true;
            handler.AllowAutoRedirect = true;
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            handler.CookieContainer = HttpWebHelper.CookieContainer;
            httpClient = new HttpClient(handler);
            httpClient.Timeout = new TimeSpan(0, 0, 30);
        }
        //public ResponseData GetRequest(string uri) {

        //    return GetRequest(uri, null);
        //}
        private HttpClient httpClient;
        public async Task<HttpResponseMessage> GetRequest(string uri)
        {
            return await HttpRequest(uri, RequestType.Get, null);
        }

        //public ResponseData PostRequest(string uri)
        //{
        //    return PostRequest(uri, null);
        //}

        public async Task<HttpResponseMessage> PostRequest(string uri)
        {
            return await PostRequest(uri, null);
        }

        public async Task<HttpResponseMessage> PostRequest(string uri, string body)
        {
            return await HttpRequest(uri, RequestType.Post, body);
        }

        //private ResponseData GetResponseValue(HttpWebResponse response, WebRequest request)
        //{
        //    string contentEncoding = response.ContentEncoding;
        //    string charSet = response.CharacterSet;
        //    //byte[] readBytes = null;
        //    //ResponseData responseData = new ResponseData()
        //    //{
        //    //    CharSet = charSet,
        //    //    ResponseUri = response.ResponseUri,
        //    //    RequestUri = request.RequestUri,
        //    //    RequestHeaders = request.Headers,
        //    //    ResponseHeaders = response.Headers
        //    //};
        //    if (contentEncoding?.ToLower() == "gzip")
        //    {
        //        using (GZipStream gz = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
        //        {
        //            byte[] ContentBytes = ReadToBytes(gz);
        //        }
        //    }
        //    else
        //    {
        //        using (Stream stream = response.GetResponseStream())
        //        {
        //            byte[] ContentBytes = ReadToBytes(stream);
        //        }
        //    }

        //    return null;
        //}

        //private byte[] ReadToBytes(Stream stream)
        //{
        //    BinaryReader br = new BinaryReader(stream);
        //    List<byte> byteList = new List<byte>();
        //    byte[] vs;
        //    do
        //    {
        //        vs = br.ReadBytes(1024);
        //        byteList.AddRange(vs);

        //    } while (vs.Length >= 1024);
        //    return byteList.ToArray();
        //}

        public virtual async Task<HttpResponseMessage> HttpRequest(string uri, RequestType type, string body) {

            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(type.ToString()), uri);
            //message.Headers.TryAddWithoutValidation("content-type", "application/json");
            //message.Headers.TryAddWithoutValidation("accept", "*/*");
            //message.Headers.Connection.Add("keep-alive");
            return await httpClient.SendAsync(message);
            //HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            //request.Method = type.ToString();
            //request.KeepAlive = true;
            //request.AllowAutoRedirect = true;
            ////InitHeader(ref request, webHeader);
            //if (!string.IsNullOrWhiteSpace(body))
            //{
            //    byte[] bytes = Encoding.UTF8.GetBytes(body);
            //    Stream stream = request.GetRequestStream();
            //    stream.Write(bytes, 0, bytes.Length);
            //}
            ////request.ClientCertificates = CertificateCollection;
            ////request.Credentials = CredentialCache.DefaultNetworkCredentials;
            //request.CookieContainer = HttpWebHelper.CookieContainer;
            //HttpWebResponse response = null;
            //try
            //{
            //    response = request.GetResponse() as HttpWebResponse;
            //}
            //catch (WebException e)
            //{
            //    response = e.Response as HttpWebResponse;
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //return GetResponseValue(response, request);
        }

        //private void SetHeader(ref HttpWebRequest request, WebHeaderCollection webHeader = null)
        //{
        //    //request.Headers = DefaultWebHeader;
        //    if (request.Headers == null)
        //    {
        //        request.Headers = new WebHeaderCollection();
        //    }
        //    if (webHeader != null)
        //    {
        //        foreach (string item in webHeader)
        //        {
        //            try
        //            {
        //                switch (item)
        //                {
        //                    case "Content-Type":
        //                        request.ContentType = webHeader[item];
        //                        continue;
        //                    case "User-Agent":
        //                        request.UserAgent = webHeader[item];
        //                        continue;
        //                    case "Accept":
        //                        request.Accept = webHeader[item];
        //                        continue;
        //                    case "Connection":
        //                        request.Connection = webHeader[item];
        //                        continue;
        //                    case "Transfer-Encoding":
        //                        request.TransferEncoding = webHeader[item];
        //                        continue;
        //                    case "Host":
        //                        request.Host = webHeader[item];
        //                        continue;
        //                    default:
        //                        request.Headers.Add(item, webHeader[item]);
        //                        continue;
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                continue;
        //            }
        //        }
        //    }
        //}
        //private void InitHeader(ref HttpWebRequest request, WebHeaderCollection webHeader = null)
        //{
        //    SetHeader(ref request, DefaultWebHeader);//先设置默认Header
        //    SetHeader(ref request, webHeader);//再设置函数Header
        //}
    }
}

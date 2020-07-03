using NetProvider.Filter;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace NetProvider.Network
{
    public class HttpClientSetting
    {
        public HttpClientSetting()
        {
            CookieContainer CookieContainer = new CookieContainer();
            HttpClientHandler ClientHandler = new HttpClientHandler();
            ClientHandler.UseDefaultCredentials = true;
            ClientHandler.UseCookies = true;
            ClientHandler.AllowAutoRedirect = true;
            ClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
            ClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            ClientHandler.CookieContainer = CookieContainer;
            Client = new HttpClient(ClientHandler);
            Client.Timeout = new TimeSpan(0, 0, 10);
        }
        public HttpClientSetting(HttpClient httpClient)
        {
            Client = httpClient;
        }
        private static object _SettingLock = new object();
        private static HttpClientSetting defaultSetting;
        /// <summary>
        /// 默认内容头
        /// </summary>
        public Dictionary<string, string> DefaultContentHeaders { get; private set; } = new Dictionary<string, string>();
        public List<IFilter> Filters { get; } = new List<IFilter>();
        public HttpClient Client { get; private set; }
        /// <summary>
        /// 设置heder
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public void SetHeader(string v1, string v2)
        {
            try
            {
                if (!Client.DefaultRequestHeaders.TryAddWithoutValidation(v1, v2))
                {
                    Client.DefaultRequestHeaders.Remove(v1);
                    Client.DefaultRequestHeaders.Add(v1, v2);
                }
            }
            catch (InvalidOperationException ex)
            {
                if (DefaultContentHeaders.ContainsKey(v1))
                {
                    DefaultContentHeaders[v1] = v2;
                }
                DefaultContentHeaders.Add(v1, v2);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// HttpClient默认配置
        /// </summary>
        public static HttpClientSetting DefaultSetting
        {
            get
            {
                lock (_SettingLock)
                {
                    if (defaultSetting == null)
                    {
                        defaultSetting = new HttpClientSetting();
                    }
                }
                return defaultSetting;
            }
        }
    }
}

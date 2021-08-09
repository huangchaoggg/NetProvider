using NetProvider.Core.Filter;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace NetProvider.Network
{
    public class HttpClientSetting
    {
        private CookieContainer CookieContainer;
        public HttpClientHandler ClientHandler { get; }
        public HttpClientSetting(params X509Certificate2[] certificate2s)
        {
            CookieContainer = new CookieContainer();
            ClientHandler = new HttpClientHandler();
            //ClientHandler.ServerCertificateCustomValidationCallback=(mesg,cret,chain,bl)=>true;
            ClientHandler.UseDefaultCredentials = true;
            ClientHandler.UseCookies = true;
            ClientHandler.AllowAutoRedirect = true;
            ClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
            ClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            ClientHandler.CookieContainer = CookieContainer;
            if (certificate2s != null && certificate2s.Length > 0)
                ClientHandler.ClientCertificates.AddRange(certificate2s);

            Buider(new HttpClient(ClientHandler));
        }
        [Obsolete("请继承该类然后调用 Buider 函数")]
        public HttpClientSetting(HttpClient httpClient)
        {
            Buider(httpClient);
        }
        /// <summary>
        /// 构建
        /// </summary>
        protected void Buider(HttpClient httpClient)
        {
            Client = httpClient;
            FilterManagement = new FilterManagement();
            DefaultContentHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// 默认内容头
        /// </summary>
        public Dictionary<string, string> DefaultContentHeaders { get; private set; }

        public FilterManagement FilterManagement { get; private set; }
        public HttpClient Client { get; private set; }

        public void AddMessageFilter(IMessageFilter filter)
        {
            FilterManagement.AddMessageFilter(filter);
        }
        public void AddExceptionFilter(IExceptionFilter filter)
        {
            FilterManagement.AddExceptionFilter(filter);
        }
        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="certificate"></param>
        public void SetCertificate(params X509Certificate[] certificate)
        {
            if (certificate != null && certificate.Length > 0)
                ClientHandler.ClientCertificates.AddRange(certificate);
        }

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
                    if (DefaultContentHeaders.ContainsKey(v1))
                    {
                        DefaultContentHeaders[v1] = v2;
                    }
                    DefaultContentHeaders.Add(v1, v2);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static object _SettingLock = new object();
        private static HttpClientSetting defaultSetting;
        /// <summary>
        /// HttpClient默认配置
        /// </summary>
        public static HttpClientSetting DefaultSetting
        {
            get
            {
                if (defaultSetting == null)
                {
                    lock (_SettingLock)
                    {
                        if (defaultSetting == null)
                        {
                            defaultSetting = new HttpClientSetting();
                        }
                    }
                }
                return defaultSetting;
            }
        }
    }
}

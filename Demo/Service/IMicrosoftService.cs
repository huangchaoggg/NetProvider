using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NetProvider;
using NetProvider.Channels;
using NetProvider.Network;

namespace Demo.Service
{
    public interface IMicrosoftService
    {
        /// <summary>
        /// 请求数据调用
        /// </summary>
        /// <returns></returns>
        [Request(RequestType.Get, "zh-cn/dotnet/breadcrumb/toc.json")]
        Task<HttpResponseMessage> GetJson();
        /// <summary>
        ///对象类型调用
        /// </summary>
        /// <returns></returns>
        [Request(RequestType.Get, "zh-cn/dotnet/breadcrumb/toc.json")]
        Task<Meta> GetMeta();
        /// <summary>
        /// 无返回值调用
        /// </summary>
        [Request(RequestType.Get, "/zh-cn/dotnet/core/tutorials/using-with-xplat-cli")]
        Task Tutorials();
        //"Accept: */*",
        //"Accept - Encoding: gzip, deflate, br",
        //"Accept - Language: zh - CN, zh;q = 0.9, en;q = 0.8, zh - TW;q = 0.7",
        //"Connection: keep - alive"
    }
    public class MicrosoftService : ChannelFactory<IMicrosoftService>, IMicrosoftService
    {
        public MicrosoftService(string url) : base(url){
            //WebHeaderCollection wc= new WebHeaderCollection();
            //wc.Add(HttpRequestHeader.Accept, "*/*");
            //wc.Add(HttpRequestHeader.AcceptEncoding, "Accept-Encoding");
            //wc.Add(HttpRequestHeader.Connection, "keep-alive");
        }
        
        public Task<HttpResponseMessage> GetJson()
        {
            return this.Channel.GetJson();
        }

        public Task<Meta> GetMeta()
        {
            return this.Channel.GetMeta();
        }

        public Task Tutorials()
        {
           return this.Channel.Tutorials();
        }
    }
}

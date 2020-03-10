using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetProvider.Network
{
    public class ResponseData: HttpResponseMessage
    {
        //public WebHeaderCollection RequestHeaders { get;internal set; }

        //public WebHeaderCollection ResponseHeaders { get;internal set; }
        ///// <summary>
        ///// 请求Uri
        ///// </summary>
        //public Uri RequestUri { get;internal set; }
        ///// <summary>
        ///// 响应uri
        ///// </summary>
        //public Uri ResponseUri { get;internal set; }

        ///// <summary>
        ///// 字节体内容表示
        ///// </summary>
        //public byte[] ContentBytes { get;internal set; }
        ///// <summary>
        ///// 字符编码
        ///// </summary>
        //public string CharSet { get;internal set; }

        //public async Task<string> ToString() {
        //    try
        //    {
        //       return await this.Content.ReadAsStringAsync();
        //    }
        //    catch
        //    {
        //        return await Content.ReadAsByteArrayAsync().ContinueWith<string>((rt) =>
        //        {
        //           return Encoding.UTF8.GetString(rt.Result);
        //        });
        //    }
        //}

        public async Task<string> ToString(Encoding encoding)
        {
            return await Content.ReadAsByteArrayAsync().ContinueWith((rt) =>
            {
                return encoding?.GetString(rt.Result);
            });  
        }

        public async Task<string> ToString(string charSet)
        {
            if (string.IsNullOrWhiteSpace(charSet))
            {
                throw new DataException("编码为空");
            }
            Encoding encoding = Encoding.GetEncoding(charSet);
            if (encoding != null)
            {
                throw new DataException("获取编码失败");
            }
            return await ToString(encoding);
        }
    }
}

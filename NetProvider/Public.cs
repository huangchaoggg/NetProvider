using System;
using System.Net;
using NetProvider.Channels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Permissions;

namespace NetProvider
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestAttribute : Attribute
    {
        public RequestType RequestType { get; set; }

        public string Uri { get; set; }

        /// <summary>
        /// 方法属性
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="uri"></param>
        public RequestAttribute(RequestType requestType,string uri) {
            RequestType = requestType;
            Uri = uri;
        }
    }
    ///// <summary>
    ///// 贴在类上的标签
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    //internal sealed class AOPAttribute : Attribute, IContributeObjectSink
    //{
    //    public AOPAttribute() : base("AOP")
    //    {
    //    }

    //    /// <summary>
    //    /// 实现消息接收器接口
    //    /// </summary>
    //    /// <param name="obj"></param>
    //    /// <param name="next"></param>
    //    /// <returns></returns>
    //    public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink next)
    //    {
    //        return new TransactionAop(obj, next);
    //    }
    //}
    public static class JsonExpand
    {
        public static string ToJsonString(this object obj)
        {
           return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 将json字符串转换为指定类型对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object ToObject(this string obj,Type t) {
            try
            {
                return JsonConvert.DeserializeObject(obj.ToString(),t);
            }
            catch(Exception e)
            {
                throw new ProviderException("转换出错",e);
                //return Activator.CreateInstance(t);
            }
        }
        /// <summary>
        /// 将json字符串转换为指定类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string str) where T:class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch
            {
                throw new FormatException("转换失败");
            }
        }
    }
}

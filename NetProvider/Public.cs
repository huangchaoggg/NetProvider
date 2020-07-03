using Newtonsoft.Json;
using System;

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
        public RequestAttribute(RequestType requestType, string uri)
        {
            RequestType = requestType;
            Uri = uri;
        }
    }
    //public class SocketSendAttribute : Attribute { }

    //public class SocketReceiveAttribute : Attribute { }

    public static class JsonExpand
    {
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static string ToJsonString(this object obj, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(obj, converters);
        }
        /// <summary>
        /// 将json字符串转换为指定类型对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object ToObject(this string obj, Type t)
        {
            try
            {
                return JsonConvert.DeserializeObject(obj.ToString(), t);
            }
            catch (Exception e)
            {
                throw new FormatException($"转换失败:{e.Message}", e);
            }
        }
        /// <summary>
        /// 将json字符串转换为指定类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string str) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception e)
            {
                throw new ProviderException($"转换失败:{e.Message}", e);
            }
        }
    }
}

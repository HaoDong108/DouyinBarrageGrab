using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BarrageGrab
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 判断字符串是否为空或空白
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// 转为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            if (obj is string)
            {
                return obj.ToString();
            }            
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 转为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T AsObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 获取Value或其Value默认值
        /// </summary>
        public static TValue ValueOrDefault<Tkey, TValue>(this IDictionary<Tkey, TValue> dic, Tkey key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            return default;
        }

        /// <summary>
        /// 获取Value或其Value默认值
        /// </summary>
        public static TValue ValueOrDefault<Tkey, TValue>(this IDictionary<Tkey, TValue> dic, Tkey key, TValue defaultValue)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// 遍历集合
        /// </summary>        
        public static void Foreach<T>(this IEnumerable<T> list, Action<T> func)
        {
            foreach (var item in list) func(item);
        }

        /// <summary>
        /// 遍历集合
        /// </summary>    
        public static void Foreach<T>(this IEnumerable<T> list, Action<T, int> func)
        {
            int i = 0;
            foreach (var item in list) func(item, i++);
        }

        
        public static CookieCollection ToCookies(this string cookieStr, string domain)
        {
            var cookies = new CookieCollection();
            if (!string.IsNullOrEmpty(cookieStr))
            {
                var cookieArray = cookieStr.Split(';');
                foreach (var cookie in cookieArray)
                {
                    var cookieParts = cookie.Split('=');
                    if (cookieParts.Length == 2)
                    {
                        var cookieName = cookieParts[0].Trim();
                        var cookieValue = cookieParts[1].Trim();
                        var newCookie = new Cookie(cookieName, Uri.EscapeDataString(cookieValue));
                        newCookie.Path = "/";
                        newCookie.Domain = domain;
                        cookies.Add(newCookie);
                    }
                }
            }
            return cookies;
        }
        
        /// <summary>
        /// 追加另一个字典
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Append(this IEnumerable<KeyValuePair<string, string>> dic, IEnumerable<KeyValuePair<string, string>> other)
        {
            var res = new Dictionary<string, string>();
            foreach (var item in dic)
            {
                res.Add(item.Key, item.Value);
            }
            foreach (var item in other)
            {
                if (res.ContainsKey(item.Key))
                {
                    res[item.Key] = item.Value;
                }
                else
                {
                    res.Add(item.Key, item.Value);
                }                
            }
            return res;
        }
        

        /// <summary>
        /// 转换为Cookie对象列表
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static CookieCollection ToCookies(this IDictionary<string, string> dic, string domain)
        {
            var cookieCollection = new CookieCollection();
            foreach (var item in dic)
            {
                cookieCollection.Add(new Cookie(item.Key, item.Value, "/", domain));
            }
            return cookieCollection;
        }

        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="dic"></param>
        /// <param name="domain"></param>
        public static void PushCookies(this CookieContainer cookies, string domain, IDictionary<string, string> dic)
        {
            if (cookies == null) return;
            var cookieCollection = dic.ToCookies(domain);
            cookies.Add(cookieCollection);
        }       
    }
}

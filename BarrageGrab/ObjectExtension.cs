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
        public static string ToJson(this object obj,bool format = false)
        {
            if (obj is string)
            {
                return obj.ToString();
            }
            return JsonConvert.SerializeObject(obj, format?Formatting.Indented:Formatting.None);
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
            return default(TValue);
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

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] args)
        {
            return args.Contains(obj);
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool LikeIn(this string str, params string[] args)
        {
            return args.Any(a => str.Contains(a));
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

        /// <summary>
        ///  将字典转为查询参数
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string ToQueryParam(this IDictionary<string, string> dic, bool encode = true)
        {
            if (dic == null || dic.Count == 0)
                return string.Empty;

            var queryParams = dic.Select(kvp => kvp.Key + "=" + (encode ? WebUtility.UrlEncode(kvp.Value) : kvp.Value));
            return string.Join("&", queryParams);

        }

        /// <summary>
        /// 赋值字典，没有则添加，有则覆盖
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        /// <summary>
        ///  赋值字典，没有则添加，有则覆盖
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="kv"></param>
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dic, KeyValuePair<TKey, TValue> kv)
        {
            dic.Set(kv.Key, kv.Value);
        }

        /// <summary>
        /// 获取Uri查询参数
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDictionary<string, string> GetQueryParams(this UriBuilder uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var queryParams = uri.Query.TrimStart('?');

            if (string.IsNullOrEmpty(queryParams))
            {
                return new Dictionary<string, string>();
            }

            return queryParams
                .Split('&')
                .Select(param => param.Split('='))
                .ToDictionary(
                    keyValuePair => Uri.UnescapeDataString(keyValuePair[0]),
                    keyValuePair => Uri.UnescapeDataString(keyValuePair.Length > 1 ? keyValuePair[1] : ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="vals"></param>
        public static void AddQueryParam(this UriBuilder uri, IEnumerable<KeyValuePair<string, string>> vals)
        {
            if (uri == null) return;
            var dic = uri.GetQueryParams();
            foreach (var item in vals)
            {
                dic.Set(item);
            }
            uri.Query = dic.ToQueryParam();
        }

        /// <summary>
        /// 删除一个query项
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="key"></param>
        public static void RemoveQueryParam(this UriBuilder uri, string key)
        {
            if (uri == null) return;
            var dic = uri.GetQueryParams();
            dic.Remove(key);
            if (dic.Count > 0)
            {
                uri.Query = "?" + dic.ToQueryParam();
            }
            else
            {
                uri.Query = "";
            }
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void AddQueryParam(this UriBuilder uri, string key, string value)
        {
            if (uri == null) return;
            uri.AddQueryParam(new[] {
               new KeyValuePair<string, string>(key,value)
            });
        }

    }
}

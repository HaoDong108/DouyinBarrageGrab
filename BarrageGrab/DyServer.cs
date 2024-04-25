using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BarrageGrab.Modles;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;

namespace BarrageGrab
{
    /// <summary>
    /// 抖音服务
    /// </summary>
    internal class DyServer
    {
        /// <summary>
        /// 获取直播礼物列表
        /// </summary>
        /// <returns></returns>
        public static async Task<WebCastGiftPack> GetGifts()
        {
            var url = "https://live.douyin.com/webcast/gift/list/";
            var qparam = new Dictionary<string, string>()
            {
                {"device_platform","webapp"},
                {"aid","6383"},
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("referer", "https://live.douyin.com/405518163654");
            client.DefaultRequestHeaders.Add("authority", "www.douyin.com");

            var builder = new UriBuilder(url);
            builder.Query = string.Join("&", qparam.Select(x => $"{x.Key}={x.Value}"));

            try
            {
                var response = await client.GetAsync(builder.Uri);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError($"响应失败: {response.StatusCode}");
                    return null;
                }
                var json = await response.Content.ReadAsStringAsync();
                var jobj = JObject.Parse(json);
                var data = jobj?["data"].ToObject<WebCastGiftPack>();
                return data;
            }
            catch (Exception ex)
            {
                Logger.LogError($"礼物信息从服务器请求失败: {ex.Message}");
                return null;
            }            
        }
    }
}

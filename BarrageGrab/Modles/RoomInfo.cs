using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BarrageGrab.Modles
{
    [Serializable]
    public class RoomInfo
    {
        /// <summary>
        /// Web房间号
        /// </summary>
        public string WebRoomId { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 直播间名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否直播中
        /// </summary>
        public bool IsLive { get; set; }

        /// <summary>
        /// 实时在线人数
        /// </summary>
        public long UserCount { get; set; }

        /// <summary>
        /// 累计在线人数
        /// </summary>
        public string TotalUserCount { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public long LikeCount { get; set; }

        /// <summary>
        /// 直播间二维码图像链接
        /// </summary>
        public string QrcodeUrl { get; set; }

        /// <summary>
        /// 直播间地址
        /// </summary>
        public string LiveUrl { get; set; }

        /// <summary>
        /// 封面链接
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 直播间管理员用户id
        /// </summary>
        public List<string> AdminUserIds { get; set; } = new List<string>();

        /// <summary>
        /// 主播信息
        /// </summary>
        public RoomAnchor Owner { get; set; }

        /// <summary>
        /// 直播间的一些配置
        /// </summary>
        public RoomAuth AuthInfo { get; set; }

        /// <summary>
        /// 自身信息，Api方式没有
        /// </summary>
        public OdinData Odin { get; set; }

        /// <summary>
        /// Response请求头设置的 Ttwid，Api方式没有
        /// </summary>
        public string Ttwid { get; set; }

        public class RoomAnchor
        {
            /// <summary>
            /// 用户ID
            /// </summary>
            public string UserId { get; set; }

            /// <summary>
            /// SecUid
            /// </summary>
            public string SecUid { get; set; }

            /// <summary>
            /// 昵称
            /// </summary>
            public string Nickname { get; set; }

            /// <summary>
            /// 头像地址
            /// </summary>
            public string HeadUrl { get; set; }

            /// <summary>
            /// 关注状态 0未关注,1已关注,...
            /// </summary>
            public int FollowStatus { get; set; }
        }

        public class RoomAuth
        {
            /// <summary>
            /// 是否允许聊天
            /// </summary>
            public bool Chat { get; set; }

            /// <summary>
            /// 是否允许弹幕
            /// </summary>
            public bool Danmaku { get; set; }

            /// <summary>
            /// 是否允许送礼物
            /// </summary>
            public bool Gift { get; set; }

            /// <summary>
            /// 是否允许发红包
            /// </summary>
            public bool LuckMoney { get; set; }

            /// <summary>
            /// 是否允许点赞
            /// </summary>
            public bool Digg { get; set; }

            /// <summary>
            /// 是否允许房间贡献者
            /// </summary>
            public bool RoomContributor { get; set; }

            /// <summary>
            /// 是否允许使用道具
            /// </summary>
            public bool Props { get; set; }

            /// <summary>
            /// 是否允许查看用户名片
            /// </summary>
            public bool UserCard { get; set; }

            /// <summary>
            /// 是否允许查看 POI
            /// </summary>
            public bool POI { get; set; }

            /// <summary>
            /// 更多主播?
            /// </summary>
            public int MoreAnchor { get; set; }

            /// <summary>
            /// banner?
            /// </summary>
            public int Banner { get; set; }

            /// <summary>
            /// 分享配置?
            /// </summary>
            public int Share { get; set; }

            /// <summary>
            /// 用户角标?
            /// </summary>
            public int UserCorner { get; set; }

            /// <summary>
            /// 横屏?
            /// </summary>
            public int Landscape { get; set; }

            /// <summary>
            /// 横屏聊天?
            /// </summary>
            public int LandscapeChat { get; set; }

            /// <summary>
            /// 公屏?
            /// </summary>
            public int PublicScreen { get; set; }

            /// <summary>
            /// 礼物主播魔法表情配置?
            /// </summary>
            public int GiftAnchorMt { get; set; }

            /// <summary>
            /// 录屏配置?
            /// </summary>
            public int RecordScreen { get; set; }

            /// <summary>
            /// 打赏贴纸配置?
            /// </summary>
            public int DonationSticker { get; set; }

            /// <summary>
            /// 小时榜配置?
            /// </summary>
            public int HourRank { get; set; }

            /// <summary>
            /// 商务名片配置?
            /// </summary>
            public int CommerceCard { get; set; }

            /// <summary>
            /// 语音聊天配置?
            /// </summary>
            public int AudioChat { get; set; }

            /// <summary>
            /// 弹幕默认开关
            /// </summary>
            public int DanmakuDefault { get; set; }

            /// <summary>
            /// KTV 点歌?
            /// </summary>
            public int KtvOrderSong { get; set; }

            /// <summary>
            /// 选集?
            /// </summary>
            public int SelectionAlbum { get; set; }

            /// <summary>
            /// 点赞?
            /// </summary>
            public int Like { get; set; }

            /// <summary>
            /// 倍速播放?
            /// </summary>
            public int MultiplierPlayback { get; set; }

            /// <summary>
            /// 下载视频?
            /// </summary>
            public int DownloadVideo { get; set; }

            /// <summary>
            /// 收藏?
            /// </summary>
            public int Collect { get; set; }

            /// <summary>
            /// 定时下播?
            /// </summary>
            public int TimedShutdown { get; set; }

            /// <summary>
            /// 快进?
            /// </summary>
            public int Seek { get; set; }

            /// <summary>
            /// 举报?
            /// </summary>
            public int Denounce { get; set; }

            /// <summary>
            /// 踩?
            /// </summary>
            public int Dislike { get; set; }

            /// <summary>
            /// 只看 TA?
            /// </summary>
            public int OnlyTa { get; set; }

            /// <summary>
            /// 投屏?
            /// </summary>
            public int CastScreen { get; set; }

            /// <summary>
            /// 评论墙?
            /// </summary>
            public int CommentWall { get; set; }

            /// <summary>
            /// 弹幕样式
            /// </summary>
            public object BulletStyle { get; set; }

            /// <summary>
            /// 显示游戏插件?
            /// </summary>
            public int ShowGamePlugin { get; set; }

            /// <summary>
            /// PK礼物?
            /// </summary>
            public int VSGift { get; set; }

            /// <summary>
            /// PK话题?
            /// </summary>
            public int VSTopic { get; set; }

            /// <summary>
            /// PK排行榜?
            /// </summary>
            public int VSRank { get; set; }

            /// <summary>
            /// 管理员评论墙?
            /// </summary>
            public int AdminCommentWall { get; set; }

            /// <summary>
            /// 商务组件?
            /// </summary>
            public int CommerceComponent { get; set; }

            /// <summary>
            /// 抖音会员?
            /// </summary>
            public int DouPlus { get; set; }

            /// <summary>
            /// 游戏积分玩法?
            /// </summary>
            public int GamePointsPlaying { get; set; }

            /// <summary>
            /// 海报?
            /// </summary>
            public int Poster { get; set; }

            /// <summary>
            /// 精彩时刻?
            /// </summary>
            public int Highlights { get; set; }

            /// <summary>
            /// 打字评论状态?
            /// </summary>
            public int TypingCommentState { get; set; }

            /// <summary>
            /// 上下滑动引导?
            /// </summary>
            public int StrokeUpDownGuide { get; set; }

            /// <summary>
            /// 右上角统计浮层?
            /// </summary>
            public int UpRightStatsFloatingLayer { get; set; }

            /// <summary>
            /// 投屏显式?
            /// </summary>
            public int CastScreenExplicit { get; set; }

            /// <summary>
            /// 选集?
            /// </summary>
            public int Selection { get; set; }

            /// <summary>
            /// 行业服务?
            /// </summary>
            public int IndustryService { get; set; }

            /// <summary>
            /// 竖屏排行榜?
            /// </summary>
            public int VerticalRank { get; set; }

            /// <summary>
            /// 进场特效?
            /// </summary>
            public int EnterEffects { get; set; }

            /// <summary>
            /// 粉丝团?
            /// </summary>
            public int FansClub { get; set; }

            /// <summary>
            /// 表情外显?
            /// </summary>
            public int EmojiOutside { get; set; }

            /// <summary>
            /// 售票?
            /// </summary>
            public int CanSellTicket { get; set; }

            /// <summary>
            /// 抖音会员人气宝石?
            /// </summary>
            public int DouPlusPopularityGem { get; set; }

            /// <summary>
            /// 任务中心?
            /// </summary>
            public int MissionCenter { get; set; }

            /// <summary>
            ///扩展屏幕?
            /// </summary>
            public int ExpandScreen { get; set; }

            /// <summary>
            /// 粉丝群?
            /// </summary>
            public int FansGroup { get; set; }

            /// <summary>
            /// 话题?
            /// </summary>
            public int Topic { get; set; }

            /// <summary>
            /// 主播任务?
            /// </summary>
            public int AnchorMission { get; set; }

            /// <summary>
            /// 跑马灯?
            /// </summary>
            public int Teleprompter { get; set; }

            /// <summary>
            /// 长按?
            /// </summary>
            public int LongTouch { get; set; }

            /// <summary>
            /// 特殊样式
            /// </summary>
            public SpecialStyle SpecialStyle { get; set; }

            /// <summary>
            /// 固定聊天?
            /// </summary>
            public int FixedChat { get; set; }

            /// <summary>
            ///  竞猜？
            /// </summary>
            public int QuizGamePointsPlaying { get; set; }
        }

        public class ChatStyle
        {
            /// <summary>
            /// 是否禁用样式
            /// </summary>
            public int UnableStyle { get; set; }

            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }

            /// <summary>
            /// 关闭类型
            /// </summary>
            public int OffType { get; set; }
        }

        public class SpecialStyle
        {
            /// <summary>
            /// 聊天样式
            /// </summary>
            public ChatStyle Chat { get; set; }

            /// <summary>
            /// 点赞样式
            /// </summary>
            public ChatStyle Like { get; set; }
        }

        public class OdinData
        {
            public string user_id { get; set; }
            public string user_type { get; set; }
            public string user_is_auth { get; set; }
            public string user_is_login { get; set; }
            public string user_unique_id { get; set; }
        }


        /// <summary>
        /// 将 直播间页面元素 转为房间信息对象
        /// </summary>
        /// <param name="html"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Tuple<int, string> TryParseRoomPageHtml(string html, out RoomInfo result)
        {
            result = null;
            if (html.IsNullOrWhiteSpace())
            {
                return Tuple.Create(-1, "无效的页面数据");
            }
            //var roomDataReg = new Regex(@"(?<=self\.__pace_f\.push\(\[1,""a:)\[.+?\](?=\\n?""\]\)[\s\n\r]*?<\/script>)");
            //var roomDataReg = new Regex(@"(?<=self\.__pace_f\.push\(\[1,\s*""0:)\[.+?\](?=\\n""?\]\)[\s\n\r]*?<\/script>)");
            //var roomDataReg = new Regex(@"(?<=self\.__pace_f\.push\(\[1,\s*""9:)\[.+?\](?=\\n""?\]\)[\s\n\r]*?<\/script>)");
            var roomDataReg = new Regex(@"(?<=self\.__pace_f\.push\(\[\d,\s*""\w:.+)\{\\""state.+?\}(?=\]\\n""\]\)[\s\n\r]*?<\/script>)");
            //字符串转义符号版本            

            var match = roomDataReg.Match(html);
            if (!match.Success)
            {
                //Logger.LogError("在通过正则匹配直播页房间信息时失败，可能是官方做了升级");
                return Tuple.Create(1, "未能匹配到房间信息");
            }
            var matchData = Regex.Unescape(match.Value);

            JObject jsonObject = null;
            try
            {
                jsonObject = JObject.Parse(matchData);
            }
            catch (Exception ex)
            {
                return Tuple.Create(2, "匹配到的房间数据格式有错误");
            }
            //var root = jsonObject["children"][3];
            //var roomInfo = root["initialState"]["roomStore"]["roomInfo"];
            //var odin = root["initialState"]["userStore"]["odin"];
            //var room = roomInfo["room"];
            //var roomOwner = roomInfo["anchor"];

            var roomInfo = jsonObject["state"]["roomStore"]["roomInfo"];
            var odin = jsonObject["state"]["userStore"]["odin"];
            var room = roomInfo["room"];
            var roomOwner = roomInfo["anchor"];

            if (room == null)
            {
                return Tuple.Create(3, "房间信息为空，获取失败");
            }

            result = new RoomInfo();
            result.Odin = odin?.ToObject<RoomInfo.OdinData>();
            result.WebRoomId = roomInfo["web_rid"]?.ToString() ?? "";
            result.RoomId = roomInfo["roomId"]?.ToString() ?? "";
            result.AdminUserIds = room["admin_user_ids_str"]?.Values<string>()?.ToList() ?? new List<string>();
            result.IsLive = room["status"]?.Value<int>() == 2;
            result.Title = room["title"]?.ToString() ?? "";
            result.UserCount = room["room_view_stats"]?["display_value"]?.Value<long>() ?? 0;
            result.TotalUserCount = room["stats"]?["total_user_str"]?.Value<string>() ?? "0";
            result.LikeCount = room["like_count"]?.Value<long>() ?? 0;
            result.QrcodeUrl = roomInfo["qrcode_url"]?.ToString() ?? "";
            result.AuthInfo = room["room_auth"]?.ToObject<RoomInfo.RoomAuth>();
            result.Cover = room["cover"]?["url_list"]?.Values<string>().FirstOrDefault() ?? "";//下播情况下没有cover字段
            if (roomOwner != null)
            {
                result.Owner = new RoomInfo.RoomAnchor()
                {
                    UserId = roomOwner["id_str"].ToString(),
                    Nickname = roomOwner["nickname"].ToString(),
                    SecUid = roomOwner["sec_uid"].ToString(),
                    HeadUrl = roomOwner["avatar_thumb"]["url_list"].Values<string>()?.FirstOrDefault() ?? "",
                    FollowStatus = roomOwner["follow_info"]["follow_status"].Value<int>()
                };
            }
            return Tuple.Create(0, "succ");
        }


        /// <summary>
        /// 从直播伴侣的直播创建回调中解析房间信息
        /// </summary>
        /// <param name="json"></param>
        /// <param name="info"></param>
        /// <param name="cache">缓存</param>
        /// <returns></returns>
        public static Tuple<int, string> TryParseStreamPusherCreate(string json, out RoomInfo info)
        {
            info = null;
            JObject res;
            try
            {
                res = JsonConvert.DeserializeObject<JObject>(json);
            }
            catch (Exception)
            {
                return Tuple.Create(4, "不是合法的json格式");
            }
            if (res == null)
            {
                return Tuple.Create(1, "正文为空");
            }

            var rootData = res["data"];
            var ownerInfo = rootData["owner"];

            var dto = new RoomInfo();
            dto.RoomId = rootData["id_str"]?.ToString() ?? "";
            dto.AdminUserIds = rootData["admin_user_ids_str"]?.Values<string>().ToList() ?? new List<string>();
            dto.IsLive = rootData["status"]?.Value<int>() == 2;
            dto.Title = rootData["title"]?.ToString() ?? "";
            dto.UserCount = long.Parse(rootData["room_view_stats"]?["display_value"]?.Value<string>() ?? "0");
            dto.TotalUserCount = rootData["room_view_stats"]?["total_user_str"]?.Value<string>() ?? "0";
            dto.LikeCount = rootData["like_count"]?.Value<long>() ?? 0;
            dto.QrcodeUrl = $"https://live.douyin.com/{ownerInfo?["display_id"] ?? ""}";
            dto.AuthInfo = rootData["room_auth"]?.ToObject<RoomInfo.RoomAuth>();
            dto.Cover = rootData["cover"]?["url_list"]?.Values<string>().FirstOrDefault() ?? "";//下播情况下没有cover字段
            dto.WebRoomId = rootData["web_rid"]?.ToString() ?? ownerInfo?["display_id"]?.ToString() ?? "";
            if (ownerInfo != null)
            {
                dto.Owner = new RoomAnchor()
                {
                    UserId = ownerInfo["id_str"].ToString(),
                    Nickname = ownerInfo["nickname"].ToString(),
                    SecUid = ownerInfo["sec_uid"].ToString(),
                    HeadUrl = ownerInfo["avatar_thumb"]?["url_list"]?.Values<string>()?.FirstOrDefault()?.ToString(),
                    FollowStatus = ownerInfo["follow_info"]?["follow_status"]?.Value<int>() ?? 0
                };
            }


          
            info = dto;

            return Tuple.Create(0, "succ");
        }
    }
}

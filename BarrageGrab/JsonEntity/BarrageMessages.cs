namespace BarrageGrab.JsonEntity
{
    /// <summary>
    /// 弹幕消息类型
    /// </summary>
    public enum BarrageMsgType
    {
        无 = 0,
        消息,
        点赞,
        进入直播间,
        关注主播,
        送礼,
        直播间统计,
        粉丝团信息
    }

    /// <summary>
    /// 粉丝团消息类型
    /// </summary>
    public enum FansclubType
    {
        无 = 0,
        粉丝团升级 = 1,
        加入粉丝团 = 2
    }

    /// <summary>
    /// 数据包装器
    /// </summary>
    public class BarrageMsgPack
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public BarrageMsgType Type { get; set; }

        /// <summary>
        /// 消息对象
        /// </summary>
        public string Data { get; set; }

        public BarrageMsgPack()
        {

        }

        public BarrageMsgPack(string data, BarrageMsgType type)
        {
            this.Data = data;
            this.Type = type;
        }
    }

    /// <summary>
    /// 消息
    /// </summary>
    public class Msg
    {
        /// <summary>
        /// 用户数据
        /// </summary>
        public MsgUser User { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        public long RoomId { get; set; }
    }

    /// <summary>
    /// 用户弹幕信息
    /// </summary>
    public class MsgUser
    {
        /// <summary>
        /// 真实ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 自定义ID
        /// </summary>
        public string DisplayId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 性别 1男 2女
        /// </summary>
        public int Gender { get; set; }

        public string GenderToString()
        {
            return Gender == 1 ? "男" : Gender == 2 ? "女" : "妖";
        }
    }

    /// <summary>
    /// 礼物消息
    /// </summary>
    public class GiftMsg : Msg
    {
        /// <summary>
        /// 礼物ID
        /// </summary>
        public long GiftId { get; set; }

        /// <summary>
        /// 礼物名称
        /// </summary>
        public string GiftName { get; set; }

        /// <summary>
        /// 礼物数量
        /// </summary>
        public long GiftCount { get; set; }

        /// <summary>
        /// 抖币价格
        /// </summary>
        public int DiamondCount { get; set; }
    }

    /// <summary>
    /// 点赞消息
    /// </summary>
    public class LikeMsg : Msg
    {
        /// <summary>
        /// 点赞数量
        /// </summary>
        public long Count { get; set; }
    }

    /// <summary>
    /// 直播间统计消息
    /// </summary>
    public class UserSeqMsg : Msg
    {
        /// <summary>
        /// 当前直播间用户数量
        /// </summary>
        public long OnlineUserCount { get; set; }

        /// <summary>
        /// 累计直播间用户数量
        /// </summary>
        public long TotalUserCount { get; set; }

        /// <summary>
        /// 累计直播间用户数量 显示文本
        /// </summary>
        public string TotalUserCountStr { get; set; }

        /// <summary>
        /// 当前直播间用户数量 显示文本
        /// </summary>
        public string OnlineUserCountStr { get; set; }
    }

    /// <summary>
    /// 粉丝团消息
    /// </summary>
    public class FansclubMsg : Msg
    {
        /// <summary>
        /// 粉丝团消息类型,升级1，加入2
        /// </summary>
        public int Type { get; set; }
    }
}
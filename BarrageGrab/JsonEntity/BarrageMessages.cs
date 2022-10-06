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
        送礼
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
        public int GiamondCount { get; set; }
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
}
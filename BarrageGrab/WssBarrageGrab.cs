using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarrageGrab.ProtoEntity;
using ProtoBuf;

namespace BarrageGrab
{
    /// <summary>
    /// 本机Wss弹幕抓取器
    /// </summary>
    public class WssBarrageGrab : IDisposable
    {
        FiddlerProxy proxy = new FiddlerProxy();
        Appsetting appsetting = Appsetting.Get();

        /// <summary>
        /// 进入直播间
        /// </summary>
        public event EventHandler<MemberMessage> OnMemberMessage;

        /// <summary>
        /// 关注
        /// </summary>
        public event EventHandler<SocialMessage> OnSocialMessage;

        /// <summary>
        /// 聊天
        /// </summary>
        public event EventHandler<ChatMessage> OnChatMessage;

        /// <summary>
        /// 点赞
        /// </summary>
        public event EventHandler<LikeMessage> OnLikeMessage;

        /// <summary>
        /// 礼物
        /// </summary>
        public event EventHandler<GiftMessage> OnGiftMessage;

        /// <summary>
        /// 直播间统计
        /// </summary>
        public event EventHandler<RoomUserSeqMessage> OnRoomUserSeqMessage;

        /// <summary>
        /// 直播间状态变更
        /// </summary>
        public event EventHandler<ControlMessage> OnControlMessage;

        /// <summary>
        /// 粉丝团消息
        /// </summary>
        public event EventHandler<FansclubMessage> OnFansclubMessage;

        public WssBarrageGrab()
        {
            proxy.OnWebSocketData += Proxy_OnWebSocketData;
        }

        public void Start()
        {
            proxy.Start();
        }

        public void Dispose()
        {
            proxy.Dispose();
        }

        //gzip解压缩
        private byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }

        private void Proxy_OnWebSocketData(object sender, FiddlerProxy.WsMessageEventArgs e)
        {
            if (!appsetting.FilterProcess.Contains(e.ProcessName)) return;
            var buff = e.Message.PayloadData;
            if (buff.Length == 0) return;
            if (buff[0] != 0x08) return;

            try
            {
                var enty = Serializer.Deserialize<WssResponse>(new ReadOnlyMemory<byte>(buff));
                if (enty == null) return;

                //检测包格式
                if (!enty.Headers.Any(a => a.Key == "compress_type" && a.Value == "gzip")) return;

                //解压gzip
                var odata = enty.Payload;
                var decomp = Decompress(odata);

                var response = Serializer.Deserialize<Response>(new ReadOnlyMemory<byte>(decomp));
                response.Messages.ForEach(f => DoMessage(f));
            }
            catch (Exception) { }
        }

        private void DoMessage(Message msg)
        {
            try
            {
                switch (msg.Method)
                {
                    //来了
                    case "WebcastMemberMessage":
                        {
                            var arg = Serializer.Deserialize<MemberMessage>(new ReadOnlyMemory<byte>(msg.Payload));
                            this.OnMemberMessage?.Invoke(this, arg);
                            break;
                        }
                    //关注
                    case "WebcastSocialMessage":
                        {
                            var arg = Serializer.Deserialize<SocialMessage>(new ReadOnlyMemory<byte>(msg.Payload));
                            this.OnSocialMessage?.Invoke(this, arg);
                            break;
                        }
                    //消息
                    case "WebcastChatMessage":
                        {
                            var arg = Serializer.Deserialize<ChatMessage>(new ReadOnlyMemory<byte>(msg.Payload));
                            this.OnChatMessage?.Invoke(this, arg);
                            break;
                        }
                    //点赞
                    case "WebcastLikeMessage":
                        {
                            var arg = Serializer.Deserialize<LikeMessage>(new ReadOnlyMemory<byte>(msg.Payload));
                            this.OnLikeMessage?.Invoke(this, arg);
                            break;
                        }
                    //礼物
                    case "WebcastGiftMessage":
                        {
                            var arg = Serializer.Deserialize<GiftMessage>(new ReadOnlyMemory<byte>(msg.Payload));

                            if (arg.Gift == null)
                            {
                                if (arg.giftId == 685)
                                {
                                    arg.Gift = new ProtoEntity.GiftStruct()
                                    {
                                        Id = arg.giftId,
                                        Name = "粉丝灯牌",
                                        diamondCount = 1
                                    };
                                }
                                else if (arg.giftId == 3389)
                                {
                                    arg.Gift = new GiftStruct()
                                    {
                                        Id = arg.giftId,
                                        Name = "欢乐盲盒",
                                        diamondCount = 10
                                    };
                                }
                                else if (arg.giftId == 4021)
                                {
                                    arg.Gift = new GiftStruct()
                                    {
                                        Id = arg.giftId,
                                        Name = "欢乐拼图",
                                        diamondCount = 10
                                    };
                                }
                                else
                                {
                                    break;
                                }
                            }

                            this.OnGiftMessage?.Invoke(this, arg);
                            break;
                        }
                    //直播间统计
                    case "WebcastRoomUserSeqMessage":
                        {
                            var arg = Serializer.Deserialize<RoomUserSeqMessage>(new ReadOnlyMemory<byte>(msg.Payload));
                            this.OnRoomUserSeqMessage?.Invoke(this, arg);
                            break;
                        }
                    //直播间状态变更
                    case "WebcastControlMessage":
                        {
                            var arg = Serializer.Deserialize<ControlMessage>(new ReadOnlyMemory<byte>(msg.Payload));
                            this.OnControlMessage?.Invoke(this, arg);
                            break;
                        }
                    //粉丝团消息
                    case "WebcastFansclubMessage":
                        {
                            var arg = Serializer.Deserialize<FansclubMessage>(new ReadOnlyMemory<byte>(msg.Payload));
                            this.OnFansclubMessage?.Invoke(this, arg);
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}

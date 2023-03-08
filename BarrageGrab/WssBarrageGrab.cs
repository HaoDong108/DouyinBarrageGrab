using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarrageGrab.ProtoEntity;
using ProtoBuf;
using ColorConsole;
using BarrageGrab.Proxy;

namespace BarrageGrab
{
    /// <summary>
    /// 本机Wss弹幕抓取器
    /// </summary>
    public class WssBarrageGrab : IDisposable
    {
        //ISystemProxy proxy = new FiddlerProxy();
        ISystemProxy proxy = new TitaniumProxy();
        Appsetting appsetting = Appsetting.Current;
        ConsoleWriter console = new ConsoleWriter();
        //解包成功的域名缓存
        List<string> succPackHostNames = new List<string>();

        //已知的弹幕域名服务器
        string[] wsHostNames = Appsetting.Current.HostNameFilter;

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
            proxy.HostNameFilter = HostNameChecker;
        }

        public void Start()
        {
            proxy.Start();
        }

        public void Dispose()
        {
            proxy.Dispose();
        }


        //域名过滤器
        private bool HostNameChecker(string hostName)
        {
            if (!Appsetting.Current.FilterHostName) return true;
            if (hostName.StartsWith("webcast")) return true;

            if (wsHostNames.Any(a => a.ToLower() == hostName.ToLower())) return true;

            return false;
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

        //ws数据处理
        private void Proxy_OnWebSocketData(object sender, WsMessageEventArgs e)
        {
            if (!appsetting.ProcessFilter.Contains(e.ProcessName)) return;
            var buff = e.Payload;
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

                if (!succPackHostNames.Contains(e.HostName))
                {
                    succPackHostNames.Add(e.HostName);
                    SaveHostNameCache();
                }
                response.Messages.ForEach(f => DoMessage(f));
            }
            catch (Exception) { }
        }

        //发送事件
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
                                    console.WriteLine("未能识别的礼物ID：" + arg.giftId, ConsoleColor.Red);
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

        //将成功解包的域名缓存到文件
        private void SaveHostNameCache()
        {
            //获取程序运行目录
            var baseDir = Directory.GetCurrentDirectory();
            var fullPath = Path.Combine(baseDir, "成功解包域名缓存.txt");

            try
            {
                //文件不存在则创建
                if (!File.Exists(fullPath))
                {
                    File.Create(fullPath);
                }
                var text = File.ReadAllText(fullPath);
                var currentHosts = text.Split('\n').Where(w => !string.IsNullOrWhiteSpace(w)).Select(s=>s.Trim().Trim('\r')).ToList();
                var newHosts = succPackHostNames.Except(currentHosts).ToList();
                //保存
                if (newHosts.Count > 0)
                {
                    File.AppendAllLines(fullPath, newHosts);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("写入成功解包域名缓存失败：" + ex.Message, ConsoleColor.Red);
            }
        }
    }
}

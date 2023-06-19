using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BarrageGrab.JsonEntity;
using BarrageGrab.ProtoEntity;
using ColorConsole;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static BarrageGrab.ProtoEntity.Image;

namespace BarrageGrab
{

    /// <summary>
    /// 弹幕服务
    /// </summary>
    internal class WsBarrageService
    {
        WebSocketServer socketServer;
        Dictionary<string, UserState> socketList = new Dictionary<string, UserState>();
        //礼物计数缓存
        Dictionary<string, Tuple<int, DateTime>> giftCountCache = new Dictionary<string, Tuple<int, DateTime>>();
        Timer dieout = new Timer(10000);
        Timer giftCountTimer = new Timer(10000);
        ConsoleWriter console = new ConsoleWriter();
        WssBarrageGrab grab = new WssBarrageGrab();
        Appsetting Appsetting = Appsetting.Current;
        bool debug = false;
        
        /// <summary>
        /// 服务关闭后触发
        /// </summary>
        public event EventHandler OnClose;

        public WsBarrageService()
        {
#if DEBUG
            debug = true;
#endif
            var socket = new WebSocketServer($"ws://127.0.0.1:{Appsetting.WsProt}");
            socket.RestartAfterListenError = true;//异常重启

            dieout.Elapsed += Dieout_Elapsed;
            giftCountTimer.Elapsed += GiftCountTimer_Elapsed;

            this.grab.OnChatMessage += Grab_OnChatMessage;
            this.grab.OnLikeMessage += Grab_OnLikeMessage;
            this.grab.OnMemberMessage += Grab_OnMemberMessage;
            this.grab.OnSocialMessage += Grab_OnSocialMessage;
            this.grab.OnSocialMessage += Grab_OnShardMessage;
            this.grab.OnGiftMessage += Grab_OnGiftMessage;
            this.grab.OnRoomUserSeqMessage += Grab_OnRoomUserSeqMessage;
            this.grab.OnFansclubMessage += Grab_OnFansclubMessage; ;
            this.grab.OnControlMessage += Grab_OnControlMessage;

            this.socketServer = socket;
            //dieout.Start();
            giftCountTimer.Start();
        }

        private void GiftCountTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            var timeOutKeys = giftCountCache.Where(w => w.Value.Item2 < now.AddSeconds(-10) || w.Value == null).Select(s => s.Key).ToList();

            //淘汰过期的礼物计数缓存
            lock (giftCountCache)
            {
                timeOutKeys.ForEach(key =>
                {
                    giftCountCache.Remove(key);

                });
            }
        }

        private bool CheckRoomId(long roomid)
        {
            return Appsetting.RoomIds.Length == 0 || Appsetting.RoomIds.Contains(roomid);
        }

        //解析用户
        private MsgUser GetUser(ProtoEntity.User data)
        {
            MsgUser user = new MsgUser()
            {
                DisplayId = data.displayId,
                ShortId = data.shortId,
                Gender = data.Gender,
                Id = data.Id,
                Level = data.Level,
                PayLevel = (int)(data.payGrade?.Level??0),
                Nickname = data.Nickname,
                HeadImgUrl = data.avatarThumb.urlLists.FirstOrDefault() ?? "",
                SecUid = data.sec_uid,
                FollowerCount = data.followInfo.followerCount,
                FollowingCount = data.followInfo.followingCount,                
                FollowStatus = data.followInfo.followStatus,
            };
            user.FansClub = new FansClubInfo()
            {
                ClubName = "",
                Level = 0
            };

            if (data.fansClub != null && data.fansClub.Data != null)
            {
                user.FansClub.ClubName = data.fansClub.Data.clubName;
                user.FansClub.Level = data.fansClub.Data.Level;
            }

            return user;
        }

        //粉丝团
        private void Grab_OnFansclubMessage(object sender, ProtoEntity.FansclubMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;
            var enty = new FansclubMsg()
            {
                MsgId = e.Common.msgId,
                Content = e.Content,
                RoomId = e.Common.roomId,
                Type = e.Type,                
                User = GetUser(e.User)
            };
            enty.Level = enty.User.FansClub.Level;
            Print(enty.User.GenderToString() + "  " + enty.Content, ConsoleColor.Blue, BarrageMsgType.粉丝团消息);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.粉丝团消息);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        //统计消息
        private void Grab_OnRoomUserSeqMessage(object sender, ProtoEntity.RoomUserSeqMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;
            var enty = new UserSeqMsg()
            {
                MsgId = e.Common.msgId,
                OnlineUserCount = e.Total,
                TotalUserCount = e.totalUser,
                TotalUserCountStr = e.totalPvForAnchor,
                OnlineUserCountStr = e.onlineUserForAnchor,
                RoomId = e.Common.roomId,
                Content = $"当前直播间人数 {e.onlineUserForAnchor}，累计直播间人数 {e.totalPvForAnchor}",
                User = null
            };
            Print(enty.Content, ConsoleColor.Magenta, BarrageMsgType.直播间统计);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.直播间统计);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        //礼物
        private void Grab_OnGiftMessage(object sender, ProtoEntity.GiftMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;

            var key = e.giftId + "-" + e.groupId.ToString();

            //判断礼物重复
            if (e.repeatEnd == 1)
            {
                //清除缓存中的key
                if (e.groupId > 0 && giftCountCache.ContainsKey(key))
                {
                    lock (giftCountCache)
                    {
                        giftCountCache.Remove(key);
                    }
                }
                return;
            }

            int lastCount = 0;
            int currCount = (int)e.repeatCount;
            var backward = currCount <= lastCount;
            if (currCount <= 0) currCount = 1;

            if (giftCountCache.ContainsKey(key))
            {
                lastCount = giftCountCache[key].Item1;
                backward = currCount <= lastCount;
                if (!backward)
                {
                    lock (giftCountCache)
                    {
                        giftCountCache[key] = Tuple.Create(currCount, DateTime.Now);
                    }
                }
            }
            else
            {
                if (e.groupId > 0 && !backward)
                {
                    lock (giftCountCache)
                    {
                        giftCountCache.Add(key, Tuple.Create(currCount, DateTime.Now));
                    }
                }
            }
            //比上次小，则说明先后顺序出了问题，直接丢掉，应为比它大的消息已经处理过了
            if (backward) return;
            

            var count = currCount - lastCount;

            var enty = new GiftMsg()
            {
                MsgId = e.Common.msgId,
                RoomId = e.Common.roomId,
                Content = $"{e.User.Nickname} 送出 {e.Gift.Name} x {currCount} 个，增量{count}个",
                DiamondCount = e.Gift.diamondCount,
                RepeatCount = currCount,
                GiftCount = count,
                GroupId = e.groupId,
                GiftId = e.giftId,
                GiftName = e.Gift.Name,
                User = GetUser(e.User)
            };
           
            
            Print($"{enty.User.GenderToString()}  {enty.Content}", ConsoleColor.Red, BarrageMsgType.礼物消息);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.礼物消息);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        //关注
        private void Grab_OnSocialMessage(object sender, ProtoEntity.SocialMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;
            if (e.Action != 1) return;
            var enty = new Msg()
            {
                MsgId = e.Common.msgId,
                Content = $"{e.User.Nickname} 关注了主播",
                RoomId = e.Common.roomId,
                User = GetUser(e.User)
            };

            Print($"{enty.User.GenderToString()}  {enty.Content}", ConsoleColor.Yellow, BarrageMsgType.关注消息);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.关注消息);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        //直播间分享
        private void Grab_OnShardMessage(object sender, ProtoEntity.SocialMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;
            if (e.Action != 3) return;
            ShareType type = ShareType.未知;
            if (Enum.IsDefined(type.GetType(), int.Parse(e.shareTarget)))
            {
                type = (ShareType)int.Parse(e.shareTarget);
            }

            var enty = new ShareMessage()
            {
                MsgId = e.Common.msgId,
                Content = $"{e.User.Nickname} 分享了直播间到{type}",
                RoomId = e.Common.roomId,
                ShareType = type,
                User = GetUser(e.User)
            };
            //shareTarget: (112:好友),(1微信)(2朋友圈)(3微博)(5:qq)(4:qq空间),shareType: 1
            Print($"{enty.User.GenderToString()}  {enty.Content}", ConsoleColor.DarkBlue, BarrageMsgType.直播间分享);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.直播间分享);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        //来了
        private void Grab_OnMemberMessage(object sender, ProtoEntity.MemberMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;

            var enty = new JsonEntity.MemberMessage()
            {
                MsgId = e.Common.msgId,
                Content = $"{e.User.Nickname} 来了 直播间人数:{e.memberCount}",
                RoomId = e.Common.roomId,
                CurrentCount = e.memberCount,
                User = GetUser(e.User)
            };
            Print($"{enty.User.GenderToString()}  {enty.Content}", ConsoleColor.Green, BarrageMsgType.进直播间);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.进直播间);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        //点赞
        private void Grab_OnLikeMessage(object sender, ProtoEntity.LikeMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;

            var enty = new LikeMsg()
            {
                MsgId = e.Common.msgId,
                Count = e.Count,
                Content = $"{e.User.Nickname} 为主播点了{e.Count}个赞，总点赞{e.Total}",
                RoomId = e.Common.roomId,
                Total = e.Total,
                User = GetUser(e.User)
            };
            Print($"{enty.User.GenderToString()}  {enty.Content}", ConsoleColor.Cyan, BarrageMsgType.点赞消息);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.点赞消息);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        //弹幕
        private void Grab_OnChatMessage(object sender, ProtoEntity.ChatMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;

            var enty = new Msg()
            {
                MsgId = e.Common.msgId,
                Content = e.Content,
                RoomId = e.Common.roomId,
                User = GetUser(e.User)
            };


            Print($"{enty.User.GenderToString()}  {enty.User.Nickname}: {enty.Content}", ConsoleColor.White, BarrageMsgType.弹幕消息);
            if (e.User.followInfo.followStatus != 1 && e.User.followInfo.followStatus != 0 && e.User.followInfo.followStatus != 2)
            {
                Console.WriteLine(e.User.followInfo.followStatus);
            }
            
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.弹幕消息);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        //直播间状态变更
        private void Grab_OnControlMessage(object sender, ControlMessage e)
        {
            if (!CheckRoomId(e.Common.roomId)) return;
            BarrageMsgPack pack = null;
            //下播
            if (e.Status == 3)
            {
                var enty = new Msg()
                {
                    MsgId = e.Common.msgId,
                    Content = "直播已结束",
                    RoomId = e.Common.roomId,
                    User = null
                };
                Print($"直播已结束", ConsoleColor.DarkCyan, BarrageMsgType.下播);
                pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.下播);
            }

            if (pack != null)
            {
                var json = JsonConvert.SerializeObject(pack);
                this.Broadcast(json);
            }
        }

        static int count = 0;
        private void Print(string msg, ConsoleColor color, BarrageMsgType bartype)
        {
            if (!Appsetting.PrintFilter.Any(a => a == bartype.GetHashCode())) return;
            if (Appsetting.PrintBarrage)
            {
                if (++count > 1000)
                {                    
                    Console.Clear();
                    Console.WriteLine("控制台已清理");
                }
                console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} [{bartype.ToString()}] " + msg + "\n", color);
                count = 0;
            }
        }

        private void Dieout_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            var dieoutKvs = socketList.Where(w => w.Value.LastPing.AddSeconds(dieout.Interval * 3) < now).ToList();
            dieoutKvs.ForEach(f => f.Value.Socket.Close());
        }

        private void Listen(IWebSocketConnection socket)
        {
            //客户端url
            string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
            if (!socketList.ContainsKey(clientUrl))
            {
                socketList.Add(clientUrl, new UserState(socket));
                console.WriteLine($"{DateTime.Now.ToLongTimeString()} 已经建立与[{clientUrl}]的连接", ConsoleColor.Green);
            }
            else
            {
                socketList[clientUrl].Socket = socket;
            }

            //接收指令
            socket.OnMessage = (message) =>
            {
                try
                {
                    var cmdPack = JsonConvert.DeserializeObject<Command>(message);
                    if (cmdPack == null) return;

                    if (cmdPack.Cmd == CommandCode.Close)
                    {
                        this.Close();
                    }
                }
                catch (Exception) {}
            };

            socket.OnClose = () =>
            {
                socketList.Remove(clientUrl);
                console.WriteLine($"{DateTime.Now.ToLongTimeString()} 已经关闭与[{clientUrl}]的连接", ConsoleColor.Red);
            };

            socket.OnPing = (data) =>
            {
                socketList[clientUrl].LastPing = DateTime.Now;
                socket.SendPong(Encoding.UTF8.GetBytes("pong"));
            };
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        public void Broadcast(string msg)
        {
            foreach (var user in socketList)
            {
                var socket = user.Value;                
                socket.Socket.Send(msg);
            }
            //删除掉线的套接字
            var offlines = socketList.Where(w=>!w.Value.Socket.IsAvailable).Select(s=>s.Key).ToList();
            offlines.ForEach(key => socketList.Remove(key));
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        public void StartListen()
        {
            this.grab.Start(); //启动代理
            this.socketServer.Start(Listen);//启动监听
            console.WriteLine($"{this.socketServer.Location} 弹幕服务已启动...", ConsoleColor.Green);
            Console.Title = $"抖音弹幕监听推送 [{this.socketServer.Location}]";
        }

        /// <summary>
        /// 关闭服务器连接，并关闭系统代理
        /// </summary>
        public void Close()
        {
            socketList.Values.ToList().ForEach(f => f.Socket.Close());
            socketList.Clear();
            socketServer.Dispose();
            grab.Dispose();

            this.OnClose?.Invoke(this, EventArgs.Empty);            
        }

        class UserState
        {
            /// <summary>
            /// 套接字
            /// </summary>
            public IWebSocketConnection Socket { get; set; }

            /// <summary>
            /// 上次发起心跳包时间
            /// </summary>
            public DateTime LastPing { get; set; } = DateTime.Now;

            public UserState()
            {

            }
            public UserState(IWebSocketConnection socket)
            {
                Socket = socket;
            }
        }
    }
}

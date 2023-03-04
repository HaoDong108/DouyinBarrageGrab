using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BarrageGrab.JsonEntity;
using ColorConsole;
using Fleck;
using Newtonsoft.Json;

namespace BarrageGrab
{
    /// <summary>
    /// 弹幕服务
    /// </summary>
    internal class WssBarrageService
    {
        WebSocketServer socketServer;
        Dictionary<string, UserState> socketList = new Dictionary<string, UserState>();
        Timer dieout = new Timer(10000);
        ConsoleWriter console = new ConsoleWriter();
        WssBarrageGrab grab = new WssBarrageGrab();
        Appsetting Appsetting = Appsetting.Get();
        
        public WssBarrageService()
        {
            var socket = new WebSocketServer($"ws://127.0.0.1:{Appsetting.WsProt}");
            socket.RestartAfterListenError = true;//异常重启

            dieout.Elapsed += Dieout_Elapsed;

            this.grab.OnChatMessage += Grab_OnChatMessage;
            this.grab.OnLikeMessage += Grab_OnLikeMessage;
            this.grab.OnMemberMessage += Grab_OnMemberMessage;
            this.grab.OnSocialMessage += Grab_OnSocialMessage;
            this.grab.OnGiftMessage += Grab_OnGiftMessage;
            this.grab.OnRoomUserSeqMessage += Grab_OnRoomUserSeqMessage;
            this.grab.OnFansclubMessage += Grab_OnFansclubMessage; ;
            //this.grab.OnControlMessage += Grab_OnControlMessage;

            this.socketServer = socket;
            //dieout.Start();
        }

        private MsgUser GetUser(dynamic obj)
        {
            MsgUser user = new MsgUser()
            {
                DisplayId = obj.User.displayId,
                Gender = obj.User.Gender,
                Id = obj.User.Id,
                Level = obj.User.Level,
                Nickname = obj.User.Nickname
            };
            return user;
        }

        private void Grab_OnFansclubMessage(object sender, ProtoEntity.FansclubMessage e)
        {
            var enty = new FansclubMsg()
            {
                Content = e.Content,
                RoomId = e.commonInfo.roomId,
                Type = e.Type,
                User = GetUser(e)
            };
            Print($"[粉丝团消息] {enty.User.GenderToString()} " + enty.Content, ConsoleColor.Blue);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.粉丝团信息);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        private void Grab_OnRoomUserSeqMessage(object sender, ProtoEntity.RoomUserSeqMessage e)
        {
            var enty = new UserSeqMsg()
            {
                OnlineUserCount = e.Total,
                TotalUserCount = e.totalUser,
                TotalUserCountStr = e.totalPvForAnchor,
                OnlineUserCountStr = e.onlineUserForAnchor,
                RoomId = e.Common.roomId,
                Content = $"当前直播间人数 {e.onlineUserForAnchor}，累计直播间人数 {e.totalPvForAnchor}",
                User = null
            };
            Print($"[直播间统计] " + enty.Content, ConsoleColor.Magenta);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.直播间统计);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        private void Grab_OnGiftMessage(object sender, ProtoEntity.GiftMessage e)
        {
            var enty = new GiftMsg()
            {
                RoomId = e.Common.roomId,
                Content = e.Common.Describe,
                DiamondCount = e.Gift.diamondCount,
                GiftCount = e.repeatCount,
                GiftId = e.giftId,
                GiftName = e.Gift.Name,
                User = GetUser(e)
            };
            Print($"[礼物消息] " + $"{enty.User.GenderToString()} " + enty.Content, ConsoleColor.Red);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.送礼);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        private void Grab_OnSocialMessage(object sender, ProtoEntity.SocialMessage e)
        {
            var enty = new Msg()
            {
                Content = $"{e.User.Nickname} 关注了主播",
                RoomId = e.Common.roomId,
                User = GetUser(e)
            };
            Print($"[关注消息] {enty.User.GenderToString()} " + enty.Content, ConsoleColor.Yellow);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.关注主播);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        private void Grab_OnMemberMessage(object sender, ProtoEntity.MemberMessage e)
        {
            var enty = new Msg()
            {
                Content = $"{e.User.Nickname} 来了",
                RoomId = e.Common.roomId,
                User = GetUser(e)
            };
            Print($"[进直播间] {enty.User.GenderToString()} " + enty.Content, ConsoleColor.Green);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.进入直播间);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        private void Grab_OnLikeMessage(object sender, ProtoEntity.LikeMessage e)
        {
            var enty = new LikeMsg()
            {
                Count = e.Count,
                Content = $"{e.User.Nickname} 为主播点了{e.Count}个赞",
                RoomId = e.Common.roomId,
                User = GetUser(e)
            };
            Print($"[点赞消息] {enty.User.GenderToString()} " + enty.Content, ConsoleColor.Cyan);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.点赞);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        private void Grab_OnChatMessage(object sender, ProtoEntity.ChatMessage e)
        {
            var enty = new Msg()
            {
                Content = e.Content,
                RoomId = e.Common.roomId,
                User = GetUser(e)
            };
            Print($"[弹幕消息] {enty.User.GenderToString()}  {enty.User.Nickname}: {enty.Content}", ConsoleColor.White);
            var pack = new BarrageMsgPack(JsonConvert.SerializeObject(enty), BarrageMsgType.消息);
            var json = JsonConvert.SerializeObject(pack);
            this.Broadcast(json);
        }

        private void Print(string msg, ConsoleColor color)
        {
            if (Appsetting.PrintBarrage)
            {
                console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} " + msg + "\n", color);
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
        }

        public void StartListen()
        {
            this.grab.Start(); //启动代理
            this.socketServer.Start(Listen);//启动监听
            console.WriteLine($"{this.socketServer.Location} 弹幕服务已启动...", ConsoleColor.Green);
            Console.Title = $"抖音弹幕监听推送 [{this.socketServer.Location}]";
        }

        /// <summary>
        /// 关闭服务器连接
        /// </summary>
        public void Close()
        {
            socketList.Values.ToList().ForEach(f => f.Socket.Close());
            socketList.Clear();
            socketServer.Dispose();
            grab.Dispose();

            console.WriteLine("服务器已关闭...");
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

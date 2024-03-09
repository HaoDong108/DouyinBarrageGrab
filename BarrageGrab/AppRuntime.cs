using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BarrageGrab.Modles;

namespace BarrageGrab
{
    /// <summary>
    /// 程序运行时信息
    /// </summary>
    public static class AppRuntime
    {
        /// <summary>
        /// Ws弹幕服务示例
        /// </summary>
        public static WsBarrageService WssService { get; private set; } = new WsBarrageService();

        /// <summary>
        /// 房间缓存信息
        /// </summary>
        public static RoomCacheManager RoomCaches { get; } = new RoomCacheManager();

        /// <summary>
        /// 程序进程信息
        /// </summary>
        public static Process CurrentProcess { get; private set; } = System.Diagnostics.Process.GetCurrentProcess();


        static AppRuntime()
        {
           
        }

        /// <summary>
        /// 隐藏/显示 控制台
        /// </summary>
        /// <param name="show"></param>
        public static void DisplayConsole(bool show)
        {
            //var hWnd = WinApi.FindWindow(null, Console.Title);            
            var hWnd = WinApi.GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                WinApi.ShowWindow(hWnd, show? WinApi.CmdShow.SW_SHOW:WinApi.CmdShow.SW_HIDE);
            }
        }

        public static void DisplayConsoleByTitle(bool show)
        {
            var hWnd = WinApi.FindWindow(null, Console.Title);            
            //var hWnd = WinApi.GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                WinApi.ShowWindow(hWnd, show ? WinApi.CmdShow.SW_SHOW : WinApi.CmdShow.SW_HIDE);
            }
        }

        /// <summary>
        /// 房间缓存管理器
        /// </summary>
        public class RoomCacheManager
        {
            /// <summary>
            /// 房间ID - 房间信息映射缓存
            /// </summary>
            public ConcurrentDictionary<string, RoomInfo> RoomInfoCache { get; } = new ConcurrentDictionary<string, RoomInfo>();

            /// <summary>
            /// 添加房间缓存时触发
            /// </summary>
            public event EventHandler<RoomCacheEventArgs> OnCache;

            /// <summary>
            /// 添加WebRoomid-房间ID 映射
            /// </summary>
            /// <param name="webrid"></param>
            /// <param name="roomid"></param>
            public void SetRoomCache(string roomid, string webrid)
            {
                if (RoomInfoCache.ContainsKey(roomid))
                {
                    RoomInfoCache[roomid].WebRoomId = webrid;
                }
                else
                {
                    var info = new RoomInfo()
                    {
                        RoomId = roomid,
                        WebRoomId = webrid,
                        Title = "房间" + webrid
                    };
                    var succ = RoomInfoCache.TryAdd(roomid,info);
                    if (succ)
                    {
                        OnCache?.Invoke(this, new RoomCacheEventArgs()
                        {
                            Model = 0,
                            RoomInfo = info
                        });
                    }
                }
            }

            /// <summary>
            /// 添加房间缓存信息
            /// </summary>
            /// <param name="roomid"></param>
            /// <param name="roomInfo"></param>
            public void AddRoomInfoCache(RoomInfo roomInfo)
            {
                if (roomInfo == null) return;
                var roomid = roomInfo.RoomId;
                if (RoomInfoCache.ContainsKey(roomid))
                {
                    RoomInfoCache[roomid] = roomInfo;
                    OnCache?.Invoke(this, new RoomCacheEventArgs()
                    {
                        Model = 1,
                        RoomInfo = roomInfo
                    });
                }
                else
                {
                    var succ = RoomInfoCache.TryAdd(roomid, roomInfo);
                    if (succ)
                    {
                        OnCache?.Invoke(this,new RoomCacheEventArgs()
                        {
                            Model = 0,
                            RoomInfo = roomInfo
                        });
                    }
                }
            }

            /// <summary>
            /// 根据roomid从缓存获取webRoomid
            /// </summary>
            /// <param name="roomid"></param>
            /// <returns></returns>
            public long GetCachedWebRoomid(string roomid)
            {
                RoomInfo value;
                if (RoomInfoCache.TryGetValue(roomid,out value)) return long.Parse(value.WebRoomId??"-1");
                return -1;
            }

            /// <summary>
            /// 根据roomid从缓存获取房间信息
            /// </summary>
            /// <param name="roomid"></param>
            /// <returns></returns>
            public RoomInfo GetCachedWebRoomInfo(string roomid)
            {
                RoomInfo value;
                if (RoomInfoCache.TryGetValue(roomid, out value)) return value;
                return null;
            }

            /// <summary>
            /// 根据webRoomid从缓存获取房间信息
            /// </summary>
            /// <param name="webrid"></param>
            /// <returns></returns>
            public RoomInfo GetByWebRoomid(string webrid)
            {
                var find = RoomInfoCache.FirstOrDefault(p => p.Value.WebRoomId == webrid);
                return find.Value;
            }


            public class RoomCacheEventArgs : EventArgs
            {
                public RoomInfo RoomInfo { get; set; }   
                
                /// <summary>
                /// 0:添加,1更新,2删除
                /// </summary>
                public int Model { get; set; }
            }
        }        
    }
}

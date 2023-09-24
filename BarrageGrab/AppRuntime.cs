using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BarrageGrab.Modles;
using ColorConsole;

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
        /// 房间缓存管理器
        /// </summary>
        public class RoomCacheManager
        {
            /// <summary>
            /// 房间ID - 房间信息映射缓存
            /// </summary>
            public ConcurrentDictionary<string, RoomInfo> RoomInfoCache { get; } = new ConcurrentDictionary<string, RoomInfo>();

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
                    RoomInfoCache.TryAdd(roomid, new RoomInfo()
                    {
                        RoomId = roomid,
                        WebRoomId = webrid,
                        Title = "房间" + webrid
                    });
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
                }
                else
                {
                    RoomInfoCache.TryAdd(roomid, roomInfo);
                }
            }

            /// <summary>
            /// 根据roomid从缓存获取webRoomid
            /// </summary>
            /// <param name="roomid"></param>
            /// <returns></returns>
            public long GetCachedWebRoomid(string roomid)
            {
                if (RoomInfoCache.TryGetValue(roomid, out var value)) return long.Parse(value.WebRoomId);
                return -1;
            }

            /// <summary>
            /// 根据roomid从缓存获取房间信息
            /// </summary>
            /// <param name="roomid"></param>
            /// <returns></returns>
            public RoomInfo GetCachedWebRoomInfo(string roomid)
            {
                if (RoomInfoCache.TryGetValue(roomid, out var value)) return value;
                return null;
            }
        }
    }

}

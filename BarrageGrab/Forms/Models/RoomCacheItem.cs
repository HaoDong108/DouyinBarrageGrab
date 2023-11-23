using BarrageGrab.Modles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Forms.Models
{
    internal class RoomCacheItem
    {
        public RoomCacheItem(RoomInfo info)
        {
            this.RoomInfo = info;
        }
        /// <summary>
        /// 数据对象
        /// </summary>
        public RoomInfo RoomInfo { get; set; }

        public override string ToString()
        {
            if (RoomInfo == null) return "NULL";
            return $"({RoomInfo.WebRoomId}){RoomInfo?.Owner?.Nickname??"-1"}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Proxy.ProxyEventArgs
{
    public class WsMessageEventArgs
    {
        /// <summary>
        /// 消息体
        /// </summary>
        public byte[] Payload { get; set; }

        /// <summary>
        /// 进程ID
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// 进程名
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 是否需要解压缩
        /// </summary>
        public bool NeedDecompress { get; set; } = true;
    }
}

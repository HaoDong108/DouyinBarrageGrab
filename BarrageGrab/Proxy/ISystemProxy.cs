using System;
using Fiddler;
using System.Reflection;
using System.Collections.Generic;

namespace BarrageGrab.Proxy
{
    internal interface ISystemProxy : IDisposable
    {
        event EventHandler<WsMessageEventArgs> OnWebSocketData;

        /// <summary>
        /// 域名过滤器
        /// </summary>
        Func<string, bool> HostNameFilter { get; set; }

        /// <summary>
        /// 开始监听
        /// </summary>
        void Start();
    }

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

    }
}
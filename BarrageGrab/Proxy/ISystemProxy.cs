using System;
using System.Reflection;
using System.Collections.Generic;
using BarrageGrab.Proxy.ProxyEventArgs;

namespace BarrageGrab.Proxy
{
    internal interface ISystemProxy : IDisposable
    {
        event EventHandler<WsMessageEventArgs> OnWebSocketData;

        event EventHandler<HttpResponseEventArgs> OnResponse;

        /// <summary>
        /// 域名过滤器
        /// </summary>
        Func<string, bool> HostNameFilter { get; set; }

        /// <summary>
        /// 开始监听
        /// </summary>
        void Start();
    }

    
}
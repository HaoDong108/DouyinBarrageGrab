using System;
using System.Reflection;
using System.Collections.Generic;
using BarrageGrab.Proxy.ProxyEventArgs;

namespace BarrageGrab.Proxy
{
    public interface ISystemProxy : IDisposable
    {
        event EventHandler<WsMessageEventArgs> OnWebSocketData;

        event EventHandler<HttpResponseEventArgs> OnFetchResponse;

        event EventHandler<SystemProxyChangeEventArgs> OnProxyStatus;

        /// <summary>
        /// Http上游代理地址
        /// </summary>
        string HttpUpstreamProxy { get; }

        /// <summary>
        /// Https 上游代理地址
        /// </summary>
        string HttpsUpstreamProxy { get; }

        /// <summary>
        /// 设置上游代理地址(http+https)
        /// </summary>
        void SetUpstreamProxy(string addr);

        /// <summary>
        /// 关闭系统代理
        /// </summary>
        void CloseSystemProxy();

        /// <summary>
        /// 注册为系统代理
        /// </summary>
        void RegisterSystemProxy();

        /// <summary>
        /// 开始监听
        /// </summary>
        void Start();
    }    
}
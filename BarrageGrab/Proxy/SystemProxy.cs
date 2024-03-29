using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;
using BarrageGrab.Proxy.ProxyEventArgs;

namespace BarrageGrab.Proxy
{
    internal abstract class SystemProxy : ISystemProxy
    {
        /// <summary>
        /// 接收到websocket消息事件
        /// </summary>
        public event EventHandler<WsMessageEventArgs> OnWebSocketData;

        /// <summary>
        /// 接收到http响应事件
        /// </summary>
        public event EventHandler<HttpResponseEventArgs> OnFetchResponse;

        /// <summary>
        /// 代理切换时触�?
        /// </summary>
        public event EventHandler<SystemProxyChangeEventArgs> OnProxyStatus;

        /// <summary>
        /// 代理端口
        /// </summary>
        public int ProxyPort { get { return Appsetting.Current.ProxyPort; } }

        public abstract string HttpUpstreamProxy { get; }

        public abstract string HttpsUpstreamProxy { get; }

        public abstract void Dispose();

        public abstract void Start();

        public abstract void SetUpstreamProxy(string addr);        

        //https://live.douyin.com/webcast/gift/list/ [礼物数据接口]

        /// <summary>
        /// 检测是否是业务相关链接
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        protected virtual bool CheckHost(string host)
        {
            host = host.Trim().ToLower();

            if (!Appsetting.Current.FilterHostName) return true;

            if (host.StartsWith("webcast")) return true;

            if (Appsetting.Current.HostNameFilter.Any(a => a.Trim().ToLower() == host)) return true;

            return false;
        }

        /// <summary>
        /// 触发websocket消息事件
        /// </summary>
        /// <param name="args"></param>
        protected void FireWsEvent(WsMessageEventArgs args)
        {
            OnWebSocketData?.Invoke(this, args);
        }

        /// <summary>
        /// 触发弹幕http弹幕事件
        /// </summary>
        /// <param name="args"></param>
        protected void FireOnFetchResponse(HttpResponseEventArgs args)
        {
            OnFetchResponse?.Invoke(this, args);
        }

        /// <summary>
        /// 获取进程名称
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        protected string GetProcessName(int processID)
        {
            try
            {
                var process = Process.GetProcessById(processID);
                if (process != null)
                {
                    return process.ProcessName;
                }
            }
            catch (Exception ex) { }
            return $"<{processID}>";
        }

        /// <summary>
        /// 注册为系统代�?
        /// </summary>
        public void RegisterSystemProxy()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", 1);
            registry.SetValue("ProxyServer", $"http=localhost:{ProxyPort};https=localhost:{ProxyPort}");
            OnProxyStatus?.Invoke(this, new SystemProxyChangeEventArgs()
            {
                Open = true
            });
        }

        /// <summary>
        /// 关闭系统代理
        /// </summary>
        public void CloseSystemProxy()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", 0);
            registry.SetValue("ProxyServer", $"http=localhost:{ProxyPort};https=localhost:{ProxyPort}");
            OnProxyStatus?.Invoke(this, new SystemProxyChangeEventArgs()
            {
                Open = false
            });
        }        

        public static bool ProxyIsOpen()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            var proxyEnable = registry.GetValue("ProxyEnable");
            if (proxyEnable != null && proxyEnable.ToString() == "1")
            {
                return true;
            }
            return false;
        }
    }
}

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

namespace BarrageGrab.Proxy
{
    internal abstract class SystemProxy : ISystemProxy
    {
        /// <summary>
        /// 域名过滤器
        /// </summary>
        public Func<string, bool> HostNameFilter { get; set; }        

        /// <summary>
        /// 接收到websocket消息事件
        /// </summary>
        public event EventHandler<WsMessageEventArgs> OnWebSocketData;

        /// <summary>
        /// 代理端口
        /// </summary>
        public int ProxyPort { get => Appsetting.Current.ProxyPort; }        

        public abstract void Dispose();

        public abstract void Start();

        //https://live.douyin.com/webcast/gift/list/ [礼物数据接口]

        /// <summary>
        /// 检测是否是业务相关链接
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        protected bool CheckHost(string host)
        {
            var result = true;

            if (HostNameFilter != null) return HostNameFilter(host);

            return result;
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
        /// 获取进程名称
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        protected string GetProcessName(int processID)
        {
            var process = Process.GetProcessById(processID);
            if (process != null)
            {
                return process.ProcessName;
            }
            return $"<{processID}>";
        }

        /// <summary>
        /// 注册系统代理
        /// </summary>
        protected void RegisterSystemProxy()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", 1);
            registry.SetValue("ProxyServer", $"http=localhost:{ProxyPort};https=localhost:{ProxyPort}");
        }

        /// <summary>
        /// 关闭系统代理
        /// </summary>
        protected void CloseSystemProxy()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", 0);
            registry.SetValue("ProxyServer", $"http=localhost:{ProxyPort};https=localhost:{ProxyPort}");
        }
    }
}

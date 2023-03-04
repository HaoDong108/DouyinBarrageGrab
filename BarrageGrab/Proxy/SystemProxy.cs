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
        //弹幕服务域名
        protected string[] barrageWsHostNames = new string[]
        {
            "webcast3-ws-web-hl.douyin.com",
            "webcast3-ws-web-lf.douyin.com",
            "frontier-im.douyin.com"
        };


        //https://live.douyin.com/webcast/gift/list/

        /// <summary>
        /// 接收到websocket消息事件
        /// </summary>
        public event EventHandler<WsMessageEventArgs> OnWebSocketData;

        /// <summary>
        /// 代理端口
        /// </summary>
        public int ProxyPort { get => Appsetting.Instanse.ProxyPort; }

        public abstract void Dispose();

        public abstract void Start();

        protected void SendWebSocketData(WsMessageEventArgs args)
        {
            OnWebSocketData?.Invoke(this, args);
        }

        protected string GetProcessName(int processID)
        {
            var process = Process.GetProcessById(processID);
            if (process != null)
            {
                return process.ProcessName;
            }
            return $"<{processID}>";
        }

        //注册系统代理
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

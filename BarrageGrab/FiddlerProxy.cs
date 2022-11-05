using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ColorConsole;
using Fiddler;
using Microsoft.Win32;

namespace BarrageGrab
{
    /// <summary>
    /// FiddlerCore 代理程序
    /// </summary>
    internal class FiddlerProxy : IDisposable
    {
        bool disposed = false;
        public event EventHandler<WsMessageEventArgs> OnWebSocketData;
        ConsoleWriter console = new ConsoleWriter();

        public FiddlerProxy()
        {
            InitFiddler();
        }

        /// <summary>
        /// 启动代理监听
        /// </summary>
        public void Start()
        {
            FiddlerApplication.Startup(8827, FiddlerCoreStartupFlags.Default);
            console.WriteLine("Fiddler代理已启动", ConsoleColor.Green);
        }

        //初始化Fiddler
        private void InitFiddler()
        {
            //这个名字随便
            FiddlerApplication.SetAppDisplayName("FiddlerCore");
            FiddlerApplication.OnWebSocketMessage += FiddlerApplication_OnWebSocketMessage;

            X509Certificate2 oRootCert;
            if (null == CertMaker.GetRootCertificate())
            {
                CertMaker.createRootCert();
                oRootCert = CertMaker.GetRootCertificate();
                X509Store certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadWrite);
                try
                {
                    certStore.Add(oRootCert);
                }
                finally
                {
                    certStore.Close();
                }
            }
            else
            {
                oRootCert = CertMaker.GetRootCertificate();
            }
            //信任证书
            CertMaker.trustRootCert();

            FiddlerApplication.oDefaultClientCertificate = oRootCert;
            //忽略服务器证书错误
            CONFIG.IgnoreServerCertErrors = true;
            CONFIG.bEnableIPv6 = false;
            //CONFIG.bCaptureCONNECT = true;
            //CONFIG.bUseEventLogForExceptions = false;
            //CONFIG.bAutoProxyLogon = false;
            //CONFIG.bMITM_HTTPS = false;
            ////CONFIG.SetNoDecryptList("*");
        }

        private void FiddlerApplication_OnWebSocketMessage(object sender, WebSocketMessageEventArgs e)
        {
            var msg = e.oWSM;
            OnWebSocketData?.Invoke(this, new WsMessageEventArgs(msg));
        }

        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "InternetSetOption", CallingConvention = CallingConvention.StdCall)]
        private static extern bool InternetSetOption(int hInternet, int dmOption, IntPtr lpBuffer, int dwBufferLength);
        private static void CloseSystemProxy()
        {
            //打开注册表
            var regKey = Registry.CurrentUser;

            const string subKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            var optionKey = regKey.OpenSubKey(subKeyPath, true);
            //更改健值，设置代理，
            if (optionKey != null) optionKey.SetValue("ProxyEnable", 0);

            //激活代理设置
            InternetSetOption(0, 39, IntPtr.Zero, 0);
            InternetSetOption(0, 37, IntPtr.Zero, 0);
        }

        public void Dispose()
        {
            if (disposed) return;
            FiddlerApplication.Shutdown();
            CloseSystemProxy();
            console.WriteLine("Fiddler代理已关闭");
            disposed = true;
        }


        public class WsMessageEventArgs
        {
            /// <summary>
            /// 消息体
            /// </summary>
            public WebSocketMessage Message { get; set; }

            /// <summary>
            /// 进程ID
            /// </summary>
            public int ProcessID { get; set; }

            /// <summary>
            /// 进程名
            /// </summary>
            public string ProcessName { get; set; }


            public WsMessageEventArgs(WebSocketMessage msg)
            {
                this.Message = msg;

                FieldInfo field = msg.GetType().GetField("_wsOwner", BindingFlags.NonPublic | BindingFlags.Instance);
                WebSocket obj = (WebSocket)field.GetValue(msg);
                field = obj.GetType().GetField("oCP", BindingFlags.NonPublic | BindingFlags.Instance);
                ClientPipe ocp = (ClientPipe)field.GetValue(obj);

                this.ProcessID = ocp.LocalProcessID;
                this.ProcessName = ocp.LocalProcessName;
            }
        }
    }
}

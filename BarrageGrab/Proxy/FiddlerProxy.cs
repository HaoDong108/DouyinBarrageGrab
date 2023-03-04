using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

namespace BarrageGrab.Proxy
{
    /// <summary>
    /// FiddlerCore 代理程序
    /// </summary>
    [Obsolete("由于框架存在内存泄漏,已弃用,请使用TitaniumProxy)", false)]
    internal class FiddlerProxy : SystemProxy
    {
        bool disposed = false;
        ConsoleWriter console = new ConsoleWriter();

        public FiddlerProxy()
        {
            InitFiddler();
        }

        //初始化Fiddler
        private void InitFiddler()
        {
            //这个名字随便
            FiddlerApplication.SetAppDisplayName("FiddlerCore");
            FiddlerApplication.OnWebSocketMessage += FiddlerApplication_OnWebSocketMessage;
            //FiddlerApplication.OnReadResponseBuffer += OnReadResponseBuffer;
            //设置pac规则，匹配douyin且排除douyincdn的host走代理，其余的直连
            FiddlerApplication.Prefs.SetStringPref("fiddler.proxy.pacfile.text", $"if (/douyin(?!cdn)/.test(host)) {{return 'PROXY 127.0.0.1:{base.ProxyPort}' }} return 'DIRECT'");


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

            FieldInfo field = msg.GetType().GetField("_wsOwner", BindingFlags.NonPublic | BindingFlags.Instance);
            WebSocket obj = (WebSocket)field.GetValue(msg);
            field = obj.GetType().GetField("oCP", BindingFlags.NonPublic | BindingFlags.Instance);

            var sessionField = obj.GetType().GetField("_mySession", BindingFlags.NonPublic | BindingFlags.Instance);

            ClientPipe ocp = (ClientPipe)field.GetValue(obj);
            Session session = (Session)sessionField.GetValue(obj);

            this.SendWebSocketData(new WsMessageEventArgs()
            {
                Payload = msg.PayloadData,
                ProcessID = ocp.LocalProcessID,
                ProcessName = ocp.LocalProcessName,
                HostName = session.hostname,
            });
        }


        public override void Start()
        {
            FiddlerApplication.Startup(Appsetting.Instanse.ProxyPort, FiddlerCoreStartupFlags.Default);
            console.WriteLine("Fiddler代理已启动", ConsoleColor.Green);
        }

        public override void Dispose()
        {
            if (disposed) return;
            FiddlerApplication.Shutdown();
            CloseSystemProxy();
            console.WriteLine("Fiddler代理已关闭");
            disposed = true;
        }
    }
}

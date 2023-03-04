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

        // 生成自签名证书
        protected X509Certificate2 EnsureCertificate(string subjectName)
        {
            // 添加证书到证书存储
            var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            //判断证书是否已经存在
            var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);
            if (certificates.Count > 0)
            {
                return certificates[0];
            }

            // 创建证书请求
            var request = new CertificateRequest($"CN={subjectName}", RSA.Create(2048), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // 添加扩展属性
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));
            request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

            // 用自己的私钥签名并生成证书
            var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(365));
            store.Add(certificate);
            store.Close();

            return certificate;
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

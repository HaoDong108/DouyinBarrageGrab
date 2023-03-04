using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.StreamExtended.Network;

namespace BarrageGrab.Proxy
{
    internal class TitaniumProxy : SystemProxy
    {
        ProxyServer proxyServer = null;
        ExplicitProxyEndPoint explicitEndPoint = null;

        public TitaniumProxy()
        {
            //注册系统代理
            //RegisterSystemProxy();
            proxyServer = new ProxyServer();

            //var cert = EnsureCertificate("localhost");

            proxyServer.ReuseSocket = false;
            proxyServer.EnableConnectionPool = true;
            proxyServer.ForwardToUpstreamGateway = true;
            proxyServer.CertificateManager.SaveFakeCertificates = true;

            proxyServer.ServerCertificateValidationCallback += ProxyServer_ServerCertificateValidationCallback; ;
            proxyServer.BeforeResponse += ProxyServer_BeforeResponse;

            explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any,ProxyPort, true);
            explicitEndPoint.BeforeTunnelConnectRequest += ExplicitEndPoint_BeforeTunnelConnectRequest;
            proxyServer.AddEndPoint(explicitEndPoint);
        }


        private Task ProxyServer_ServerCertificateValidationCallback(object sender, CertificateValidationEventArgs e)
        {
            // set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == SslPolicyErrors.None)
            {
                e.IsValid = true;
            }

            return Task.CompletedTask;
        }


        private async Task ProxyServer_BeforeResponse(object sender, SessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;
            if (!barrageWsHostNames.Contains(hostname)) return;

            if (e.HttpClient.ConnectRequest?.TunnelType == TunnelType.Websocket)
            {
                e.DataReceived += WebSocket_DataReceived;
            }
        }

        //List<string> hostlist = new List<string>();

        private async Task ExplicitEndPoint_BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;
            if (!barrageWsHostNames.Contains(hostname))
            {
                e.DecryptSsl = false;
            }
        }

        private async void WebSocket_DataReceived(object sender, DataEventArgs e)
        {
            var args = (SessionEventArgs)sender;

            string hostname = args.HttpClient.Request.RequestUri.Host;

            var processid = args.HttpClient.ProcessId.Value;

            var frames = args.WebSocketDecoderReceive.Decode(e.Buffer, e.Offset, e.Count).ToList();
            //Console.WriteLine(frames.Count);

            foreach (var frame in frames)
            {
                //Console.WriteLine(frame.GetText());
                base.SendWebSocketData(new WsMessageEventArgs()
                {
                    ProcessID = processid,
                    HostName = hostname,
                    Payload = frame.Data.ToArray(),
                    ProcessName = base.GetProcessName(processid)
                });
            }
            //Console.WriteLine("获取到弹幕信息:" + hostname);

            
        }

        /// <summary>
        /// 释放资源，关闭系统代理
        /// </summary>
        override public void Dispose()
        {
            proxyServer.Stop();
            proxyServer.Dispose();
            CloseSystemProxy();
        }

        /// <summary>
        /// 启动监听
        /// </summary>
        override public void Start()
        {
            proxyServer.Start();
            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
        }
    }
}

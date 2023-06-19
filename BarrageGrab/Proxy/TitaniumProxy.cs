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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BarrageGrab.Proxy.ProxyEventArgs;
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

            proxyServer.ReuseSocket = false;
            proxyServer.EnableConnectionPool = true;
            proxyServer.ForwardToUpstreamGateway = true;
            proxyServer.CertificateManager.SaveFakeCertificates = true;

            proxyServer.CertificateManager.RootCertificate = proxyServer.CertificateManager.LoadRootCertificate();
            if (proxyServer.CertificateManager.RootCertificate == null)
            {
                Console.WriteLine("正在进行证书安装，需要安装证书才可进行https解密，若有提示请确定");
                proxyServer.CertificateManager.CreateRootCertificate();
            }

            proxyServer.ServerCertificateValidationCallback += ProxyServer_ServerCertificateValidationCallback; ;
            proxyServer.BeforeResponse += ProxyServer_BeforeResponse;
            proxyServer.AfterResponse += ProxyServer_AfterResponse;

            explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, ProxyPort, true);
            explicitEndPoint.BeforeTunnelConnectRequest += ExplicitEndPoint_BeforeTunnelConnectRequest;            
            proxyServer.AddEndPoint(explicitEndPoint);
        }

        private Task ProxyServer_AfterResponse(object sender, SessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;
            var processid = e.HttpClient.ProcessId.Value;

            this.FireOnResponse(new HttpResponseEventArgs()
            {
                ProcessID = processid,
                HostName = hostname,
                ProcessName = base.GetProcessName(processid),
                HttpClient = e.HttpClient
            });

            return Task.CompletedTask;
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
            string uri = e.HttpClient.Request.RequestUri.ToString();
            string hostname = e.HttpClient.Request.RequestUri.Host;
            if (e.HttpClient.ConnectRequest?.TunnelType == TunnelType.Websocket)
            {
                e.DataReceived += WebSocket_DataReceived;
            }

            //判断响应内容是否为js application/javascript
            if (e.HttpClient.Response.ContentType != null &&
                e.HttpClient.Response.ContentType.Trim().ToLower().Contains("application/javascript") &&
                hostname == "lf-cdn-tos.bytescm.com"
                )
            {
                var js = await e.GetResponseBodyAsString();
                
                //修改js,绕过页面无操作检测
                var reg1 = new Regex(@"start\(\)\{Dt\.enable\(\),this\.tracker&&this\.tracker\.enable\(\)\}");
                var match = reg1.Match(js);
                if (match.Success)
                {
                    js = reg1.Replace(js, "start(){return;Dt.enable(),this.tracker&&this.tracker.enable()}");
                    e.SetResponseBodyString(js);
                    return;
                }

                var reg2 = new Regex(@"if\(!N.DJ\(\)&&(?<variable>\S).current\)\{");
                match = reg2.Match(js);
                if (match.Success)
                {
                    js = reg2.Replace(js, "if(!N.DJ()&&${variable}.current){return;");
                    e.SetResponseBodyString(js);
                    return;
                }
            }
        }

        private async Task ExplicitEndPoint_BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string url = e.HttpClient.Request.RequestUri.ToString();
            string hostname = e.HttpClient.Request.RequestUri.Host;
            if (hostname == "lf-cdn-tos.bytescm.com")
            {
                e.DecryptSsl = true;
                return;
            }
            
            if (!CheckHost(hostname))
            {
                e.DecryptSsl = false;
            }
        }

        private async void WebSocket_DataReceived(object sender, DataEventArgs e)
        {
            var args = (SessionEventArgs)sender;

            string hostname = args.HttpClient.Request.RequestUri.Host;

            var processid = args.HttpClient.ProcessId.Value;

            List<byte> messageData = new List<byte>();

            try
            {
                foreach (var frame in args.WebSocketDecoderReceive.Decode(e.Buffer, e.Offset, e.Count))
                {
                    if (frame.OpCode == WebsocketOpCode.Continuation)
                    {
                        messageData.AddRange(frame.Data.ToArray());
                        continue;
                    }
                    else
                    {
                        byte[] payload;
                        if (messageData.Count > 0)
                        {
                            messageData.AddRange(frame.Data.ToArray());
                            payload = messageData.ToArray();
                            messageData.Clear();
                        }
                        else
                        {
                            payload = frame.Data.ToArray();
                        }

                        base.FireWsEvent(new WsMessageEventArgs()
                        {
                            ProcessID = processid,
                            HostName = hostname,
                            Payload = payload,
                            ProcessName = base.GetProcessName(processid)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("解析某个WebSocket包出错：" + ex.Message);
            }

            if (messageData.Count > 0)
            {
                // 没有收到 WebSocket 帧的结束帧，抛出异常或者进行处理
            }

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

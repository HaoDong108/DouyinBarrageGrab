using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BarrageGrab.Modles;
using BarrageGrab.Modles.JsonEntity;
using BarrageGrab.Proxy.ProxyEventArgs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.StreamExtended.Network;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Net.Http;
using System.Text;
using BrotliSharpLib;
using System.IO.Compression;

namespace BarrageGrab.Proxy
{
    internal class TitaniumProxy : SystemProxy
    {
        ProxyServer proxyServer = null;
        ExplicitProxyEndPoint explicitEndPoint = null;
        ExternalProxy upStreamProxy = null;

        const string SCRIPT_HOST = "lf-cdn-tos.bytescm.com";
        const string LIVE_HOST = "live.douyin.com";
        const string DOUYIN_HOST = "www.douyin.com";
        const string USER_INFO_PATH = "/webcast/user/me/";
        const string BARRAGE_POOL_PATH = "/webcast/im/fetch";
        const string LIVE_SCRIPT_PATH = "/obj/static/webcast/douyin_live";
        const string WEBCAST_AMEMV_HOST = "webcast.amemv.com";

        public override string HttpUpstreamProxy { get { return proxyServer?.UpStreamHttpProxy?.ToString() ?? ""; } }

        public override string HttpsUpstreamProxy { get { return proxyServer?.UpStreamHttpsProxy?.ToString() ?? ""; } }

        static TitaniumProxy()
        {
            // 设置代理过滤规则
            string[] bypassList = { "localhost", "127.*", "10.*", "172.16.*", "172.17.*", "172.18.*", "172.19.*",
                                "172.20.*", "172.21.*", "172.22.*", "172.23.*", "172.24.*", "172.25.*",
                                "172.26.*", "172.27.*", "172.28.*", "172.29.*", "172.30.*", "172.31.*",
                                "192.168.*" };

            // 创建WebProxy对象，并设置代理过滤规则
            WebProxy proxy = new WebProxy
            {
                BypassList = bypassList,
                UseDefaultCredentials = true
            };

            // 设置不使用回环地址
            proxy.BypassProxyOnLocal = true;

            try
            {
                // 将代理设置为系统默认代理
                WebRequest.DefaultWebProxy = proxy;
            }
            catch (Exception ex)
            {
                Logger.LogWarn("代理环境设置失败：" + ex.Message);
            }
        }

        public TitaniumProxy()
        {
            //注册系统代理
            //RegisterSystemProxy();
            proxyServer = new ProxyServer();

            proxyServer.ReuseSocket = false;
            proxyServer.EnableConnectionPool = true;
            proxyServer.ForwardToUpstreamGateway = true;

            proxyServer.CertificateManager.CertificateValidDays = 365 * 10;
            proxyServer.CertificateManager.SaveFakeCertificates = true;
            proxyServer.CertificateManager.RootCertificate = GetCert();
            if (proxyServer.CertificateManager.RootCertificate == null)
            {
                Logger.PrintColor("正在进行证书安装，需要信任该证书才可进行https解密，若有提示请确定");
                proxyServer.CertificateManager.CreateRootCertificate();
            }

            //信任证书
            proxyServer.CertificateManager.CreateRootCertificate();
            proxyServer.CertificateManager.TrustRootCertificate(true);

            proxyServer.ServerCertificateValidationCallback += ProxyServer_ServerCertificateValidationCallback;
            proxyServer.BeforeResponse += ProxyServer_BeforeResponse;
            //proxyServer.AfterResponse += ProxyServer_AfterResponse;            

            explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, ProxyPort, true);
            explicitEndPoint.BeforeTunnelConnectRequest += ExplicitEndPoint_BeforeTunnelConnectRequest;
            proxyServer.AddEndPoint(explicitEndPoint);
        }

        private X509Certificate2 GetCert()
        {
            X509Certificate2 result = proxyServer.CertificateManager.LoadRootCertificate();

            if (result != null) return result;

            return null;

            // 打开“受信任的根证书颁发机构”存储区
            using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);

                // 遍历证书集合
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    try
                    {
                        // 尝试使用证书创建 X509Certificate2 实例
                        X509Certificate2 certificate = new X509Certificate2(cert);

                        //判断过期
                        if (DateTime.Now > certificate.NotAfter) continue;

                        //判断黑名单
                        var black = new string[] { "localhost" };
                        if (certificate.FriendlyName.ToLower().LikeIn(black) ||
                            certificate.Subject.ToLower().LikeIn(black)
                           ) continue;


                        if (certificate.FriendlyName.ToLower().LikeIn("titanium"))
                        {
                            result = certificate;
                            break;
                        }

                        // 打印证书信息
                        //Console.WriteLine("证书: " + certificate.Subject);
                        //Console.WriteLine("颁发者: " + certificate.Issuer);
                        //Console.WriteLine("有效期: " + certificate.NotBefore + " - " + certificate.NotAfter);

                        //result = certificate;
                        //break;
                    }
                    catch (CryptographicException ex)
                    {
                        //捕获加密异常，如果证书需要密码，这里会抛出异常
                        throw new Exception("证书加载失败: " + ex.Message);
                    }
                }

                store.Close();
            }

            return result;
        }

        public override void SetUpstreamProxy(string upstreamProxyAddr)
        {
            if (string.IsNullOrWhiteSpace(upstreamProxyAddr)) return;
            upstreamProxyAddr = upstreamProxyAddr.Trim();
            var reg = new Regex(@"[a-zA-Z0-9\.]+:\d+");
            if (!reg.IsMatch(upstreamProxyAddr))
            {
                throw new Exception("上游代理地址格式不正确，必须为ip:port格式");
            }
            //设置上游代理地址
            //var upstreamProxyAddr = Appsetting.Current.UpstreamProxy;
            if (!upstreamProxyAddr.IsNullOrWhiteSpace())
            {
                upStreamProxy = new ExternalProxy()
                {
                    HostName = upstreamProxyAddr.Split(':')[0],
                    Port = int.Parse(upstreamProxyAddr.Split(':')[1]),
                    ProxyType = ExternalProxyType.Http
                };
                proxyServer.UpStreamHttpProxy = upStreamProxy;
                proxyServer.UpStreamHttpsProxy = upStreamProxy;
            }
        }

        private bool CheckBrowser(string processName)
        {
            return AppSetting.Current.ProcessFilter.Contains(processName) && processName!="直播伴侣" && processName!= "douyin";
        }

        private async Task ProxyServer_BeforeResponse(object sender, SessionEventArgs e)
        {
            string uri = e.HttpClient.Request.RequestUri.ToString();
            string hostname = e.HttpClient.Request.RequestUri.Host;
            var processid = e.HttpClient.ProcessId.Value;
            var processName = base.GetProcessName(processid);
            var contentType = e.HttpClient.Response.ContentType ?? "";

            //处理直播伴侣开播更新
            await HookSelfLive(e);

            //处理弹幕
            await HookBarrage(e);

            //处理JS注入
            await HookPageAsync(e);

            //处理脚本拦截修改
            await HookScriptAsync(e);
        }

        // Hook 直播伴侣开播信息并更新
        private async Task HookSelfLive(SessionEventArgs e)
        {
            //Hook 直播伴侣直播间创建 https://webcast.amemv.com/webcast/room/create/?ac=wifi&app_name=webcast_mate&version_code=7.3.3&device_platform=windows&webcast_sdk_version=1520&resolution=1707%2A1067&os_version=10.0.22621&language=zh&aid=2079&live_id=1&channel=online&device_id=2164319493312045&iid=42200736232026&extra_first_tag_id=22&extra_second_tag_id=22093&extra_third_tag_id=22093195&extra_encoder_core=qsv&extra_codec_name=h264_qsv_ex&extra_codec_is_ex=1&extra_use_265=0&msToken=8tJ0NCHWun7wHpdPd_fd0_nlUmgRwM8sQYThkqkq4-qR00mKiJ3Wd3h05r4mm5HO_R_qA2qeTIn8qR2yjcXXoh5mmXkewUuTS4G1Yoi_D-m8EZiacZVWoDqgnqw=&X-Bogus=DFSzswVLJzC4fUiKt5a4a3JCqOA1&_signature=_02B4Z6wo00001fHp2wAAAIDCdmADbSJfNUnx6d-AABpim6xRe8bmnvrrC1Z7GWTwK8sPujtot.bkv7h8bk-nde0WvO-78H3cwglXRzZk8uHTs3ZKWlZqOqaBVjgdHIRritV.peh4bkRETofZ7f
            string uri = e.HttpClient.Request.RequestUri.ToString();
            var urix = new Uri(uri);
            var processid = e.HttpClient.ProcessId.Value;
            var processName = base.GetProcessName(processid);
            var response = e.HttpClient.Response;
            if (urix.AbsolutePath != "/webcast/room/create/") return;
            if (processName != "直播伴侣") return;
            if (response.StatusCode != 200) return;
            var reponse = await e.GetResponseBodyAsString();
            RoomInfo roomInfo;
            //缓存直播间信息
            var tupe = RoomInfo.TryParseStreamPusherCreate(reponse, out roomInfo);
            var code = tupe.Item1;
            var msg = tupe.Item2;

            if (code != 0)
            {
                Logger.LogWarn($"直播伴侣开播房间资料缓存失败，原因:{msg}");
                return;
            }

            var jobj = JsonConvert.DeserializeObject<JObject>(reponse);

            var roomid = jobj["data"]?["id_str"]?.Value<string>();
            var sec_uid = jobj["data"]?["owner"]?["sec_uid"]?.Value<string>();
            var nickname = jobj["data"]?["owner"]?["nickname"]?.Value<string>();
            var displayId = jobj["data"]?["owner"]?["display_id"]?.Value<string>();

            if (roomInfo != null && !roomid.IsNullOrWhiteSpace())
            {
                roomInfo.RoomId = roomid;
                roomInfo.Title = jobj["data"]?["title"]?.Value<string>();
                Logger.LogInfo($"直播伴侣开播，开播账号:{displayId} {nickname} ，更新RoomId={roomInfo.RoomId}");
            }

            if (roomInfo != null)
            {
                AppRuntime.RoomCaches.AddRoomInfoCache(roomInfo);
            }
        }

        // Hook 弹幕
        private async Task HookBarrage(SessionEventArgs e)
        {
            string uri = e.HttpClient.Request.RequestUri.ToString();
            string hostname = e.HttpClient.Request.RequestUri.Host;
            var processid = e.HttpClient.ProcessId.Value;
            var processName = base.GetProcessName(processid);
            var contentType = e.HttpClient.Response.ContentType ?? "";

            //ws 方式
            if (
                e.HttpClient.ConnectRequest?.TunnelType == TunnelType.Websocket &&
                hostname.StartsWith("webcast")
               )
            {
                e.DataReceived += WebSocket_DataReceived;
            }
            //轮询方式(当抖音ws连接断开后，客户端也会降级使用轮询模式获取弹幕)
            if (uri.Contains(BARRAGE_POOL_PATH) && contentType.Contains("application/protobuffer"))
            {
                var payload = await e.GetResponseBody();

                base.FireOnFetchResponse(new HttpResponseEventArgs()
                {
                    HttpClient = e.HttpClient,
                    ProcessID = processid,
                    HostName = hostname,
                    ProcessName = base.GetProcessName(processid),
                    Payload = payload
                });
            }
        }

        /// <summary>
        /// 获取配置的注入的js
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetInjectScript(string name)
        {
            //获取exe所在目录路径            
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "inject", name + ".js");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return null;
        }

        // Hook 直播页面
        private async Task HookPageAsync(SessionEventArgs e)
        {
            string uri = e.HttpClient.Request.RequestUri.ToString();
            string hostname = e.HttpClient.Request.RequestUri.Host;
            string url = e.HttpClient.Request.Url;
            var urlNoQuery = url.Split('?')[0];
            var processid = e.HttpClient.ProcessId.Value;
            var processName = base.GetProcessName(processid);
            var liveRoomMactch = Regex.Match(uri.Trim(), @".*:\/\/live.douyin\.com\/([0-9a-zA-Z_]+)");
            var liveHomeMatch = Regex.Match(uri.Trim(), @".*:\/\/live.douyin\.com\/?$");
            var contentType = e.HttpClient.Response.ContentType ?? "";

            //检测是否为dom页，用于脚本注入
            if (!contentType.Contains("text/html")) return;

            //如果响应头含有 CSP(https://blog.csdn.net/qq_30436011/article/details/127485927 会阻止内嵌脚本执行) 则删除
            var csp = e.HttpClient.Response.Headers.GetFirstHeader("Content-Security-Policy");
            if (csp != null)
            {
                e.HttpClient.Response.Headers.RemoveHeader("Content-Security-Policy");
            }

            //获取 content-type                
            if (liveRoomMactch.Success)
            {
                string webrid = liveRoomMactch.Groups[1].Value;
                //获取直播页注入js
                string liveRoomInjectScript = GetInjectScript("livePage");

                //注入上下文变量;
                var scriptContext = BuildContext(new Dictionary<string, string>()
                {
                    {"PROCESS_NAME","'{processName}'"},
                    {"AUTOPAUSE",AppSetting.Current.AutoPause.ToString().ToLower()}
                });
                liveRoomInjectScript = scriptContext + liveRoomInjectScript;
                var html = await e.GetResponseBodyAsString();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                if (!liveRoomInjectScript.IsNullOrWhiteSpace())
                {
                    //利用 HtmlAgilityPack 在尾部注入script 标签
                    RoomInfo roominfo;
                    var tup = RoomInfo.TryParseRoomPageHtml(html, out roominfo);
                    int code = tup.Item1;
                    string msg = tup.Item2;

                    if (code == 0)
                    {
                        Logger.LogInfo($"直播页{webrid}房间信息已采集到缓存");
                        roominfo.WebRoomId = webrid;
                        roominfo.LiveUrl = url;
                        AppRuntime.RoomCaches.AddRoomInfoCache(roominfo);
                    }
                    else
                    {
                        roominfo = new RoomInfo();
                        roominfo.WebRoomId = webrid;
                        roominfo.LiveUrl = url;
                        //正则匹配主播标题
                        //<div class="st8eGKi4" data-e2e="live-room-nickname">和平精英小夜y</div>
                        var match = Regex.Match(html, @"(?<=live-room-nickname""\>).+(?=<\/div>)");
                        if (match.Success)
                        {
                            roominfo.Owner = new RoomInfo.RoomAnchor()
                            {
                                Nickname = match.Value,
                                UserId = "-1"
                            };
                        }
                        AppRuntime.RoomCaches.AddRoomInfoCache(roominfo);
                    }

                    try
                    {

                        //找到body标签,在尾部注入script标签
                        var body = doc.DocumentNode.SelectSingleNode("//body");
                        if (body != null)
                        {
                            var script = doc.CreateElement("script");
                            script.InnerHtml = liveRoomInjectScript;
                            body.AppendChild(script);
                            html = doc.DocumentNode.OuterHtml;
                            Logger.PrintColor($"直播页{urlNoQuery},用户脚本已成功注入!\n", ConsoleColor.Green);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"直播页{url},用户脚本注入异常");
                    }
                }

                //给script标签 src加上时间戳避免缓存
                if (AppSetting.Current.DisableLivePageScriptCache)
                {
                    ScriptAddTocks(doc);
                }

                html = doc.DocumentNode.OuterHtml;

                e.SetResponseBodyString(html);
            }

            //直播主页
            if (liveHomeMatch.Success)
            {
                //获取直播页注入js
                string liveHoomInjectScript = GetInjectScript("livePage");

                //注入上下文变量;
                var scriptContext = BuildContext(new Dictionary<string, string>()
                {
                    {"PROCESS_NAME","'{processName}'"},
                    {"AUTOPAUSE",AppSetting.Current.AutoPause.ToString().ToLower()}
                });
                liveHoomInjectScript = scriptContext + liveHoomInjectScript;


                if (!liveHoomInjectScript.IsNullOrWhiteSpace())
                {
                    //利用 HtmlAgilityPack 在尾部注入script 标签
                    var html = await e.GetResponseBodyAsString();
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    //找到body标签,在尾部注入script标签
                    var body = doc.DocumentNode.SelectSingleNode("//body");
                    if (body != null)
                    {
                        var script = doc.CreateElement("script");
                        script.InnerHtml = liveHoomInjectScript;
                        body.AppendChild(script);
                        var newHtml = doc.DocumentNode.OuterHtml;
                        e.SetResponseBodyString(newHtml);
                        Logger.PrintColor($"直播首页{urlNoQuery},用户脚本已成功注入!\n", ConsoleColor.Green);
                    }
                }
            }
        }

        //给部分脚本加上时间戳避免缓存
        private void ScriptAddTocks(HtmlDocument doc)
        {
            var scripts = doc.DocumentNode.SelectNodes("//script[@src]");
            if (scripts != null)
            {
                var ticks = DateTime.Now.Ticks;
                foreach (var script in scripts)
                {
                    var src = script.Attributes["src"].Value;

                    var srcUri = new Uri(src);
                    if (!CheckHost(srcUri.Host)) continue;

                    var fileName = Path.GetFileName(src.Split('?')[0]);
                    //目前只需要用到相关这些js
                    if (!fileName.StartsWith("island") && !src.Contains(LIVE_SCRIPT_PATH)) continue;

                    if (src.Contains("?"))
                    {
                        src += "&_t=" + ticks;
                    }
                    else
                    {
                        src += "?_t=" + ticks;
                    }
                    script.Attributes["src"].Value = src;
                }

            }
        }

        //生成注入上下文
        private string BuildContext(IDictionary<string, string> constVals)
        {
            var scriptContext = string.Join("\r\n", constVals.Select(s =>
            {
                return "const " + s.Key + " = " + s.Value + ";";
            }));
            return scriptContext;
        }

        // Hook Script 脚本
        private async Task HookScriptAsync(SessionEventArgs e)
        {
            string uri = e.HttpClient.Request.RequestUri.ToString();
            string hostname = e.HttpClient.Request.RequestUri.Host;
            string url = e.HttpClient.Request.Url;
            var urlNoQuery = url.Split('?')[0];
            var processid = e.HttpClient.ProcessId.Value;
            var processName = base.GetProcessName(processid);
            var contentType = e.HttpClient.Response.ContentType ?? "";
            var fileName = Path.GetFileName(urlNoQuery);

            if (contentType == null) return;
            if (!contentType.Trim().ToLower().Contains("application/javascript")) return;
            if (e.HttpClient.Response.StatusCode != 200) return;

            //判断响应内容是否为js application/javascript
            if (hostname == SCRIPT_HOST
                //这俩个进程不需要注入
                && processName != "直播伴侣" && processName != "douyin"
                && fileName.StartsWith("island")
                )
            {
                var js = await e.GetResponseBodyAsString();
                var reg2 = new Regex(@"if\(!\((?<v1>\d,\w{1,2})\.DJ\)\(\)&&");
                var match = reg2.Match(js);
                if (match.Success)
                {
                    js = reg2.Replace(js, "if(false &&");
                    e.SetResponseBodyString(js);
                    Logger.PrintColor($"已成功绕过页面无操作检测\n", ConsoleColor.Green);
                }
            }

            if (url.Contains(LIVE_SCRIPT_PATH) && AppSetting.Current.ForcePolling)
            {
                var reg = new Regex(@"if\s*\((?<patt>!this\.stopPolling)\)");
                var js = await e.GetResponseBodyAsString();
                var match = reg.Match(js);
                if (match.Success)
                {
                    var pollingIntervalReg = new Regex(@"this\.errorInterval\s*=(?<value>.+?),");
                    var pollingIntervalMatch = pollingIntervalReg.Match(js);
                    if (pollingIntervalMatch.Success)
                    {
                        var myValue = AppSetting.Current.PollingInterval;
                        js = pollingIntervalReg.Replace(js, $"this.pollingInterval={myValue},");
                        Logger.PrintColor($"直播间已成功修改轮询间隔为{myValue}ms", ConsoleColor.Green);
                    }

                    js = reg.Replace(js, "if(true)");
                    e.SetResponseBodyString(js);
                    Logger.PrintColor($"直播间已强制启用Http轮询模式", ConsoleColor.Green);
                }
            }
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

        //控制要解密SSL的域名
        private async Task ExplicitEndPoint_BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string url = e.HttpClient.Request.RequestUri.ToString();
            string hostname = e.HttpClient.Request.RequestUri.Host;

            e.DecryptSsl = CheckHost(hostname);
        }

        //检测域名白名单
        protected override bool CheckHost(string hostname)
        {
            //需要解析SSL的域名 放在这里，全开会导致性能问题，应只解析业务需要的域名
            var decryptSsls = new string[]
            {
                SCRIPT_HOST,
                LIVE_HOST,
                WEBCAST_AMEMV_HOST , //直播伴侣开播请求地址
            };

            if (decryptSsls.Contains(hostname))
            {
                return true;
            }

            hostname = hostname.Trim().ToLower();
            return base.CheckHost(hostname);
        }

        //WebSocket 流读取
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
                        //读取完毕
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
                Logger.PrintColor("解析某个WebSocket包出错：" + ex.Message);
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
            if (AppSetting.Current.UsedProxy)
            {
                CloseSystemProxy();
            }
        }

        /// <summary>
        /// 启动监听
        /// </summary>
        override public void Start()
        {
            proxyServer.Start(false);

            if (AppSetting.Current.UsedProxy)
            {
                base.RegisterSystemProxy();
                Logger.LogInfo($"系统代理代理已启动，127.0.0.1:{base.ProxyPort}");
                //使用其自带的系统代理设置可能会导致格式问题
                //proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
                //proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
            }
            else
            {
                Logger.LogInfo($"代理已启动(局域代理)，127.0.0.1:{base.ProxyPort}");
            }
        }
    }
}

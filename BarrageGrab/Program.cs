using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using Jint.Runtime;
using System.Windows.Forms;

namespace BarrageGrab
{
    public class Program
    {
        static FormView mainForm = null;
        static bool exited = false;
        static bool formExited = false;
        static WinApi.ControlCtrlDelegate controlCtr = ControlCtrlHandle;

        static void Main(string[] args)
        {
            //JintTest();
            //Console.WriteLine("结束");
            //Console.ReadKey();
            //return;
            if (CheckAlreadyRunning())
            {
                Logger.PrintColor("已经有一个监听程序在运行，按任意键退出...");
                Console.ReadKey();
                return;
            }
            SetTitle("抖音弹幕监听推送");

            try
            {
                Init();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "程序初始化错误");
                MessageBox.Show(ex.Message, "程序初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                exited = true;
            }

            while (!exited)
            {               
                Thread.Sleep(500);
            }

            if (!AppRuntime.WsServer.IsDisposed)
            {
                AppRuntime.WsServer.Dispose();                
            }

            Logger.PrintColor("服务器已关闭...");
            WinApi.SetConsoleCtrlHandler(controlCtr, false);//反注册捕获控制台关闭            

        }

        private static void Init()
        {
            AppRuntime.Init();
            WinApi.SetConsoleCtrlHandler(controlCtr, true);//捕获控制台关闭
            WinApi.DisableQuickEditMode();//禁用控制台快速编辑模式
            AppRuntime.DisplayConsole(!AppSetting.Current.HideConsole);//控制控制台可见
            AppRuntime.WsServer.Grab.Proxy.SetUpstreamProxy(AppSetting.Current.UpstreamProxy);//设置上游代理
            AppRuntime.WsServer.OnClose += (s, e) =>
            {
                exited = true;
            };

            //串口写入服务
            if (!AppSetting.Current.ComPort.IsNullOrWhiteSpace())
            {
                AppRuntime.ComPortServer.OpenStart();
            }

            //显示窗体
            if (AppSetting.Current.ShowWindow)
            {
                var uiThread = new Thread(new ThreadStart(() =>
                {
                    mainForm = new FormView();
                    //开启消息循环
                    System.Windows.Forms.Application.Run(mainForm);
                    formExited = true;
                    AppRuntime.WsServer.Dispose();//Close后自动释放资源
                }));
                uiThread.SetApartmentState(ApartmentState.STA);
                uiThread.IsBackground = true;
                uiThread.Start();
            }

            AppRuntime.WsServer.StartListen();//启动WS以及代理服务
            Logger.LogSucc($"{AppRuntime.WsServer.ServerLocation} 弹幕服务已启动...");
            SetTitle($"抖音弹幕监听推送 [{AppRuntime.WsServer.ServerLocation}]");
        }

        //检测设置控制台标题
        private static void SetTitle(string title)
        {
            if (WinApi.GetConsoleWindow() != IntPtr.Zero)
            {
                Console.Title = title;
            }
        }

        //监听控制台消息事件
        private static bool ControlCtrlHandle(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    //Logger.PrintColor("0工具被强制关闭"); //Ctrl+C关闭
                    //server.Close();
                    break;
                case 2:
                    Logger.PrintColor("2工具被强制关闭");//按控制台关闭按钮关闭
                    AppRuntime.WsServer.Dispose();
                    break;
            }
            return false;
        }

        //检测程序是否多开
        private static bool CheckAlreadyRunning()
        {
            const string mutexName = "DyBarrageGrab";
            // Try to create a new named mutex.
            bool createdNew;
            using (Mutex mutex = new Mutex(true, mutexName, out createdNew))
            {
                return !createdNew;
            }
        }

        private static void JintTest()
        {
            var jsEng = JsEngine.CreateNewEngine();
            var jsFile = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", "engine", "comPortFilter.js"), Encoding.UTF8);
            jsEng.Execute(jsFile);
            var result = jsEng.Invoke("onPackData", 1, new
            {
                Aporp = 142,
                Bprop = "48777",
                Cprop = new string[] { "123" },
                Dprop = new Dictionary<string, string>()
                {
                    { "key1","value1" }
                }
            });

            Console.WriteLine("返回类型:" + result.Type.ToString());
            if (result.Type == Types.String)
            {
                Console.WriteLine("返回String： " + result.ToString());
            }
            if (result.Type == Types.Object)
            {
                var obj = result.ToObject();
                if (obj is byte[])
                {
                    Console.WriteLine("返回byte[]");
                }
            }
            if (result.Type == Types.Symbol)
            {
                Console.WriteLine("返回Symbol： " + result.ToString());
            }
        }
    }
}

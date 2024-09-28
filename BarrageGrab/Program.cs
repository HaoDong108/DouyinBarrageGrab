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
        static Mutex mutex = new Mutex(false, "DyBarrageGrab");
        static void Main(string[] args)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Console.WriteLine("另一个实例已在运行。");
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
                Logger.LogError(ex, $"程序初始化错误，{ex.Message}");
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
            LiveCompanHelper.SwitchSetup();
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
            Logger.PrintColor($"{AppRuntime.WsServer.ServerLocation} 弹幕服务已启动，其他端可通过此地址获取到弹幕流信息", ConsoleColor.Green);

            Version version = System.Reflection.Assembly.GetAssembly(typeof(Program)).GetName().Version;
            SetTitle($"抖音弹幕监听推送 v{version}  [{AppRuntime.WsServer.ServerLocation}]");
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
    }
}

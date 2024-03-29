using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BarrageGrab
{
    public class Program
    {
        static FormView mainForm = null;
        static bool exited = false;
        static bool formExited = false;

        static void Main(string[] args)
        {
            if (CheckAlreadyRunning())
            {
                Logger.PrintColor("已经有一个监听程序在运行，按任意键退出...");
                Console.ReadKey();
                return;
            }
            SetTitle("抖音弹幕监听推送");

            WinApi.SetConsoleCtrlHandler(ControlCtrlDelegate, true);//捕获控制台关闭
            WinApi.DisableQuickEditMode();//禁用控制台快速编辑模式
            AppRuntime.DisplayConsole(!Appsetting.Current.HideConsole);//控制控制台可见
            AppRuntime.WssService.Grab.Proxy.SetUpstreamProxy(Appsetting.Current.UpstreamProxy);//设置上游代理
            AppRuntime.WssService.StartListen();//启动WS服务

            Logger.LogSucc($"{AppRuntime.WssService.ServerLocation} 弹幕服务已启动...");
            SetTitle($"抖音弹幕监听推送 [{AppRuntime.WssService.ServerLocation}]");

            AppRuntime.WssService.OnClose += (s, e) => exited = true;

            //显示窗体
            if (Appsetting.Current.ShowWindow)
            {
                var uiThread = new Thread(new ThreadStart(() =>
                {
                    mainForm = new FormView();
                    //开启消息循环
                    System.Windows.Forms.Application.Run(mainForm);
                    formExited = true;
                }));
                uiThread.SetApartmentState(ApartmentState.STA);
                uiThread.IsBackground = true;
                uiThread.Start();
            }

            while (!exited)
            {
                if (formExited && !exited)
                {
                    AppRuntime.WssService.Close();
                }                    
                Thread.Sleep(500);
            }

            Logger.PrintColor("服务器已关闭...");

            //if (!mainForm.IsDisposed)
            //{
            //    mainForm.Invoke(new Action(() =>
            //    {
            //        mainForm.Close();
            //    }));
            //}            

            //退出程序,不显示 按任意键退出
            //Environment.Exit(0);
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
        private static bool ControlCtrlDelegate(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    //Logger.PrintColor("0工具被强制关闭"); //Ctrl+C关闭
                    //server.Close();
                    break;
                case 2:
                    Logger.PrintColor("2工具被强制关闭");//按控制台关闭按钮关闭
                    AppRuntime.WssService.Close();
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
    }
}

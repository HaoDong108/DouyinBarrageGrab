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
        static void Main(string[] args)
        {            
            if (CheckAlreadyRunning())
            {
                Console.WriteLine("已经有一个监听程序在运行，按任意键退出...");
                Console.ReadKey();
                return;
            }

            WinApi.SetConsoleCtrlHandler(cancelHandler, true);//捕获控制台关闭
            WinApi.DisableQuickEditMode();//禁用控制台快速编辑模式            
            Console.Title = "抖音弹幕监听推送";
            AppRuntime.DisplayConsole(!Appsetting.Current.HideConsole);

            bool exited = false;
            AppRuntime.WssService.StartListen();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{AppRuntime.WssService.ServerLocation} 弹幕服务已启动...");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Title = $"抖音弹幕监听推送 [{AppRuntime.WssService.ServerLocation}]";
            

            AppRuntime.WssService.OnClose += (s, e) =>
            {
                //退出程序
                exited = true;
            };

            while (!exited)
            {
                Thread.Sleep(500);
            }

            Console.WriteLine("服务器已关闭...");
            
            //退出程序,不显示 按任意键退出
            Environment.Exit(0);
        }

        private static WinApi.ControlCtrlDelegate cancelHandler = new WinApi.ControlCtrlDelegate((CtrlType) =>
        {
            switch (CtrlType)
            {
                case 0:
                    //Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭  
                    //server.Close();
                    break;
                case 2:
                    Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭
                    AppRuntime.WssService.Close();
                    break;
            }
            return false;
        });

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

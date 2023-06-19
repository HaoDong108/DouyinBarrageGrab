using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarrageGrab
{
    public class Program
    {
        private delegate bool ControlCtrlDelegate(int CtrlType);

        static WsBarrageService server = null;
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(cancelHandler, true);//捕获控制台关闭            
            Console.Title = "抖音弹幕监听推送";            
            DisableQuickEditMode();
            bool exited = false;
            server = new WsBarrageService();
            server.StartListen();            
            server.OnClose += (s, e) =>
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


        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    //Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭  
                    //server.Close();
                    break;
                case 2:
                    Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭
                    server.Close();
                    break;
            }
            return false;
        }


        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int hConsoleHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

        public static void EnableQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode |= ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(hStdin, mode);
        }

        public static void DisableQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(hStdin, mode);
        }

    }
}

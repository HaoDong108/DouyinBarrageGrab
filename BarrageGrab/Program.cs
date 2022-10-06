using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab
{
    public class Program
    {
        private delegate bool ControlCtrlDelegate(int CtrlType);

        static WssBarrageService server = null;
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(cancelHandler, true);//捕获控制台关闭

            server = new WssBarrageService();
            server.StartListen();

            while (true)
            {
                var input = Console.ReadKey();
                switch (input.Key)
                {
                    case ConsoleKey.Escape: goto end;
                }
            }

        end:
            Console.ReadKey();
            Console.WriteLine("服务器已正常关闭，按任意键结束...");
            Console.ReadKey();
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
    }
}

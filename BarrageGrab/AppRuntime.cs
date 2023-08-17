using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BarrageGrab.Modles;
using ColorConsole;

namespace BarrageGrab
{
    /// <summary>
    /// 程序运行时信息
    /// </summary>
    public static class AppRuntime
    {
        /// <summary>
        /// Ws弹幕服务示例
        /// </summary>
        public static WsBarrageService WssService { get; private set; } = new WsBarrageService();

        /// <summary>
        /// 程序进程信息
        /// </summary>
        public static Process CurrentProcess { get; private set; } = System.Diagnostics.Process.GetCurrentProcess();


        static AppRuntime()
        {
           
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab
{
    public static class WinApi
    {
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

        /// <summary>
        /// 控制台系统操作回调委托
        /// </summary>
        public delegate bool ControlCtrlDelegate(int CtrlType);

        /// <summary>
        /// 设置控制台系统操作回调
        /// </summary>
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int hConsoleHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

        /// <summary>
        /// 启用控制台快速编辑
        /// </summary>
        public static void EnableQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode |= ENABLE_QUICK_EDIT_MODE;
            SetConsoleMode(hStdin, mode);
        }

        /// <summary>
        /// 禁用控制台快速编辑
        /// </summary>
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

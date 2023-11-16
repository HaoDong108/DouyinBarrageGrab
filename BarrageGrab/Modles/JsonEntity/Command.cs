using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Modles.JsonEntity
{
    /*
     * 例如发送 {"Cmd":1,"Data":true} 到ws连接地址 关闭程序
     * 前往 http://wstool.jackxiang.com/ 在线ws测试
     */

    public enum CommandCode
    {
        /// <summary>
        /// 空指令
        /// </summary>
        None = 0, 

        /// <summary>
        /// 安全关闭程序
        /// </summary>
        Close = 1,

        /// <summary>
        /// 启用系统代理 Data:bool
        /// </summary>
        EnableProxy = 2,

        /// <summary>
        /// 是否显示控制台 Data:bool
        /// </summary>
        DisplayConsole = 3,
    }

    public class Command
    {
        /// <summary>
        /// 指令标识
        /// </summary>
        public CommandCode Cmd { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object Data { get; set; }
    }
}

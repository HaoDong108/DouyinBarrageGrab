using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.JsonEntity
{
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

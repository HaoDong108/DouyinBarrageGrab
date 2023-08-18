using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Modles
{
    /// <summary>
    /// 业务异常
    /// </summary>
    public class BusinessExecption : Exception
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; } = -1;

        /// <summary>
        /// 异常附加数据
        /// </summary>
        public object ErrorTarget { get; set; } = null;

        public BusinessExecption()
        {

        }

        public BusinessExecption(string msg) : base(msg)
        {
            this.Code = -1;
        }

        public BusinessExecption(string msg,object data) : base(msg)
        {
            this.ErrorTarget = data;
        }

        public BusinessExecption(string msg, int code) : this(msg)
        {
            this.Code = code;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Proxy.ProxyEventArgs
{
    public class SystemProxyChangeEventArgs:EventArgs
    {
        /// <summary>
        /// True 以开启:False: 已关闭
        /// </summary>
        public bool Open { get; set; }       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Http;

namespace BarrageGrab.Proxy.ProxyEventArgs
{
    /// <summary>
    /// Http 响应事件参数
    /// </summary>
    public class HttpResponseEventArgs : EventArgs
    {
        /// <summary>
        /// 响应对象
        /// </summary>
        public HttpWebClient HttpClient { get; set; }

        /// <summary>
        /// 进程ID
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// 进程名
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string HostName { get; set; }
    }
}

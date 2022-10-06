using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Configuration.ConfigurationManager;
namespace BarrageGrab
{
    internal class Appsetting
    {
        private static readonly Appsetting ins = new Appsetting();

        public static Appsetting Get()
        {
            return ins;
        }

        /// <summary>
        /// 过滤的进程
        /// </summary>
        public string[] FilterProcess { get => AppSettings["filterProcess"].Trim().Split(','); }

        /// <summary>
        /// 端口号
        /// </summary>
        public int WsProt { get => int.Parse(AppSettings["wsListenPort"]); }
    }
}

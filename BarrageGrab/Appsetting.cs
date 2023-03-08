using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarrageGrab.JsonEntity;
using static System.Configuration.ConfigurationManager;
namespace BarrageGrab
{
    internal class Appsetting
    {
        private static readonly Appsetting ins = new Appsetting();

        public static Appsetting Current { get => ins; }

        public Appsetting()
        {
            try
            {
                ProcessFilter = AppSettings["processFilter"].Trim().Split(',');
                WsProt = int.Parse(AppSettings["wsListenPort"]);
                PrintBarrage = AppSettings["printBarrage"].ToLower() == "true";
                ProxyPort = int.Parse(AppSettings["proxyPort"]);
                PrintFilter = Enum.GetValues(typeof(BarrageMsgType)).Cast<int>().ToArray();
                FilterHostName = bool.Parse(AppSettings["filterHostName"].Trim());
                HostNameFilter = AppSettings["hostNameFilter"].Trim().Split(',');
                //RoomIds = AppSettings["roomIds"].Trim().Split(',').Where(w=>!string.IsNullOrWhiteSpace(w)).Select(s => long.Parse(s)).ToArray();

                var printFilter = AppSettings["printFilter"].Trim().ToLower();
                if (printFilter != "all")
                {
                    if (string.IsNullOrWhiteSpace(printFilter)) PrintFilter = new int[0];
                    else PrintFilter = printFilter.Split(',').Select(x => int.Parse(x)).ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("配置文件读取失败,请检查配置文件是否正确");
                throw ex;
            }            
        }

        /// <summary>
        /// 过滤的进程
        /// </summary>
        public string[] ProcessFilter { get; private set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int WsProt { get; private set; }

        /// <summary>
        /// 控制台打印消息开关
        /// </summary>
        public bool PrintBarrage { get; private set; }

        /// <summary>
        /// 代理端口
        /// </summary>
        public int ProxyPort { get; private set; } = 8827;

        /// <summary>
        /// 控制台输出过滤器
        /// </summary>
        public int[] PrintFilter { get; private set; }

        /// <summary>
        /// 监听的房间号
        /// </summary>
        public long[] RoomIds { get; private set; } = new long[0];

        /// <summary>
        /// 使用域名过滤
        /// </summary>
        public bool FilterHostName { get; private set; } = true;

        /// <summary>
        /// 域名白名单列表
        /// </summary>
        public string[] HostNameFilter { get; private set; } = new string[0];
    }
}

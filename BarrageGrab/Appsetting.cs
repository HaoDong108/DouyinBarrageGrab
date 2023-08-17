using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarrageGrab.Modles.JsonEntity;
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
                HostNameFilter = AppSettings["hostNameFilter"].Trim().Split(',').Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
                RoomIds = AppSettings["roomIds"].Trim().Split(',').Where(w => !string.IsNullOrWhiteSpace(w)).Select(s => long.Parse(s)).ToArray();
                UsedProxy = bool.Parse(AppSettings["usedProxy"].Trim());                
                ListenAny = bool.Parse(AppSettings["listenAny"].Trim());

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
        /// 使用系统代理
        /// </summary>
        public bool UsedProxy { get; private set; } = true;

        /// <summary>
        /// 过滤的进程
        /// </summary>
        public string[] ProcessFilter { get; private set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int WsProt { get; private set; } = 8888;

        /// <summary>
        /// true:监听在0.0.0.0，接受任意Ip连接，false:监听在127.0.0.1，仅接受本机连接
        /// </summary>
        public bool ListenAny { get; private set; } = true;

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

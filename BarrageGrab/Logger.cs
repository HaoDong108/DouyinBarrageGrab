using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using BarrageGrab.Modles.JsonEntity;
using System.IO;

namespace BarrageGrab
{
    public static class Logger
    {
        private static NLog.Config.ISetupBuilder builder = LogManager.Setup().LoadConfigurationFromFile("nlog.config");

        private static readonly NLog.Logger logger = builder.GetLogger("*");

        // 记录日志方法
        public static void LogTrace(string message)
        {
            logger.Trace(message);
        }

        public static void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public static void LogInfo(string message)
        {
            logger.Info(message);
        }

        public static void LogWarn(string message)
        {
            logger.Warn(message);
        }

        public static void LogError(string message)
        {
            logger.Error(message);
        }

        public static void LogError(Exception ex, string message)
        {
            logger.Error(ex, message);
        }

        public static void LogFatal(string message)
        {
            logger.Fatal(message);
        }

        public static void LogFatal(Exception ex, string message)
        {
            logger.Fatal(ex, message);
        }

        /// <summary>
        /// 写入弹幕日志
        /// </summary>
        /// <param name="type">弹幕类型</param>
        /// <param name="msg">弹幕内容</param>
        public static void LogBarrage(PackMsgType type, Msg msg)
        {
            if (!Appsetting.Current.BarrageLog) return;
            if (type == PackMsgType.无) return;
            if (Appsetting.Current.LogFilter.Any() && !Appsetting.Current.LogFilter.Contains(type.GetHashCode())) return;

            try
            {
                var dir = Path.Combine(AppContext.BaseDirectory, "logs", "弹幕日志");
                var room = AppRuntime.RoomCaches.GetCachedWebRoomInfo(msg.RoomId.ToString());
                var date = DateTime.Now.ToString("yyyy年MM月dd日直播");
                dir = Path.Combine(dir, $"({room.WebRoomId}){room?.Owner?.Nickname ?? "-1"}", date);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var path = Path.Combine(dir, type + ".txt");
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                var writer = new StreamWriter(path, true, Encoding.UTF8);
                var line = LogText(msg, type);
                writer.WriteLine(line);
                writer.Close();
            }
            catch (Exception ex)
            {
                LogWarn("弹幕日志写入失败，" + ex.Message);
                return;
            }
        }

        private static string LogText(Msg msg, PackMsgType barType)
        {
            var rinfo = AppRuntime.RoomCaches.GetCachedWebRoomInfo(msg.RoomId.ToString());
            var text = $"{DateTime.Now.ToString("HH:mm:ss")} [{barType}]";

            if (msg.User != null)
            {
                text += $" [{msg.User?.GenderToString()}] ";
            }

            ConsoleColor color = Appsetting.Current.ColorMap[barType].Item1;
            var append = msg.Content;
            switch (barType)
            {
                case PackMsgType.弹幕消息: append = $"{msg?.User?.Nickname}: {msg.Content}"; break;
                case PackMsgType.下播: append = $"直播已结束"; break;
                default: break;
            }

            text += append;
            return text;
        }

        /// <summary>
        /// 写入弹幕抓包日志
        /// </summary>
        /// <param name="group">分组名</param>
        /// <param name="name">包名</param>
        /// <param name="buff">数据内容</param>
        /// <param name="maxCount">分组下最大存储包数量</param>
        public static void LogBarragePack(string group, string name, byte[] buff, int maxCount = 5)
        {
            if (buff.Length <= 10) return;
            if (group.IsNullOrWhiteSpace()) return;
            if (name.IsNullOrWhiteSpace()) return;
            var filename = name + ".bin";
            var dir = Path.Combine(AppContext.BaseDirectory, "logs", "弹幕包解析", group);
            var fullPath = Path.Combine(dir, filename);

            try
            {
                //获取该文件在该目录下的数量
                var fiels = Directory.GetFiles(dir, filename);
                var count = fiels.Length;
                if (count > 0)
                {
                    filename = $"{name}({count}).bin";
                    fullPath = Path.Combine(dir, filename);
                }
                File.WriteAllBytes(fullPath, buff);

                if (++count > maxCount)
                {
                    //删除最早的文件
                    var first = fiels.Select(s=>new FileInfo(s)).OrderBy(o=>o.CreationTime).FirstOrDefault();
                    File.Delete(first.FullName);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarn("写入包日志时失败，错误信息:" + ex.Message);
            }
        }
    }
}

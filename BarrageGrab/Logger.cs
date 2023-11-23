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
    }
}

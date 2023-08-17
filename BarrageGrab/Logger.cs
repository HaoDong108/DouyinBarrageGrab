using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

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
    }
}


/*
 NLog中Logger类的方法解释:
1. Trace - 最常见的记录信息，一般用于普通输出 
2. Debug - 同样是记录信息，不过出现的频率要比Trace少一些，一般用来调试程序 
3. Info - 信息类型的消息 
4. Warn - 警告信息，一般用于比较重要的场合 
5. Error - 错误信息 
6. Fatal - 致命异常信息。一般来讲，发生致命异常之后程序将无法继续执行。
 */

using System;
using System.Text;
using NLog;

namespace Msg.Tools.Logging
{
    /// <summary>
    /// 系统日志类
    /// </summary>

    public class LogHelper
    {
        // Fields
        private static readonly bool Isinit = false;
        private static bool _logComplementEnable = false;
        private static bool _logDubugEnable = false;
        private static bool _logErrorEnable = false;
        private static bool _logExceptionEnable = false;
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static bool _logInfoEnable = false;

        // Methods
        static LogHelper()
        {
            if (!Isinit)
            {
                Isinit = true;
                SetConfig();
            }
        }

        private static string BuildMessage(string info, Exception ex = null)
        {
            var sb = new StringBuilder();
           
            sb.AppendLine(string.Format("############服务器日志{0}##################", DateTime.Now.ToString()));
            sb.AppendFormat("\r\n 内容：{0}\r\n", info);
           
            if (ex != null)
            {

                sb.AppendLine("Source:" + ex.Source);
                sb.AppendLine("Message:" + ex.Message);
                sb.AppendLine("StackTrace:" + ex.StackTrace);
                sb.AppendLine("Exception:" + ex.ToString());

                if (ex.InnerException != null)
                {
                    sb.AppendLine("InnerException Source:" + ex.Source);
                    sb.AppendLine("InnerException Message:" + ex.Message);
                    sb.AppendLine("InnerException StackTrace:" + ex.StackTrace);
                    sb.AppendLine("InnerException:" + ex.InnerException.ToString());
                }
            }
            sb.Append("#######################################");
            sb.AppendLine();
            return sb.ToString();
        }

        private static void SetConfig()
        {
            _logInfoEnable = Logger.IsInfoEnabled;
            _logErrorEnable = Logger.IsErrorEnabled;
            _logExceptionEnable = Logger.IsErrorEnabled;
            _logComplementEnable = Logger.IsTraceEnabled;
            _logDubugEnable = Logger.IsDebugEnabled;
        }

        /// <summary>
        /// Writes the trace.
        /// </summary>
        /// <param name="info">The information.</param>
        public static void WriteTrace(string info)
        {
            if (_logComplementEnable)
            {
                Logger.Trace(BuildMessage(info));
            }
        }

        /// <summary>
        /// Writes the trace.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="ex">The ex.</param>
        public static void WriteTrace(string info, Exception ex)
        {
            if (_logComplementEnable)
            {
                Logger.Trace(BuildMessage(info, ex));
            }
        }

        /// <summary>
        /// 记录自定义信息
        /// </summary>
        /// <param name="message">记录消息</param>
        /// <param name="dirOrPrefix">自定义日志生成文件夹，只需要传入文件夹名称；例如：test</param>
        public static void WriteCustom(string message, string dirOrPrefix)
        {
            WriteCustom(message, dirOrPrefix, null);
        }


        /// <summary>
        /// 记录自定义信息
        /// </summary>
        /// <param name="message">记录消息</param>
        /// <param name="dirOrPrefix">自定义日志生成文件夹，只需要传入文件夹名称；例如：test</param>
        /// <param name="suffix">日志文件名称后缀</param>
        public static void WriteCustom(string message, string dirOrPrefix, string suffix)
        {
           
            Logger logger1 = NLog.LogManager.GetLogger("LogCustom");
            var logEvent = new LogEventInfo(LogLevel.Warn, logger1.Name, message);
            logEvent.Properties["DirOrPrefix"] = dirOrPrefix;
            if (suffix != null)
            {
                logEvent.Properties["Suffix"] = suffix;
            }
            logger1.Log(logEvent);
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="info"></param>
        public static void WriteDebug(string info)
        {
            if (_logDubugEnable)
            {
                Logger.Debug(BuildMessage(info));
            }
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="info"></param>
        public static void WriteError(string info)
        {
            if (_logErrorEnable)
            {
                Logger.Error(BuildMessage(info));
            }
        }

        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="ex">异常信息</param>
        public static void WriteException(string info, Exception ex)
        {
            if (_logExceptionEnable)
            {
                Logger.Error(BuildMessage(info, ex));
            }
        }

        /// <summary>
        /// 记录致命级错误
        /// </summary>
        /// <param name="info"></param>
        public static void WriteFatal(string info)
        {
            if (_logErrorEnable)
            {
                Logger.Fatal(BuildMessage(info));
            }
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="info"></param>
        public static void WriteInfo(string info)
        {
            if (_logInfoEnable)
            {
                Logger.Info(BuildMessage(info));
            }
        }
    }

}

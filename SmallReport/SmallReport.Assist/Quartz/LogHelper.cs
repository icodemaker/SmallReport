using log4net;
using System;
using System.Text;

namespace SmallReport.Assist.Quartz
{
    public static class LogHelper
    {
        #region Debug

        public static void Debug(Exception exception)
        {
            Debug(string.Empty, exception);
        }

        public static void Debug(string message, Exception exception = null)
        {
            Debug(message, exception, LogType);
        }

        private static void Debug(string message, Exception exception, LoggerType loggerType)
        {
            var logger = GetLogger(loggerType);

            if (!logger.IsDebugEnabled) return;

            if (null == exception)
                logger.Debug(message);
            else
                logger.Debug(message, exception);
        }

        #endregion

        #region Info

        public static void Info(Exception exception)
        {
            Info(string.Empty, exception);
        }

        public static void Info(string message, Exception exception = null)
        {
            Info(message, exception, LogType);
        }

        private static void Info(string message, Exception exception, LoggerType loggerType)
        {
            var logger = GetLogger(loggerType);

            if (!logger.IsInfoEnabled) return;

            if (null == exception)
                logger.Info(message);
            else
                logger.Info(message, exception);
        }

        #endregion

        #region Warn

        public static void Warn(Exception exception)
        {
            Warn(string.Empty, exception);
        }

        private static void Warn(string message, Exception exception = null)
        {
            Warn(message, exception, LogType);
        }

        public static void Error(object p, Exception ex)
        {
            throw new NotImplementedException();
        }

        private static void Warn(string message, Exception exception, LoggerType loggerType)
        {
            var logger = GetLogger(loggerType);

            if (!logger.IsWarnEnabled) return;

            if (null == exception)
                logger.Warn(message);
            else
                logger.Warn(message, exception);
        }

        #endregion

        #region Error

        public static void Error(Exception exception)
        {
            Error(string.Empty, exception);
        }

        public static void Error(string message, Exception exception = null)
        {
            Error(message, exception, LogType);
        }

        private static void Error(string message, Exception exception, LoggerType loggerType)
        {
            var logger = GetLogger(loggerType);

            if (!logger.IsErrorEnabled) return;

            if (null == exception)
                logger.Error(message);
            else
                logger.Error(message, exception);
        }

        #endregion

        #region Fata

        public static void Fata(Exception exception)
        {
            Fata(string.Empty, exception);
        }

        public static void Fata(string message, Exception exception = null)
        {
            Fata(message, exception, LogType);
        }

        private static void Fata(string message, Exception exception, LoggerType loggerType)
        {
            var logger = GetLogger(loggerType);

            if (!logger.IsFatalEnabled) return;

            if (null == exception)
                logger.Fatal(message);
            else
                logger.Fatal(message, exception);
        }

        #endregion

        #region
        private static ILog GetLogger(LoggerType loggerType)
        {
            var logger = LogManager.GetLogger(loggerType == LoggerType.SqlServer ? "SQLServerLogger" : "FileLogger");

            return logger;
        }

        private static string ExtractException(Exception exception)
        {
            if (exception == null) return string.Empty;

            var errorStr = new StringBuilder($"Message:{exception.Message} Source:{exception.Source} StackTrace:{exception.StackTrace}");
            if (exception.InnerException != null)
            {
                errorStr.AppendLine(ExtractException(exception.InnerException));
            }
            return errorStr.ToString();
        }

        private static LoggerType LogType => LoggerType.File;

        private enum LoggerType
        {
            File,
            SqlServer
        }
        #endregion
    }
}

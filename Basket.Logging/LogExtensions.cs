using Basket.Logging.Interface;
using System;

namespace Basket.Logging
{
    public static class LogExtensions
    {
        #region Public Methods

        public static void Log(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LogType.Info, message));
        }

        public static void Log(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LogType.Error, exception.Message, exception));
        }

        public static void Log(this ILogger logger, string message, Exception exception)
        {
            logger.Log(new LogEntry(LogType.Error, message, exception));
        }

        #endregion Public Methods
    }
}
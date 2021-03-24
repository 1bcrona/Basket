using NLog;
using ILogger = Basket.Logging.Interface.ILogger;

namespace Basket.Logging.Impl
{
    public class Logger : ILogger
    {
        #region Private Fields

        private static readonly NLog.Logger _Logger = LogManager.GetCurrentClassLogger();

        #endregion Private Fields

        #region Public Methods

        public void Log(LogEntry logEntry)
        {
            var level = LogLevel.Off;
            switch (logEntry.LogType)
            {
                case LogType.Trace:
                    level = LogLevel.Trace;
                    break;

                case LogType.Debug:
                    level = LogLevel.Debug;
                    break;

                case LogType.Info:
                    level = LogLevel.Info;
                    break;

                case LogType.Warn:
                    level = LogLevel.Warn;
                    break;

                case LogType.Error:
                    level = LogLevel.Error;
                    break;

                case LogType.Fatal:
                    level = LogLevel.Fatal;
                    break;
            }

            _Logger.Log(level, logEntry.Exception, logEntry.Message);
        }

        #endregion Public Methods
    }
}
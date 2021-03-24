using System;

namespace Basket.Logging
{
    public class LogEntry
    {
        #region Public Constructors

        public LogEntry(LogType logType, string message, Exception exception = null)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message == string.Empty) throw new ArgumentException("empty", "message");
            LogType = logType;
            Message = message;
            Exception = exception;
        }

        #endregion Public Constructors

        #region Public Properties

        public Exception Exception { get; }
        public LogType LogType { get; }
        public string Message { get; }

        #endregion Public Properties
    }
}
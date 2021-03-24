namespace Basket.Logging.Interface
{
    public interface ILogger
    {
        #region Public Methods

        void Log(LogEntry logEntry);

        #endregion Public Methods
    }
}
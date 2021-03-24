using Basket.Service.Exception.Base;
using System.Collections.Generic;

namespace Basket.Service.Exception.Impl
{
    public class InsufficientException : ServiceException
    {
        #region Public Constructors

        public InsufficientException(string argName) : this(argName, string.Empty, null)
        {
        }

        public InsufficientException(string argName, string additionalMessage, Dictionary<string, string> args) : this(
            $"{argName} is insufficient. {additionalMessage}", args)
        {
        }

        public InsufficientException(string message, Dictionary<string, string> args) : base(message, args)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public override string Name => "InsufficientException";
        public override string StatusCode => "500";

        #endregion Public Properties
    }
}
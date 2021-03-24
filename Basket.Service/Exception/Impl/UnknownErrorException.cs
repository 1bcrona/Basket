using Basket.Service.Exception.Base;
using System.Collections;
using System.Collections.Generic;

namespace Basket.Service.Exception.Impl
{
    public class UnknownErrorException : ServiceException
    {
        #region Private Fields

        private readonly System.Exception _Exception;

        #endregion Private Fields

        #region Public Constructors

        public UnknownErrorException(string message) : base(message)
        {
        }

        public UnknownErrorException(string message, Dictionary<string, string> args) : base(message, args)
        {
        }

        public UnknownErrorException(System.Exception ex)
        {
            _Exception = ex;
        }

        #endregion Public Constructors

        #region Public Properties

        public override IDictionary Data => _Exception?.Data ?? base.Data;
        public override string HelpLink => _Exception?.HelpLink ?? base.HelpLink;
        public override string Message => _Exception?.Message ?? base.Message;
        public override string Name => "UnknownErrorException";
        public override string Source => _Exception?.Source ?? base.Source;
        public override string StackTrace => _Exception?.StackTrace ?? base.StackTrace;
        public override string StatusCode => "500";

        #endregion Public Properties
    }
}
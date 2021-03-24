using Basket.Library.Exception;
using System.Collections.Generic;

namespace Basket.Service.Exception.Base
{
    public abstract class ServiceException : BaseException
    {
        #region Protected Constructors

        protected ServiceException()
        {
        }

        protected ServiceException(string message) : base(message)
        {
        }

        protected ServiceException(string message, Dictionary<string, string> args) : base(message, args)
        {
        }

        #endregion Protected Constructors

        #region Public Properties

        public abstract string StatusCode { get; }

        #endregion Public Properties
    }
}
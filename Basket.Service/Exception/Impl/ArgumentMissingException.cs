using Basket.Service.Exception.Base;

namespace Basket.Service.Exception.Impl
{
    public class ArgumentMissingException : ServiceException
    {
        #region Public Constructors

        public ArgumentMissingException(string argName) : base($"{argName} is missing")
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public override string Name => "ArgumentMissingException";
        public override string StatusCode => "500";

        #endregion Public Properties
    }
}
using Basket.Service.Exception.Base;

namespace Basket.Service.Exception.Impl
{
    public class NotFoundException : ServiceException
    {
        #region Public Constructors

        public NotFoundException(string argName) : base($"{argName} not found")
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public override string Name => "NotFoundException";
        public override string StatusCode => "404";

        #endregion Public Properties
    }
}
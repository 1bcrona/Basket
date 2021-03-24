using Basket.Service.Interception;
using Basket.Service.Interface;

namespace Basket.Service.Base
{
    public abstract class ServiceBase : IService
    {
        #region Public Methods

        public T CreateServiceProxy<T>() where T : class, IService
        {
            return ServiceFactory.CreateService<T>();
        }

        #endregion Public Methods
    }
}
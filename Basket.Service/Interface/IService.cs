namespace Basket.Service.Interface
{
    public interface IService
    {
        #region Public Methods

        public T CreateServiceProxy<T>() where T : class, IService;

        #endregion Public Methods
    }
}
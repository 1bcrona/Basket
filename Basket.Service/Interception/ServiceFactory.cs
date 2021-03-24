using Castle.DynamicProxy;
using Basket.IoC;
using Basket.Service.Interface;

namespace Basket.Service.Interception
{
    public static class ServiceFactory
    {
        #region Private Fields

        private static readonly IAsyncInterceptor s_Interceptor =
            ContainerHandler.Instance.Container.GetInstance<IAsyncInterceptor>();

        private static readonly ProxyGenerator s_ProxyGenerator = new ProxyGenerator();

        #endregion Private Fields

        #region Public Methods

        public static T CreateService<T>() where T : class, IService
        {
            return CreateService<T>(s_Interceptor);
        }

        public static T CreateService<T>(IAsyncInterceptor interceptor) where T : class, IService
        {
            var type = ContainerHandler.Instance.Container.GetInstance<T>();
            return s_ProxyGenerator.CreateInterfaceProxyWithTarget(type, interceptor);
        }

        #endregion Public Methods
    }
}
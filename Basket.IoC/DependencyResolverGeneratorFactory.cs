using Basket.IoC.Interface;

namespace Basket.IoC
{
    public class DependencyResolverGeneratorFactory<T> : IGeneratorFactory<T> where T : class
    {
        #region Public Methods

        public T Create()
        {
            return ContainerHandler.Instance.Container.GetInstance<T>();
        }

        #endregion Public Methods
    }
}
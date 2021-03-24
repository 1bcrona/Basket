using SimpleInjector;

namespace Basket.IoC
{
    public class ContainerChangedEventArgs
    {
        #region Public Properties

        public Container Container { get; set; }

        #endregion Public Properties
    }
}
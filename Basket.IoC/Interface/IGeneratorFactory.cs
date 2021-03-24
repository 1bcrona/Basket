namespace Basket.IoC.Interface
{
    public interface IGeneratorFactory<out T> where T : class
    {
        #region Public Methods

        T Create();

        #endregion Public Methods
    }
}
namespace Basket.Data.Interface
{
    public interface IEntity<TKey>
    {
        #region Public Properties

        TKey Id { get; set; }

        #endregion Public Properties
    }
}
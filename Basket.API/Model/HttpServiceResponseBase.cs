namespace Basket.API.Model
{
    public class HttpServiceResponseBase<TData>
    {
        #region Public Properties

        public TData Data { get; set; }
        public ErrorModel Error { get; set; }
        public bool HasError => Error != null;

        #endregion Public Properties
    }
}
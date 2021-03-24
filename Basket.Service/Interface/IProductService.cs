using Basket.Data.Impl;
using System.Threading.Tasks;

namespace Basket.Service.Interface
{
    public interface IProductService : IService
    {
        #region Public Methods

        public Task<Product> DecreaseProductQuantity(Product product);

        public Task<Product> Get(string id);

        #endregion Public Methods
    }
}
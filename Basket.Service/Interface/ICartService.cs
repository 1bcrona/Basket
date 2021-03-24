using Basket.Data.Impl;
using System.Threading.Tasks;

namespace Basket.Service.Interface
{
    public interface ICartService : IService
    {
        #region Public Methods

        public Task<Cart> AddProductToCart(string cartId, string productId);

        #endregion Public Methods
    }
}
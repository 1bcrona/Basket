using Basket.Data.Impl;
using Basket.Repository.Interface;
using Basket.Service.Base;
using Basket.Service.Exception.Impl;
using Basket.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basket.Service.Impl
{
    public class CartService : ServiceBase, ICartService
    {
        #region Public Fields

        public IProductService _ProductService;
        public IUnitOfWork _UnitOfWork;

        #endregion Public Fields

        #region Public Constructors

        public CartService(IUnitOfWork unitOfWork, IProductService productService)
        {
            _UnitOfWork = unitOfWork;
            _ProductService = productService;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<Cart> AddProductToCart(string cartId, string productId)
        {
            var productObjectId = (productId ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(productObjectId)) throw new ArgumentMissingException(nameof(productId));

            var repository = _UnitOfWork.GetRepository<Cart, string>();

            var cartObjectId = (cartId ?? string.Empty).Trim();

            var cart = string.IsNullOrEmpty(cartObjectId) ? new Cart() : await repository.Get(cartObjectId);
            if (cart == null) throw new NotFoundException(nameof(cart));

            var product = await _ProductService.Get(productId);

            if (product == null) throw new NotFoundException(nameof(product));

            if (product.Quantity < 1) throw new InsufficientException("Product Quantity");

            cart.ProductIds ??= new List<string>();
            cart.ProductIds.Add(product.Id);

            if (cart.Id == null)
            {
                await repository.Add(cart);
            }
            else
            {
                await repository.Update(cart);
            }

            await _ProductService.DecreaseProductQuantity(product);

            return cart;
        }

        #endregion Public Methods
    }
}
using Basket.Data.Impl;
using Basket.Repository.Interface;
using Basket.Service.Attribute;
using Basket.Service.Base;
using Basket.Service.Exception.Impl;
using Basket.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Basket.Service.Impl
{
    public class ProductService : ServiceBase, IProductService
    {
        #region Public Fields

        public IUnitOfWork _UnitOfWork;

        #endregion Public Fields

        #region Public Constructors

        public ProductService(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<Product> DecreaseProductQuantity(Product product)
        {
            if (product == null) throw new ArgumentMissingException(nameof(product));
            if (product.Id == null) throw new ArgumentMissingException(nameof(product.Id));

            var repository = _UnitOfWork.GetRepository<Product, string>();

            var existingProduct = await repository.Get(product.Id);

            if (existingProduct == null) throw new NotFoundException($"Product Id With {product.Id}");

            existingProduct.Quantity = Math.Max(0, existingProduct.Quantity - 1);

            return await repository.Update(existingProduct);
        }

        [Cache(300)]
        public async Task<Product> Get(string id)
        {
            var repository = _UnitOfWork.GetRepository<Product, string>();
            return await repository.Get(id);
        }

        #endregion Public Methods
    }
}
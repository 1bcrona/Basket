using Castle.DynamicProxy;
using Basket.Caching.Impl;
using Basket.Caching.Interface;
using Basket.Data.Impl;
using Basket.IoC;
using Basket.Repository.InMemory;
using Basket.Repository.InMemory.Interface;
using Basket.Repository.Interface;
using Basket.Service.Exception.Impl;
using Basket.Service.Impl;
using Basket.Service.Interception;
using Basket.Service.Interface;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.Test
{
    [TestFixture]
    public class DefaultTest
    {
        #region Public Methods

        [OneTimeSetUp]
        public void Setup()
        {
            ContainerHandler.Instance.Container.Options.DefaultLifestyle = Lifestyle.Scoped;
            ContainerHandler.Instance.Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            var interceptorDef = new DIDefinition
            {
                ServiceTypeObj = typeof(IAsyncInterceptor),
                ImplementationTypeObj = typeof(ServiceInterceptor),
                DIScope = DIDefinition.Scope.Singleton
            };

            ContainerHandler.Instance.SetupDependency(interceptorDef);

            var logDefinition = new DIDefinition
            {
                ServiceTypeObj = typeof(Logging.Interface.ILogger),
                ImplementationTypeObj = typeof(Logging.Impl.Logger),
                DIScope = DIDefinition.Scope.Singleton
            };

            ContainerHandler.Instance.SetupDependency(logDefinition);

            var cacheDefinition = new DIDefinition
            {
                ServiceTypeObj = typeof(ICache),
                ImplementationTypeObj = typeof(InMemoryCache),
                DIScope = DIDefinition.Scope.Singleton
            };

            ContainerHandler.Instance.SetupDependency(cacheDefinition);

            var unitofWorkDefintion = new DIDefinition
            {
                ServiceTypeObj = typeof(IUnitOfWork),
                ImplementationTypeObj = typeof(InMemoryUnitOfWork),
                DIScope = DIDefinition.Scope.Scoped
            };
            ContainerHandler.Instance.SetupDependency(unitofWorkDefintion);

            var dbContextDefinition = new DIDefinition
            {
                ServiceTypeObj = typeof(IInMemoryContext),
                ImplementationTypeObj = typeof(InMemoryContext),
                DIScope = DIDefinition.Scope.Scoped
            };
            ContainerHandler.Instance.SetupDependency(dbContextDefinition);

            var cartService = new DIDefinition
            {
                ServiceTypeObj = typeof(ICartService),
                ImplementationTypeObj = typeof(CartService),
                DIScope = DIDefinition.Scope.Scoped
            };
            ContainerHandler.Instance.SetupDependency(cartService);

            var productService = new DIDefinition
            {
                ServiceTypeObj = typeof(IProductService),
                ImplementationTypeObj = typeof(ProductService),
                DIScope = DIDefinition.Scope.Scoped
            };
            ContainerHandler.Instance.SetupDependency(productService);
            AsyncScopedLifestyle.BeginScope(ContainerHandler.Instance.Container);

            _ = InitializeDb();
        }

        [Test]
        public async Task Successfull_Add_ToExistingCart()
        {
            var unitOfWork = ContainerHandler.Instance.Container.GetInstance<IUnitOfWork>();
            var cartService = ServiceFactory.CreateService<ICartService>();

            var productRepository = unitOfWork.GetRepository<Product, string>();
            var cartRepository = unitOfWork.GetRepository<Cart, string>();

            var c = new Cart();
            var p = new Product { Name = "P_1", Description = "P_1", Price = 1, Quantity = 1 };

            var addedCart = await cartRepository.Add(c);

            Assert.NotNull(addedCart);
            Assert.NotNull(addedCart.Id);

            var added = await productRepository.Add(p);

            Assert.NotNull(added);
            Assert.NotNull(added.Id);
            Assert.NotNull(added.Name);
            Assert.NotNull(added.Quantity);
            Assert.NotNull(added.Price);

            Assert.AreEqual(added, p);

            var cart = await cartService.AddProductToCart(addedCart.Id, added.Id);

            Assert.NotNull(cart);
            Assert.NotNull(cart.Id);
            Assert.NotNull(cart.ProductIds);

            var dbCart = await cartRepository.Get(cart.Id);
            Assert.NotNull(dbCart);
            Assert.NotNull(dbCart.Id);
            Assert.NotNull(dbCart.ProductIds);

            Assert.AreEqual(cart.Id, dbCart.Id);
            Assert.True(cart.ProductIds.All(dbCart.ProductIds.Contains));

            var updatedProduct = await productRepository.Get(p.Id);

            Assert.NotNull(updatedProduct);
            Assert.NotNull(updatedProduct.Id);
            Assert.NotNull(updatedProduct.Name);
            Assert.NotNull(updatedProduct.Quantity);
            Assert.NotNull(updatedProduct.Description);
            Assert.NotNull(updatedProduct.Price);

            Assert.AreEqual(updatedProduct.Id, p.Id);
            Assert.AreEqual(updatedProduct.Name, p.Name);
            Assert.AreEqual(updatedProduct.Description, p.Description);
            Assert.AreEqual(updatedProduct.Price, p.Price);
            Assert.AreEqual(updatedProduct.Quantity, p.Quantity - 1);
        }

        [Test]
        public async Task Successfull_Add_ToNewCart()
        {
            var unitOfWork = ContainerHandler.Instance.Container.GetInstance<IUnitOfWork>();
            var cartService = ServiceFactory.CreateService<ICartService>();

            var productRepository = unitOfWork.GetRepository<Product, string>();
            var cartRepository = unitOfWork.GetRepository<Cart, string>();

            var p = new Product { Name = "P_1", Description = "P_1", Price = 1, Quantity = 1 };

            var added = await productRepository.Add(p);

            Assert.NotNull(added);
            Assert.NotNull(added.Id);
            Assert.NotNull(added.Name);
            Assert.NotNull(added.Quantity);
            Assert.NotNull(added.Price);

            Assert.AreEqual(added, p);

            var cart = await cartService.AddProductToCart(null, added.Id);

            Assert.NotNull(cart);
            Assert.NotNull(cart.Id);
            Assert.NotNull(cart.ProductIds);

            var dbCart = await cartRepository.Get(cart.Id);
            Assert.NotNull(dbCart);
            Assert.NotNull(dbCart.Id);
            Assert.NotNull(dbCart.ProductIds);

            Assert.AreEqual(cart.Id, dbCart.Id);
            Assert.True(cart.ProductIds.All(dbCart.ProductIds.Contains));

            var updatedProduct = await productRepository.Get(p.Id);

            Assert.NotNull(updatedProduct);
            Assert.NotNull(updatedProduct.Id);
            Assert.NotNull(updatedProduct.Name);
            Assert.NotNull(updatedProduct.Quantity);
            Assert.NotNull(updatedProduct.Description);
            Assert.NotNull(updatedProduct.Price);

            Assert.AreEqual(updatedProduct.Id, p.Id);
            Assert.AreEqual(updatedProduct.Name, p.Name);
            Assert.AreEqual(updatedProduct.Description, p.Description);
            Assert.AreEqual(updatedProduct.Price, p.Price);
            Assert.AreEqual(updatedProduct.Quantity, p.Quantity - 1);
        }

        [Test]
        public void Throws_Exception_If_Cart_NotFound()
        {
            var cartService = ServiceFactory.CreateService<ICartService>();

            Assert.ThrowsAsync<NotFoundException>(async () => await cartService.AddProductToCart("TEST_CART", "TEST_PRODUCT_ID"));
        }

        [Test]
        public void Throws_Exception_If_Product_NotFound()
        {
            var cartService = ServiceFactory.CreateService<ICartService>();

            Assert.ThrowsAsync<NotFoundException>(async () => await cartService.AddProductToCart(null, "TEST_PRODUCT_ID"));
        }

        [Test]
        public void Throws_Exception_If_Product_Null()
        {
            var cartService = ServiceFactory.CreateService<ICartService>();

            Assert.ThrowsAsync<ArgumentMissingException>(async () => await cartService.AddProductToCart(null, null));
        }

        [Test]
        public async Task Throws_Insufficient_Exception()
        {
            var unitOfWork = ContainerHandler.Instance.Container.GetInstance<IUnitOfWork>();
            var cartService = ServiceFactory.CreateService<ICartService>();

            var productRepository = unitOfWork.GetRepository<Product, string>();
            var cartRepository = unitOfWork.GetRepository<Cart, string>();

            var p = new Product { Name = "P_1", Description = "P_1", Price = 1, Quantity = 1 };

            var added = await productRepository.Add(p);

            Assert.NotNull(added);
            Assert.NotNull(added.Id);
            Assert.NotNull(added.Name);
            Assert.NotNull(added.Quantity);
            Assert.NotNull(added.Price);

            Assert.AreEqual(added, p);

            var cart = await cartService.AddProductToCart(null, added.Id);

            Assert.NotNull(cart);
            Assert.NotNull(cart.Id);
            Assert.NotNull(cart.ProductIds);

            var dbCart = await cartRepository.Get(cart.Id);
            Assert.NotNull(dbCart);
            Assert.NotNull(dbCart.Id);
            Assert.NotNull(dbCart.ProductIds);

            var updatedProduct = await productRepository.Get(p.Id);

            Assert.NotNull(updatedProduct);
            Assert.NotNull(updatedProduct.Id);
            Assert.NotNull(updatedProduct.Name);
            Assert.NotNull(updatedProduct.Quantity);
            Assert.NotNull(updatedProduct.Description);
            Assert.NotNull(updatedProduct.Price);

            Assert.AreEqual(updatedProduct.Id, p.Id);
            Assert.AreEqual(updatedProduct.Name, p.Name);
            Assert.AreEqual(updatedProduct.Description, p.Description);
            Assert.AreEqual(updatedProduct.Price, p.Price);
            Assert.AreEqual(updatedProduct.Quantity, p.Quantity - 1);

            Assert.ThrowsAsync<InsufficientException>(async () => await cartService.AddProductToCart(null, p.Id));
        }

        #endregion Public Methods

        #region Private Methods

        private async Task InitializeDb()
        {
            var unitOfWork = ContainerHandler.Instance.Container.GetInstance<IUnitOfWork>();
            var productRepository = unitOfWork.GetRepository<Product, string>();
            var cartRepository = unitOfWork.GetRepository<Cart, string>();

            await productRepository.DeleteAll();
            await cartRepository.DeleteAll();
        }

        #endregion Private Methods
    }
}
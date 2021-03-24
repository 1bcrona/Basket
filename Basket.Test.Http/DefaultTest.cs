using Basket.API;
using Basket.API.Model;
using Basket.Data.Impl;
using Basket.IoC;
using Basket.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleInjector.Lifestyles;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Test.Http
{
    [TestFixture]
    public class DefaultTest : IDisposable
    {
        #region Private Fields

        private HttpClient _HttpClient;
        private TestServer _TestServer;

        #endregion Private Fields

        #region Private Destructors

        ~DefaultTest()
        {
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _TestServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _HttpClient = _TestServer.CreateClient();
            _ = InitializeDb();
        }

        [Test]
        public async Task Successfull_Add_ToExistingCart()
        {
            await using (AsyncScopedLifestyle.BeginScope(ContainerHandler.Instance.Container))
            {
                var unitOfWork = ContainerHandler.Instance.Container.GetInstance<IUnitOfWork>();
                var productRepository = unitOfWork.GetRepository<Product, string>();
                var cartRepository = unitOfWork.GetRepository<Cart, string>();
                var c = new Cart();
                var p = new Product { Name = "P_1", Description = "P_1", Price = 1, Quantity = 1 };

                var addedCart = await cartRepository.Add(c);

                Assert.NotNull(addedCart);
                Assert.NotNull(addedCart.Id);

                var addedProduct = await productRepository.Add(p);

                Assert.NotNull(addedProduct);
                Assert.NotNull(addedProduct.Id);
                Assert.NotNull(addedProduct.Name);
                Assert.NotNull(addedProduct.Quantity);
                Assert.NotNull(addedProduct.Price);

                Assert.AreEqual(addedProduct, p);

                var model = new AddToCartModel { CartId = addedCart.Id, ProductId = addedProduct.Id };

                var response = await _HttpClient.PostAsync($"/api/cart/",
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                var cartId = JsonConvert.DeserializeObject<HttpServiceResponseBase<string>>(await response.Content.ReadAsStringAsync()).Data;

                Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
                Assert.NotNull(cartId);
                Assert.IsNotEmpty(cartId);
            }
        }

        [Test]
        public async Task Successfull_Add_ToNewCart()
        {
            await using (AsyncScopedLifestyle.BeginScope(ContainerHandler.Instance.Container))
            {
                var unitOfWork = ContainerHandler.Instance.Container.GetInstance<IUnitOfWork>();
                var productRepository = unitOfWork.GetRepository<Product, string>();

                var p = new Product { Name = "P_1", Description = "P_1", Price = 1, Quantity = 1 };

                var addedProduct = await productRepository.Add(p);

                Assert.NotNull(addedProduct);
                Assert.NotNull(addedProduct.Id);
                Assert.NotNull(addedProduct.Name);
                Assert.NotNull(addedProduct.Quantity);
                Assert.NotNull(addedProduct.Price);

                Assert.AreEqual(addedProduct, p);

                var model = new AddToCartModel { CartId = null, ProductId = addedProduct.Id };

                var response = await _HttpClient.PostAsync($"/api/cart/",
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                var cartId = JsonConvert.DeserializeObject<HttpServiceResponseBase<string>>(await response.Content.ReadAsStringAsync()).Data;

                Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
                Assert.NotNull(cartId);
                Assert.IsNotEmpty(cartId);
            }
        }

        [Test]
        public async Task Throws_Exception_If_Cart_NotFound()
        {
            await using (AsyncScopedLifestyle.BeginScope(ContainerHandler.Instance.Container))
            {
                var model = new AddToCartModel { CartId = "TEST_CART", ProductId = "TEST_PRODUCT" };

                var invalidResponse = await _HttpClient.PostAsync($"/api/cart/",
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                var invalidResponseString =
                    JsonConvert.DeserializeObject<HttpServiceResponseBase<string>>(
                        await invalidResponse.Content.ReadAsStringAsync());

                Assert.AreEqual(invalidResponse.StatusCode, HttpStatusCode.OK);
                Assert.NotNull(invalidResponseString.Error);
                Assert.AreEqual(invalidResponseString.Error.StatusCode, "404");
                Assert.AreEqual(invalidResponseString.Error.ErrorCode, "NotFoundException");
            }
        }

        [Test]
        public async Task Throws_Exception_If_Product_NotFound()
        {
            await using (AsyncScopedLifestyle.BeginScope(ContainerHandler.Instance.Container))
            {
                var model = new AddToCartModel { CartId = null, ProductId = "TEST_PRODUCT" };

                var invalidResponse = await _HttpClient.PostAsync($"/api/cart/",
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                var invalidResponseString =
                    JsonConvert.DeserializeObject<HttpServiceResponseBase<string>>(
                        await invalidResponse.Content.ReadAsStringAsync());

                Assert.AreEqual(invalidResponse.StatusCode, HttpStatusCode.OK);
                Assert.NotNull(invalidResponseString.Error);
                Assert.AreEqual(invalidResponseString.Error.StatusCode, "404");
                Assert.AreEqual(invalidResponseString.Error.ErrorCode, "NotFoundException");
            }
        }

        [Test]
        public async Task Throws_Exception_If_Product_Null()
        {
            await using (AsyncScopedLifestyle.BeginScope(ContainerHandler.Instance.Container))
            {
                var model = new AddToCartModel { CartId = null, ProductId = null };

                var invalidResponse = await _HttpClient.PostAsync($"/api/cart/",
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                var responseString = await invalidResponse.Content.ReadAsStringAsync();
                Assert.AreEqual(invalidResponse.StatusCode, HttpStatusCode.BadRequest);
                Assert.AreEqual(responseString, "MODEL_PRODUCT_ID_IS_NULL");
            }
        }

        [Test]
        public async Task Throws_Insufficient_Exception()
        {
            await using (AsyncScopedLifestyle.BeginScope(ContainerHandler.Instance.Container))
            {
                var unitOfWork = ContainerHandler.Instance.Container.GetInstance<IUnitOfWork>();

                var productRepository = unitOfWork.GetRepository<Product, string>();

                var p = new Product { Name = "P_1", Description = "P_1", Price = 1, Quantity = 1 };

                var addedProduct = await productRepository.Add(p);

                Assert.NotNull(addedProduct);
                Assert.NotNull(addedProduct.Id);
                Assert.NotNull(addedProduct.Name);
                Assert.NotNull(addedProduct.Quantity);
                Assert.NotNull(addedProduct.Price);

                Assert.AreEqual(addedProduct, p);

                var model = new AddToCartModel { CartId = null, ProductId = addedProduct.Id };

                var validResponse = await _HttpClient.PostAsync($"/api/cart/", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                var cartId = JsonConvert.DeserializeObject<HttpServiceResponseBase<string>>(await validResponse.Content.ReadAsStringAsync()).Data;

                Assert.AreEqual(validResponse.StatusCode, HttpStatusCode.OK);
                Assert.NotNull(cartId);
                Assert.IsNotEmpty(cartId);

                var invalidResponse = await _HttpClient.PostAsync($"/api/cart/",
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                var invalidResponseString =
                    JsonConvert.DeserializeObject<HttpServiceResponseBase<string>>(
                        await invalidResponse.Content.ReadAsStringAsync());

                Assert.AreEqual(invalidResponse.StatusCode, HttpStatusCode.OK);
                Assert.NotNull(invalidResponseString.Error);
                Assert.AreEqual(invalidResponseString.Error.StatusCode, "500");
                Assert.AreEqual(invalidResponseString.Error.ErrorCode, "InsufficientException");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _HttpClient?.Dispose();
                _TestServer?.Dispose();
            }
        }

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
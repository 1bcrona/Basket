using Castle.DynamicProxy;
using Basket.Caching.Impl;
using Basket.Caching.Interface;
using Basket.IoC;
using Basket.Logging.Impl;
using Basket.Logging.Interface;
using Basket.Repository.Interface;
using Basket.Repository.Mongo;
using Basket.Repository.Mongo.Interface;
using Basket.Service.Impl;
using Basket.Service.Interception;
using Basket.Service.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Basket.API
{
    public class Startup
    {
        #region Public Constructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion Public Constructors

        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion Public Properties

        #region Public Methods

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSimpleInjector(ContainerHandler.Instance.Container);
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            ContainerHandler.Instance.Container.Verify();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            InitializeContainer();
            services.AddScoped(serviceProvider => ServiceFactory.CreateService<IProductService>());
            services.AddScoped(serviceProvider => ServiceFactory.CreateService<ICartService>());
            services.AddControllers();

            IntegrateSimpleInjector(services);

        }

        #endregion Public Methods

        #region Private Methods

        private void InitializeContainer()
        {
            ContainerHandler.Instance.Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            var interceptorDef = new DIDefinition
            {
                ServiceTypeObj = typeof(IAsyncInterceptor),
                ImplementationTypeObj = typeof(ServiceInterceptor)
            };

            ContainerHandler.Instance.SetupDependency(interceptorDef);

            var logDefinition = new DIDefinition
            {
                ServiceTypeObj = typeof(ILogger),
                ImplementationTypeObj = typeof(Logger),
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
                ImplementationTypeObj = typeof(MongoUnitOfWork),
                DIScope = DIDefinition.Scope.Scoped
            };
            ContainerHandler.Instance.SetupDependency(unitofWorkDefintion);

            var dbContextDefinition = new DIDefinition
            {
                ServiceTypeObj = typeof(IMongoContext),
                ImplementationTypeObj = typeof(MongoContext),
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
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            services.AddSimpleInjector(ContainerHandler.Instance.Container);
            services.UseSimpleInjectorAspNetRequestScoping(ContainerHandler.Instance.Container);
        }

        #endregion Private Methods
    }
}
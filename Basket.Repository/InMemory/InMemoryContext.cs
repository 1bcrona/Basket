using Basket.Data.Interface;
using Basket.Repository.Attribute;
using Basket.Repository.InMemory.Interface;
using Basket.Repository.Interface;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Basket.Repository.InMemory
{
    public class InMemoryContext : IInMemoryContext
    {
        #region Private Destructors

        ~InMemoryContext()
        {
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Methods

        public async Task Connect()
        {
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<ICollection<T, TKey>> GetCollection<T, TKey>() where T : IEntity<TKey>
        {
            var collection = GetCollectionName<T>();

            return await Task.FromResult(InMemoryCollectionHelper.GetCollection<T, TKey>(collection));
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion Protected Methods

        #region Private Methods

        private string GetCollectionName<T>()
        {
            var type = typeof(T).GetTypeInfo();

            var attr = type.GetCustomAttribute<CollectionName>();

            return attr == null ? type.Name.ToLowerInvariant() : attr.Name;
        }

        #endregion Private Methods
    }
}
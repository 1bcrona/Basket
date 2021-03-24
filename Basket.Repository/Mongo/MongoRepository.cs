using Basket.Data.Interface;
using Basket.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Basket.Repository.Mongo
{
    public class MongoRepository<T, TKey> : IRepository<T, TKey> where T : IEntity<TKey>
    {
        #region Private Fields

        private readonly IDbContext _DbContext;

        #endregion Private Fields

        #region Public Constructors

        public MongoRepository(IDbContext dbContext)
        {
            _DbContext = dbContext;
            _ = GetCollection().ConfigureAwait(false);
        }

        #endregion Public Constructors

        #region Public Properties

        public ICollection<T, TKey> Collection { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<T> Add(T entity)
        {
            await Collection.Insert(entity);
            return entity;
        }

        public async Task Add(IEnumerable<T> entities)
        {
            await Collection.Insert(entities);
        }

        public async Task<long> Count()
        {
            return await Collection.Count();
        }

        public async Task Delete(Expression<Func<T, bool>> predicate)
        {
            await Collection.DeleteAll(predicate);
        }

        public async Task Delete(T entity)
        {
            await Collection.Delete(entity);
        }

        public async Task Delete(TKey id)
        {
            await Collection.Delete(id);
        }

        public async Task DeleteAll()
        {
            await Collection.DeleteAll(x => true);
        }

        public async Task<bool> Exists(T entity)
        {
            return await Exists(entity.Id);
        }

        public async Task<bool> Exists(TKey entityId)
        {
            var product = await Collection.Get(entityId);
            return product != null;
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate,
                            Expression<Func<T, object>> sortBy = null)
        {
            return await Collection.GetAll(predicate);
        }

        public async Task<T> Get(TKey id)
        {
            return await Collection.Get(id);
        }

        public async Task<T> Update(T entity)
        {
            await Collection.Update(entity);
            return entity;
        }

        #endregion Public Methods

        #region Private Methods

        private async Task GetCollection()
        {
            Collection = await _DbContext.GetCollection<T, TKey>();
        }

        #endregion Private Methods
    }
}
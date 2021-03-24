using Basket.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Basket.Repository.Interface
{
    public interface IRepository<T, in TKey> where T : IEntity<TKey>
    {
        #region Public Methods

        public Task<T> Add(T entity);

        public Task Add(IEnumerable<T> entities);

        public Task<long> Count();

        public Task Delete(TKey id);

        public Task Delete(T entity);

        public Task Delete(Expression<Func<T, bool>> predicate);

        public Task DeleteAll();

        public Task<bool> Exists(T entity);

        public Task<bool> Exists(TKey entityId);

        public Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate,
                            Expression<Func<T, object>> sortBy = null);

        public Task<T> Get(TKey id);

        public Task<T> Update(T entity);

        #endregion Public Methods
    }
}
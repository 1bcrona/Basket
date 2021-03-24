using Basket.Data.Interface;
using System;

namespace Basket.Repository.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        #region Public Methods

        public IRepository<T, TKey> GetRepository<T, TKey>() where T : IEntity<TKey>;

        #endregion Public Methods
    }
}
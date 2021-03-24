using Basket.Data.Interface;
using Basket.Repository.Interface;
using Basket.Repository.Mongo.Interface;
using System;

namespace Basket.Repository.Mongo
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        #region Private Fields

        private readonly IDbContext _Context;

        #endregion Private Fields

        #region Public Constructors

        public MongoUnitOfWork(IMongoContext context)
        {
            _Context = context;
        }

        #endregion Public Constructors

        #region Private Destructors

        ~MongoUnitOfWork()
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

        public IRepository<T, TKey> GetRepository<T, TKey>() where T : IEntity<TKey>
        {
            return new MongoRepository<T, TKey>(_Context);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Context?.Dispose();
            }
        }

        #endregion Protected Methods
    }
}
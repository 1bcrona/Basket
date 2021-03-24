using Basket.Data.Interface;
using System;
using System.Threading.Tasks;

namespace Basket.Repository.Interface
{
    public interface IDbContext : IDisposable
    {
        #region Public Methods

        public Task Connect();

        public Task<ICollection<T, TKey>> GetCollection<T, TKey>() where T : IEntity<TKey>;

        #endregion Public Methods
    }
}
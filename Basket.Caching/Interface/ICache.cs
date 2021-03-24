using System;
using System.Threading.Tasks;

namespace Basket.Caching.Interface
{
    public interface ICache : IDisposable
    {
        #region Public Methods

        Task<bool> Delete(String key);

        Task<bool> Exists(String key);

        Task<bool> Expire(String key, TimeSpan expiry);

        Task Flush();

        Task<T> Get<T>(String key, T defaultValue = default(T));

        Task<bool> Persist(String key);

        Task<bool> Set(String key, object value, TimeSpan? expiry = null);

        #endregion Public Methods
    }
}
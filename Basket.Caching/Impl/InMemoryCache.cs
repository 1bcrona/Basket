using Basket.Caching.Interface;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Basket.Caching.Impl
{
    public class InMemoryCache : ICache
    {
        #region Private Fields

        private const string DEFAULT_KEY_VALUE_STORE_NAME = "__kv_default__";

        private ConcurrentDictionary<string, KeyValueStoreEntry> _Cache = new ConcurrentDictionary<string, KeyValueStoreEntry>();

        #endregion Private Fields

        #region Public Constructors

        public InMemoryCache()
        {
            this.Name = DEFAULT_KEY_VALUE_STORE_NAME;
        }

        #endregion Public Constructors

        #region Private Destructors

        ~InMemoryCache()
        {
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Properties

        public string Name
        {
            get => _Name;
            set => _Name = value;
        }

        #endregion Public Properties

        #region İnternal Properties

        internal string _Name { get; private set; }

        #endregion İnternal Properties

        #region Public Methods

        public Task Connect()
        {
            return Task.FromResult(true);
        }

        public async Task<bool> Delete(string key)
        {
            return await Task.FromResult(_Cache.TryRemove(key, out _));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<bool> Exists(string key)
        {
            return await Task.FromResult(GetEntry(key) != null);
        }

        public async Task<bool> Expire(string key, TimeSpan expiry)
        {
            var entry = GetEntry(key);

            if (entry == null) return await Task.FromResult(false);

            entry.Key = key;
            entry.CreationDate = DateTime.UtcNow;
            entry.ExpireDate = entry.CreationDate.Add(expiry);
            _Cache[key] = entry;
            return await Task.FromResult(true);
        }

        public Task Flush()
        {
            _Cache.Clear();

            return Task.FromResult(true);
        }

        public async Task<T> Get<T>(string key, T defaultValue = default)
        {
            var entry = GetEntry(key);
            return await Task.FromResult(CastValue(entry, defaultValue));
        }

        public async Task<bool> Persist(string key)
        {
            var entry = GetEntry(key, false);

            if (entry == null) return await Task.FromResult(false);

            entry.ExpireDate = DateTime.MaxValue;
            _Cache[key] = entry;

            return await Task.FromResult(true);
        }

        public async Task<bool> Set(string key, object value, TimeSpan? expiry = null)
        {
            var entry = GetEntry(key);

            var creationDate = DateTime.UtcNow;

            entry ??= new KeyValueStoreEntry();
            entry.Key = key;
            entry.Item = value;
            entry.CreationDate = creationDate;
            entry.ExpireDate = expiry.HasValue ? creationDate.Add(expiry.Value) : DateTime.MaxValue;

            _Cache.AddOrUpdate(key, entry, (s, oldValue) => entry);

            return await Task.FromResult(true);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var dict = Interlocked.Exchange(ref _Cache, null);
                dict?.Clear();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private T CastValue<T>(KeyValueStoreEntry entry, T defaultValue = default(T))
        {
            if (entry == null)
            {
                return defaultValue;
            }

            return CastValue(entry.Key, defaultValue);
        }

        private T CastValue<T>(string key, T defaultValue = default(T))
        {
            var type = typeof(T);
            if (_Cache.TryGetValue(key, out var existingEntry))
            {
                var value = existingEntry.Item;

                try
                {
                    if (type.IsSerializable)
                    {
                        return (T)Convert.ChangeType(value, type);
                    }
                    else
                    {
                        var js = JsonSerializer.Serialize(value);
                        return JsonSerializer.Deserialize<T>(js);
                    }
                }
                catch (Exception)
                {
                    return default(T);
                }
            }

            return defaultValue;
        }

        private KeyValueStoreEntry GetEntry(string key, bool deleteIfExpire = true)
        {
            KeyValueStoreEntry rv = null;
            try
            {
                if (_Cache.TryGetValue(key, out rv))
                {
                    if (deleteIfExpire)
                    {
                        if (DateTime.UtcNow > rv.ExpireDate)
                        {
                            _Cache.TryRemove(key, out rv);
                            return null;
                        }
                    }
                    return rv;
                }
            }
            catch (Exception) { }

            return rv;
        }

        #endregion Private Methods
    }
}
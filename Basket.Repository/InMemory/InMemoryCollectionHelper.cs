using Basket.Data.Interface;
using Basket.Repository.InMemory.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Basket.Repository.InMemory
{
    public static class InMemoryCollectionHelper
    {
        #region Private Fields

        private static readonly ConcurrentDictionary<string, IInMemoryCollection> _Collection = new ConcurrentDictionary<string, IInMemoryCollection>();
        private static readonly JsonSerializerSettings _Settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        #endregion Private Fields

        #region Public Methods

        public static T DeepCopy<T>(this T original)
        {
            var js = JsonConvert.SerializeObject(original, Formatting.None, _Settings);
            return JsonConvert.DeserializeObject<T>(js, _Settings);
        }

        public static InMemoryCollection<T, TKey> GetCollection<T, TKey>(string name) where T : IEntity<TKey>
        {
            if (!_Collection.ContainsKey(name))
            {
                _Collection[name] = new InMemoryCollection<T, TKey>(name);
            }

            return (InMemoryCollection<T, TKey>)_Collection[name];
        }

        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary)
                .Remove(new KeyValuePair<TKey, TValue>(key, value));
        }

        #endregion Public Methods
    }
}
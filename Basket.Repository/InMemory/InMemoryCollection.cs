using Basket.Data.Interface;
using Basket.Repository.InMemory.Interface;
using Basket.Repository.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Basket.Repository.InMemory
{
    public class InMemoryCollection<T, TKey> : ICollection<T, TKey>, IInMemoryCollection where T : IEntity<TKey>
    {
        #region Private Fields

        private ConcurrentDictionary<TKey, T> _ConcurrentDictionary = new ConcurrentDictionary<TKey, T>();

        #endregion Private Fields

        #region Public Constructors

        public InMemoryCollection(string name)
        {
            this._Name = name;
        }

        #endregion Public Constructors

        #region Private Destructors

        ~InMemoryCollection()
        {
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Properties

        public string _Name { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task<long> Count()
        {
            return await Task.FromResult(_ConcurrentDictionary.Count);
        }

        public async Task<long> Count(Expression<Func<T, bool>> filter)
        {
            var func = filter.Compile();
            return await Task.FromResult(_ConcurrentDictionary.Values.Count(func));
        }

        public async Task<bool> Delete(T document)
        {
            return await Task.FromResult(_ConcurrentDictionary.TryRemove(document.Id, out _));
        }

        public async Task<bool> Delete(TKey id)
        {
            return await Task.FromResult(_ConcurrentDictionary.TryRemove(id, out _));
        }

        public async Task<long> DeleteAll(Expression<Func<T, bool>> filter)
        {
            var func = filter.Compile();
            var elements = _ConcurrentDictionary.Values.Where(func);
            var count = elements.LongCount(el => _ConcurrentDictionary.TryRemove(el.Id, out _));

            return await Task.FromResult(count);
        }

        public async Task<long> DeleteAll()
        {
            var docCount = _ConcurrentDictionary.Count;
            _ConcurrentDictionary.Clear();
            return await Task.FromResult((long)docCount);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<T> Get(TKey id)
        {
            _ConcurrentDictionary.TryGetValue(id, out var record);
            return await Task.FromResult(record);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            var func = filter.Compile();
            return await Task.FromResult(_ConcurrentDictionary.Values.FirstOrDefault(func));
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await Task.FromResult(_ConcurrentDictionary.Values.AsEnumerable());
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter)
        {
            var func = filter.Compile();
            return await Task.FromResult(_ConcurrentDictionary.Values.Where(func));
        }

        public Task Insert(T document)
        {
            if (!CheckIdentifier(document)) throw new NullReferenceException("document.Id");

            return Task.FromResult(_ConcurrentDictionary.TryAdd(document.Id, document.DeepCopy()));
        }

        public Task Insert(IEnumerable<T> documents)
        {
            var enumerable = documents.ToList();
            if (documents == null || !enumerable.Any()) throw new ArgumentNullException(nameof(documents));

            foreach (var document in enumerable)
            {
                if (!CheckIdentifier(document)) throw new NullReferenceException("document.Id");

                _ConcurrentDictionary.TryAdd(document.Id, document.DeepCopy());
            }

            return Task.FromResult(true);
        }

        public Task<bool> Update(T document)
        {
            return Task.FromResult(_ConcurrentDictionary.TryGetValue(document.Id, out var old) && _ConcurrentDictionary.TryUpdate(document.Id, document.DeepCopy(), old));
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var dict = Interlocked.Exchange(ref _ConcurrentDictionary, null);
                dict?.Clear();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private bool CheckIdentifier(T document)
        {
            var identifierValue = document.Id;

            if (identifierValue != null) return true;

            var typeOfId = document.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);

            if (typeOfId?.PropertyType != typeof(string)) return false;

            document.Id = (TKey)(object)Guid.NewGuid().ToString();
            return true;
        }

        #endregion Private Methods
    }
}
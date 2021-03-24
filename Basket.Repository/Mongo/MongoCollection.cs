using Basket.Data.Interface;
using Basket.Repository.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Basket.Repository.Mongo
{
    public class MongoCollection<T, TKey> : ICollection<T, TKey> where T : IEntity<TKey>
    {
        #region Private Fields

        private IMongoCollection<T> _Collection;

        #endregion Private Fields

        #region Public Constructors

        public MongoCollection(IMongoCollection<T> collection)
        {
            _Collection = collection;
        }

        #endregion Public Constructors

        #region Private Destructors

        ~MongoCollection()
        {
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Methods

        public async Task<long> Count()
        {
            var count = await Count(f => true);
            return count;
        }

        public async Task<long> Count(Expression<Func<T, bool>> filter)
        {
            var count = await _Collection.CountDocumentsAsync(filter);
            return count;
        }

        public async Task<bool> Delete(T document)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, document.Id);

            var result = await _Collection.DeleteOneAsync(filter);
            return result.DeletedCount == 1;
        }

        public async Task<bool> Delete(TKey id)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);

            var result = await _Collection.DeleteOneAsync(filter);
            return result.DeletedCount == 1;
        }

        public async Task<long> DeleteAll(Expression<Func<T, bool>> filter)
        {
            var result = await _Collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteAll()
        {
            var result = await _Collection.DeleteManyAsync(Builders<T>.Filter.Empty);
            return result.DeletedCount;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<T> Get(TKey id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await _Collection.FindAsync(filter);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            var result = await _Collection.FindAsync<T>(filter);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var result = await _Collection.FindAsync(new BsonDocument());
            return result.ToList();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter)
        {
            var result = await _Collection.FindAsync(filter);
            return result.ToList();
        }

        public async Task Insert(T document)
        {
            if (!CheckIdentifier(document)) throw new Exception("ID_IS_NULL");
            await _Collection.InsertOneAsync(document);
        }

        public async Task Insert(IEnumerable<T> documents)
        {
            var enumerable = documents.ToList();
            if (enumerable.All(CheckIdentifier)) await _Collection.InsertManyAsync(enumerable);
            throw new Exception("ID_IS_NULL");
        }

        public async Task<bool> Update(T document)
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, document.Id);
            var result = await _Collection.ReplaceOneAsync(filter, document);
            return result.ModifiedCount == 1;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Collection = null;
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

            var objectId = ObjectId.GenerateNewId();

            document.Id = (TKey)(object)objectId.ToString();
            return true;
        }

        #endregion Private Methods
    }
}
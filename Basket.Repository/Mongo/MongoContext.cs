using Basket.Data.Interface;
using Basket.Repository.Attribute;
using Basket.Repository.Interface;
using Basket.Repository.Mongo.Interface;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Basket.Library;

namespace Basket.Repository.Mongo
{
    public class MongoContext : IMongoContext
    {
        #region Private Fields

        private readonly string _ConnectionString;
        private readonly string _DatabaseName;
        private MongoClient _Client;
        private IMongoDatabase _Database;

        #endregion Private Fields

        #region Public Constructors

        public MongoContext()
        {
            var url = new MongoUrl(ConfigurationHelper.Configuration.GetConnectionString("DefaultConnection"));
            _ConnectionString = url.Url;
            _DatabaseName = url.DatabaseName;
        }

        #endregion Public Constructors

        #region Private Destructors

        ~MongoContext()
        {
            ReleaseUnmanagedResources();
        }

        #endregion Private Destructors

        #region Public Properties

        public bool Open => _Client != null;

        #endregion Public Properties

        #region Public Methods

        public async Task Connect()
        {
            _Client = new MongoClient(_ConnectionString);
            _Database = _Client.GetDatabase(_DatabaseName);
            await Task.CompletedTask;
        }

        public async Task<ICollection<T, TKey>> GetCollection<T, TKey>() where T : IEntity<TKey>
        {
            if (!Open) await Connect().ConfigureAwait(false);

            RegisterUniqueKey<T, TKey>();

            var collection = _Database.GetCollection<T>(GetCollectionName<T>());

            return new MongoCollection<T, TKey>(collection);
        }

        #endregion Public Methods



        #region Private Methods

        private string GetCollectionName<T>()
        {
            var type = typeof(T).GetTypeInfo();

            var attr = type.GetCustomAttribute<CollectionName>();

            return attr == null ? type.Name.ToLowerInvariant() : attr.Name;
        }

        private void RegisterUniqueKey<T, TKey>() where T : IEntity<TKey>
        {
            var type = typeof(T);

            if (BsonClassMap.IsClassMapRegistered(type) == false)
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    if (cm.IdMemberMap == null) cm.MapIdProperty("Id");
                });
                return;
            }

            BsonClassMap.LookupClassMap(type);
        }

        #endregion Private Methods

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            _Client = null;
            _Database = null;
        }
    }
}
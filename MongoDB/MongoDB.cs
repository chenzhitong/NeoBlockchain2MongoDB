using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB
{
    public static class MongoDB<T> where T : class
    {
        private static MongoClient client;
        private static IMongoDatabase database;
        private static IMongoCollection<T> collection;

        public static void Initialize(string collectionId, string[] indexNames = null)
        {
            client = new MongoClient(ConfigurationManager.AppSettings["connectionString"]);
            database = client.GetDatabase(ConfigurationManager.AppSettings["database"]);
            collection = database.GetCollection<T>(collectionId);
            if (indexNames != null)
            {
                foreach (var item in indexNames)
                    CreateIndex(collectionId, item);
            }
        }

        public static void CreateIndex(string collectionId, string indexName)
        {
            var options = new CreateIndexOptions() { Unique = false };
            var field = new StringFieldDefinition<T>(indexName);
            var indexDefinition = new IndexKeysDefinitionBuilder<T>().Ascending(field);
            database.GetCollection<T>(collectionId).Indexes.CreateOneAsync(indexDefinition, options).Wait();
        }

        public static void Insert(T doc)
        {
            collection.InsertOne(doc);
        }

        public static void Insert(IEnumerable<T> documents)
        {
            collection.InsertMany(documents);
        }

        public static uint Count()
        {
            return (uint)collection.AsQueryable().Count();
        }

        public static T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return collection.AsQueryable().FirstOrDefault(predicate);
        }

        public static IQueryable<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            return collection.AsQueryable().Where(predicate);
        }

        public static T UpdateItem(Expression<Func<T, bool>> predicate, Expression<Func<T, Object>> field, Object value)
        {
            return collection.FindOneAndUpdate(predicate, Builders<T>.Update.Set(field, value));
        }

        public static void DeleteOne(FilterDefinition<T> filter)
        {
            collection.DeleteOne(filter);
        }

        public static void DeleteMany(FilterDefinition<T> filter)
        {
            collection.DeleteMany(filter);
        }
    }
}

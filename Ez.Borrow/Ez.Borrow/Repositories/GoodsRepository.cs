using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ez.Borrow.Repositories
{
    public class GoodsRepository<T> : IDataRepository<T>
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public IMongoCollection<T> Collection => _database.GetCollection<T>(typeof(T).Name);

        public GoodsRepository(MongoDBConfiguration _option)
        {
            _client = new MongoClient(_option.DefaultConnection);
            _database = _client.GetDatabase(_option.DatabaseName);
        }

        public T Get(Expression<Func<T, bool>> expression)
        {
            return Collection.Find(expression).FirstOrDefault();
        }

        public List<T> List(Expression<Func<T, bool>> expression)
        {
            return Collection.Find(expression).ToList();
        }

        public void Create(T document)
        {
            Collection.InsertOne(document);
        }

        public void DeleteOne(Expression<Func<T, bool>> expression)
        {

            Collection.DeleteMany(expression);
        }

        public void UpdateOne(Expression<Func<T, bool>> expression, T document)
        {
            Collection.ReplaceOne(expression, document);
        }
    }
}

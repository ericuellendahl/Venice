using MongoDB.Driver;
using Venice.Infra.Models;

namespace Venice.Infra.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<OrderItemDocument> OrderItems =>
            _database.GetCollection<OrderItemDocument>("OrderItems");
    }
}

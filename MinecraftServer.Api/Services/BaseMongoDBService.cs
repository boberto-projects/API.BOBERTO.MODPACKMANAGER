using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text.Json;

namespace MinecraftServer.Api.Services
{
    public abstract class BaseMongoDBService
    {
        private readonly IMongoCollection<BsonDocument> _mongoDBConnection;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        public abstract string CollectionName { get; }

        protected BaseMongoDBService(IOptions<MongoDatabaseSettings> mongoDBSettings)
        {
            var config = mongoDBSettings.Value.ObterPorColecao(CollectionName);

            _mongoClient = new MongoClient(config.ConnectionString);

            _mongoDatabase = _mongoClient.GetDatabase(config.DatabaseName);

            _mongoDBConnection = _mongoDatabase.GetCollection<BsonDocument>(config.CollectionName);
        }

        public virtual async Task<List<T>> GetAsync<T>()
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            var resultado = await _mongoDBConnection.FindAsync(filter);
            var list = new List<T>();
            foreach (var item in resultado.ToList())
            {
                var model = BsonSerializer.Deserialize<T>(item);
                list.Add(model);
            }

            return list;
        }

        public virtual async Task<T?> GetAsync<T>(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var resultado = await _mongoDBConnection.Find(filter).FirstOrDefaultAsync();
            if (resultado != null)
            {
                return BsonSerializer.Deserialize<T>(resultado);
            }
            return default;
        }

        public virtual async Task<T?> GetAsync<T>(string field, string value)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(field, value);
            var resultado = await _mongoDBConnection.Find(filter).FirstOrDefaultAsync();
            if (resultado != null)
            {
                return BsonSerializer.Deserialize<T>(resultado);
            }
            return default;
        }

        public virtual async Task CreateAsync<T>(T value) =>
            await _mongoDBConnection.InsertOneAsync(value.ToBsonDocument());

        public virtual async Task UpdateAsync<T>(ObjectId id, T value)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            await _mongoDBConnection.ReplaceOneAsync(filter, value.ToBsonDocument());
        }

        public virtual async Task<bool> UpdateKeyPairAsync(ObjectId id, Dictionary<string, object> dictionary)
        {
            UpdateDefinition<BsonDocument> update = null!;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var changesJson = JsonSerializer.Serialize(dictionary, options);
            var changesDocument = BsonDocument.Parse(changesJson);

            var updateOptions = new UpdateOptions
            {
                IsUpsert = false
            };

            update = new BsonDocumentUpdateDefinition<BsonDocument>(new BsonDocument("$set", changesDocument));

            await _mongoDBConnection.UpdateOneAsync(filter, update, updateOptions);

            return true;
        }

        public virtual async Task<bool> RemoveAsync(ObjectId id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            await _mongoDBConnection.DeleteOneAsync(filter);
            return true;
        }
    }
}

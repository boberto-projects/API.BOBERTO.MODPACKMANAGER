using Microsoft.Extensions.Options;
using MinecraftServer.Api.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MinecraftServer.Api.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<BsonDocument> _mongoDBConnection;

        public MongoDBService(IOptions<MongoDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _mongoDBConnection = mongoDatabase.GetCollection<BsonDocument>(
                bookStoreDatabaseSettings.Value.CollectionName);
        }

        public async Task<List<ModPackModel>> GetAsync<T>()
        {
            var filter = Builders<BsonDocument>.Filter.Empty;
            var resultado = await _mongoDBConnection.Find(filter).ToListAsync();
            return BsonSerializer.Deserialize<List<ModPackModel>>((MongoDB.Bson.IO.IBsonReader)resultado).ToList();
        }

        public async Task<ModPackModel?> GetAsync(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            var resultado = await _mongoDBConnection.Find(filter).FirstOrDefaultAsync();
            if(resultado != null)
            {
                return BsonSerializer.Deserialize<ModPackModel>(resultado);
            }
            return null;
        }

        public async Task CreateAsync(ModPackModel newBook) =>
            await _mongoDBConnection.InsertOneAsync(newBook.ToBsonDocument());

        public async Task UpdateAsync(string id, ModPackModel updatedModPack)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
            await _mongoDBConnection.ReplaceOneAsync(filter, updatedModPack.ToBsonDocument());

        }

        public async Task UpdateKeyPairAsync(string id, Dictionary<string, object> dictionary)
        {
            UpdateDefinition<BsonDocument> update = null!;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));

            var changesJson = JsonSerializer.Serialize(dictionary);
            var changesDocument = BsonDocument.Parse(changesJson);

            foreach (var item in changesDocument)
            {
                if (update == null)
                {
                    var builder = Builders<BsonDocument>.Update;
                    update = builder.Set(item.Name, item.Value);
                }
                else
                {
                    update = update.Set(item.Name, item.Value);
                }
            }

            var updateOptions = new UpdateOptions
            {
                IsUpsert = false
            };

            update = new BsonDocumentUpdateDefinition<BsonDocument>(new BsonDocument("$set", changesDocument));

            await _mongoDBConnection.UpdateOneAsync(filter, update);
        }



        public async Task RemoveAsync(string id){
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            await _mongoDBConnection.DeleteOneAsync(filter);
        }
    }
}

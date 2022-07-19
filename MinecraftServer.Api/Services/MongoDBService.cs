using Microsoft.Extensions.Options;
using MinecraftServer.Api.Models;
using MongoDB.Driver;

namespace MinecraftServer.Api.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<ModPackModel> _mongoDBConnection;

        public MongoDBService(IOptions<MongoDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _mongoDBConnection = mongoDatabase.GetCollection<ModPackModel>(
                bookStoreDatabaseSettings.Value.CollectionName);
        }

        public async Task<List<ModPackModel>> GetAsync() =>
            await _mongoDBConnection.Find(_ => true).ToListAsync();

        public async Task<ModPackModel?> GetAsync(string id) =>
            await _mongoDBConnection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(ModPackModel newBook) =>
            await _mongoDBConnection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, ModPackModel updatedBook) =>
            await _mongoDBConnection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _mongoDBConnection.DeleteOneAsync(x => x.Id == id);
    }
}

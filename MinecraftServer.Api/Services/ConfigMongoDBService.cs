using Microsoft.Extensions.Options;

namespace MinecraftServer.Api.Services
{
    public class ConfigMongoDBService : BaseMongoDBService
    {
        public override string CollectionName { get => "Config"; }

        public ConfigMongoDBService(IOptions<MongoDatabaseSettings> mongoDBSettings) : base(mongoDBSettings)
        {

        }
    }
}

using Microsoft.Extensions.Options;

namespace MinecraftServer.Api.Services
{
    public class ConfigMongoDBService : BaseMongoDBService
    {
        public ConfigMongoDBService(IOptions<MongoDatabaseSettings> mongoDBSettings) : base(mongoDBSettings)
        {

        }
    }
}

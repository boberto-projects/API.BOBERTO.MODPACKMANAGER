using Microsoft.Extensions.Options;

namespace MinecraftServer.Api.Services
{
    public class ModPackMongoDBService : BaseMongoDBService
    {
        public override string CollectionName { get => "ModPacks"; }

        public ModPackMongoDBService(IOptions<MongoDatabaseSettings> mongoDBSettings) : base(mongoDBSettings)
        {

        }
    }
}

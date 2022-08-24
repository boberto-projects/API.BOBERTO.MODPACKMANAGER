using Microsoft.Extensions.Options;

namespace MinecraftServer.Api.Services
{
    public class LauncherConfigMongoDBService : BaseMongoDBService
    {
        public override string CollectionName { get => "Config"; }

        public LauncherConfigMongoDBService(IOptions<MongoDatabaseSettings> mongoDBSettings) : base(mongoDBSettings)
        {

        }
    }
}

using Microsoft.Extensions.Options;

namespace MinecraftServer.Api.Services
{
    public class LauncherVersionMongoDBService : BaseMongoDBService
    {
        public override string CollectionName { get => "LauncherVersion"; }

        public LauncherVersionMongoDBService(IOptions<MongoDatabaseSettings> mongoDBSettings) : base(mongoDBSettings)
        {

        }
    }
}

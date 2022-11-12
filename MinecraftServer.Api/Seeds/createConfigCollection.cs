using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.Services;

namespace MinecraftServer.Api.Seeds
{
    public static class createConfigCollection
    {
        public static void CreateConfigDefaulCollection(IServiceProvider services)
        {
            try
            {
                var mongoDBService = services.GetService<LauncherConfigMongoDBService>();

                var config = mongoDBService.GetAsync<ConfigModel>().Result;
                var lastConfig = config.FirstOrDefault();

                if (lastConfig != null)
                {
                    return;
                }

                var defaultConfig = new ConfigModel()
                {
                    Maintenance = false,
                    MaintenanceMessage = "Hora da manutenção, amiguinhos!",
                    Offline = false,
                    ClientId = ""
                };
                mongoDBService.CreateAsync(defaultConfig).Wait();
            }
            catch (Exception e)
            {

            }
        }
    }
}

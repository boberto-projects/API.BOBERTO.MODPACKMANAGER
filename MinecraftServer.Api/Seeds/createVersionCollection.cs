using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.Services;

namespace MinecraftServer.Api.Seeds
{

    public static class createVersionCollection
    {
        public static void CreateVersionDefaulCollection(IServiceProvider services)
        {
            try
            {
                var mongoDBService = services.GetService<LauncherVersionMongoDBService>();

                var config = mongoDBService.GetAsync<LauncherVersionModel>().Result;
                var lastConfig = config.FirstOrDefault();

                if (lastConfig != null)
                {
                    return;
                }

                var defaultConfig = new LauncherVersionModel()
                {
                    Packages = new LauncherVersionModel.PackagesEntity()
                    {
                        Win64 = new LauncherVersionModel.Win64Entity()
                        {
                            Url = ""
                        },
                        Linux64 = new LauncherVersionModel.Linux64Entity()
                        {
                            Url = ""
                        },
                        Mac64 = new LauncherVersionModel.Mac64Entity()
                        {
                            Url = ""
                        }
                    },
                    Version = "1.0.0"
                };
                mongoDBService.CreateAsync(defaultConfig).Wait();
            }
            catch (Exception e)
            {
            }
        }
 
    }
}

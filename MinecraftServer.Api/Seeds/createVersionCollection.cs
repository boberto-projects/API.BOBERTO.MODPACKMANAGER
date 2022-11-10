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

                //var defaultConfig = new LauncherVersionModel()
                //{
                    
                //};
                //mongoDBService.CreateAsync(defaultConfig).Wait();
            }
            catch (Exception e)
            {
            }
        }
 
    }
}

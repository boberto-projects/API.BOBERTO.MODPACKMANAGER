using Microsoft.Extensions.Options;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.Services;

namespace MinecraftServer.Api
{
    public static class AutoStartup
    {
        public static void Start(this IServiceCollection services, IConfigurationRoot config)
        {
            CriarPastaModPacks(config);
            CriarPastaLauncherVersions(config);
            CriarCacheInicial(services, config);
        }

        static void CriarCacheInicial(IServiceCollection services, IConfigurationRoot config)
        {
            var serviceProvider = services.BuildServiceProvider();
            var redisService = serviceProvider.GetService<IRedisService>();
            var mongoDBService = serviceProvider.GetService<ModPackMongoDBService>();
            var modPacks = mongoDBService.GetAsync<ModPackModel>().Result;
            var apiConfig = serviceProvider.GetService<IOptions<ApiConfig>>();
            var logger = serviceProvider.GetService<ILogger<ApiCicloDeVida>>();

            foreach (var modpack in modPacks)
            {
                var files = Utils.ListarArquivosRecursivos(apiConfig.Value, modpack);
                if (redisService.Exists(modpack.Id))
                {
                    logger.LogInformation($"Already exists cache redis for {modpack.Id}");
                    return;
                }
                redisService.Set(modpack.Id, files);
                logger.LogInformation($"Redis cache created for {modpack.Id}");
            }
        }

        static void CriarPastaModPacks(IConfigurationRoot config)
        {
            var dirMods = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.GetSection("ApiConfig").Get<ApiConfig>().ModPackDir);
            if (Directory.Exists(dirMods) == false)
            {
                Directory.CreateDirectory(dirMods);
            }
        }

        static void CriarPastaLauncherVersions(IConfigurationRoot config)
        {
            var dirLauncherVersions = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.GetSection("ApiConfig").Get<ApiConfig>().LauncherVersionDir);
            if (Directory.Exists(dirLauncherVersions) == false)
            {
                Directory.CreateDirectory(dirLauncherVersions);
            }
        }

    }

}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using MinecraftServer.Api.Middlewares;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.Seeds;
using MinecraftServer.Api.Services;

namespace MinecraftServer.Api
{
    public static class DependencyInjection
    {
        public static void RegistrarDI(this IServiceCollection services, IConfigurationRoot config)
        {
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = long.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            services.Configure<ApiConfig>(options => config.GetSection("ApiConfig").Bind(options));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = ObterRedisContext();
            });

            string ObterRedisContext()
            {
                var redisContextUrl = config.GetConnectionString("Redis");
                Uri redisUrl;
                bool isRedisUrl = Uri.TryCreate(redisContextUrl, UriKind.Absolute, out redisUrl);
                if (isRedisUrl)
                {
                    redisContextUrl = string.Format("{0}:{1},password={2}", redisUrl.Host, redisUrl.Port, redisUrl.UserInfo.Split(':')[1]);
                }
                return redisContextUrl;
            }

            services.AddSingleton<ApiCicloDeVida>();
            services.AddDirectoryBrowser();
            services.AddAuthentication("BasicAuthentication")
                            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
                            ("BasicAuthentication", null);

            services.AddSingleton<IRedisService, RedisService>();
            services.Configure<MongoDatabaseSettings>(options => config.GetSection("MongoConnections").Bind(options));
            services.AddSingleton<ModPackMongoDBService>();
            services.AddSingleton<LauncherVersionMongoDBService>();
            services.AddSingleton<LauncherConfigMongoDBService>();

            var sp = services.BuildServiceProvider();
            createConfigCollection.CreateConfigDefaulCollection(sp);
            createVersionCollection.CreateVersionDefaulCollection(sp);

            services.AddAuthorization();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}

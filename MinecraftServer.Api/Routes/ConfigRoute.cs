using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.Services;
using MongoDB.Bson;

namespace MinecraftServer.Api.Routes
{
    public static class ConfigRoute
    {
        private const string BaseUrl = "/config";
        //há um metódo pra usar o BaseUrl. Vou ignorar por agora.
        public static void CriarRota(this WebApplication app)
        {
            app.MapGet(BaseUrl, async ([FromServices] ConfigMongoDBService mongoDbService) =>
            {
                var config = await mongoDbService.GetAsync<ConfigModel>();
                var lastConfig = config.FirstOrDefault();
                return Results.Ok(lastConfig);
            });

            app.MapPut(BaseUrl, async ([FromBody] Dictionary<string, object> request, [FromServices] ConfigMongoDBService mongoDbService) =>
            {
                var config = await mongoDbService.GetAsync<ConfigModel>();
                var lastConfig = config.FirstOrDefault();

                if (lastConfig == null)
                {
                    return Results.NotFound("Config não encontrado.");
                }

                await mongoDbService.UpdateKeyPairAsync(ObjectId.Parse(lastConfig.Id), request);

                return Results.Ok();
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api.MongoModels;
using MinecraftServer.Api.RequestModels;
using MinecraftServer.Api.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MinecraftServer.Api.Routes
{
    public static class LauncherVersionRoute
    {
        private const string BaseUrl = "/launcher";
        public static void CriarRota(this WebApplication app)
        {
            app.MapPut(BaseUrl, async (ObjectId id, [FromBody] Dictionary<string, object> request, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            {
                var launcherConfig = await mongoDbService.GetAsync<LauncherVersionModel>(id);

                if (launcherConfig == null)
                {
                    return Results.NotFound("Config não encontrado.");
                }

                await mongoDbService.UpdateKeyPairAsync(id, request);

                return Results.Ok();
            });

            app.MapPost(BaseUrl, async (LauncherVersionRequest request, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            {
                var launcherVersion = await mongoDbService.GetAsync<LauncherVersionModel>();
                var ultimoLauncher = launcherVersion.FirstOrDefault();
                if (ultimoLauncher == null)
                {
                    await mongoDbService.CreateAsync(request.ToMap());
                    return Results.Ok("Criado com sucesso.");
                }
                return Results.NoContent();
            });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.Services;
using MongoDB.Bson;

namespace MinecraftServer.Api.Routes
{
    public static class ConfigRoute
    {
        private const string BaseUrl = "/config";
    
        public async static Task<IResult> GetLauncherConfig([FromServices] LauncherConfigMongoDBService mongoDbService)
        {
            var config = await mongoDbService.GetAsync<ConfigModel>();
            var lastConfig = config.FirstOrDefault();
            return Results.Ok(lastConfig);
        }
        //vamos começar a estudar os testes com esse metódo e refatorar o esquema de rotas.
        //delegando toda lógica de negócio pra um serviço

        public static void CriarRota(this WebApplication app)
        {
         
            app.MapGet(BaseUrl, GetLauncherConfig)
                .WithTags("Launcher Config");

            app.MapPut(BaseUrl, [Authorize] async ([FromBody] Dictionary<string, object> request, [FromServices] LauncherConfigMongoDBService mongoDbService) =>
            {
                var config = await mongoDbService.GetAsync<ConfigModel>();
                var lastConfig = config.FirstOrDefault();

                if (lastConfig == null)
                {
                    return Results.NotFound("Config não encontrado.");
                }

                await mongoDbService.UpdateKeyPairAsync(ObjectId.Parse(lastConfig.Id), request);

                return Results.Ok();
            }).WithTags("Launcher Config");
        }
    }
}

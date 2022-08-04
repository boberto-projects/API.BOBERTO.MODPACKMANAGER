using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.RequestModels;
using MinecraftServer.Api.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MinecraftServer.Api.Routes
{
    public static class LauncherVersionRoute
    {
        private const string BaseUrl = "/launcher";
        //há um metódo pra usar o BaseUrl. Vou ignorar por agora.
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
                }
                return Results.Ok("Config criado com sucesso.");
            });

            app.MapGet(BaseUrl, async ([FromServices] LauncherVersionMongoDBService mongoDbService) =>
            {
                var launcherVersion = await mongoDbService.GetAsync<LauncherVersionModel>();
                var ultimoLauncher = launcherVersion.FirstOrDefault();    
                return Results.Ok(ultimoLauncher);
            });
             
            app.MapPost(BaseUrl + "/upload/{system}", async ([FromRoute] SystemEnum system, HttpRequest request, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            {
                var fields = new Dictionary<string, object>();
                var launcherVersion = await mongoDbService.GetAsync<LauncherVersionModel>();
                var ultimoLauncher = launcherVersion.FirstOrDefault();

                if (ultimoLauncher == null)
                {
                    return Results.NotFound("Config não encontrado.");
                }

                string path = Path.Combine(Directory.GetCurrentDirectory(), Config.CaminhoLauncherVersions);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var file = request.Form.Files.First();

                FileInfo fileInfo = new FileInfo(file.FileName);

                string fileNameWithPath = Path.Combine(path, file.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var url = $"http://localhost/{fileNameWithPath}";
                switch (system)
                {
                    case SystemEnum.WINDOWS:
                        fields.Add("packages.Win64", new LauncherVersionModel.Win64Entity()
                        {
                            Url = url,
                        });
                    break;

                    case SystemEnum.MAC:
                        fields.Add("packages.Mac64", new LauncherVersionModel.Mac64Entity()
                        {
                            Url = url,
                        });
                    break;

                    case SystemEnum.LINUX:
                        fields.Add("packages.Linux64", new LauncherVersionModel.Linux64Entity()
                        {
                            Url = url,
                        });
                    break;

                }
                await mongoDbService.UpdateKeyPairAsync(ObjectId.Parse(ultimoLauncher.Id), fields);

                return Results.Ok();
            })
      .Accepts<IFormFile>("multipart/form-data")
      .Produces(200);
        }
    }
}

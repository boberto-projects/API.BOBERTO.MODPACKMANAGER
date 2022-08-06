using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.RequestModels;
using MinecraftServer.Api.Services;
using MongoDB.Bson;
using System.IO.Compression;
using System.Reflection;

namespace MinecraftServer.Api.Routes
{
    public static class ModPackRoute
    {
        private const string BaseUrl = "/modpacks/";

        public static void CriarRota(this WebApplication app)
        {
            app.MapGet("/modpack/files/{id}/{generateCache}", async ([FromRoute]  ObjectId id, [FromRoute] bool generateCache, 
                [FromServices] ModPackMongoDBService mongoDbService,
                [FromServices] IRedisService redisService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    return Results.NotFound("MODPACK não encontrado.");
                }

                var idRedis = id.ToString();


                if (!redisService.Exists(idRedis) || generateCache)
                {
                    var files = Utils.ListarArquivosRecursivos(modpack);
                    redisService.Set(idRedis, files, 3600);
                }

                return Results.Ok(redisService.Get<List<ModPackFileInfo>>(idRedis));
            });

            app.MapGet("/modpack/{id}", async (ObjectId id, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    return Results.NotFound("MODPACK não encontrado.");
                }

                return Results.Ok(modpack);
            });

            app.MapGet("/modpack", async ([FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>();
                return Results.Ok(modpack);
            });


            app.MapPut("/modpack/update/{id}", async (ObjectId id, [FromBody] Dictionary<string, object> request, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    return Results.NotFound("MODPACK não encontrado.");
                }

                await mongoDbService.UpdateKeyPairAsync(id, request);

                return Results.Ok();
            });

            app.MapPost("/modpack/add", async (ModPackRequest request, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                request.DatetimeCreatAt = DateTime.Now;
                await mongoDbService.CreateAsync(request.ToMap());

                return Results.Ok();
            });

            app.MapPost("/modpack/upload/{id}", async (ObjectId id, HttpRequest request, [FromServices] ModPackMongoDBService mongoDbService) =>
            {

                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    return Results.NotFound("MODPACK não encontrado.");
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.CaminhoModPacks);

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

                if (Directory.Exists(fileNameWithPath))
                {
                    Directory.Delete(fileNameWithPath);
                }

                ZipFile.ExtractToDirectory(fileNameWithPath, Path.Combine(Config.CaminhoModPacks, modpack.Directory), true);

                if (File.Exists(fileNameWithPath))
                {
                    File.Delete(fileNameWithPath);
                }
                //  await mongoDbService.CreateAsync(request);
                 return Results.Ok();
            })
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200);

         
        }

        

    }
}

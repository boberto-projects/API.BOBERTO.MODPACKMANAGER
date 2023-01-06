using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinecraftServer.Api.Exceptions;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.RequestModels;
using MinecraftServer.Api.Services;
using MongoDB.Bson;
using System.IO.Compression;

namespace MinecraftServer.Api.Routes
{
    public static class ModPackRoute
    {
        private const string BaseUrl = "/modpack";

        public static void CriarRota(this WebApplication app)
        {
            app.MapGet(BaseUrl + "/files/{id}/{forceGenerateCache}", async ([FromRoute] ObjectId id, [FromServices] IOptions<ApiConfig> apiConfig, [FromRoute] bool forceGenerateCache,
                [FromServices] ModPackMongoDBService mongoDbService,
                [FromServices] IRedisService redisService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    throw new CasimiroException(ExceptionType.Validacao, "MODPACK não encontrado.");
                }

                var idRedis = id.ToString();

                if (!redisService.Exists(idRedis) || forceGenerateCache)
                {
                    var files = Utils.ListarArquivosRecursivos(apiConfig.Value, modpack);
                    redisService.Set(idRedis, files, 3600);
                }
                var response = redisService.Get<List<ModPackFileInfo>>(idRedis);
                return Results.Ok(response);
            }).WithTags("ModPack Manager");

            app.MapGet(BaseUrl + "/{id}", async (ObjectId id, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    throw new CasimiroException(ExceptionType.Validacao, "MODPACK não encontrado.");
                }

                return Results.Ok(modpack);
            }).WithTags("ModPack Manager");

            app.MapGet(BaseUrl, async ([FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>();
                return Results.Ok(modpack);
            }).WithTags("ModPack Manager");

            app.MapDelete(BaseUrl + "/{id}", [Authorize] async (ObjectId id, [FromServices] IOptions<ApiConfig> apiConfig, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    throw new CasimiroException(ExceptionType.Validacao, "MODPACK não encontrado.");
                }

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.ModPackDir);
                string modpackDir = Path.Combine(path, modpack.Directory);

                mongoDbService.InitTransaction();

                if (Directory.Exists(modpackDir))
                {
                    Directory.Delete(modpackDir, true);
                }

                await mongoDbService.RemoveAsync(id);
                mongoDbService.SaveChanges();
                return Results.Ok();

            }).WithTags("ModPack Manager");

            app.MapPut(BaseUrl + "/update/{id}", [Authorize] async (ObjectId id, [FromBody] Dictionary<string, object> request,
                [FromServices] IOptions<ApiConfig> apiConfig,
                [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    throw new CasimiroException(ExceptionType.Validacao, "MODPACK não encontrado.");
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.ModPackDir);
                string modPackDirectory = Path.Combine(path, modpack.Directory);
                var directory = request.TryGetValue("directory", out object newModPackDirObject);
                var isNewModPackDir = directory && newModPackDirObject.ToString().Contains(modpack.Directory) == false;

                mongoDbService.InitTransaction();
                await mongoDbService.UpdateKeyPairAsync(id, request);
                if (isNewModPackDir)
                {
                    string newModPackDirectory = Path.Combine(path, newModPackDirObject.ToString());
                    Utils.CopyDirectory(modPackDirectory, newModPackDirectory, true);
                    Directory.Delete(modPackDirectory, true);
                }
                mongoDbService.SaveChanges();
                return Results.Ok();
            }).WithTags("ModPack Manager");

            app.MapPost(BaseUrl + "/add", [Authorize] async (ModPackRequest request, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                if (string.IsNullOrEmpty(request.Id) == false)
                {
                    var objectId = ObjectId.Parse(request.Id);

                    var modpack = await mongoDbService.GetAsync<ModPackModel>(objectId);

                    if (modpack != null)
                    {
                        throw new CasimiroException(ExceptionType.Validacao, "MODPACK já existe.");
                    }
                }

                request.DatetimeCreatAt = DateTime.Now;
                await mongoDbService.CreateAsync(request.ToMap());


                return Results.Ok();

            }).WithTags("ModPack Manager");

            app.MapPost(BaseUrl + "/upload/{id}/{forceGenerateCache}", [Authorize] async (ObjectId id, HttpRequest request, [FromServices] IOptions<ApiConfig> apiConfig,
                [FromServices] ModPackMongoDBService mongoDbService, [FromServices] IRedisService redisService, [FromRoute] bool forceGenerateCache) =>
            {

                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    throw new CasimiroException(ExceptionType.Validacao, "MODPACK não encontrado.");
                }

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.ModPackDir);
                string outputPath = Path.Combine(path, modpack.Directory);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var file = request.Form.Files.First();
                if (file.FileName.EndsWith(".zip") == false)
                {
                    throw new CasimiroException(ExceptionType.Validacao, "Só é possível fazer upload de arquivos .zip");
                }

                FileInfo fileInfo = new FileInfo(file.FileName);

                string fileNameWithPath = Path.Combine(path, file.FileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if (Directory.Exists(outputPath))
                {
                    Directory.Delete(outputPath, true);
                }

                ZipFile.ExtractToDirectory(fileNameWithPath, outputPath);

                if (File.Exists(fileNameWithPath))
                {
                    File.Delete(fileNameWithPath);
                }

                var idRedis = id.ToString();

                if (!redisService.Exists(idRedis) || forceGenerateCache)
                {
                    var files = Utils.ListarArquivosRecursivos(apiConfig.Value, modpack);
                    redisService.Set(idRedis, files, 3600);
                }

                return Results.Ok();
            }).WithTags("ModPack Manager")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200);
        }
    }
}

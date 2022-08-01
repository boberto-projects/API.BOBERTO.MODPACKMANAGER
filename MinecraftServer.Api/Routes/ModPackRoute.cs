using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.RequestModels;
using MinecraftServer.Api.Services;
using MongoDB.Bson;
using System.IO.Compression;
using System.Text.Json;

namespace MinecraftServer.Api.Routes
{
    public static class ModPackRoute
    {
        public static void CriarRota(this WebApplication app)
        {

            app.MapGet("/modpack/{id}", async (ObjectId id, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    return Results.NotFound("MODPACK não encontrado.");
                }

                return Results.Ok(Utils.ListarArquivosRecursivos(modpack));
            });


            app.MapPost("/modpack/update/{id}", async (ObjectId id, [FromBody] Dictionary<string, object> request, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    return Results.NotFound("MODPACK não encontrado.");
                }

                await mongoDbService.UpdateKeyPairAsync(id, request);

                return Results.Ok();
            });

            app.MapPost("/modpack/upload/{id}", async (ObjectId id, HttpRequest request, [FromServices] ModPackMongoDBService mongoDbService) =>
            {

                var modpack = await mongoDbService.GetAsync<ModPackModel>(id);

                if (modpack == null)
                {
                    return Results.NotFound("MODPACK não encontrado.");
                }
                string path = Path.Combine(Directory.GetCurrentDirectory(), Config.CaminhoModPacks);

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

            app.MapPost("/modpack/add", async (ModPackRequest request, [FromServices] ModPackMongoDBService mongoDbService) =>
            {
                request.DatetimeCreatAt = DateTime.Now;
                await mongoDbService.CreateAsync(request.ToMap());

                return Results.Ok();
            });
        }

        

    }
}

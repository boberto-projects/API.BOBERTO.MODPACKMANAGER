using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinecraftServer.Api.Exceptions;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.Services;
using MongoDB.Bson;

namespace MinecraftServer.Api.Routes
{
    public static class LauncherVersionRoute
    {
        private const string BaseUrl = "/launcher";
        //há um metódo pra usar o BaseUrl. Vou ignorar por agora.
        public static void CriarRota(this WebApplication app)
        {
            app.MapPost(BaseUrl + "/upload/{versionTag}", [Authorize] async (Stream body, [FromRoute] string versionTag, [FromHeader(Name = "X-File-Name")] string name, [FromServices] IOptions<ApiConfig> apiConfig, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion, versionTag);
                string latestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion, "latest");

                Stream stream = new MemoryStream();
                await body.CopyToAsync(stream);
                FormFile file = new FormFile(stream, 0, stream.Length, name, name);

                var launcherVersions = await mongoDbService.GetAsync<LauncherVersionModel>();
                var recentLauncher = launcherVersions.ToList().OrderByDescending(x => new Version(x.Version)).FirstOrDefault();
                var recentLauncherVersion = new Version(recentLauncher?.Version ?? "0.0.0");
                var currentVersion = new Version(versionTag);
                var versionCompare = currentVersion.CompareTo(recentLauncherVersion);

                var firstLauncher = await mongoDbService.GetAsync<LauncherVersionModel>("version", versionTag);

                if (firstLauncher == null)
                {
                    var launcherModel = new LauncherVersionModel()
                    {
                        Arch = "x64",
                        Name = name,
                        Version = versionTag,
                        System = "other"
                    };
                    await mongoDbService.CreateAsync(launcherModel);
                    if (versionCompare == 1 && Directory.Exists(latestPath))
                    {
                        Directory.Delete(latestPath, true);
                    }
                }

                CreateFolderPath(path);
                MoveFileTest(path, file);

                if (versionCompare >= 0)
                {
                    CreateFolderPath(latestPath);
                    MoveFileTest(latestPath, file);
                }

                return Results.Ok();

                void MoveFileTest(string folderPath, IFormFile file)
                {
                    string fileNameWithPath = Path.Combine(folderPath, file.FileName);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                void CreateFolderPath(string folderPath)
                {
                    if (Directory.Exists(folderPath) == false)
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }

            }).WithTags("Launcher Version")
            .Produces(200);



            app.MapDelete(BaseUrl + "/remove/{versionTag}", [Authorize] async (Stream body, [FromRoute] string versionTag, [FromServices] IOptions<ApiConfig> apiConfig, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            {
                var pathDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion);
                var latestDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion, "latest");
                var pathDirWithVersion = Path.Combine(pathDir, versionTag);

                var launcherVersions = await mongoDbService.GetAsync<LauncherVersionModel>();
                var recentLauncherList = launcherVersions.OrderByDescending(x => new Version(x.Version)).ToList();//
                var recentLauncherVersion = new Version(recentLauncherList?.First().Version ?? "0.0.0");
                var currentVersion = new Version(versionTag);
                var versionCompare = currentVersion.CompareTo(recentLauncherVersion);
                var firstLauncher = await mongoDbService.GetAsync<LauncherVersionModel>("version", versionTag);
                if (firstLauncher == null)
                {
                    throw new CasimiroException(ExceptionType.Negocio, "Versão não encontrada.");
                }

                var launcherVersionId = ObjectId.Parse(firstLauncher.Id);
                await mongoDbService.RemoveAsync(launcherVersionId);

                if (versionCompare >= 0 && Directory.Exists(latestDir))
                {
                    foreach (var file in Directory.GetFiles(latestDir))
                    {
                        File.Delete(file);
                    }

                    var lastLauncher = recentLauncherList[1];
                    var originPath = Path.Combine(pathDir, lastLauncher.Version);

                    foreach (var file in Directory.GetFiles(originPath))
                    {
                        File.Copy(file, file.Replace(originPath, latestDir), true);
                    }

                }
                if (Directory.Exists(pathDirWithVersion))
                {
                    Directory.Delete(pathDirWithVersion, true);
                }

                return Results.Ok();
            }).WithTags("Launcher Version");



            //      app.MapPut(BaseUrl, [Authorize] async ([FromBody] Dictionary<string, object> request, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            //      {
            //          var launcherVersion = await mongoDbService.GetAsync<LauncherVersionModel>();
            //          var ultimoLauncher = launcherVersion.FirstOrDefault();

            //          if (ultimoLauncher == null)
            //          {
            //              throw new CasimiroException(ExceptionType.Validacao, "Config não encontrado.");
            //          }

            //          await mongoDbService.UpdateKeyPairAsync(ObjectId.Parse(ultimoLauncher.Id), request);

            //          return Results.Ok();
            //      }).WithTags("Launcher Version");

            //      app.MapPost(BaseUrl, [Authorize] async (LauncherVersionRequest request, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            //      {
            //          var launcherVersion = await mongoDbService.GetAsync<LauncherVersionModel>();
            //          var ultimoLauncher = launcherVersion.FirstOrDefault();
            //          if (ultimoLauncher == null)
            //          {
            //              await mongoDbService.CreateAsync(request.ToMap());
            //          }
            //          return Results.Ok("Config criado com sucesso.");
            //      }).WithTags("Launcher Version");

            //      app.MapGet(BaseUrl, async ([FromServices] LauncherVersionMongoDBService mongoDbService) =>
            //      {
            //          var launcherVersion = await mongoDbService.GetAsync<LauncherVersionModel>();
            //          var ultimoLauncher = launcherVersion.FirstOrDefault();    
            //          return Results.Ok(ultimoLauncher);
            //      }).WithTags("Launcher Version");

            //      app.MapPost(BaseUrl + "/upload/{system}", [Authorize] async (HttpRequest request, [FromRoute] SystemEnum system, [FromServices] IOptions<ApiConfig> apiConfig, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            //      {
            //          var fields = new Dictionary<string, object>();
            //          var launcherVersion = await mongoDbService.GetAsync<LauncherVersionModel>();
            //          var ultimoLauncher = launcherVersion.FirstOrDefault();

            //          if (ultimoLauncher == null)
            //          {
            //              throw new CasimiroException(ExceptionType.Negocio, "Config não encontrado.");
            //          }

            //          string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion);

            //          if (Directory.Exists(path) == false)
            //          {
            //              Directory.CreateDirectory(path);
            //          }

            //          var file = request.Form.Files.First();

            //          FileInfo fileInfo = new FileInfo(file.FileName);

            //          string fileNameWithPath = Path.Combine(path, file.FileName);

            //          using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            //          {
            //              file.CopyTo(stream);
            //          }

            //          var url = $"{apiConfig.Value.VersionDirUrl}/{Path.GetFileName(fileNameWithPath)}";
            //          switch (system)
            //          {
            //              case SystemEnum.WINDOWS:
            //                  fields.Add("packages.win64", new LauncherVersionModel.Win64Entity()
            //                  {
            //                      Url = url,
            //                  });
            //              break;

            //              case SystemEnum.MAC:
            //                  fields.Add("packages.mac64", new LauncherVersionModel.Mac64Entity()
            //                  {
            //                      Url = url,
            //                  });
            //              break;

            //              case SystemEnum.LINUX:
            //                  fields.Add("packages.linux64", new LauncherVersionModel.Linux64Entity()
            //                  {
            //                      Url = url,
            //                  });
            //              break;
            //          }

            //          await mongoDbService.UpdateKeyPairAsync(ObjectId.Parse(ultimoLauncher.Id), fields);

            //          return Results.Ok();
            //      }).WithTags("Launcher Version")
            //.Accepts<IFormFile>("multipart/form-data")
            //.Produces(200);
            //  }
        }


    }
}

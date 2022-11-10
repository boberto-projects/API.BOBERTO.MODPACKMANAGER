using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinecraftServer.Api.Exceptions;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.MongoEntities;
using MinecraftServer.Api.Services;

namespace MinecraftServer.Api.Routes
{
    public static class LauncherVersionRoute
    {
        private const string BaseUrl = "/launcher";
        //há um metódo pra usar o BaseUrl. Vou ignorar por agora.
        public static void CriarRota(this WebApplication app)
        {
            app.MapPost(BaseUrl + "/upload/{versionTag}/{name}", [Authorize] async (string name, string versionTag, HttpRequest request, [FromServices] IOptions<ApiConfig> apiConfig, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            {
            var files = request.Form.Files;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion, versionTag);
            string latestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion, "latest");

            RecreateFolderPath(path);

            ///copio todos os arquivos enviados para a pasta versão
            MoveFiles(path, files);

            var launcherModel = new LauncherVersionModel()
            {
                Arch = "x64",
                Version = versionTag,
                Name = name,
                System = "other"
            };
            var launcherVersions = await mongoDbService.GetAsync<LauncherVersionModel>();
            var firstLauncher = launcherVersions.FirstOrDefault(ver => ver.Version.Equals(versionTag));
            var mostRecentLauncher = launcherVersions.OrderByDescending(x => new Version(x.Version)).FirstOrDefault();

            if (firstLauncher == null)
            {
                await mongoDbService.CreateAsync(launcherModel);
                if(launcherVersions.Count == 0)
                {
                    RecreateFolderPath(latestPath);
                    MoveFiles(latestPath, files);
                }
                if (launcherVersions.Count() > 0)
                {
                    var recentLauncherVersion = new Version(mostRecentLauncher.Version);
                    var currentVersion = new Version(versionTag);

                    if(currentVersion.CompareTo(recentLauncherVersion) == 1)
                    {
                            RecreateFolderPath(latestPath);
                            MoveFiles(latestPath, files);
                    }
                   
                }
                    ///crio pasta com última versão

                  
                }

            
       


                void MoveFiles(string folderPath, IFormFileCollection files)
                {
                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file.FileName);
                        string fileNameWithPath = Path.Combine(folderPath, file.FileName);
                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                }
                void RecreateFolderPath(string folderPath)
                {
                    ///se for enviado novamente, removo a pasta com essa versão e removo todos os arquivos.
                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                    }
                    /// se a versão não existir, crio uma pasta para ela.

                    if (Directory.Exists(folderPath) == false)
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }

                return Results.Ok();   
            }).WithTags("Launcher Version")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200);

            //app.MapGet(BaseUrl + "/latest", async ([FromServices] LauncherVersionMongoDBService mongoDbService) =>
            //{
            //    var launcherVersion = await mongoDbService.GetAsync<LauncherVersionModel>();
            //    var lastVersion = launcherVersion.OrderByDescending(x => new Version(x.Version)).FirstOrDefault();

               
            //    return Results.Ok();
            //}).WithTags("Launcher Version");



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

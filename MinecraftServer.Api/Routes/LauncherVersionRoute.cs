using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
            app.MapPost(BaseUrl + "/upload/{versionTag}/{name}", [Authorize] async (Stream body, [FromRoute] string versionTag, [FromRoute] string name, [FromServices] IOptions<ApiConfig> apiConfig, [FromServices] LauncherVersionMongoDBService mongoDbService) =>
            {

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion, versionTag);
                string latestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.CaminhoLauncherVersion, "latest");
                using Stream stream = new MemoryStream();
                await body.CopyToAsync(stream);

                var file = new FormFile(stream, 0, stream.Length, name, name);


                var launcherVersions = await mongoDbService.GetAsync<LauncherVersionModel>();
                var recentLauncher = launcherVersions.ToList().OrderByDescending(x => new Version(x.Version)).FirstOrDefault();
                var recentLauncherVersion = new Version(recentLauncher?.Version ?? versionTag);
                var currentVersion = new Version(versionTag);
                var versionCompare = currentVersion.CompareTo(recentLauncherVersion);

                if (versionCompare == 1)
                {
                    CreateFolderPath(path);
                    ///copio todos os arquivos enviados para a pasta versão
                    MoveFileTest(path, file);

                    CreateFolderPath(latestPath);
                    MoveFileTest(latestPath, file);
                }
                if (versionCompare == 0)
                {
                    //recrio a pasta
                    RecreateFolderPath(path);
                    ///copio todos os arquivos enviados para a pasta versão
                    MoveFileTest(path, file);

                    RecreateFolderPath(latestPath);
                    MoveFileTest(latestPath, file);
                }
                var firstLauncher = await mongoDbService.GetAsync<LauncherVersionModel>("version", versionTag);

                if (firstLauncher == null)
                {
                    var launcherModel = new LauncherVersionModel()
                    {
                        Arch = "x64",
                        Version = versionTag,
                        Files = new List<string>() { name },
                        System = "other"
                    };
                    await mongoDbService.CreateAsync(launcherModel);

                    //launcherVersions = await mongoDbService.GetAsync<LauncherVersionModel>();
                    //recentLauncher = launcherVersions.ToList().OrderByDescending(x => new Version(x.Version)).FirstOrDefault();
                }
                else
                {
                    firstLauncher.Files.Add(name);
                    await mongoDbService.UpdateAsync(ObjectId.Parse(firstLauncher.Id), firstLauncher);
                }

                //launcherVersions = await mongoDbService.GetAsync<LauncherVersionModel>();
                //recentLauncher = launcherVersions.ToList().OrderByDescending(x => new Version(x.Version)).FirstOrDefault();

                //if (launcherVersions.Count == 0)
                //{
                //    CreateFolderPath(latestPath);
                //    MoveFileTest(latestPath, file);
                //    return Results.Ok();
                //}



                return Results.Ok();

                void MoveFileTest(string folderPath, IFormFile file)
                {
                    string fileNameWithPath = Path.Combine(folderPath, file.FileName);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.OpenOrCreate))
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

            }).WithTags("Launcher Version")
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

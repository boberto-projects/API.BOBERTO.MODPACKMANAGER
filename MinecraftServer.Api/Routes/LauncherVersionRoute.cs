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
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.LauncherVersionDir, versionTag);
                string latestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.LauncherVersionDir, "latest");

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
                    ///Arquitetura e sistema serão sempre os momentos até começar a compilar para outras plataformas também.
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
                var pathDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.LauncherVersionDir);
                var latestDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, apiConfig.Value.LauncherVersionDir, "latest");
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

                //SE UMA VERSÃO MAIS RECENTE OU IGUAL A VERSÃO ATUAL FOR REMOVIDA, A ÚLTIMA VERSÃO SE TORNA A ÚLTIMA RELEASE.
                if (versionCompare >= 0 && Directory.Exists(latestDir))
                {
                    foreach (var file in Directory.GetFiles(latestDir))
                    {
                        File.Delete(file);
                    }

                    var lastLauncher = recentLauncherList[1];
                    var originPath = Path.Combine(pathDir, lastLauncher.Version);

                    /// movo os arquivos da nova versão para a pasta release. A versão anterior a versão mais recente removida é sempre a nova release.
                    foreach (var file in Directory.GetFiles(originPath))
                    {
                        File.Copy(file, file.Replace(originPath, latestDir), true);
                    }

                }
                ///Removo a pasta com a versão
                if (Directory.Exists(pathDirWithVersion))
                {
                    Directory.Delete(pathDirWithVersion, true);
                }

                return Results.Ok();
            }).WithTags("Launcher Version");
        }


    }
}

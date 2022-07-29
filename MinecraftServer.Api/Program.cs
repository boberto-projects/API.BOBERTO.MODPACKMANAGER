
using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api;
using MinecraftServer.Api.Config;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.RequestModels;
using MinecraftServer.Api.Services;
using MinecraftServer.Api.Utils;
using MongoDB.Bson;
using System.IO.Compression;
using System.Text.Json;
/// <summary>
/// Refatoração API BOBERTO PHP para C# estilo minimal api 18/07/2022 - 21:43
/// </summary>
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    //options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;

    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
});

var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        config.GetConnectionString("Redis");
});

builder.Services.Configure<MongoDatabaseSettings>(
    builder.Configuration.GetSection("MongoDatabase"));

builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddSingleton<IRedisService, RedisService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/modpack/{id}", async (string id, [FromServices] MongoDBService mongoDbService) =>
{
    var modpack = await mongoDbService.GetAsync(id);

    if (modpack == null)
    {
       return Results.NotFound("MODPACK não encontrado.");
    }

    return Results.Ok(Utils.ListarArquivosRecursivos(modpack));
});


app.MapPost("/modpack/update/{id}", async (string id, [FromBody] Dictionary<string, object> request, [FromServices] MongoDBService mongoDbService) =>
{
    //  request.DatetimeUpdatAt = DateTime.Now;
    var modpack = await mongoDbService.GetAsync(id);

    if (modpack == null)
    {
        return "MODPACK não encontrado.";
    }

    await mongoDbService.UpdateKeyPairAsync(id, request);

    return "OK";
});

app.MapPost("/modpack/upload/{id}", async (string id, HttpRequest request, [FromServices] MongoDBService mongoDbService) =>
{

    var modpack = await mongoDbService.GetAsync(id);

    if(modpack == null)
    {
        return "MODPACK não encontrado.";
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
    return "OK";
})
.Accepts<IFormFile>("multipart/form-data")
.Produces(200);

app.MapPost("/modpack/add", async (ModPackRequest request, [FromServices] MongoDBService mongoDbService) =>
{
    request.DatetimeCreatAt = DateTime.Now;
    await mongoDbService.CreateAsync(request.ToMap());

    return "OK";
});

app.Run();


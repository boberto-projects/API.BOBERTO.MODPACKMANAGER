
using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api;
using MinecraftServer.Api.Config;
using MinecraftServer.Api.Models;
using MinecraftServer.Api.Services;
using MinecraftServer.Api.Utils;
/// <summary>
/// Refatoração API BOBERTO PHP para C# estilo minimal api 18/07/2022 - 21:43
/// </summary>
var builder = WebApplication.CreateBuilder(args);
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


app.MapGet("/modpack/{id}", (string id) =>
{
    var modpack = Utils.ObterModPacks().First(e => e.Id.Equals(id));
    return Utils.ListarArquivosRecursivos(modpack);
    //   return "OK";
});

app.MapPost("/modpack/add", async (ModPackModel request, [FromServices] MongoDBService mongoDbService) =>
{

    await mongoDbService.CreateAsync(request);
    return "OK";
    //   return "OK";
});

app.Run();


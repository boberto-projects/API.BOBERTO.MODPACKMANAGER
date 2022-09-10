
using MinecraftServer.Api;
using MinecraftServer.Api.Services;
using MinecraftServer.Api.Routes;
using MinecraftServer.Api.Middlewares;
using System.Text.Json.Serialization;
using MinecraftServer.Api.Seeds;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;
using MinecraftServer.Api.Models;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Refatoração API BOBERTO PHP para C# estilo minimal api 18/07/2022 - 21:43
/// </summary>
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = null);

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

MongoDBServiceDI.RegistrarDI(builder.Services, config);
builder.Services.AddSingleton<ApiCicloDeVida>();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddDirectoryBrowser();

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; 
});
builder.Services.Configure<ApiConfig>(options => config.GetSection("ApiConfig").Bind(options));


var app = builder.Build();


app.CriarMiddlewareCasimiro();

app.MapGet("", ([FromServices] ApiCicloDeVida apiCicloDeVida) =>
{
    return "Último deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
}).WithTags("Health Check");

//app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

ModPackRoute.CriarRota(app);
LauncherVersionRoute.CriarRota(app);
ConfigRoute.CriarRota(app);


if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(
           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.GetSection("ApiConfig").GetSection("CaminhoModPacks").Value), Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.None),
    RequestPath = "/files",
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.GetSection("ApiConfig").GetSection("CaminhoModPacks").Value), Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.None),
    RequestPath = "/files",

});

app.Run();

public static class MongoDBServiceDI {

    public static void RegistrarDI(this IServiceCollection services, IConfigurationRoot config)
    {
        services.Configure<MongoDatabaseSettings>(options => config.GetSection("MongoConnections").Bind(options));
        services.AddSingleton<ModPackMongoDBService>();
        services.AddSingleton<LauncherVersionMongoDBService>();
        services.AddSingleton<LauncherConfigMongoDBService>();
       
        var sp = services.BuildServiceProvider();
        createConfigCollection.CreateConfigDefaulCollection(sp);
        createVersionCollection.CreateVersionDefaulCollection(sp);
    }
}
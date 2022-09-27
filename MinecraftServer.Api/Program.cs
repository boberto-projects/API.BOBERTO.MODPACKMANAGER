
using MinecraftServer.Api;
using MinecraftServer.Api.Services;
using MinecraftServer.Api.Routes;
using MinecraftServer.Api.Middlewares;
using MinecraftServer.Api.Seeds;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;
using MinecraftServer.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using ConfigurationSubstitution;

/// <summary>
/// Refatoração API BOBERTO PHP para C# estilo minimal api 18/07/2022 - 21:43
/// </summary>
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(o => o.Limits.MaxRequestBodySize = null);

//alterando configuração de ambientes. Agora vamos subir no Dokku de forma mais gerenciada.

var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .EnableSubstitutions("%", "%")
            .Build();

builder.Services.Configure<ApiConfig>(options => config.GetSection("ApiConfig").Bind(options));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = config.GetConnectionString("Redis");
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
builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
                ("BasicAuthentication", null);

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();


app.MapGet("", ([FromServices] ApiCicloDeVida apiCicloDeVida) =>
{
    var ultimoDeploy =  "Último deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
    var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var appsettings = config.GetConnectionString("Redis");
    return ultimoDeploy + Environment.NewLine + "Ambiente:" + ambiente + Environment.NewLine + appsettings;
}).WithTags("Health Check");



app.CriarMiddlewareCasimiro();
app.UseAuthentication();
app.UseAuthorization();

CriarPastaModPacks();
CriarPastaLauncherVersions();

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

void CriarPastaModPacks()
{
    var dirMods = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.GetSection("ApiConfig").GetSection("CaminhoModPacks").Value);
    if (Directory.Exists(dirMods) == false)
    {
        Directory.CreateDirectory(dirMods);
    }
};

void CriarPastaLauncherVersions()
{
    var dirLauncherVersions = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.GetSection("ApiConfig").GetSection("CaminhoLauncherVersion").Value);
    if (Directory.Exists(dirLauncherVersions) == false)
    {
        Directory.CreateDirectory(dirLauncherVersions);
    }
};

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




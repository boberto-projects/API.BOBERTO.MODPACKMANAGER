
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

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


//Alterando configuração de ambientes. Agora vamos subir no Dokku de forma mais segredada por ambiente.

var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .EnableSubstitutions("%", "%")
            .Build();

DependencyInjection.RegistrarDI(builder.Services, config);
AutoStartup.Start(builder.Services, config);

var app = builder.Build();


app.CriarMiddlewareCasimiro();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", ([FromServices] ApiCicloDeVida apiCicloDeVida) =>
{
    var ultimoDeploy =  "Último deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
    var upTime = DateTime.Now.Subtract(apiCicloDeVida.iniciouEm).ToString("c");
    var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    
    return ultimoDeploy + Environment.NewLine + "Ambiente:" + ambiente + Environment.NewLine + upTime;
}).WithTags("Health Check");


ModPackRoute.CriarRota(app);
LauncherVersionRoute.CriarRota(app);
ConfigRoute.CriarRota(app);

if (config.GetSection("ApiConfig").Get<ApiConfig>().Swagger)
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
//launcher version
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(
           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.GetSection("ApiConfig").GetSection("CaminhoLauncherVersion").Value), Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.None),
    RequestPath = "/launcher",
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.GetSection("ApiConfig").GetSection("CaminhoLauncherVersion").Value), Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.None),
    RequestPath = "/launcher",
});

app.Run();






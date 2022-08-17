
using MinecraftServer.Api;
using MinecraftServer.Api.Services;
using MinecraftServer.Api.Routes;
using MinecraftServer.Api.Middlewares;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MinecraftServer.Api.Seeds;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Microsoft.AspNetCore.Http.Features;

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

builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // ignore omitted parameters on models to enable optional params (e.g. User update)
});

//builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

MongoDBServiceDI.RegistrarDI(builder.Services, config);

builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddDirectoryBrowser();
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
});
var app = builder.Build();

//app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

ModPackRoute.CriarRota(app);
LauncherVersionRoute.CriarRota(app);
ConfigRoute.CriarRota(app);

if (app.Environment.IsDevelopment())
{
    app.CriarMiddlewareCasimiro();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(
           Path.Join(AppDomain.CurrentDomain.BaseDirectory, Config.CaminhoModPacks), Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.None),
    RequestPath = "/files",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
             "Content-Type", "application/force-download");
    }
});

//app.UseDirectoryBrowser(new DirectoryBrowserOptions
//{
//    FileProvider = new PhysicalFileProvider(
//           Path.Combine(builder.Environment.ContentRootPath, Config.CaminhoModPacks), Microsoft.Extensions.FileProviders.Physical.ExclusionFilters.None),
//    RequestPath = "/files",

//});

app.Run();

public static class MongoDBServiceDI {

    public static void RegistrarDI(this IServiceCollection services, IConfigurationRoot config)
    {
        services.Configure<MongoDatabaseSettings>(options => config.GetSection("MongoConnections").Bind(options));
        services.AddSingleton<ModPackMongoDBService>();
        services.AddSingleton<LauncherVersionMongoDBService>();
        services.AddSingleton<ConfigMongoDBService>();

        var sp = services.BuildServiceProvider();
        createConfigCollection.CreateConfigDefaulCollection(sp);
        createVersionCollection.CreateVersionDefaulCollection(sp);
    }
}
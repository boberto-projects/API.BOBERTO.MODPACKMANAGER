
using MinecraftServer.Api;
using MinecraftServer.Api.Services;
using MinecraftServer.Api.Routes;
using MinecraftServer.Api.Middlewares;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MinecraftServer.Api.Seeds;

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

builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    // ignore omitted parameters on models to enable optional params (e.g. User update)
    x.JsonSerializerOptions.IgnoreNullValues = true;
});

//builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

MongoDBServiceDI.RegistrarDI(builder.Services, config);

builder.Services.AddSingleton<IRedisService, RedisService>();

var app = builder.Build();
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

ModPackRoute.CriarRota(app);
LauncherVersionRoute.CriarRota(app);
ConfigRoute.CriarRota(app);

if (app.Environment.IsDevelopment())
{
    app.CriarMiddlewareCasimiro();
    app.UseSwagger();
    app.UseSwaggerUI();
}
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
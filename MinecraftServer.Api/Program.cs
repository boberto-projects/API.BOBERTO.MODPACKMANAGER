
using MinecraftServer.Api.Config;
using MinecraftServer.Api.Utils;
/// <summary>
/// Refatoração API BOBERTO PHP para C# estilo minimal api 18/07/2022 - 21:43
/// </summary>
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/modpack/{id}", (string id) =>
{
    var modpack = Utils.ObterModPacks().First(e=>e.Id.Equals(id));
    return Utils.ListarArquivosRecursivos(modpack);
 //   return "OK";
})
.WithName("minecraft");


app.Run();


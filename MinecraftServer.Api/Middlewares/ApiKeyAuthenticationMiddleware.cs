using System.Reflection.PortableExecutable;

namespace MinecraftServer.Api.Middlewares
{
    //middleware based on https://www.c-sharpcorner.com/article/using-api-key-authentication-to-secure-asp-net-core-web-api/
    //because i need to fix this and the time is short.
    // oh no. I need to play minecraft now because i lost my billing account to digital ocean.
    //Now i need to force a deploy.
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private
        const string APIKEY = "ApiKey";
        public ApiKeyAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            var apiConfig = appSettings.GetSection("ApiConfig").GetSection("Authorization");

            if (!apiConfig.GetValue<bool>("Activate"))
            {
                await _next(context);
            }

            if (!context.Request.Headers.TryGetValue(apiConfig.GetValue<string>("ApiHeader"), out
                    var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided ");
                return;
            }
            if (!apiConfig.GetValue<string>("ApiKey").Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            await _next(context);
        }
    }
}

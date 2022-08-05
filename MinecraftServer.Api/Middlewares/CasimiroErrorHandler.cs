using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Web;

namespace MinecraftServer.Api.Middlewares
{
    public static class CasimiroErrorHandler
    {
        private const string STACKOVERFLOW_URL = "https://stackoverflow.com/search?q=";

        public static void CriarMiddlewareCasimiro(this WebApplication app)
        {
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerPathFeature>();

                    var frasesCasimiro = new List<string>() { "Meteu essa?", "Isso que dá gastar dinheiro com merda!",
                        "DENTROOOOO!", "Nerdola meteu essa. kkkkkkk" };
                    var rnd = new Random();

                    var message = new
                    {
                        Frase = frasesCasimiro[rnd.Next(frasesCasimiro.Count())],
                        StackOverFlow = STACKOVERFLOW_URL + HttpUtility.UrlEncode(error?.Error.Message),
                        StackTrace = error?.Error.StackTrace
                    };

                    await context.Response.WriteAsJsonAsync(message);


              
                });
            });
        }
    }
}

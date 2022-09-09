using Microsoft.AspNetCore.Diagnostics;
using MinecraftServer.Api.Exceptions;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Web;

namespace MinecraftServer.Api.Middlewares
{
    public static class CasimiroErrorHandler
    {

        public static void CriarMiddlewareCasimiro(this WebApplication app)
        {
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {

                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerPathFeature>();



                    switch (error?.Error)
                    {
                        case GenericValidateException:
                            var genericValidate = (GenericValidateException)error.Error;
                            context.Response.StatusCode = (int)genericValidate.Type;
                            var fraseCasimiro = ObterFraseDeEfeitoCasimiro(genericValidate);
                            await context.Response.WriteAsJsonAsync(JsonSerializer.Serialize(fraseCasimiro));

                            break;
                    }





                });
            });

            CasimiroMessage ObterFraseDeEfeitoCasimiro(GenericValidateException genericError)
            {
                return new CasimiroMessage(genericError.Type, genericError.StackTrace, genericError.Message);
            }
        }
    }

    public class CasimiroMessage
    {
        private const string STACKOVERFLOW_URL = "https://stackoverflow.com/search?q=";

        private List<string> FrasesCasimiroValidacao = new List<string>() { "Meteu essa?", "Isso que dá gastar dinheiro com merda!",
                        "DENTROOOOO! Só que não, né doidão?!", "Nerdola meteu essa. kkkkkkk", "Porra. Cartão amarelo, doidão. Faz certo que dá certo." };

        private List<string> FrasesCasimiroErroGenerico = new List<string>() { "Todo dia sai na rua dois otários. Hoje você além de otário, quebrou o servidor",
                "hmmmmm, que papinho, hein?!", "Porra mané, nem se eu fosse um corsa eu capotava assim.", "Muito otário, mané. Toma um link do stackoverflow pra ficar esperto."};

        public string Frase { get; set; }
        public string StackOverFlow { get; set; }
        public string StackTrace { get; set; }

        public CasimiroMessage()
        {
            Frase = "Porra, mané. Meteu essa? Nem sei que porra é essa.";
        }

        public CasimiroMessage(string frase)
        {
            Frase = frase;
        }

        public CasimiroMessage(ExceptionType type, string stackTrace, string errorMessage)
        {
            StackOverFlow = "Que papinho, hein?! Não fez merda hoje.";
            Random rnd;
            string fraseGenerica = "";
            switch (type)
            {
                case ExceptionType.Generico:
                    rnd = new Random();
                    fraseGenerica = FrasesCasimiroErroGenerico[rnd.Next(FrasesCasimiroErroGenerico.Count())];
                    Frase = fraseGenerica;
                    StackTrace = stackTrace;
                    StackOverFlow = STACKOVERFLOW_URL + HttpUtility.UrlEncode(errorMessage);
                    break;

                case ExceptionType.Validacao:
                    rnd = new Random();
                    fraseGenerica = FrasesCasimiroValidacao[rnd.Next(FrasesCasimiroValidacao.Count())];
                    Frase = fraseGenerica;
                    StackTrace = stackTrace;
                    break;
            }
        }
    }
}

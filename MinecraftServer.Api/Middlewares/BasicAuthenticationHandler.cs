using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace MinecraftServer.Api.Middlewares
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IOptions<ApiConfig> _apiConfig { get; set; }
        public BasicAuthenticationHandler(IOptions<ApiConfig> apiConfig, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _apiConfig = apiConfig;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_apiConfig.Value.Authorization.Activate == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Api key não informada"));
            }
     
            if (Request.Headers.TryGetValue(_apiConfig.Value.Authorization.ApiHeader, out
                   var extractedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("Api key não informada"));
            }

            if (_apiConfig.Value.Authorization.ApiKey.Equals(extractedApiKey) == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Não autorizado"));
            }

            return Task.FromResult(AuthenticateResult.NoResult());
        }
    } 
}

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WeatherService.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string ClaimsIdentity = "ApiKeyIdentity";
        public const string ApiKeyIdentifier = "key";

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string key = string.Empty;

            if (Request.Headers[ApiKeyIdentifier].Any())
                key = Request.Headers[ApiKeyIdentifier].FirstOrDefault();
            else if (Request.Query.ContainsKey(ApiKeyIdentifier))
                if (Request.Query.TryGetValue(ApiKeyIdentifier, out var queryKey))
                    key = queryKey;

            if (string.IsNullOrWhiteSpace(key))
                return Task.FromResult(AuthenticateResult.Fail("No API Key provided"));
            
            if (Options.ApiKey.All(k => k != key))
                return Task.FromResult(AuthenticateResult.Fail("The API Key provided is not valid"));

            var identities = new List<ClaimsIdentity> { new ClaimsIdentity(ClaimsIdentity) };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
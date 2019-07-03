using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace WeatherService.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "ApiKey";
        public string Scheme => DefaultScheme;
        public StringValues ApiKey { get; set; }
    }
}
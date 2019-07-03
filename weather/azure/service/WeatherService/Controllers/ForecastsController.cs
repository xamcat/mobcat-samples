using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherService.Controllers
{
    [Route("api/forecasts")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ForecastsController : ControllerBase
    {
        const string OpenWeatherMapImperialUnits = "imperial";
        const string OpenWeatherMapMetricUnits = "metric";
        static HttpClient _client;
        static string _appId;
        readonly IHttpClientFactory _httpClientFactory;
        readonly IConfiguration _configuration;
        readonly ILogger _logger;
        readonly IDistributedCache _cache;

        string AppId => _appId ?? (_appId = ResolveAppId());

        HttpClient OpenWeatherMapHttpClient => _client ?? (_client = _httpClientFactory.CreateClient(ServiceConstants.OpenWeatherMapHttpClientIdentifier));

        public ForecastsController(IConfiguration config, ILogger<ForecastsController> logger, IHttpClientFactory httpClientFactory, IDistributedCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = config;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet("coordinates")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotAcceptable)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> WeatherByCoordinates([FromQuery]double latitude, [FromQuery]double longitude, [FromQuery]string units = default(string))
        {
            string notAcceptableResponse = string.Empty;

            if (latitude < -90 || latitude > 90)
                notAcceptableResponse = $"The {latitude} parameter must be between -90 and 90";

            if (longitude < -180 || longitude > 180)
                notAcceptableResponse = $"The {longitude} parameter must be between -180 and 180";

            if (!string.IsNullOrWhiteSpace(notAcceptableResponse))
                return new ContentResult { StatusCode = (int)HttpStatusCode.NotAcceptable, Content = notAcceptableResponse };

            if (string.IsNullOrWhiteSpace(units))
                units = OpenWeatherMapMetricUnits;
            else
                units = units.ToLower();

            if (units != OpenWeatherMapImperialUnits && units != OpenWeatherMapMetricUnits)
                return new ContentResult { StatusCode = (int)HttpStatusCode.NotAcceptable, Content = $"Units must be either {OpenWeatherMapMetricUnits} or {OpenWeatherMapImperialUnits}" };

            object responseObject = null;

            try
            {
                string json = string.Empty;
                var coordinateKey = $"{latitude.ToString().Replace(".", "")}{longitude.ToString().Replace(".", "")}";
                var cacheKey = $"{ServiceConstants.CachePrefixForecasts}-{coordinateKey}-{units}";
                var cachedResponse = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrWhiteSpace(cachedResponse))
                    json = cachedResponse;
                else
                    json = await OpenWeatherMapHttpClient.GetStringAsync($"?lat={latitude}&lon={longitude}&units={units}&appid={AppId}");

                if (string.IsNullOrWhiteSpace(cachedResponse) && !string.IsNullOrWhiteSpace(json))
                    await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

                var jo = JObject.Parse(json);

                responseObject = new
                {
                    name = jo.Value<string>("name"),
                    currentTemperature = jo["main"].Value<string>("temp"),
                    minTemperature = jo["main"].Value<string>("temp_min"),
                    maxTemperature = jo["main"].Value<string>("temp_max"),
                    id = jo.Value<string>("id"),
                    description = ((JArray)jo["weather"])[0].Value<string>("description"),
                    overview = ((JArray)jo["weather"])[0].Value<string>("main"),
                    country = jo["sys"].Value<string>("country")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            return new JsonResult(responseObject);
        }

        [HttpGet("{name}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotAcceptable)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> WeatherByCity(string name, [FromQuery]string units = default(string))
        {
            name = name.ToLower();

            if (string.IsNullOrWhiteSpace(units))
                units = OpenWeatherMapMetricUnits;
            else
                units = units.ToLower();

            if (units != OpenWeatherMapImperialUnits && units != OpenWeatherMapMetricUnits)
            {
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.NotAcceptable,
                    Content = $"Units must be either {OpenWeatherMapMetricUnits} or {OpenWeatherMapImperialUnits}"
                };
            }                

            object responseObject = null;

            try
            {
                string json = string.Empty;
                var cacheKey = $"{ServiceConstants.CachePrefixForecasts}-{name}-{units}";
                var cachedResponse = await _cache.GetStringAsync(cacheKey);

                if (!string.IsNullOrWhiteSpace(cachedResponse))
                    json = cachedResponse;
                else
                    json = await OpenWeatherMapHttpClient.GetStringAsync($"?q={name}&units={units}&appid={AppId}");

                if (string.IsNullOrWhiteSpace(cachedResponse) && !string.IsNullOrWhiteSpace(json))
                    await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

                var jo = JObject.Parse(json);

                responseObject = new
                {
                    name = jo.Value<string>("name"),
                    currentTemperature = jo["main"].Value<string>("temp"),
                    minTemperature = jo["main"].Value<string>("temp_min"),
                    maxTemperature = jo["main"].Value<string>("temp_max"),
                    id = jo.Value<string>("id"),
                    description = ((JArray)jo["weather"])[0].Value<string>("description"),
                    overview = ((JArray)jo["weather"])[0].Value<string>("main"),
                    country = jo["sys"].Value<string>("country")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }            

            return new JsonResult(responseObject);
        }

        private string ResolveAppId()
        {
            var openWeatherMapAppIdSettingName = _configuration.GetValue<string>(ServiceConstants.OpenWeatherMapAppIdSettingName);
            var openWeatherMapAppId = _configuration.GetValue<string>(openWeatherMapAppIdSettingName);
            return openWeatherMapAppId;
        }
    }
}
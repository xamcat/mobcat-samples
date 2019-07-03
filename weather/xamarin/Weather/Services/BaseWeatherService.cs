using System;
using System.Net.Http;
using Microsoft.MobCAT.Services;

namespace Weather.Services
{
	public class BaseWeatherService : BaseHttpService
    {
        readonly string _apiKey;

        protected string ApiKey => _apiKey;

        public BaseWeatherService(string apiBaseAddress, string apiKey, HttpMessageHandler handler = null) 
            : base(apiBaseAddress, handler)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException(nameof(apiKey));

            _apiKey = apiKey;

            Serializer = new NewtonsoftJsonSerializer();
        }

        internal void SetApiKeyHeader(HttpRequestMessage request)
        {
            request.Headers.Add(ServiceConstants.ApiKeyHeaderName, _apiKey);
        }
    }
}
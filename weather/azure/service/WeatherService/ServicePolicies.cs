using System;
using System.Net;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace WeatherService
{
    internal static class ServicePolicies
    {
        static Random _jitter = new Random();

        internal static IAsyncPolicy<HttpResponseMessage> ExponentialRetryPolicyWithJitter()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(_jitter.Next(0, 100)));
        }

        internal static IAsyncPolicy<HttpResponseMessage> ExponentialRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        internal static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        internal static IAsyncPolicy<HttpResponseMessage> FallbackPolicy(Action fallbackAction, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .FallbackAsync<HttpResponseMessage>((token) => 
                {
                    fallbackAction?.Invoke();
                    return System.Threading.Tasks.Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(statusCode));
                });
        }
    }
}
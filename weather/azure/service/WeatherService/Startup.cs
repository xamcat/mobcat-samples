using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using WeatherService.Authentication;

namespace WeatherService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var apiAuthKeySettingName = Configuration.GetValue<string>(ServiceConstants.ApiKeySettingName);
            var apiAuthKey = Configuration.GetValue<string>(apiAuthKeySettingName);

            var cacheConnectionStringSettingName = Configuration.GetValue<string>(ServiceConstants.CacheConnectionStringSettingName);
            var cacheConnectionString = Configuration.GetValue<string>(cacheConnectionStringSettingName);

            var appInsightsInstrumentationKeySettingName = Configuration.GetValue<string>(ServiceConstants.AppInsightsSettingName);
            var appInsightsInstrumentationKey = Configuration.GetValue<string>(appInsightsInstrumentationKeySettingName);

            var cognitiveServicesBingImageKeySettingName = Configuration.GetValue<string>(ServiceConstants.CognitiveServicesSearchSettingName);
            var cognitiveServicesBingImageKey = Configuration.GetValue<string>(cognitiveServicesBingImageKeySettingName);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, (options) =>
            {
                options.ApiKey = apiAuthKey;
            });

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = cacheConnectionString;
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Info
                {
                    Title = "Weather Service",
                    Version = "v1.0",
                    Description = "ASP.NET Core service to support the Xamarin Weather app sample",
                    Contact = new Contact
                    {
                        Name = "Microsoft Mobile CAT (Customer Advisory Team)",
                        Url = "https://github.com/xamarin/mobcat"
                    }
                });

                var securityRequirement = new Dictionary<string, IEnumerable<string>>
                {
                    { ApiKeyAuthenticationHandler.ApiKeyIdentifier, new string[] { } }
                };

                options.AddSecurityDefinition(ApiKeyAuthenticationHandler.ApiKeyIdentifier, new ApiKeyScheme
                {
                    Description = $"API key header using the {ApiKeyAuthenticationHandler.ApiKeyIdentifier} scheme. Example: {ApiKeyAuthenticationHandler.ApiKeyIdentifier}: key_value",
                    Name = ApiKeyAuthenticationHandler.ApiKeyIdentifier,
                    In = "header",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(securityRequirement);
            });

            services.AddApplicationInsightsTelemetry(appInsightsInstrumentationKey);

            services.AddHttpClient(ServiceConstants.GeneralHttpClientIdentifier)
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(ServicePolicies.ExponentialRetryPolicyWithJitter())
                .AddPolicyHandler(ServicePolicies.CircuitBreakerPolicy())
                .AddPolicyHandler(ServicePolicies.FallbackPolicy(ResetService));

            services.AddHttpClient(ServiceConstants.OpenWeatherMapHttpClientIdentifier, client =>
            {
                client.BaseAddress = new Uri(ServiceConstants.OpenWeatherMapApiBaseAddress);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(ServicePolicies.ExponentialRetryPolicyWithJitter())
            .AddPolicyHandler(ServicePolicies.CircuitBreakerPolicy())
            .AddPolicyHandler(ServicePolicies.FallbackPolicy(ResetService));

            services.AddHttpClient(ServiceConstants.BingImagesSearchHttpClientIdentifier, client =>
            {
                client.BaseAddress = new Uri(ServiceConstants.BingImagesSearchApiBaseAddress);
                client.DefaultRequestHeaders.Add(ServiceConstants.BingImagesSearchApiKeyHeaderName, cognitiveServicesBingImageKey);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(ServicePolicies.ExponentialRetryPolicyWithJitter())
            .AddPolicyHandler(ServicePolicies.CircuitBreakerPolicy())
            .AddPolicyHandler(ServicePolicies.FallbackPolicy(ResetService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseAuthentication();

            loggerFactory.AddConsole();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Weather Service v1.0");
                options.DocumentTitle = "Weather Service Documentation";
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void ResetService()
        {
            Program.TriggerShutdown();
        }        
    }
}
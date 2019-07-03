using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System.Threading;

namespace WeatherService
{
    public class Program
    {
        private static CancellationTokenSource cancellationTokenSource;

        internal static void TriggerShutdown()
        {
            cancellationTokenSource.Cancel();
        }

        public static void Main(string[] args)
        {
            cancellationTokenSource = new CancellationTokenSource();
            BuildWebHost(args).RunAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    string keyVaultEndpoint = string.Empty;

                    builder.AddJsonFile("localsettings.json", true, false);
                    var localConfiguration = builder.Build();
                    var useKeyVault = localConfiguration.GetValue<bool>(ServiceConstants.UseKeyVaultSettingName, true);

                    if (!useKeyVault)
                    {
                        // This file should not be committed to source. 
                        // It should be used only when using KeyVault from a local workstation is not possible
                        builder.AddJsonFile("localsecrets.json", true, false);
                        return;
                    }

                    keyVaultEndpoint = localConfiguration.GetValue<string>(ServiceConstants.KeyVaultEndpointSettingName, string.Empty);

                    if (!string.IsNullOrWhiteSpace(keyVaultEndpoint))
                    {
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                        builder.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                    }
                })
            .Build();
    }
}
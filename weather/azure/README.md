# WeatherService on Azure
The [Weather app](https://github.com/xamarin/mobcat/blob/dev/samples/weather/xamarin/README.md) is underpinned by a basic web service built with [asp.net core 2.1](https://blogs.msdn.microsoft.com/dotnet/2018/05/30/announcing-net-core-2-1/). You will need to host this service yourself in order to run the client sample.  

This folder contains the source code for the **WeatherService** along with the supporting scripts and templates for provisioning the underlying cloud resources to an [Azure subscription](https://azure.microsoft.com/en-gb/pricing/purchase-options/). 

## Solution overview
The following [Azure](https://portal.azure.com) resources are provisioned as part of the solution.  

- [App Service](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-overview)
- [Application Insights](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-detect-triage-diagnose)
- [Cognitive Services](https://azure.microsoft.com/en-gb/services/cognitive-services/)
- [KeyVault](https://azure.microsoft.com/en-gb/services/key-vault/)
- [Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview)
- [Redis Cache](https://azure.microsoft.com/en-gb/services/cache/)

## Prerequisites
The following dependencies are required in order to follow the steps outlined below.  

- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Microsoft Azure Subscription](https://azure.microsoft.com/en-gb/pricing/purchase-options/)
- [.NET Core 2.1 SDK](https://www.microsoft.com/net/download/dotnet-core/2.1)
- [OpenWeatherMap APP ID](https://openweathermap.org/appid)

You can sign up for a [free trial](https://azure.microsoft.com/free/) of Azure in order to further explore the concepts in this sample if you do not have a subscription available for these purposes.  

You must have permission to create **Resource Groups** on the **Azure Subscription** in order to complete the steps below.

## Provisioning the service
Follow the steps below to provision the **WeatherService**, and the underlying resources, to your [Azure Subscription](https://portal.azure.com). In the interest of brevity, the term '*CMD*' applies to both the Windows **Command Prompt** and the macOS **Terminal**.  

### Acquiring an AppID for OpenWeatherMap
1. Navigate to the **OpenWeatherMap** [sign up page](https://home.openweathermap.org/users/sign_up) 
2. Follow the steps to create a free account (you can use your existing account if you have one)
3. Click on the **API Keys** tab from the [home page](https://home.openweathermap.org/)
3. Take a note of your **Default** key for use in later steps

**NOTE:** There may be a delay between sign up and the key being activated. While this will prevent the use of the **WeatherService** it should not prevent you from following the remaining steps in this section.

### Validate you have Azure CLI and .NET Core SDK installed
1. Open the **CMD**
2. Validate you have **Azure CLI** version *2.0* or higher

    ```
    az --version
    ```
    
    In order to install or update the **Azure CLI** follow this [complete guide](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) or use the following instructions:

    **macOS:** 
    ```
    brew update
    brew install azure-cli
    brew upgrade azure-cli
    ```
    **Windows:** [MSI installer](https://aka.ms/installazurecliwindows)

3. Validate you have **.NET Core SDK** version *2.1* or higher

    ```
    dotnet --version
    ```

    In order to install or update the SDK download the [.NET Core SDK Installer](https://dotnet.microsoft.com/download)

### Executing the provisioning script
1. Open the **CMD** (if you do not have it open from the previous steps)
2. Change the directory to the '*mobcat\samples\weather\azure\infrastructure\Scripts*' folder
3. Login to **Azure** using the **Azure CLI** login command

    ```
    az login
    ```
4. Follow the **browser** prompts to login to **Azure**
5. Return to **CMD** and review the details of the subscriptions listed e.g.

    ```
    {
        "cloudName": "AzureCloud",
        "id": "<subscription_id>",
        "isDefault": true,
        "name": "<subscription_name>",
        "state": "Enabled",
        "tenantId": "<tenant_id>",
        "user": {
            "name": "<account_name>",
            "type": "user"
        }
    }
    ```
6. Take note of the following values for your chosen **Azure Subscription**. You may have more than one subscription listed:

    - subscription_id
    - tenant_id
    - account_name
7. Choose (or generate) an API key for **WeatherService**. This API key will be used to prevent the **WeatherService** unauthorized access. Use it as the **your_api_key** parameter later. There are a number of [guid generator](https://www.bing.com/search?q=guid+generator) tools available online which might help with this, however a passphrase would suffice for the purposes of this sample
8. From **CMD**, call the respective **weather_deploy** script passing in the requisite parameters. For example:

    **macOS:**
    ```
    ./weather_deploy.sh --tenant <tenant_id> --subscription <subscription_id> --admin <account_name> --openweathermap-appid <openweathermap_key> --api-key <your_api_key>
    ```

    **Windows:**
    ```
    weather_deploy.bat --tenant <tenant_id> --subscription <subscription_id> --admin <account_name> --openweathermap-appid <openweathermap_key> --api-key <your_api_key>
    ```

**NOTE:** The script may take upwards of 25 minutes to complete when executed for the first time. Templated deployments are incremental and subsequent executions will take significantly less time to complete.  

### Reviewing the deployment
1. Navigate to the [Azure Portal](https://portal.azure.com)
2. Find the **WeatherSample** Resource Group using the **Resource Groups** tab
3. Validate that all [expected resources](#solution-overview) have been provisioned
3. Click on the **App Service** resource (starting '*asapi*')
4. Click on the link (marked **URL**) from within the **Overview** section
5. Click the **Authorize** button at the top of the [Swagger UI](https://swagger.io/tools/swagger-ui/)
6. Paste in your **API key** (the one you used for the '*--api-key*' script parameter) into the **Value** field, then click **Authorize**
7. The following API endpoints should appear in the list:
    ```
    /api/forecasts/coordinates
    /api/forecasts/{name}
    /api/images/{name}
    /api/images/downloads
    ```
8. Try the APIs via the **Try it out** feature

## Further Considerations

### Running locally
You can also run the **WeatherService** locally for development and testing purposes using [Visual Studio](https://visualstudio.microsoft.com/vs/). The respective solution/workspace files can be found within the '*mobcat\samples\weather\azure*' directory.  

To open with [Visual Studio 2017](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Enterprise&rel=15) or [Visual Studio for Mac](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio-mac/?sku=enterprisemac&rel=15) **double-click** on **WeatherService.sln** or **weather_service.code-workspace** to open with [Visual Studio Code](https://code.visualstudio.com/). 

#### Security warnings
Your browser may present a security warning if you have not installed and trusted the appropraite local certificate. To address this you can  execute the *dev-certs* command (provided as part of the **.NET Core 2.1** tooling). For example:  

```
dotnet dev-certs https --trust
```

#### Working with KeyVault locally using MSI (Managed Service Identity)
When running the service locally you should ensure you are signed into Azure (via the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)) using the account that was used execute the script or one that has been subsequently given the same or greater [KeyVault](https://azure.microsoft.com/en-gb/services/key-vault/) permissions via **Access Policy**. Alternatively, you can use the [Azure Services Authentication](https://marketplace.visualstudio.com/items?itemName=chrismann.MicrosoftVisualStudioAsalExtension) extension for [Visual Studio 2017](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Enterprise&rel=15). Once installed, you can go to *Tools > Options > Azure Service Authentication* to choose an account and sign in.  

You will also need to create a file called **localsettings.json** within the **WeatherService** folder (at the same level as the **WeatherService.csproj** file). This must contain the endpoint for your [KeyVault](https://azure.microsoft.com/en-gb/services/key-vault/) resource. For example:

```
{
  "KEYVAULT_ENDPOINT": "https://<keyvault_name>.vault.azure.net"
}
```

If you are unable to work with [KeyVault](https://azure.microsoft.com/en-gb/services/key-vault/) directly in this way (or choose to skip this at any time), you can update **appsettings.Development.json** setting the **USE_KEYVAULT** flag to **false**. You can also edit the application settings that would be picked up at runtime when hosted on [App Service](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-overview). For example: 

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "APP_INSIGHTS_KEY_NAME": "appinskey",
  "CACHE_CONNECTION_STRING_KEY_NAME": "cacheconnstr",
  "COGNITIVE_SERVICES_SEARCH_KEY_NAME": "csacckey",
  "OPEN_WEATHER_MAP_APP_ID_KEY_NAME": "owmapid",
  "API_KEY_NAME": "apikey",
  "USE_KEYVAULT": false
}
```

In the event that you choose to set **USE_KEYVAULT** to false, you will need to ensure there is a file called **localsecrets.json** placed within the **WeatherService** folder (at the same level as the **WeatherService.csproj** file). This must contain the expected keys and values. For example:

```
{
  "apikey": "<api_key>",
  "appinskey": "<aplication_insights_instrumentation_key>",
  "cacheconnstr": "<redis_cache_connection_string>=",
  "csacckey": "<cognitive_services_account_key>",
  "owmapid": "<openweathermap_api_key>"
}
```

**NOTE:** This has already been added to the **gitignore** file.

#### Azure costs
There is a monthly cost associated with the Azure resources that are provisioned as part of this sample. This is primarily associated with the [App Service](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-overview) and the [Redis Cache](https://azure.microsoft.com/en-gb/services/cache/) to a lesser degree. The total cost could be reduced by moving to a [lower tier](https://azure.microsoft.com/en-gb/pricing/details/app-service/plans/) (Basic or Free) and updating the service to use [in-memory cache](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.1) instead of the [Redis Cache](https://azure.microsoft.com/en-gb/services/cache/). 

### Deploying the service
The script has been provided for convenience as this is not the focus of this sample. However, there are a number of ways to deploy to **App Service**. These include:

- [App Service VS Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azureappservice)
- [Continuous Deployment](https://docs.microsoft.com/en-us/azure/app-service/app-service-continuous-deployment)
- [Publish with Visual Studio](https://docs.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-webapp-using-vs?view=aspnetcore-2.1)
- [Upload via FTP](https://docs.microsoft.com/en-us/azure/app-service/app-service-deploy-zip#create-a-project-zip-file)
- [ZIP or WAR file](https://docs.microsoft.com/en-us/azure/app-service/app-service-deploy-zip#create-a-project-zip-file)

### Modifying the provisioning script
You can modify the **weather_deploy** scripts using any plain text editor of choice. You can also **double-click** the **weather_infrastructure.code-workspace** file from within the '*mobcat\samples\weather\azure*' directory to open [Visual Studio Code](https://code.visualstudio.com/).  

For convenience, a task has been configured enabling the use of the **build** shortcut (**CTRL+SHIFT+B** / **CMD+SHIFT+B** for Windows/macOS respectively) in [Visual Studio Code](https://code.visualstudio.com/). To take advantage of this, you must ensure that a file named **parameters** exists within the '*infrastructure/Scripts*' subdirectory with a **.bat** extension for **Windows** and **.sh** for **macOS**. For **.sh** files, you must also execute the **chmod** command to ensure it can be called by the **vscode_deploy.sh** script:

```
chmod a+x parameters.sh
```

**NOTE:** The command snippet above assumes that **Terminal** is set to the parent **Scripts** directory.
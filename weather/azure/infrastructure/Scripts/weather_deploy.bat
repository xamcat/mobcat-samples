@echo off
rem Script for provisioning the weather sample backend via Azure CLI

echo.
echo ^========== WeatherSample Provisioning started ==========
echo.

set targetTenant=
set targetSubscription=
set adminUpnOrObjectId=
set openWeatherMapAppId=
set apiKey=

rem Resolve parameters
:parse-params
if not "%1"=="" (
    if "%1"=="--tenant" (
        SET targetTenant=%2
        SHIFT
    )
    if "%1"=="--subscription" (
        SET targetSubscription=%2
        SHIFT
    )
    if "%1"=="--admin" (
        SET adminUpnOrObjectId=%2
        SHIFT
    )
    if "%1"=="--openweathermap-appid" (
        SET openWeatherMapAppId=%2
        SHIFT
    )
    if "%1"=="--api-key" (
        SET apiKey=%2
        SHIFT
    )
    SHIFT
    GOTO :parse-params
)

rem Verify all parameters have values
if not defined targetTenant (
    echo ^Missing --tenant parameter
    exit /B 1
)

if not defined targetSubscription (
    echo ^Missing --subscription parameter
    exit /B 1
)

if not defined adminUpnOrObjectId (
    echo ^Missing --admin parameter
    exit /B 1
)

if not defined apiKey (
    echo ^Missing --api-key parameter
    exit /B 1
)

if not defined openWeatherMapAppId (
    echo ^Missing --openweathermap-appid parameter
    exit /B 1
)

echo ^Account:
echo %adminUpnOrObjectId%
echo.

echo ^Tenant:
echo %targetTenant%
echo.

echo ^Subscription:
echo %targetSubscription%
echo.

set resourceGroupName=WeatherSample
set resourceGroupLocation=northeurope
set keyVaultAuditStorageAccountName=kvastg
set keyVaultName=kv
set searchAccountName=csacc
set appServicePlanName=asasp
set apiAppName=asapi
set appInsightsName=appins
set cacheName=rcache
set keyVaultEndpointSettingKey=KEYVAULT_ENDPOINT
set searchAccountInstrumentationKeyName=csacckey
set searchAccountInstrumentationKeyNameSettingKey=COGNITIVE_SERVICES_SEARCH_KEY_NAME
set appInsightsInstrumentationKeyName=appinskey
set appInsightsInstrumentationKeyNameSettingKey=APP_INSIGHTS_KEY_NAME
set cacheConnectionStringKeyName=cacheconnstr
set cacheConnectionStringKeyNameSettingKey=CACHE_CONNECTION_STRING_KEY_NAME
set apiKeyName=apikey
set apiKeyNameSettingKey=API_KEY_NAME
set openWeatherMapAppIdKeyName=owmapid
set openWeatherMapAppIdKeyNameSettingKey=OPEN_WEATHER_MAP_APP_ID_KEY_NAME

rem Login for Azure CLI must be handled as prerequisite!
echo ^Validating Azure Login
set token=
for /f "usebackq" %%i in ( `az account get-access-token --output tsv` ) do set token=%%i

if not defined token (
    echo ^Login required. Aborting
    exit /B 1
)

call az account set --subscription %targetSubscription% >nul 2>&1

rem Resolve adminId from user principal name or objectId
for /f "usebackq" %%i in ( `az ad user show --upn %adminUpnOrObjectId% --query "objectId" --output tsv` ) do set adminId=%%i

echo ^Creating or updating resource group
call az group create --name %resourceGroupName% --location %resourceGroupLocation% >nul 2>&1

echo ^Preparing globally unique resource names
for /f "usebackq" %%i in ( `az group deployment create --name GloballyUniqueNameDeployment --resource-group %resourceGroupName% --template-file ../Templates/globallyuniquename.json --query "properties.outputs.uniqueValue.value" --output tsv` ) do set uniquePostfix=%%i

set keyVaultAuditStorageAccountName=%keyVaultAuditStorageAccountName%%uniquePostfix%
set keyVaultName=%keyVaultName%%uniquePostfix%
set searchAccountName=%searchAccountName%%uniquePostfix%
set appServicePlanName=%appServicePlanName%%uniquePostfix%
set apiAppName=%apiAppName%%uniquePostfix%
set appInsightsName=%appInsightsName%%uniquePostfix%
set cacheName=%cacheName%%uniquePostfix%

echo ^Creating Application Insights
call az group deployment create --name AppInsightsDeployment --resource-group %resourceGroupName% --template-file ../Templates/appinsights.json --parameters appInsightsName=%appInsightsName% >nul 2>&1

echo ^Creating Storage for KeyVault Audit
call az group deployment create --name KvStorageAccountDeployment --resource-group %resourceGroupName% --template-file ../Templates/storageaccount.json --parameters storageAccountName=%keyVaultAuditStorageAccountName% >nul 2>&1

echo ^Creating KeyVault
call az group deployment create --name KvDeployment --resource-group %resourceGroupName% --template-file ../Templates/keyvault.json --parameters keyVaultName=%keyVaultName% tenantId=%targetTenant% objectId=%adminId% >nul 2>&1

echo ^Creating KeyVault: Configuring diagnostic settings
for /f "usebackq" %%i in ( `az keyvault list --resource-group %resourceGroupName% --query "[?name=='%keyVaultName%'].id" --output tsv` ) do set keyVaultResourceId=%%i

call az monitor diagnostic-settings create --name KeyVaultAuditDiagnosticSettings --resource %keyVaultResourceId% --resource-group %resourceGroupName% --storage-account %keyVaultAuditStorageAccountName% --logs "[{ ""category"": ""AuditEvent"", ""enabled"": true, ""retentionPolicy"": { ""days"": 0, ""enabled"": true } }]" >nul 2>&1

echo ^Creating Cognitive Services Bing Search
call az group deployment create --name CognitiveServicesBingDeployment --resource-group %resourceGroupName% --template-file ../Templates/cognitiveservicesbingsearch.json --parameters searchAccountName=%searchAccountName% >nul 2>&1

echo ^Creating Redis Cache (can take upwards of 20 minutes)
call az group deployment create --name RedisCacheDeployment --resource-group %resourceGroupName% --template-file ../Templates/rediscache.json --parameters cacheName=%cacheName% >nul 2>&1

echo ^Creating App Service Plan
call az group deployment create --name AppServicePlanDeployment --resource-group %resourceGroupName% --template-file ../Templates/appserviceplan.json --parameters servicePlanName=%appServicePlanName% >nul 2>&1

echo ^Creating API App
call az group deployment create --name ApiAppDeployment --resource-group %resourceGroupName% --template-file ../Templates/apiapp.json --parameters  appName=%apiAppName% servicePlanName=%appServicePlanName% >nul 2>&1

echo ^Creating API App: Configuring App Settings

set keyVaultEndpoint="https://%keyVaultName%.vault.azure.net"

call az webapp config appsettings set --resource-group %resourceGroupName% --name %apiAppName% --settings %searchAccountInstrumentationKeyNameSettingKey%=%searchAccountInstrumentationKeyName% >nul 2>&1
call az webapp config appsettings set --resource-group %resourceGroupName% --name %apiAppName% --settings %appInsightsInstrumentationKeyNameSettingKey%=%appInsightsInstrumentationKeyName% >nul 2>&1
call az webapp config appsettings set --resource-group %resourceGroupName% --name %apiAppName% --settings %openWeatherMapAppIdKeyNameSettingKey%=%openWeatherMapAppIdKeyName% >nul 2>&1
call az webapp config appsettings set --resource-group %resourceGroupName% --name %apiAppName% --settings %apiKeyNameSettingKey%=%apiKeyName% >nul 2>&1
call az webapp config appsettings set --resource-group %resourceGroupName% --name %apiAppName% --settings %cacheConnectionStringKeyNameSettingKey%=%cacheConnectionStringKeyName% >nul 2>&1
call az webapp config appsettings set --resource-group %resourceGroupName% --name %apiAppName% --settings %keyVaultEndpointSettingKey%=%keyVaultEndpoint% >nul 2>&1

echo ^Creating API App: Configuring Managed Service Identity
for /f "usebackq" %%i in ( `az webapp identity assign --name %apiAppName% --resource-group %resourceGroupName% --query "principalId" --output tsv` ) do set msiPrincipalId=%%i

echo ^Creating API App: Configuring Acess Policy on KeyVault
call az keyvault set-policy --name %keyVaultName% --object-id %msiPrincipalId% --secret-permissions get list >nul 2>&1

echo ^Configuring KeyVault: Retrieving secrets from resources
for /f "usebackq" %%i in ( `az cognitiveservices account keys list --name %searchAccountName% --resource-group %resourceGroupName% --query "key1" --output tsv` ) do set searchAccountInstrumentationKey=%%i
for /f "usebackq" %%i in ( `az resource show --name %appInsightsName% --resource-group %resourceGroupName% --resource-type "Microsoft.Insights/components" --query "properties.InstrumentationKey" --output tsv` ) do set appInsightsInstrumentationKey=%%i
for /f "usebackq" %%i in ( `az redis list-keys --name %cacheName% --resource-group %resourceGroupName% --query "primaryKey" --output tsv` ) do set cachePrimaryKey=%%i

set cacheConnectionString=%cacheName%.redis.cache.windows.net,abortConnect=false,ssl=true,password=%cachePrimaryKey%

echo ^Configuring KeyVault: Writing secrets
call az keyvault secret set --vault-name %keyVaultName% --name %searchAccountInstrumentationKeyName% --value %searchAccountInstrumentationKey% >nul 2>&1
call az keyvault secret set --vault-name %keyVaultName% --name %appInsightsInstrumentationKeyName% --value %appInsightsInstrumentationKey% >nul 2>&1
call az keyvault secret set --vault-name %keyVaultName% --name %cacheConnectionStringKeyName% --value %cacheConnectionString% >nul 2>&1
call az keyvault secret set --vault-name %keyVaultName% --name %apiKeyName% --value %apiKey% >nul 2>&1
call az keyvault secret set --vault-name %keyVaultName% --name %openWeatherMapAppIdKeyName% --value %openWeatherMapAppId% >nul 2>&1

echo ^Publishing API App

cd ../../service/WeatherService

echo ^Publishing API App: Creating application
call dotnet publish -c release >nul 2>&1

echo ^Publishing API App: Packaging application files
call powershell Compress-Archive -Path bin/release/netcoreapp2.1/publish/* -DestinationPath bin/release/netcoreapp2.1/publish.zip >nul 2>&1

echo ^Publishing API App: Deploying application package
call az webapp deployment source config-zip --resource-group %resourceGroupName% --name %apiAppName% --src bin/release/netcoreapp2.1/publish.zip >nul 2>&1

echo.
echo ^========== WeatherSample Provisioning completed ==========
echo.
# Script for provisioning the weather sample backend via Azure CLI

echo ""
echo "========== WeatherSample Provisioning started =========="
echo ""

# Resolve parameters
for i in "$@"; do
    case $1 in
        "" ) break ;;
        -t | --tenant  ) targetTenant="$2"; shift ;;
        -s | --subscription ) targetSubscription="$2"; shift ;;
        -a | --admin ) adminUpnOrObjectId="$2"; shift ;;
        -k | --api-key ) apiKey="$2"; shift ;;
        -i | --openweathermap-appid ) openWeatherMapAppId="$2"; shift ;;
        -* | --*) echo "Unknown option: '$1'"; exit 1 ;;
        * ) echo "Unknown argument: '$1'"; exit 1 ;;
    esac
    shift
done

# Verify all parameters have values
if [ -z "$targetTenant" ]
then
    echo "Missing --tenant parameter"
    exit 1
fi

if [ -z "$targetSubscription" ]
then
    echo "Missing --subscription parameter"
    exit 1
fi

if [ -z "$adminUpnOrObjectId" ]
then
    echo "Missing --admin parameter"
    exit 1
fi

if [ -z "$apiKey" ]
then
    echo "Missing --api-key parameter"
    exit 1
fi

if [ -z "$openWeatherMapAppId" ]
then
    echo "Missing --openweathermap-appid parameter"
    exit 1
fi

echo "Account:"
echo $adminUpnOrObjectId
echo ""

echo "Tenant:"
echo $targetTenant
echo ""

echo "Subscription:"
echo $targetSubscription
echo ""

resourceGroupName="WeatherSample"
resourceGroupLocation="northeurope"
keyVaultAuditStorageAccountName="kvastg"
keyVaultName="kv"
searchAccountName="csacc"
appServicePlanName="asasp"
apiAppName="asapi"
appInsightsName="appins"
cacheName="rcache"
keyVaultEndpointSettingKey="KEYVAULT_ENDPOINT"
searchAccountInstrumentationKeyName="csacckey"
searchAccountInstrumentationKeyNameSettingKey="COGNITIVE_SERVICES_SEARCH_KEY_NAME"
appInsightsInstrumentationKeyName="appinskey"
appInsightsInstrumentationKeyNameSettingKey="APP_INSIGHTS_KEY_NAME"
cacheConnectionStringKeyName="cacheconnstr"
cacheConnectionStringKeyNameSettingKey="CACHE_CONNECTION_STRING_KEY_NAME"
apiKeyName="apikey"
apiKeyNameSettingKey="API_KEY_NAME"
openWeatherMapAppIdKeyName="owmapid"
openWeatherMapAppIdKeyNameSettingKey="OPEN_WEATHER_MAP_APP_ID_KEY_NAME"

# Login for Azure CLI must be handled as prerequisite!
echo "Validating Azure Login"
token="$(az account get-access-token --output tsv)"

if [ -z "$token" ]
then
    echo "Login required. Aborting"
    exit 1
fi

az account set --subscription $targetSubscription 1> /dev/null

# Resolve adminId from user principal name or objectId
adminId=$(az ad user show --upn $adminUpnOrObjectId --query "objectId" --output tsv)

echo "Creating or updating resource group"
az group create \
    --name $resourceGroupName \
    --location $resourceGroupLocation \
    1> /dev/null

echo "Preparing globally unique resource names"

uniquePostfix=$(az group deployment create \
    --name GloballyUniqueNameDeployment \
    --resource-group $resourceGroupName \
    --template-file ../Templates/globallyuniquename.json \
    --query "properties.outputs.uniqueValue.value" \
    --output tsv)

keyVaultAuditStorageAccountName=$keyVaultAuditStorageAccountName$uniquePostfix
keyVaultName=$keyVaultName$uniquePostfix
searchAccountName=$searchAccountName$uniquePostfix
appServicePlanName=$appServicePlanName$uniquePostfix
apiAppName=$apiAppName$uniquePostfix
appInsightsName=$appInsightsName$uniquePostfix
cacheName=$cacheName$uniquePostfix

echo "Creating Application Insights"
az group deployment create \
    --name AppInsightsDeployment \
    --resource-group $resourceGroupName \
    --template-file ../Templates/appinsights.json \
    --parameters appInsightsName=$appInsightsName \
    1> /dev/null

echo "Creating Storage for KeyVault Audit"
az group deployment create \
    --name KvStorageAccountDeployment \
    --resource-group $resourceGroupName \
    --template-file ../Templates/storageaccount.json \
    --parameters storageAccountName=$keyVaultAuditStorageAccountName \
    1> /dev/null

echo "Creating KeyVault"
az group deployment create \
    --name KvDeployment \
    --resource-group $resourceGroupName \
    --template-file ../Templates/keyvault.json \
    --parameters keyVaultName=$keyVaultName tenantId=$targetTenant objectId=$adminId \
    1> /dev/null

echo "Creating KeyVault: Configuring diagnostic settings"
keyVaultResourceId=$(az keyvault list \
    --resource-group $resourceGroupName \
    --query "[?name=='$keyVaultName'].id" \
    --output tsv)

az monitor diagnostic-settings create \
 --name KeyVaultAuditDiagnosticSettings \
 --resource $keyVaultResourceId \
 --resource-group $resourceGroupName \
 --storage-account $keyVaultAuditStorageAccountName \
 --logs '[{ "category": "AuditEvent", "enabled": true, "retentionPolicy": { "days": 0, "enabled": true } }]' \
 1> /dev/null

echo "Creating Cognitive Services Bing Search"
az group deployment create \
    --name CognitiveServicesBingDeployment \
    --resource-group $resourceGroupName \
    --template-file ../Templates/cognitiveservicesbingsearch.json \
    --parameters searchAccountName=$searchAccountName \
    1> /dev/null

echo "Creating Redis Cache (can take upwards of 20 minutes)"
az group deployment create \
    --name RedisCacheDeployment \
    --resource-group $resourceGroupName \
    --template-file ../Templates/rediscache.json \
    --parameters cacheName=$cacheName \
    1> /dev/null

echo "Creating App Service Plan"
az group deployment create \
    --name AppServicePlanDeployment \
    --resource-group $resourceGroupName \
    --template-file ../Templates/appserviceplan.json \
    --parameters servicePlanName=$appServicePlanName \
    1> /dev/null

echo "Creating API App"
az group deployment create \
    --name ApiAppDeployment \
    --resource-group $resourceGroupName \
    --template-file ../Templates/apiapp.json \
    --parameters appName=$apiAppName servicePlanName=$appServicePlanName \
    1> /dev/null

echo "Creating API App: Configuring App Settings"

az webapp config appsettings set \
    --resource-group $resourceGroupName \
    --name $apiAppName \
    --settings $searchAccountInstrumentationKeyNameSettingKey=$searchAccountInstrumentationKeyName \
    1> /dev/null

az webapp config appsettings set \
    --resource-group $resourceGroupName \
    --name $apiAppName \
    --settings $appInsightsInstrumentationKeyNameSettingKey=$appInsightsInstrumentationKeyName \
    1> /dev/null

az webapp config appsettings set \
    --resource-group $resourceGroupName \
    --name $apiAppName \
    --settings $openWeatherMapAppIdKeyNameSettingKey=$openWeatherMapAppIdKeyName \
    1> /dev/null

az webapp config appsettings set \
    --resource-group $resourceGroupName \
    --name $apiAppName \
    --settings $apiKeyNameSettingKey=$apiKeyName \
    1> /dev/null

az webapp config appsettings set \
    --resource-group $resourceGroupName \
    --name $apiAppName \
    --settings $cacheConnectionStringKeyNameSettingKey=$cacheConnectionStringKeyName \
    1> /dev/null

keyVaultEndpoint="https://$keyVaultName.vault.azure.net"

az webapp config appsettings set \
    --resource-group $resourceGroupName \
    --name $apiAppName \
    --settings $keyVaultEndpointSettingKey=$keyVaultEndpoint \
    1> /dev/null

echo "Creating API App: Configuring Managed Service Identity"
msiPrincipalId=$(az webapp identity assign \
    --name $apiAppName \
    --resource-group $resourceGroupName \
    --query "principalId" \
    --output tsv)

echo "Creating API App: Configuring Acess Policy on KeyVault"
az keyvault set-policy \
    --name $keyVaultName \
    --object-id $msiPrincipalId \
    --secret-permissions get list \
    1> /dev/null

echo "Configuring KeyVault: Retrieving secrets from resources"

searchAccountInstrumentationKey=$(az cognitiveservices account keys list \
    --name $searchAccountName \
    --resource-group $resourceGroupName \
    --query key1 \
    --output tsv)

appInsightsInstrumentationKey=$(az resource show \
    --name $appInsightsName \
    --resource-group $resourceGroupName \
    --resource-type "Microsoft.Insights/components" \
    --query properties.InstrumentationKey \
    --output tsv)

cachePrimaryKey=$(az redis list-keys \
    --name $cacheName \
    --resource-group $resourceGroupName \
    --query primaryKey \
    --output tsv)

cacheConnectionString="$cacheName.redis.cache.windows.net,abortConnect=false,ssl=true,password=$cachePrimaryKey"

echo "Configuring KeyVault: Writing secrets"
az keyvault secret set \
    --vault-name $keyVaultName \
    --name $searchAccountInstrumentationKeyName \
    --value $searchAccountInstrumentationKey \
    1> /dev/null

az keyvault secret set \
    --vault-name $keyVaultName \
    --name $appInsightsInstrumentationKeyName \
    --value $appInsightsInstrumentationKey \
    1> /dev/null

az keyvault secret set \
    --vault-name $keyVaultName \
    --name $cacheConnectionStringKeyName \
    --value $cacheConnectionString \
    1> /dev/null

az keyvault secret set \
    --vault-name $keyVaultName \
    --name $apiKeyName \
    --value $apiKey \
    1> /dev/null

az keyvault secret set \
    --vault-name $keyVaultName \
    --name $openWeatherMapAppIdKeyName \
    --value $openWeatherMapAppId \
    1> /dev/null

echo "Publishing API App"

cd ../../service/WeatherService

echo "Publishing API App: Creating application"
dotnet publish -c release 1> /dev/null

cd bin/Release/netcoreapp2.1/publish

echo "Publishing API App: Packaging application files"
zip -r ../publish.zip * 1> /dev/null

cd ..

echo "Publishing API App: Deploying application package"
az webapp deployment source config-zip \
    --resource-group $resourceGroupName \
    --name $apiAppName \
    --src publish.zip \
    1> /dev/null

echo ""
echo "========== WeatherSample Provisioning completed =========="
echo ""
# This is purely to aid in local dev
# Login should be executed via bash as a prerequisite!
# Parameter values should be passed into the script in the same manner

# Define these parameters within local parameters.sh
# NOTE: The parameters.sh file should not be committed to source

if [ ! -f Scripts/parameters.sh ]
then 
    echo "Missing parameters.sh in Scripts. Expected variables:"
    echo ""
    echo 'TARGET_TENANT="{value}"'
    echo 'TARGET_SUBSCRIPTION="{value}"'
    echo 'ADMIN_UPN_OR_OBJECT_ID="{value}"'
    echo 'OPEN_WEATHER_MAP_APP_ID="{value}"'
    echo 'API_KEY="{value}"'
    echo ""
    exit 1
fi

source Scripts/parameters.sh

# Verify all parameters have values
if [ -z "$TARGET_TENANT" ]
then
    echo "The TARGET_TENANT variable is not defined"
    exit 1
fi

if [ -z "$TARGET_SUBSCRIPTION" ]
then
    echo "The TARGET_SUBSCRIPTION variable is not defined"
    exit 1
fi

if [ -z "$ADMIN_UPN_OR_OBJECT_ID" ]
then
    echo "The ADMIN_UPN_OR_OBJECT_ID variable is not defined"
    exit 1
fi

if [ -z "$OPEN_WEATHER_MAP_APP_ID" ]
then
    echo "The OPEN_WEATHER_MAP_APP_ID variable is not defined"
    exit 1
fi

if [ -z "$API_KEY" ]
then
    echo "The API_KEY variable is not defined"
    exit 1
fi

token="$(az account get-access-token --output tsv)"

if [ -z "$token" ]
then
    echo "Logging into the Azure CLI"
    az login --tenant $TARGET_TENANT 1> /dev/null
fi

token="$(az account get-access-token --output tsv)"

if [ -z "$token" ]
then
    echo "Login failed. Aborting"
    exit 1
fi

cd Scripts
./weather_deploy.sh --tenant $TARGET_TENANT --subscription $TARGET_SUBSCRIPTION --admin $ADMIN_UPN_OR_OBJECT_ID --openweathermap-appid $OPEN_WEATHER_MAP_APP_ID --api-key $API_KEY
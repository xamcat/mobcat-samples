@echo off

rem This is purely to aid in local dev
rem Login should be executed via bash as a prerequisite!
rem Parameter values should be passed into the script in the same manner

rem Define these parameters within local parameters.sh
rem NOTE: The parameters.sh file should not be committed to source

if not exist Scripts/parameters.bat (
    echo ^Missing parameters.sh in Scripts. Expected variables:
    echo.
    echo ^TARGET_TENANT="{value}"
    echo ^TARGET_SUBSCRIPTION="{value}"
    echo ^ADMIN_UPN_OR_OBJECT_ID="{value}"
    echo ^OPEN_WEATHER_MAP_APP_ID="{value}"
    echo ^API_KEY="{value}"
    echo.
    exit 1
)

call Scripts/parameters.bat

rem Verify all parameters have values

if not defined TARGET_TENANT (
    echo ^The TARGET_TENANT variable is not defined
    exit 1
)

if not defined TARGET_SUBSCRIPTION (
    echo ^The TARGET_SUBSCRIPTION variable is not defined
    exit 1
)

if not defined ADMIN_UPN_OR_OBJECT_ID (
    echo ^The ADMIN_UPN_OR_OBJECT_ID variable is not defined
    exit 1
)

if not defined OPEN_WEATHER_MAP_APP_ID (
    echo ^The OPEN_WEATHER_MAP_APP_ID variable is not defined
    exit 1
)

if not defined API_KEY (
    echo ^The TARGET_TENANT variable is not defined
    exit 1
)

set token=
for /f "usebackq" %%i in ( `az account get-access-token --output tsv` ) do set token=%%i

if not defined token (
    echo ^Logging into the Azure CLI
    az login --tenant %TARGET_TENANT% >nul 2>&1
)

cd Scripts
call weather_deploy.bat --tenant %TARGET_TENANT% --subscription %TARGET_SUBSCRIPTION% --admin %ADMIN_UPN_OR_OBJECT_ID% --openweathermap-appid %OPEN_WEATHER_MAP_APP_ID% --api-key %API_KEY%
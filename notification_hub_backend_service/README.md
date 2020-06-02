# Push notifications using Azure Notification Hubs via a backend service

## Overview

Sample demonstrating the use [Azure Notification Hubs](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview) via a backend service to send push notifications to **Android** and **iOS** applications.  

An [ASP.NET Core Web API](https://dotnet.microsoft.com/apps/aspnet/apis) backend is used to handle [device registration](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-registration-management#what-is-device-registration) on behalf of the client using the latest and best [Installation](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-registration-management#installations) approach via the [Notification Hubs SDK for backend operations](https://www.nuget.org/packages/Microsoft.Azure.NotificationHubs/), as shown in the guidance topic [Registering from your app backend](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-registration-management#registration-management-from-a-backend). 

A cross-platform [Xamarin.Forms](https://dotnet.microsoft.com/apps/xamarin/xamarin-forms) application is used to demonstrate the use of the backend service using explicit register/deregister actions. 

<img src="illustrations/pushdemo_mainpage.png" alt="PushDemo Sample - MainPage" height="350" style="display:inline-block;" />

Alert style notifications will appear in the notification center (when the app is stopped or in the background). 

<img src="illustrations/pushdemo_notification.png" alt="PushDemo Sample - Notification" height="175" style="display:inline-block;" />

If a notification contains an action and is received when app is in the foreground, or where an alert-style notification is used to launch the application from notification center, a message is presented identifying the action specified.

<img src="illustrations/pushdemo_action_alert.png" alt="PushDemo Sample - Action Alert" height="350" style="display:inline-block;" />

> [!NOTE]
> You would typically perform the registration (and deregisration) actions during the appropriate point in the application lifecycle (or as part of your first-run experience perhaps) without explicit user register/deregister inputs. However, this example will require explicit user input to allow this functionality to be explored and tested more easily. The notifications are defined client-side using [custom templates](https://docs.microsoft.com/azure/notification-hubs/notification-hubs-templates-cross-platform-push-messages) in this case but could be handled server-side in future. 

## Prerequisites
To run this sample your will need:

- An [Azure subscription](https://portal.azure.com) where you can create and manage resources.
- A Mac with [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/) installed (or a PC running [Visual Studio 2019](https://visualstudio.microsoft.com/vs) with the **Mobile Development with .NET** workload). 
- The ability to run the app on either **Android** (physical or emulator devices) or **iOS** (physical devices only). 

For Android, you must have:

- A developer unlocked phyical device or an emulator *(running API 26 and above with Google Play Services installed)*.

For iOS, you must have:

- An active [Apple Developer Account](https://developer.apple.com). 
- A physical iOS device that is [registered to your developer account](https://help.apple.com/developer-account/#/dev40df0d9fa) *(running iOS 13.0 and above)*.
- A **.p12** [development certificate](https://help.apple.com/developer-account/#/dev04fd06d56) installed in your **keychain** allowing you to [run an app on a physical device](#https://help.apple.com/xcode/mac/current/#/dev5a825a1ca).



> [!NOTE] 
> The iOS Simulator does not support remote notifications and so a physical device is required when exploring this sample on iOS. 

## Getting Started

### Platform Dependencies

#### Apple Push Notification Service (APNS)

1. Register an [explicit App ID](https://help.apple.com/developer-account/#/dev1b35d6f83) and configure it to [enable push notifications](https://help.apple.com/developer-account/#/dev4cb6dfbdb).

1. Create a [Provisioning Profile](https://help.apple.com/developer-account/#/devf2eb157f8) that can be used with your explicit **App ID**, **.p12** development certificate, and physical device.

#### Firebase Cloud Messaging (FCM) 

1. Create a [Google Firebase project](https://cloud.google.com/solutions/mobile/mobile-firebase-app-engine-flexible#creating-project) 

1. Register an [Android app](https://firebase.google.com/docs/cloud-messaging/android/first-message#register_your_app_with_firebase) with your **Google Firebase Project**.

### Backend

1. Provision a [Notification Hub](https://docs.microsoft.com/azure/notification-hubs) in the [Azure Portal](https://portal.azure.com) and configure it for use with [Firebase Cloud Messaging](https://docs.microsoft.com/azure/notification-hubs/configure-google-firebase-cloud-messaging) and [APNS](https://docs.microsoft.com/azure/notification-hubs/notification-hubs-push-notification-http2-token-authentication) (using the **Sandbox** option).

1. Host the backend service (locally or using a suitable [compute resource](https://docs.microsoft.com/azure/architecture/guide/technology-choices/compute-decision-tree#understand-the-basic-features) e.g. [Azure App Service](https://docs.microsoft.com/azure/app-service)).

1. Configure the [required app settings](#configure-backend-app-settings) for the backend service

### Mobile App

1. Update the placeholder [client config values](#update-mobile-app-config) to use the appropriate service endpoint and api key values.

#### Android

1. Update the **Package name** so it matches the value you defined in the [Android app](https://firebase.google.com/docs/cloud-messaging/android/first-message#register_your_app_with_firebase) you registered.

1. Download the [**google-services.json** file](https://support.google.com/firebase/answer/7015592?hl=en) from your **Firebase Project Settings page** and add it to the Android target/project.

#### iOS

1. Update the **Bundle identifier** so it matches the [App ID](https://help.apple.com/developer-account/#/dev1b35d6f83) that you registered in the [Apple Developer Portal](https://developer.apple.com).

1. Ensure that the appropriate **.p12** certificate and **Provisioning Profile** has been downloaded and is being used for **Bundle Signing**.

### Supporting details

#### Configure backend app settings

The backend service expects to have the following app settings defined:

- **Authentication:ApiKey**  
Choose your own API key e.g. a randomly generated GUID.

- **NotificationHub:Name**  
Found in the **Essentials** summary at the top of the [notification hub](https://docs.microsoft.com/azure/notification-hubs) **Overview** section.  

- **NotificationHub:ConnectionString**  
Use the connection string associated with the *DefaultFullSharedAccessSignature* **Access Policy**.

If you are publishing the service to [Azure App Service](https://docs.microsoft.com/azure/app-service), these would be added to the [app settings](https://docs.microsoft.com/azure/app-service/configure-common#configure-app-settings).  

> [!NOTE] 
> Be sure to restart the service if you add/modify these settings after you have published the service.  

If you are running the service locally, these can be set via **Terminal** using the following commands with the [Secret Manager tool](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=linux#secret-manager) from the project directory.

```shell
dotnet user-secrets init
dotnet user-secrets set "NotificationHub:Name" <value>
dotnet user-secrets set "NotificationHub:ConnectionString" <value> 
dotnet user-secrets set "Authentication:ApiKey" <value>
```

#### Update mobile app config

Find and replace the **API_KEY**, and **BACKEND_SERVICE_ENDPOINT** placeholder text with your own values. For example, in the Xamarin.Forms **PushDemo** project, these are defined within **Config.cs**.

```cs
public static string ApiKey = "API_KEY";
public static string BackendServiceEndpoint = "BACKEND_SERVICE_ENDPOINT";
```

The **API_KEY** should match the **Authentication:ApiKey** app setting that you set for the backend service.  

The **BACKEND_SERVICE_ENDPOINT** should be the base address for the backend service (ending in a '/') e.g.

```
https://localhost:5001/
```

If you are hosting the service using an [API App](https://azure.microsoft.com/en-gb/services/app-service/api) in [Azure App Service](https://docs.microsoft.com/azure/app-service) then the URL should be in the following format:  

```
https://<api_app_name>.azurewebsites.net/
```


## Browse code

- [Backend Service (ASP.NET Core)](https://github.com/xamcat/mobcat-samples/tree/master/notification_hub_backend_service/src/azure)  
- [Mobile App (Xamarin.Forms)](https://github.com/xamcat/mobcat-samples/tree/master/notification_hub_backend_service/src/xamarin)  
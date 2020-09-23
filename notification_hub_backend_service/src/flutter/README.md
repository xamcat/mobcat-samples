# Push notifications in Flutter apps using Azure Notification Hubs via a backend service

Sample demonstrating the use of [Azure Notification Hubs](https://docs.microsoft.com/azure/notification-hubs/notification-hubs-push-notification-overview) via a backend service to send push notifications to **Android** and **iOS** applications built with [Flutter](https://flutter.dev).  

An accompanying [tutorial](https://docs.microsoft.com/azure/developer/mobile-apps/notification-hubs-backend-service-flutter) was written alongside this sample providing detailed steps on how to build it from scratch. High-level steps are provided in the [main read me](https://github.com/xamcat/mobcat-samples/tree/master/notification_hub_backend_service) for convenience.

> [!NOTE]
> Be sure to complete the [client apps](https://github.com/xamcat/mobcat-samples/tree/master/notification_hub_backend_service#client-apps) steps provided in the [main read me](https://github.com/xamcat/mobcat-samples/tree/master/notification_hub_backend_service).
>
> Once you have cloned this repo, you must also install the pods required by the [flutter secure storage](https://pub.dev/packages/flutter_secure_storage) package dependency. To do this from **Terminal**, change directory to the **ios** folder (for the Flutter project). Then, execute the [pod install](https://guides.cocoapods.org/using/getting-started.html#installation) command.

## Prerequisites

To run this sample, you require:

* An [Azure subscription](https://portal.azure.com) where you can create and manage resources.
* The [Flutter](https://flutter.dev/docs/get-started/install) toolkit (along with its prerequisites).
* [Visual Studio Code](https://code.visualstudio.com) with the [Flutter and Dart plugins](https://flutter.dev/docs/get-started/editor?tab=vscode) installed.
* [CocoaPods](https://guides.cocoapods.org/using/getting-started.html#installation) installed for managing library dependencies.
* The ability to run the app on either **Android** (physical or emulator devices) or **iOS** (physical devices only).

For Android, you must have:

* A developer unlocked physical device or an emulator *(running API 26 and above with Google Play Services installed)*.

For iOS, you must have:

* An active [Apple Developer Account](https://developer.apple.com).
* A physical iOS device that is [registered to your developer account](https://help.apple.com/developer-account/#/dev40df0d9fa) *(running iOS 13.0 and above)*.
* A **.p12** [development certificate](https://help.apple.com/developer-account/#/dev04fd06d56) installed in your **keychain** allowing you to [run an app on a physical device](https://help.apple.com/xcode/mac/current/#/dev5a825a1ca).

> [!NOTE]
> The iOS Simulator does not support remote notifications and so a physical device is required when exploring this sample on iOS. However, you do not need to run the app on both **Android** and **iOS** in order to complete this tutorial.

# Introduction 
This repository contains sample projects that demonstrate the use the MobCAT [library](https://github.com/xamcat/mobcat-library) or addresses specific scenarios or challenges. 

## Build Status

|                               | __master__ | __dev__ |
|:-----------------------------:|:---------:|:--------:|
| __Weathertron 9000__             | [![weathericon_release]][weatherlink_release] | [![weathericon_dev]][weatherlink_dev]
| __News-Man 9000 2.0__             | [![newsicon_release]][newslink_release] | [![newsicon_dev]][newslink_dev]

[weathericon_dev]: https://dotnetcst.visualstudio.com/MobCAT/_apis/build/status/Weather/MobCAT-Weather-dev?branchName=dev
[weatherlink_dev]: https://dotnetcst.visualstudio.com/MobCAT/_apis/build/status/Weather/MobCAT-Weather-dev?branchName=dev
[weathericon_release]: https://dotnetcst.visualstudio.com/MobCAT/_apis/build/status/Weather/MobCAT-Weather-Release?branchName=dev
[weatherlink_release]: https://dotnetcst.visualstudio.com/MobCAT/_build/latest?definitionId=73&branchName=dev

[newsicon_dev]: https://dotnetcst.visualstudio.com/MobCAT/_apis/build/status/News/MobCAT-News-Dev?branchName=dev
[newslink_dev]: https://dotnetcst.visualstudio.com/MobCAT/_build/latest?definitionId=70&branchName=dev
[newsicon_release]: https://dotnetcst.visualstudio.com/MobCAT/_apis/build/status/News/MobCAT-News-Release?branchName=dev
[newslink_release]: https://dotnetcst.visualstudio.com/MobCAT/_build/latest?definitionId=74&branchName=dev

## Directory 
### Sample Apps

[weather](https://github.com/xamcat/mobcat-samples/tree/master/weather)  
This is a single page Xamarin.Forms sample that displays the weather forecast using a public Weather API 

[news](https://github.com/xamcat/mobcat-samples/tree/master/news)  
This is a tabbed view Xamarin.Forms sample that displays news using a public News API

### Other samples, prototypes and scratch projects

[azure media services](https://github.com/xamcat/mobcat-samples/tree/master/azure_media_services)  
Prototype demonstrating the ability to natively stream, download and play offline video protected by [Apple FairPlay](https://developer.apple.com/streaming/fps/) using [Azure Media Services](https://docs.microsoft.com/en-gb/azure/media-services/) and [Xamarin.iOS](https://docs.microsoft.com/en-us/xamarin/ios/).

[binding kotlin frameworks for xamarin](https://github.com/xamcat/xamarin-binding-kotlin-framework)  
Walkthrough on binding third-party Android frameworks that are written in Kotlin so they can be used in Xamarin.Android applications. 

[binding swift frameworks for xamarin](https://github.com/xamcat/xamarin-binding-swift-framework)  
Walkthrough on binding third-party iOS frameworks that are written in Swift so they can be used in Xamarin.iOS applications. 

[cpp with xamarin](https://github.com/xamcat/mobcat-samples/tree/master/cpp_with_xamarin)  
Walkthrough of an approach to transforming some basic C/C++ source code into a cross-platform Xamarin library so it can be shared via NuGet and consumed in a Xamarin.Forms app.

[in memory repository](https://github.com/xamcat/mobcat-samples/tree/master/in_memory_repository)  
Demonstrates how to implement an in-memory repository by leveraging the base classes provided by the [MobCAT library](https://github.com/xamcat/mobcat-library/blob/master/docs/repository-inmemory-gettingstarted.md).

[notification hub rest api](https://github.com/xamcat/mobcat-samples/tree/master/notification_hub_rest)  
Walkthrough on how to create an [Azure Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs), configure it for use with [APNS](https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server), then work with it directly, using the preferred [installation approach](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-registration-management#installations), from a Swift-based client application using the [Notification Hub REST API](https://msdn.microsoft.com/en-us/library/azure/dn223264.aspx).

[push notifications using azure notification hubs via a backend service](https://github.com/xamcat/mobcat-samples/tree/master/notification_hub_backend_service)  
Sample demonstrating how to use [Azure Notification Hubs](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview) via a backend service to send push notifications to cross-platform **Android** and **iOS** applications.  

[sas token generation](https://github.com/xamcat/mobcat-samples/tree/master/sas_token_generation)  
Prototype demonstrating how to programmatically generate a [SAS token](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-sas#overview-of-sas) for use with Azure services such as [Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview) using Objective-C or Swift. The [official documentation](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token) already provides samples for other languages including [C#](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#c35).
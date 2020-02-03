# Introduction 
This repository contains sample projects that use the MobCAT [framework](https://github.com/xamcat/mobcat/tree/master/mobcat_shared)

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

### Sample Apps (coming soon)

[communicator]()  
Coming soon.

[doggo]()  
Woof coming soon. Bow wow.

### Other samples, prototypes and scratch projects

[azure media services](https://github.com/xamcat/mobcat-samples/tree/master/azure_media_services)  
Prototype demonstrating the ability to natively stream, download and play offline video protected by [Apple FairPlay](https://developer.apple.com/streaming/fps/) using [Azure Media Services](https://docs.microsoft.com/en-gb/azure/media-services/) and [Xamarin.iOS](https://docs.microsoft.com/en-us/xamarin/ios/)

[cpp with xamarin](https://github.com/xamcat/mobcat-samples/tree/master/cpp_with_xamarin)  
Walkthrough of an approach to transforming some basic C/C++ source code into a cross-platform Xamarin library so it can be shared via NuGet and consumed in a Xamarin.Forms app
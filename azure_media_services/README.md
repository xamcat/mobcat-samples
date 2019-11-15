# Introduction 
This prototype demonstrates the ability to natively stream, download and play offline video protected by [Apple FairPlay](https://developer.apple.com/streaming/fps/) using [Azure Media Services](https://docs.microsoft.com/en-gb/azure/media-services/) and [Xamarin.iOS](https://docs.microsoft.com/en-us/xamarin/ios/).   

The Xamarin project is a like-for-like port of the Swift-based HLSCatalog sample, provided by Apple, as part of the [FairPlay Streaming Server SDK](https://developer.apple.com/streaming/fps/). It has been adapted to work with Azure Media Services using the steps described in the [Offline FairPlay Streaming for iOS](https://docs.microsoft.com/en-gb/azure/media-services/previous/media-services-protect-hls-with-offline-fairplay#sample-ios-player) article. The intent was to keep it as conceptually close as possible to the original Swift sample to aid in familiarization and comparison.   

In addition to the Swift-based sample, the [FairPlay SDK](https://developer.apple.com/services-account/download?path=/Developer_Tools/FairPlay_Streaming_Server_SDK/FairPlay_Streaming_Server_SDK_v4.2.0.zip) contains a programming guide and read me explaining the concepts and supporting components in greater depth.

The original HLSCatalog sample for FPS offline mode can be found in the *FairPlay Streaming Server SDK v4.2.0* folder under: 
```
Development\Client\HLSCatalogWithFPS - AVContentKeySession
```

Streams.plist has been updated to use the test sample provided by the [Azure Media Services demo site](https://openidconnectweb.azurewebsites.net) addressing the [offline playback scenario](http://aka.ms/poc#22) as described in the [Offline FairPlay Streaming for iOS article](https://docs.microsoft.com/en-gb/azure/media-services/previous/media-services-protect-hls-with-offline-fairplay#integrated-test).  

# Getting Started
Download or clone this repo and then:
1.	Open SampleNativeVideo.sln in Visual Studio for Mac
2.	Restore NuGet packages for the solution
3.	Build and run on a physical iOS device (not simulator)

**NOTE:**  
You will need an [Apple Developer](https://developer.apple.com) account along with a suitable [certificate](https://developer.apple.com/account/ios/certificate) and [provisioning profile](https://developer.apple.com/account/ios/profile/) in order to deploy to an iOS device.

# Notes
1. This prototype was created in order to quickly demonstate the respective scenario working in Xamarin.iOS. It should not be considered production quality nor a representation of recommended practices.  

2. The requisite APIs can be used on a physical device only. Running this in the simulator will result in an exception being thrown due to API compatability.

3. The [application certificate](https://openidconnectweb.azurewebsites.net/Content/FPSAC.cer), provided for the respective [test sample](https://openidconnectweb.azurewebsites.net/CHT#22), has been embedded into the prototype for convenience. This will need replacing if it expires.  

# Resources
- [FairPlay Streaming Server SDK (4.2.0)](https://developer.apple.com/services-account/download?path=/Developer_Tools/FairPlay_Streaming_Server_SDK/FairPlay_Streaming_Server_SDK_v4.2.0.zip)
- [Use Azure Media Services to Stream your HLS content Protected with Apple FairPlay](https://azure.microsoft.com/en-gb/resources/samples/media-services-dotnet-dynamic-encryption-with-fairplay/)
- [Troubleshooting FAQ (Offline FairPlay Streaming for iOS)](https://docs.microsoft.com/en-us/azure/media-services/previous/media-services-protect-hls-with-offline-fairplay#faq)
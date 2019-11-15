# Introduction 
These prototypes demonstrate how to programmatically generate a [SAS token](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-sas#overview-of-sas) for use with Azure services such as [Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview) using Objective-C and Swift.  

At time of writing, the [official documentation](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token) for this topic does not provide these samples. However, samples are provided in a number of popular languages including:  

- [Bash](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#bash)
- [C#](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#c35)
- [Java](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#java)  
- [NodeJS](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#nodejs)
- [PHP](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#php)
- [PowerShell](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#powershell)
- [Python](https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token#python)

The intent is to provide a helpful starting point for iOS developers looking to take advantage of those Azure services requiring the use of a [SAS token](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-sas#overview-of-sas)  of this kind.

In this case, the samples demonstrate the use of the [SAS token](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-sas#overview-of-sas) with the [Notification Hub REST API](https://msdn.microsoft.com/en-us/library/azure/dn223264.aspx), however the approach could be adapted to work with other types of resources as well such as [Event Hub](https://docs.microsoft.com/en-us/azure/event-hubs/), [Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/) and [Storage](https://docs.microsoft.com/en-us/azure/storage/common/storage-introduction). 

# SAS (Shared Access Signature) Tokens Overview
Regardless of the language being used, programmatically generating the SAS Token involves the same 6 key steps:  

1. 	Computing the expiry in [UNIX Epoch time](https://en.wikipedia.org/wiki/Unix_time) format (seconds elapsed since 00:00:00 UTC 1 January 1970)
2.  Formatting the **ResourceUrl** (representing the resource we are trying to access e.g. *'https://\<namespace\>.servicebus.windows.net/\<notification_hub\>'*) so it is percent-encoded and lowercase
3.  Preparing the **StringToSign**, which is comprised of *'\<**UrlEncodedResourceUrl**\>\n\<**ExpiryEpoch**\>'*
4.  Computing (and base 64 encoding) the **Signature** using the **HMAC-SHA256** of the **StringToSign** value with the **Key** for the respective **Authorization Rule** 
5.  Formatting the base 64 encoded **Signature** so it is percent encoded
6.  Constructing the **token** in the expect format using the **UrlEncodedSignature**, **ExpiryEpoch**, **KeyName**, and **UrlEncodedResourceUrl** values

The resulting token will be in the following format: 

```
SharedAccessSignature sig=<UrlEncodedSignature>&se=<ExpiryEpoch>&skn=<KeyName>&sr=<UrlEncodedResourceUri>
```

See the [Azure Service Bus documentation](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-sas) for a more thorough overview of **Shared Access Signature** and how it is used by other Azure resources.  

# Sample Overview
Both Objective-C and Swift samples are comprised of a static library project with an accompanying integration test demonstrating the use of the programmatically generated SAS token by [sending a templated notification](https://docs.microsoft.com/en-us/previous-versions/azure/reference/dn223267(v%3dazure.100)) using the [Notification Hub REST API](https://msdn.microsoft.com/en-us/library/azure/dn223264.aspx).  

The components of interest include: 

**TokenData:**  
Basic struct representing the token data. This includes the token string value itself along with its expiration date.  

**TokenUtility:**  
Encapsulates the logic for programmatically generating the SAS token for a given resource URL. 

# Running the sample
You will need a [Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview) resource in order to execute the *testSasTokenGeneration* **XCTest** provided.  If you do not have one, you can [create a Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-windows-store-dotnet-get-started-wns-push-notification#create-a-notification-hub) for free in the [Azure Portal](https://portal.azure.com).  

You will need to replace the placeholder values in *SasTokens<ObjectiveC/Swift>_Tests* using the details from your own [Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview). For example:  

**Objective-C**

```
// Expected Inputs
NSString* resourceUrl = @"https://<namespace>.servicebus.windows.net/<hub_name>";
NSString* secretKeyName = @"DefaultFullSharedAccessSignature";
NSString* secretKey = @"<secret_key>";
````  

**Swift:**  

```
// Expected Inputs
let resourceUrl = "https://<namespace>.servicebus.windows.net/<hub_name>"
let secretKeyName = "DefaultFullSharedAccessSignature"
let secretKey = "<secret_key>"
```

The *<secret_key>* value is found by navigating to **Access Policies** on your [Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview) resource in the [Azure Portal](https://portal.azure.com), then choosing the **DefaultFullSharedAccessSignature**. You can then copy the connection string and then extract the secretKey from the value prefixed with 'SharedAccessKey='. The connection string should be in the following format:  

 ```
Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<secret_key_name>;SharedAccessKey=<secret_key>
```

Once these values have been updated, you can run the *testSasTokenGeneration* in Xcode.
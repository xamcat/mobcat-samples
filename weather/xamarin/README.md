# WeatherTron 9000
WeatherTron 9000 is a single view sample app that demonstrates a number of recommended patterns and practices for building, testing, and deploying mobile apps using [Xamarin](https://visualstudio.microsoft.com/xamarin/), [Azure DevOps](https://azure.microsoft.com/en-us/solutions/devops/), and [AppCenter](https://appcenter.ms/).

<p>
<img src="../readme_illustrations/weather_app_android.png" alt="Weather App for Android" height="350" style="display:inline-block;" />

<img src="../readme_illustrations/weather_app_ios.png" alt="Weather App for iOS" height="350" style="display:inline-block;" />
</p>

## Solution Overview
The solution consists of the following projects:  

**MobCAT:**  
Foundational components supporting the implementation of recommended patterns and practices in a simple manner

**MobCAT.Forms:**  
Foundational components specific to [Xamarin.Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/)  

**Weather:**  
The Weather Forms project which contains the XAML and shared code  

**Weather.Android:**  
The Weather Android specific project contains Android assets  

**Weather.iOS:**  
The Weather iOS specific project contains iOS assets  

**Weather.UITests:**  
The UI Test project  

**Weather.UnitTests:**  
The Unit Test project


## Getting Started
You must perform the following prerequisite actions in order to run this sample app:  

### 1. Create an app in the App Center Portal
The sample demonstrates analytics and crash reporting via [App Center](https://appcenter.ms) using the [NuGet packages](https://www.nuget.org/packages/Microsoft.AppCenter) provided.  

To enable this you must [create an app in the App Center Portal](https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/xamarin#2-create-your-app-in-the-app-center-portal-to-obtain-the-app-secret) for each platform.  

Once you have created the apps, you can obtain the **Android AppCenter App Secret** and **iOS AppCenter App Secret** values on the **Getting Started** or **Manage App** sections of the [App Center Portal](https://appcenter.ms).  

### 2. Provision the weather service
The sample is underpinned by a [web service](https://github.com/xamarin/mobcat/tree/dev/samples/weather/azure), built with [asp.net core 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1), which you must host yourself in order to run the app. Once you have [provisioned the service to Azure](https://github.com/xamarin/mobcat/blob/dev/samples/weather/azure/README.md), you should have the following details for the service:  

- **API Key** (you created this as a part of the [executing the provisioning script](https://github.com/xamarin/mobcat/tree/dev/samples/weather/azure#executing-the-provisioning-script) step)
- **API Endpoint URL** e.g. *https://<app_name>.azurewebsites.net/api/*

### 3. Set Environment Variables for the app secrets

The following app secrets are defined in the **ServiceConfig** class and need to be replaced with the values generated in the steps above.

1. WeatherServiceApiKey
2. WeatherServiceUrl
3. AndroidAppCenterSecret
4. iOSAppCenterSecret

Be sure to configure the variables before deploying the app to avoid runtime errors. 

**NOTE:** Make sure the **API Endpoint URL** ends with **/api/**

## Key Concepts

### MVVM
The app uses the [MVVM (Model-View-ViewModel)](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm) pattern to decouple business logic & presentation code. The relevant supporting components can be found in [MobCAT/MVVM](https://github.com/xamarin/mobcat/tree/dev/mobcat_shared/MobCAT/MVVM).

### Service Container
The MobCAT library includes a **ServiceContainer** class as a light-weight and simple mechanism for implementing the [service locator](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff648968(v=pandp.10)) IoC (Inversion of Control) design pattern. This enables both platform-specific and platform-agnostic components to be registered and subsequently resolved via a common interface within shared code. It also enables mock services to be registered for UI test or unit test purposes.

### Bootstrap
The **Bootstrap** class centralizes the process of initializing and registering the respective services using the **ServiceContainer** class. Platform-specific startup actions can also be invoked using the optional **Func** parameter in the Begin method. The Begin method is called from the **AppDelegate** for iOS, and **MainActivity** for Android respectively.

### BaseHttpService
The **BaseHttpService** base class simplifies working with REST APIs in a reliable manner by encapsulating the [correct usage of HttpClient](https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/), handling common tasks such as serialization and implementing recommended [cloud design patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/) such as [retry](https://docs.microsoft.com/en-us/azure/architecture/patterns/retry) and [circuit breaker](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker).

### Localization
Localization is implemented using .resx resources based on the [official Xamarin sample](https://github.com/xamarin/xamarin-forms-samples/tree/master/UsingResxLocalization). Malay (ms) and Arabic (ar) localization has been implemented with RTL (Right-to-Left) language support.

<p>
<img src="../readme_illustrations/redmond_malay.png" alt="Weather App in Malay" height="350" style="display:inline-block;" />

<img src="../readme_illustrations/redmond_english.png" alt="Weather App in English" height="350" style="display:inline-block;" />

<img src="../readme_illustrations/redmond_arabic.png" alt="Weather App in Arabic" height="350" style="display:inline-block;" />
</p>

### Text Size Accessibility
Accessibility text size is supported using a custom control on iOS, the AccessibilityLabel. On Android, text size accessibility is supported out of the box.

<p>
<img src="../readme_illustrations/accessibility_small.png" alt="Weather App with small text" height="350" style="display:inline-block;" />

<img src="../readme_illustrations/redmond_english.png" alt="Weather App with default text size" height="350" style="display:inline-block;" />

<img src="../readme_illustrations/accessibility_large.png" alt="Weather App with large text" height="350" style="display:inline-block;" />
</p>

### Logging ###
Logging is implemented using the [AppCenterLoggingService](https://github.com/xamarin/mobcat/blob/dev/samples/weather/xamarin/Weather/Services/AppCenterLoggingService.cs) which logs exceptions to AppCenter for release builds as configured in [Bootstrap.cs](https://github.com/xamarin/mobcat/blob/dev/samples/weather/xamarin/Weather/Bootstrap.cs). Non-release builds will log exceptions and warnings to the console. 

### Unit Tests ###
Unit tests are run using [XUnit](https://xunit.github.io). App service functionality is mocked using [Moq](https://github.com/moq/moq4) which highlights a benefit of using a Service Container.

### UI Tests ###
UI tests were created to adhere to the [Page Object Pattern](https://github.com/xamarin-automation-service/uitest-pop-example) using the [Page Object Pattern Library](https://www.nuget.org/packages/Xamarin.UITest.POP/) where each page has a Page Object which tests are run on. [Learn more about best practices for User Interface Automation](https://channel9.msdn.com/Shows/XamarinShow/Best-Practices-for-User-Interface-Automation).



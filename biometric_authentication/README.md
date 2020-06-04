# Xamarin.Forms Cross Platform Biometric Auth sample

## Xamarin Docs followed for reference :

- [Xamarin.iOS Biometric Auth Docs](https://docs.microsoft.com/en-us/xamarin/ios/platform/touch-id-face-id)

- [Xamarin.Android Biometeric Auth Docs](https://docs.microsoft.com/en-us/xamarin/android/platform/fingerprint-authentication/)



### Solution Structure :

The solution includes a Xamarin.Forms sample app called **BioAuthSample**, use the **BioAuthSample.Android** or **BioAuthSample.iOS** in the Target Folder to see how this works.

> The samples use [MobCAT library](https://github.com/xamcat/mobcat-samples) *ServiceContainer Service*

- **BiometricAuthentication.NetCore** project contains the shared interface *IBiometricAuthenticationService*

```
 public interface IBiometricAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string alertMessage = null);

        BiometricType AvailableBiometricType { get; }
    }
```



- **BiometricAuthentication.Droid** and **BiometricAuthentication.iOS** projects contain the respective native implementation of Biometric authentication, in file : *BiometricAuthenticationService.cs*

    - Promptfor iOS is handled by the OS system, the context text can be adjusted via localizedReason variable.
    - Prompt for Android can be edited, default Fragment is provided in **FingerprintManagerFragment.cs** 


## NOTE

> To use in project, remember to set the following in **TARGET PROJECTS** (OS needs to be given permissions to use the biometrics feature). 
> - iOS : Add following in Info.plist
```
<key>NSFaceIDUsageDescription</key>
<string>Need your face to unlock Secrets!</string>
```

> - Android : Add following in AndroidManifest.xml
```
<uses-permission android:name="android.permission.USE_FINGERPRINT" />
```

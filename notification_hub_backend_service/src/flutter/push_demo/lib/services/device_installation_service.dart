import 'package:flutter/services.dart';

class DeviceInstallationService {
  static const deviceInstallation = const MethodChannel('com.mobcat.pushdemo/deviceinstallation');
  static const String getDeviceIdChannelMethod = "getDeviceId";
  static const String getDeviceTokenChannelMethod = "getDeviceToken";
  static const String getDevicePlatformChannelMethod = "getDevicePlatform";

  Future<String> getDeviceId() {
    return deviceInstallation.invokeMethod(getDeviceIdChannelMethod);
  }

  Future<String> getDeviceToken() {
    return deviceInstallation.invokeMethod(getDeviceTokenChannelMethod);
  }

  Future<String> getDevicePlatform() {
    return deviceInstallation.invokeMethod(getDevicePlatformChannelMethod);
  }
}
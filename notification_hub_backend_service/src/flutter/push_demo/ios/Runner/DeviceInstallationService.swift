//
//  DeviceInstallationService.swift
//  Runner
//
//  Created by Mike Parker on 07/07/2020.
//

import Foundation

class DeviceInstallationService {
    
    enum DeviceRegistrationError: Error {
        case notificationSupport(message: String)
    }
    
    var token : Data? = nil
    
    let DEVICE_INSTALLATION_CHANNEL = "com.mobcat.pushdemo/deviceinstallation"
    let GET_DEVICE_ID = "getDeviceId"
    let GET_DEVICE_TOKEN = "getDeviceToken"
    let GET_DEVICE_PLATFORM = "getDevicePlatform"
    
    private let deviceInstallationChannel : FlutterMethodChannel
    
    var notificationsSupported : Bool {
        get {
            if #available(iOS 13.0, *) {
                return true
            }
            else {
                return false
            }
        }
    }
    
    init(withBinaryMessenger binaryMessenger : FlutterBinaryMessenger) {
        deviceInstallationChannel = FlutterMethodChannel(name: DEVICE_INSTALLATION_CHANNEL, binaryMessenger: binaryMessenger)
        deviceInstallationChannel.setMethodCallHandler(handleDeviceInstallationCall)
    }
    
    func getDeviceId() -> String {
        return UIDevice.current.identifierForVendor!.description
    }
    
    func getDeviceToken() throws -> String {
        if(!notificationsSupported) {
            let notificationSupportError = getNotificationsSupportError()
            throw DeviceRegistrationError.notificationSupport(message: notificationSupportError)
        }
        
        if (token == nil) {
            throw DeviceRegistrationError.notificationSupport(message: "Unable to resolve token for APNS.")
        }
        
        return token!.reduce("", {$0 + String(format: "%02X", $1)})
    }
    
    func getDevicePlatform() -> String {
        return "apns"
    }
    
    private func handleDeviceInstallationCall(call: FlutterMethodCall, result: @escaping FlutterResult) {
        switch call.method {
        case GET_DEVICE_ID:
            result(getDeviceId())
        case GET_DEVICE_TOKEN:
            getDeviceToken(result: result)
        case GET_DEVICE_PLATFORM:
            result(getDevicePlatform())
        default:
            result(FlutterMethodNotImplemented)
        }
    }
    
    private func getDeviceToken(result: @escaping FlutterResult) {
        do {
            let token = try getDeviceToken()
            result(token)
        }
        catch let error {
            result(FlutterError(code: "UNAVAILABLE", message: error.localizedDescription, details: nil))
        }
    }
    
    private func getNotificationsSupportError() -> String {
        
        if (!notificationsSupported) {
            return "This app only supports notifications on iOS 13.0 and above. You are running \(UIDevice.current.systemVersion)"
        }
        
        return "An error occurred preventing the use of push notifications."
    }
}

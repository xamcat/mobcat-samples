//
//  NotificationRegistrationService.swift
//  Runner
//
//  Created by Mike Parker on 07/07/2020.
//

import Foundation

class NotificationRegistrationService {
    
    let NOTIFICATION_REGISTRATION_CHANNEL = "com.mobcat.pushdemo/notificationregistration"
    let REFRESH_REGISTRATION = "refreshRegistration"
    
    private let notificationRegistrationChannel : FlutterMethodChannel
    
    init(withBinaryMessenger binaryMessenger : FlutterBinaryMessenger) {
       notificationRegistrationChannel = FlutterMethodChannel(name: NOTIFICATION_REGISTRATION_CHANNEL, binaryMessenger: binaryMessenger)
    }
    
    func refreshRegistration() {
        notificationRegistrationChannel.invokeMethod(REFRESH_REGISTRATION, arguments: nil)
    }
    
}

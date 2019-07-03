//
//  DeviceInstallation.swift
//  NotificationHubSample
//
//  Created by Mike Parker on 04/05/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

import Foundation

struct DeviceInstallation : Codable {
    let installationId : String
    let pushChannel : String
    let platform : String = "apns"
    var tags : [String]
    var templates : Dictionary<String, PushTemplate>
    
    init(withInstallationId installationId : String, andPushChannel pushChannel : String) {
        self.installationId = installationId
        self.pushChannel = pushChannel
        self.tags = [String]()
        self.templates = Dictionary<String, PushTemplate>()
    }
}

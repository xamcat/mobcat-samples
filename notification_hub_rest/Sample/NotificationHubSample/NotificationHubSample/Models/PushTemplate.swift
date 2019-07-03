//
//  PushTemplate.swift
//  NotificationHubSample
//
//  Created by Mike Parker on 04/05/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

import Foundation

struct PushTemplate : Codable {
    let body : String
    
    init(withBody body : String) {
        self.body = body
    }
}

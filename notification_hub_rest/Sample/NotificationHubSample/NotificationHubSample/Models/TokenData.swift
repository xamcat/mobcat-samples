//
//  TokenData.swift
//  NotificationHubSample
//
//  Created by Mike Parker on 04/05/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

import Foundation

struct TokenData {
    
    let token : String
    let expiration : Int

    init(withToken token : String, andTokenExpiration expiration : Int) {
        self.token = token
        self.expiration = expiration
    }
}

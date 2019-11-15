//
//  TokenData.swift
//  SasTokensSwift
//
//  Created by Mike Parker on 26/11/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

import Foundation

struct TokenData {
    
    let token : String
    let expiration : Date
    
    init(withToken token : String, andTokenExpiration expiration : Date) {
        self.token = token
        self.expiration = expiration
    }
}

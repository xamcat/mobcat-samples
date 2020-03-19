//
//  PushTemplate.swift
//  PushDemo
//
//  Created by Mike Parker on 27/04/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

import Foundation

struct PushTemplate : Codable {
    let body : String
    
    init(withBody body : String) {
        self.body = body
    }
}

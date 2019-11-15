//
//  TokenData.m
//  SasTokensObjectiveC
//
//  Created by Mike Parker on 26/11/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef struct {
    NSString* token;
    NSDate* expiration;
} TokenData;

TokenData InitTokenDataWithTokenAndExpiration(NSString* token, NSDate* expiration) {
    
    TokenData tokenData;
    tokenData.token = token;
    tokenData.expiration = expiration;
    
    return tokenData;
}

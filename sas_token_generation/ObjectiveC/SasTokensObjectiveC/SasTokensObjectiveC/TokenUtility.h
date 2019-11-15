//
//  SasTokensObjectiveC.h
//  SasTokensObjectiveC
//
//  Created by Mike Parker on 26/11/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "TokenData.m"

@interface TokenUtility : NSObject

+ (TokenData)getSasTokenForResourceUrl:(NSString*)resourceUrl withKeyName:(NSString*)keyName andKey:(NSString*)key andExpiryInSeconds:(NSInteger)expiryInSeconds;

+ (TokenData)getSasTokenForResourceUrl:(NSString*)resourceUrl withKeyName:(NSString*)keyName andKey:(NSString*)key;

@end

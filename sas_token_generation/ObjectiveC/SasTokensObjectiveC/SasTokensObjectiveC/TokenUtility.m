//
//  TokenData.m
//  SasTokensObjectiveC
//
//  Created by Mike Parker on 26/11/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

#import "TokenUtility.h"
#import <CommonCrypto/CommonHMAC.h>

@implementation TokenUtility

+ (TokenData)getSasTokenForResourceUrl:(NSString*)resourceUrl withKeyName:(NSString*)keyName andKey:(NSString*)key andExpiryInSeconds:(NSInteger)expiryInSeconds {
    
    NSString* expiry = [NSString stringWithFormat:@"%i", (int)[[[NSDate alloc] initWithTimeIntervalSinceNow: 3600] timeIntervalSince1970]];
    NSString* encodedUrl = [TokenUtility URLEncodedStringWithString:resourceUrl];
    NSString* stringToSign = [NSString stringWithFormat:@"%@%@%@", encodedUrl, @"\n", expiry];
    
    NSData* hashValue = [TokenUtility
                         sha256HMacWithData:[stringToSign dataUsingEncoding:NSUTF8StringEncoding]
                         withKey:[key dataUsingEncoding:NSUTF8StringEncoding]];
    
    NSString* signature = [hashValue base64EncodedStringWithOptions:0];
    NSString* encodedSignature = [TokenUtility URLEncodedStringWithString:signature];
    
    NSString* sasToken = [NSString stringWithFormat:
                          @"SharedAccessSignature sr=%@&sig=%@&se=%@&skn=%@",
                          encodedUrl,
                          encodedSignature,
                          expiry,
                          keyName];
    
    NSDate* expiration = [[NSDate alloc] initWithTimeIntervalSinceNow: 3600];

    TokenData tokenData = InitTokenDataWithTokenAndExpiration(sasToken, expiration);
    
    return tokenData;
}

+ (TokenData)getSasTokenForResourceUrl:(NSString*)resourceUrl withKeyName:(NSString*)keyName andKey:(NSString*)key {
    return [TokenUtility getSasTokenForResourceUrl:resourceUrl withKeyName:keyName andKey:key andExpiryInSeconds:3600];
}

+ (NSData *)sha256HMacWithData:(NSData *)data withKey:(NSData *)key {
    
    CCHmacContext context;
    CCHmacInit(&context, kCCHmacAlgSHA256, [key bytes], [key length]);
    CCHmacUpdate(&context, [data bytes], [data length]);
    unsigned char digestRaw[CC_SHA256_DIGEST_LENGTH];
    NSInteger digestLength = CC_SHA256_DIGEST_LENGTH;
    CCHmacFinal(&context, digestRaw);
    
    return [NSData dataWithBytes:digestRaw length:digestLength];
}

// This should be adequate for some parts of the query, but it's not accurate in all cases.
// Either way, it's better than the built-in NSString percent encoding.
// It should be fine for the values in a SAS token (including the sig), which is what we're currently using it for.
// See: https://github.com/Azure/azure-storage-ios/blob/master/Lib/Azure%20Storage%20Client%20Library/Azure%20Storage%20Client%20Library/AZSUtil.m
+ (NSString *) URLEncodedStringWithString:(NSString* )stringToConvert {
    
    NSMutableString *encodedString = [NSMutableString string];
    const char *sourceUTF8 = [stringToConvert cStringUsingEncoding:NSUTF8StringEncoding];
    unsigned long length = strlen(sourceUTF8);
    
    for (int i = 0; i < length; i++)
    {
        const char currentChar = sourceUTF8[i];
        if (currentChar == '.' || currentChar == '-' || currentChar == '_' || currentChar == '~' ||
            (currentChar >= 'a' && currentChar <= 'z') ||
            (currentChar >= 'A' && currentChar <= 'Z') ||
            (currentChar >= '0' && currentChar <= '9'))
        {
            [encodedString appendFormat:@"%c", currentChar];
        }
        else
        {
            [encodedString appendFormat:@"%%%02x", currentChar];
        }
    }
    
    return encodedString;
}

@end

//
//  SasTokensSwift.swift
//  SasTokensSwift
//
//  Created by Mike Parker on 26/11/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

import Foundation

struct TokenUtility {
    typealias Context = UnsafeMutablePointer<CCHmacContext>
    
    static func getSasToken(forResourceUrl resourceUrl : String, withKeyName keyName : String, andKey key : String, andExpiryInSeconds expiryInSeconds : Int = 3600) -> TokenData {
        let expiry = (Int(NSDate().timeIntervalSince1970) + expiryInSeconds).description
        let encodedUrl = urlEncodedString(withString: resourceUrl)
        let stringToSign = "\(encodedUrl)\n\(expiry)"
        let hashValue = sha256HMac(withData: stringToSign.data(using: .utf8)!, andKey: key.data(using: .utf8)!)
        let signature = hashValue.base64EncodedString(options: .init(rawValue: 0))
        let encodedSignature = urlEncodedString(withString: signature)
        let sasToken = "SharedAccessSignature sr=\(encodedUrl)&sig=\(encodedSignature)&se=\(expiry)&skn=\(keyName)"
        let expiration = Date.init(timeInterval: TimeInterval(exactly: expiryInSeconds)!, since: Date())
        let tokenData = TokenData(withToken: sasToken, andTokenExpiration: expiration)
        
        return tokenData
    }
    
    private static func sha256HMac(withData data : Data, andKey key : Data) -> Data {
        let context = Context.allocate(capacity: 1)
        CCHmacInit(context, CCHmacAlgorithm(kCCHmacAlgSHA256), (key as NSData).bytes, size_t((key as NSData).length))
        CCHmacUpdate(context, (data as NSData).bytes, (data as NSData).length)
        var hmac = Array<UInt8>(repeating: 0, count: Int(CC_SHA256_DIGEST_LENGTH))
        CCHmacFinal(context, &hmac)
        
        let result = NSData(bytes: hmac, length: hmac.count)
        context.deallocate()
        
        return result as Data
    }
    
    // This should be adequate for some parts of the query, but it's not accurate in all cases.
    // Either way, it's better than the built-in NSString percent encoding.
    // It should be fine for the values in a SAS token (including the sig), which is what we're currently using it for.
    // See: https://github.com/Azure/azure-storage-ios/blob/master/Lib/Azure%20Storage%20Client%20Library/Azure%20Storage%20Client%20Library/AZSUtil.m
    private static func urlEncodedString(withString stringToConvert : String) -> String {
        var encodedString = ""
        let sourceUtf8 = (stringToConvert as NSString).utf8String
        let length = strlen(sourceUtf8)
        
        let charArray: [Character] = [ ".", "-", "_", "~", "a", "z", "A", "Z", "0", "9"]
        let asUInt8Array = String(charArray).utf8.map{ Int8($0) }
        
        for i in 0..<length {
            let currentChar = sourceUtf8![i]
            
            if (currentChar == asUInt8Array[0] || currentChar == asUInt8Array[1] || currentChar == asUInt8Array[2] || currentChar == asUInt8Array[3] ||
                (currentChar >= asUInt8Array[4] && currentChar <= asUInt8Array[5]) ||
                (currentChar >= asUInt8Array[6] && currentChar <= asUInt8Array[7]) ||
                (currentChar >= asUInt8Array[8] && currentChar <= asUInt8Array[9])) {
                encodedString += String(format:"%c", currentChar)
            }
            else {
                encodedString += String(format:"%%%02x", currentChar)
            }
        }
        
        return encodedString
    }
}

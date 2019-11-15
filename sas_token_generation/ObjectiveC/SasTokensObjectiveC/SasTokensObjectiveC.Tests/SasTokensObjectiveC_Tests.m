//
//  SasTokensObjectiveC_Tests.m
//  SasTokensObjectiveC.Tests
//
//  Created by Mike Parker on 26/11/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

#import <XCTest/XCTest.h>
#import "TokenUtility.h"

@interface SasTokensObjectiveC_Tests : XCTestCase

@end

@implementation SasTokensObjectiveC_Tests

- (void)setUp {
    // Put setup code here. This method is called before the invocation of each test method in the class.
}

- (void)tearDown {
    // Put teardown code here. This method is called after the invocation of each test method in the class.
}

- (void)testSasTokenGeneration {
    // This is a quick integration test case demonstrating the use of the generated SAS token with the Azure Notification Hub REST API.
    // Update the expected inputs below (using the values from your own Notification Hub).
    // This uses the REST API to send a template notification
    // See: https://msdn.microsoft.com/en-us/library/azure/dn495827.aspx
    
    // Expected Inputs
    NSString* resourceUrl = @"https://<namespace>.servicebus.windows.net/<hub_name>";
    NSString* secretKeyName = @"DefaultFullSharedAccessSignature";
    NSString* secretKey = @"<secret_key>";
    
    // Generate Token
    TokenData generatedToken = [TokenUtility getSasTokenForResourceUrl:resourceUrl withKeyName:secretKeyName andKey:secretKey];
    
    // Call Notification Hub API using the generated token
    // This will result in a 201 Created success status code (even if there are not device installations)
    // See: https://docs.microsoft.com/en-us/previous-versions/azure/reference/mt621172(v%3dazure.100)
    __block NSInteger apiResponse = 0;
    XCTestExpectation* expectation = [self expectationWithDescription:@"Validating Token"];
    NSURL* testEndpoint = [[NSURL alloc] initWithString: [NSString stringWithFormat:@"%@/messages?api-version=2015-01", resourceUrl]];
    
    NSMutableURLRequest* request = [[NSMutableURLRequest alloc] initWithURL:testEndpoint];
    request.HTTPMethod = @"POST";
    [request addValue:@"2015-01" forHTTPHeaderField:@"x-ms-version"];
    [request addValue:@"application/json;charset=utf-8." forHTTPHeaderField:@"Content-Type"];
    [request addValue:@"template" forHTTPHeaderField:@"ServiceBusNotification-Format"];
    [request addValue:generatedToken.token forHTTPHeaderField:@"Authorization"];
    request.HTTPBody = [@"{ \"message\" : \"Testing\" }" dataUsingEncoding: NSUTF8StringEncoding];
    
    [[NSURLSession.sharedSession dataTaskWithRequest:request
                completionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
                    apiResponse = ((NSHTTPURLResponse *)response).statusCode;
                    [expectation fulfill];
                    
                }] resume];
    
    [self waitForExpectationsWithTimeout: 5 handler:nil];
    
    // Should get a 201 status code
    XCTAssert(apiResponse == 201);
}

@end

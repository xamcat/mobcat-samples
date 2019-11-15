//
//  SasTokensSwift_Tests.swift
//  SasTokensSwift.Tests
//
//  Created by Mike Parker on 26/11/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

import XCTest
@testable import SasTokensSwift

class SasTokensSwift_Tests: XCTestCase {

    override func setUp() {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDown() {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }

    func testSasTokenGeneration() {
        // This is a quick integration test case demonstrating the use of the generated SAS token with the Azure Notification Hub REST API.
        // Update the expected inputs below (using the values from your own Notification Hub).
        // This uses the REST API to send a template notification
        // See: https://msdn.microsoft.com/en-us/library/azure/dn495827.aspx
        
        // Expected Inputs
        let resourceUrl = "https://<namespace>.servicebus.windows.net/<hub_name>"
        let secretKeyName = "DefaultFullSharedAccessSignature"
        let secretKey = "<secret_key>"
        
        // Generate Token
        let generatedToken = TokenUtility.getSasToken(forResourceUrl: resourceUrl, withKeyName:secretKeyName, andKey:secretKey);
        
        // Call Notification Hub API using the generated token
        // This will result in a 201 Created success status code (even if there are not device installations)
        // See: https://docs.microsoft.com/en-us/previous-versions/azure/reference/mt621172(v%3dazure.100)
        var apiResponse: Int = 0
        let expectation = self.expectation(description: "Validating Token")
        let testEndpoint = URL(string: "\(resourceUrl)/messages?api-version=2015-01")
        
        let request = NSMutableURLRequest(url: testEndpoint!)
        request.httpMethod = "POST"
        request.addValue("2015-01", forHTTPHeaderField: "x-ms-version")
        request.addValue("application/json;charset=utf-8.", forHTTPHeaderField: "Content-Type")
        request.addValue("template", forHTTPHeaderField: "ServiceBusNotification-Format")
        request.addValue(generatedToken.token, forHTTPHeaderField: "Authorization")
        request.httpBody = "{ \"message\" : \"Testing\" }".data(using: .utf8)

        URLSession.shared.dataTask(with: request as URLRequest) { (data, response, error) in
            apiResponse = (response as! HTTPURLResponse).statusCode
            expectation.fulfill()
        }.resume()
        
        waitForExpectations(timeout: 5, handler: nil)
        
        // Should get a 201 status code
        XCTAssert(apiResponse == 201)
    }
}

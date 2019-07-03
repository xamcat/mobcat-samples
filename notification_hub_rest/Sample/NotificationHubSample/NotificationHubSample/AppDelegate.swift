//
//  AppDelegate.swift
//  NotificationHubSample
//
//  Created by Mike Parker on 27/04/2018.
//  Copyright © 2018 mobcat. All rights reserved.
//

import UIKit
import UserNotifications

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate, UNUserNotificationCenterDelegate {

    var window: UIWindow?
    var configValues: NSDictionary?
    var registrationService : NotificationRegistrationService?
    var notificationHubNamespace : String?
    var notificationHubName : String?
    var notificationHubKeyName : String?
    var notificationHubKey : String?
    let tags = ["12345"]
    let genericTemplate = PushTemplate(withBody: "{\"aps\":{\"alert\":\"$(message)\"}}")

    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
        
        if let path = Bundle.main.path(forResource: "devsettings", ofType: "plist") {
            if let configValues = NSDictionary(contentsOfFile: path) {
                self.notificationHubNamespace = configValues["notificationHubNamespace"] as? String
                self.notificationHubName = configValues["notificationHubName"] as? String
                self.notificationHubKeyName = configValues["notificationHubKeyName"] as? String
                self.notificationHubKey = configValues["notificationHubKey"] as? String
            }
        }
        
        if #available(iOS 10.0, *){
            UNUserNotificationCenter.current().delegate = self
            UNUserNotificationCenter.current().requestAuthorization(options: [.alert, .sound, .badge]) {
                (granted, error) in
                
                if (granted)
                {
                    DispatchQueue.main.async {
                        application.registerForRemoteNotifications()
                    }
                }
                else {
                    
                }
            }
        }
        else {
            UIApplication.shared.registerForRemoteNotifications(matching: [.badge, .alert, .sound])
        }

        return true
    }
    
    func application(_ application: UIApplication, didRegisterForRemoteNotificationsWithDeviceToken deviceToken: Data) {
        // Get the installation id and push channel values
        let installationId = (UIDevice.current.identifierForVendor?.description)!
        let pushChannel = deviceToken.reduce("", {$0 + String(format: "%02X", $1)})

        // Initialize the Notification Registration Service
        self.registrationService = NotificationRegistrationService(
            withInstallationId: installationId,
            andPushChannel: pushChannel,
            andHubNamespace: notificationHubNamespace!,
            andHubName: notificationHubName!,
            andKeyName: notificationHubKeyName!,
            andKey: notificationHubKey!)
        
        self.registrationService!.register(withTags: tags, andTemplates: ["genericTemplate" : self.genericTemplate]) { (result) in
            if !result {
                print("Registration issue")
            } else {
                print("Registered")
            }
        }
    }
    
    func application(_ application: UIApplication, didReceiveRemoteNotification userInfo: [AnyHashable : Any]) {
        showAlert(withText: userInfo.description)
    }
    
    func application(_ application: UIApplication, didFailToRegisterForRemoteNotificationsWithError error: Error) {
        showAlert(withText: error.localizedDescription)
    }
    
    @available(iOS 10.0, *)
    func userNotificationCenter(_ center: UNUserNotificationCenter, willPresent notification: UNNotification, withCompletionHandler completionHandler: @escaping (UNNotificationPresentationOptions) -> Void) {
        showAlert(withText: notification.request.content.body)
    }
    
    @available(iOS 10.0, *)
    func userNotificationCenter(_ center: UNUserNotificationCenter, didReceive response: UNNotificationResponse, withCompletionHandler completionHandler: @escaping () -> Void) {
        showAlert(withText: response.notification.request.content.body)
    }
    
    func showAlert(withText text : String) {
        let alertController = UIAlertController(title: "PushDemo", message: text, preferredStyle: UIAlertController.Style.alert)
        alertController.addAction(UIAlertAction(title: "OK", style: UIAlertAction.Style.default,handler: nil))
        self.window?.rootViewController?.present(alertController, animated: true, completion: nil)
    }

    func applicationWillResignActive(_ application: UIApplication) {
        // Sent when the application is about to move from active to inactive state. This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) or when the user quits the application and it begins the transition to the background state.
        // Use this method to pause ongoing tasks, disable timers, and invalidate graphics rendering callbacks. Games should use this method to pause the game.
    }

    func applicationDidEnterBackground(_ application: UIApplication) {
        // Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later.
        // If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
    }

    func applicationWillEnterForeground(_ application: UIApplication) {
        // Called as part of the transition from the background to the active state; here you can undo many of the changes made on entering the background.
    }

    func applicationDidBecomeActive(_ application: UIApplication) {
        // Restart any tasks that were paused (or not yet started) while the application was inactive. If the application was previously in the background, optionally refresh the user interface.
    }

    func applicationWillTerminate(_ application: UIApplication) {
        // Called when the application is about to terminate. Save data if appropriate. See also applicationDidEnterBackground:.
    }
}


import UIKit
import Flutter

@UIApplicationMain
@objc class AppDelegate: FlutterAppDelegate {
    
    var deviceInstallationService : DeviceInstallationService?
    var notificationRegistrationService : NotificationRegistrationService?
    var notificationActionService : NotificationActionService?
        
    override func application(
        _ application: UIApplication,
        didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
        
        GeneratedPluginRegistrant.register(with: self)
        
        let controller : FlutterViewController = window?.rootViewController as! FlutterViewController
        
        deviceInstallationService = DeviceInstallationService(withBinaryMessenger: controller.binaryMessenger)
        notificationRegistrationService = NotificationRegistrationService(withBinaryMessenger: controller.binaryMessenger)
        notificationActionService = NotificationActionService(withBinaryMessenger: controller.binaryMessenger)
        
        if #available(iOS 13.0, *) {
            UNUserNotificationCenter.current().requestAuthorization(options: [.alert, .sound, .badge]) {
                (granted, error) in

                if (granted)
                {
                    DispatchQueue.main.async {
                        let pushSettings = UIUserNotificationSettings(types: [.alert, .sound, .badge], categories: nil)
                        application.registerUserNotificationSettings(pushSettings)
                        application.registerForRemoteNotifications()
                    }
                }
            }
        }
        
        if let userInfo = launchOptions?[.remoteNotification] as? [AnyHashable : Any] {
            processNotificationActions(userInfo: userInfo, launchAction: true)
        }

        return super.application(application, didFinishLaunchingWithOptions: launchOptions)
    }
    
    override func application(_ application: UIApplication, didRegisterForRemoteNotificationsWithDeviceToken deviceToken: Data) {
        
        deviceInstallationService?.token = deviceToken
        notificationRegistrationService?.refreshRegistration()
    }
    
    override func application(_ application: UIApplication, didFailToRegisterForRemoteNotificationsWithError error: Error) {
        print(error);
    }
    
    override func application(_ application: UIApplication, didReceiveRemoteNotification userInfo: [AnyHashable : Any]) {
        processNotificationActions(userInfo: userInfo)
    }
    
    func processNotificationActions(userInfo: [AnyHashable : Any], launchAction: Bool = false) {
        if let action = userInfo["action"] as? String {
            if (launchAction) {
                notificationActionService?.launchAction = action
            }
            else {
                notificationActionService?.triggerAction(action: action)
            }
        }
    }
}

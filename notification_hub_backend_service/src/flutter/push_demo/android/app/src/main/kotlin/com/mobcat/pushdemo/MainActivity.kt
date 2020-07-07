package com.mobcat.pushdemo

import android.content.Intent
import android.os.Bundle
import com.google.android.gms.tasks.OnCompleteListener
import com.google.firebase.iid.FirebaseInstanceId
import com.mobcat.pushdemo.services.NotificationActionService
import com.mobcat.pushdemo.services.NotificationRegistrationService
import io.flutter.embedding.android.FlutterActivity

class MainActivity: FlutterActivity() {

    private lateinit var deviceInstallationService: DeviceInstallationService

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        flutterEngine?.let {
            deviceInstallationService = DeviceInstallationService(context, it)
            PushNotificationsFirebaseMessagingService.notificationActionService = NotificationActionService(it)
            PushNotificationsFirebaseMessagingService.notificationRegistrationService = NotificationRegistrationService(it)
        }

        if(deviceInstallationService?.playServicesAvailable) {
            FirebaseInstanceId.getInstance().instanceId
                    .addOnCompleteListener(OnCompleteListener { task ->
                        if (!task.isSuccessful)
                            return@OnCompleteListener
                        PushNotificationsFirebaseMessagingService.token = task.result?.token
                        PushNotificationsFirebaseMessagingService.notificationRegistrationService?.refreshRegistration()
                    })
        }

        processNotificationActions(this.intent, true)
    }

    override fun onNewIntent(intent: Intent) {
        super.onNewIntent(intent)
        processNotificationActions(intent)
    }

    private fun processNotificationActions(intent: Intent, launchAction: Boolean = false) {
        if (intent.hasExtra("action")) {
            var action = intent.getStringExtra("action");

            if (action.isNotEmpty()) {
                if (launchAction) {
                    PushNotificationsFirebaseMessagingService.notificationActionService?.launchAction = action
                }
                else {
                    PushNotificationsFirebaseMessagingService.notificationActionService?.triggerAction(action)
                }
            }
        }
    }
}
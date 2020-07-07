package com.mobcat.pushdemo

import android.annotation.SuppressLint
import android.content.Context
import android.provider.Settings.Secure
import com.google.android.gms.common.ConnectionResult
import com.google.android.gms.common.GoogleApiAvailability
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugin.common.MethodCall
import io.flutter.plugin.common.MethodChannel

@SuppressLint("HardwareIds")
class DeviceInstallationService {

    companion object {
        const val DEVICE_INSTALLATION_CHANNEL = "com.mobcat.pushdemo/deviceinstallation"
        const val GET_DEVICE_ID = "getDeviceId"
        const val GET_DEVICE_TOKEN = "getDeviceToken"
        const val GET_DEVICE_PLATFORM = "getDevicePlatform"
    }

    private var context: Context
    private var deviceInstallationChannel : MethodChannel

    val playServicesAvailable
        get() = GoogleApiAvailability.getInstance().isGooglePlayServicesAvailable(context) == ConnectionResult.SUCCESS

    constructor(context: Context, flutterEngine: FlutterEngine) {
        this.context = context
        deviceInstallationChannel = MethodChannel(flutterEngine.dartExecutor.binaryMessenger, DEVICE_INSTALLATION_CHANNEL)
        deviceInstallationChannel.setMethodCallHandler { call, result -> handleDeviceInstallationCall(call, result) }
    }

    fun getDeviceId() : String
        = Secure.getString(context.applicationContext.contentResolver, Secure.ANDROID_ID)

    fun getDeviceToken() : String {
        if(!playServicesAvailable) {
            throw Exception(getPlayServicesError())
        }

        val token = PushNotificationsFirebaseMessagingService.token

        if (token.isNullOrBlank()) {
            throw Exception("Unable to resolve token for FCM.")
        }

        return token
    }

    fun getDevicePlatform() : String = "fcm"

    private fun handleDeviceInstallationCall(call: MethodCall, result: MethodChannel.Result) {
        when (call.method) {
            GET_DEVICE_ID -> {
                result.success(getDeviceId())
            }
            GET_DEVICE_TOKEN -> {
                getDeviceToken(result)
            }
            GET_DEVICE_PLATFORM -> {
                result.success(getDevicePlatform())
            }
            else -> {
                result.notImplemented()
            }
        }
    }

    private fun getDeviceToken(result: MethodChannel.Result) {
        try {
            val token = getDeviceToken()
            result.success(token)
        }
        catch (e: Exception) {
            result.error("ERROR", e.message, e)
        }
    }

    private fun getPlayServicesError(): String {
        val resultCode = GoogleApiAvailability.getInstance().isGooglePlayServicesAvailable(context)

        if (resultCode != ConnectionResult.SUCCESS) {
            return if (GoogleApiAvailability.getInstance().isUserResolvableError(resultCode)){
                GoogleApiAvailability.getInstance().getErrorString(resultCode)
            } else {
                "This device is not supported"
            }
        }

        return "An error occurred preventing the use of push notifications"
    }
}
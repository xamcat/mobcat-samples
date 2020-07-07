import 'package:flutter/services.dart';
import 'dart:async';
import 'package:push_demo/models/push_demo_action.dart';

class NotificationActionService {
  static const notificationAction =
      const MethodChannel('com.mobcat.pushdemo/notificationaction');
  static const String triggerActionChannelMethod = "triggerAction";
  static const String getLaunchActionChannelMethod = "getLaunchAction";

  final actionMappings = {
    'action_a' : PushDemoAction.actionA,
    'action_b' : PushDemoAction.actionB
  };

  final actionTriggeredController = StreamController.broadcast();

  NotificationActionService() {
    notificationAction
        .setMethodCallHandler(handleNotificationActionCall);
  }

  Stream get actionTriggered => actionTriggeredController.stream;

  Future<void> triggerAction({action: String}) async {

    if (!actionMappings.containsKey(action)) {
      return;
    }

    actionTriggeredController.add(actionMappings[action]); 
  }

  Future<void> checkLaunchAction() async {
    final launchAction = await notificationAction.invokeMethod(getLaunchActionChannelMethod) as String;

    if (launchAction != null) {
      triggerAction(action: launchAction);
    }
  }

  Future<void> handleNotificationActionCall(MethodCall call) async {
    switch (call.method) {
      case triggerActionChannelMethod:
        return triggerAction(action: call.arguments as String);
      default:
        throw MissingPluginException();
        break;
    }
  }
}
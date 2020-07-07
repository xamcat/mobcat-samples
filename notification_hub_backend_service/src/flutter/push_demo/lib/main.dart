import 'package:flutter/material.dart';
import 'package:push_demo/models/push_demo_action.dart';
import 'package:push_demo/services/notification_action_service.dart';
import 'package:push_demo/main_page.dart';

final navigatorKey = GlobalKey<NavigatorState>();
final notificationActionService = NotificationActionService();

void main() async {
  runApp(MaterialApp(home: MainPage(), navigatorKey: navigatorKey,));
  notificationActionService.actionTriggered.listen((event) { notificationActionTriggered(event as PushDemoAction); });
  await notificationActionService.checkLaunchAction();
}

void notificationActionTriggered(PushDemoAction action) {
  showActionAlert(message: "${action.toString().split(".")[1]} action received"); 
}

Future<void> showActionAlert({ message: String }) async {
  return showDialog<void>(
    context: navigatorKey.currentState.overlay.context,
    barrierDismissible: false, 
    builder: (BuildContext context) {
      return AlertDialog(
        title: Text('PushDemo'),
        content: SingleChildScrollView(
          child: ListBody(
            children: <Widget>[
              Text(message),
            ],
          ),
        ),
        actions: <Widget>[
          FlatButton(
            child: Text('OK'),
            onPressed: () {
              Navigator.of(context).pop();
            },
          ),
        ],
      );
    },
  );
}
import 'package:flutter/material.dart';
import 'package:push_demo/services/notification_registration_service.dart';

import 'config.dart';

class MainPage extends StatefulWidget {
  @override
  _MainPageState createState() => _MainPageState();
}

class _MainPageState extends State<MainPage> {
  final notificationRegistrationService = NotificationRegistrationService(Config.backendServiceEndpoint, Config.apiKey);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        body: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20.0, vertical: 40.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.end,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: <Widget>[
              FlatButton(
                child: Text("Register"),
                onPressed: registerButtonClicked,
              ),
              FlatButton(
                child: Text("Deregister"),
                onPressed: deregisterButtonClicked,
              ),
            ],
          ),
        ),
      );
  }

  void registerButtonClicked() async {
    try {
      await notificationRegistrationService.registerDevice(List<String>());
      await showAlert(message: "Device registered");
    }
    catch (e) {
      await showAlert(message: e);
    }
  }

  void deregisterButtonClicked() async {
    try {
      await notificationRegistrationService.deregisterDevice();
      await showAlert(message: "Device deregistered");
    }
    catch (e) {
      await showAlert(message: e);
    }
  }

  Future<void> showAlert({ message: String }) async {
    return showDialog<void>(
      context: context,
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
}
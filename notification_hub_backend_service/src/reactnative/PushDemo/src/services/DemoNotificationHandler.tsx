import PushNotification from 'react-native-push-notification';

class DemoNotificationHandler {
  private _onRegister: any;
  private _onNotification: any;
  
  onNotification(notification: any) {
    console.log('NotificationHandler:', notification);

    if (typeof this._onNotification === 'function') {
      this._onNotification(notification);
    }
  }

  onRegister(token: any) {
    console.log('NotificationHandler:', token);

    if (typeof this._onRegister === 'function') {
      this._onRegister(token);
    }
  }

  attachTokenReceived(handler: any) {
    this._onRegister = handler;
  }

  attachNotificationReceived(handler: any) {
    this._onNotification = handler;
  }
}

const handler = new DemoNotificationHandler();

PushNotification.configure({
  onRegister: handler.onRegister.bind(handler),
  onNotification: handler.onNotification.bind(handler),
  permissions: {
    alert: true,
    badge: true,
    sound: true,
  },
  popInitialNotification: true,
  requestPermissions: true,
});

export default handler;

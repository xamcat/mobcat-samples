import React, { Component } from 'react';
import {
  StyleSheet,
  View,
  Text,
  Alert,
  Button,
  ActivityIndicator,
} from 'react-native';
import DeviceInfo from 'react-native-device-info';
import DemoNotificationService from './DemoNotificationService';

declare const global: { HermesInternal: null | {} };

interface IState {
  status: string,
  registeredOS: string,
  registerToken: string,
  isBusy: boolean,
}

class App extends Component<IState> {
  notificationService: DemoNotificationService;
  state: IState;

  constructor(props: any) {
    super(props);
    this.state = {
      status: "Push notifications registration status is unknown",
      registeredOS: "",
      registerToken: "",
      isBusy: false,
    };
    this.notificationService = new DemoNotificationService(
      this.onRegister.bind(this),
      this.onNotification.bind(this),
    );
  }

  render() {
    return (
      <View style={styles.container}>
        <Text style={styles.title}>Welcome to PushDemo app</Text>
        <Text style={styles.subtitle}>PushDemo is a React Native application which registers for push notifications with Azure Web API backend and subscribes to receive push updates</Text>
        <Text style={styles.status}>Status: {this.state.status}</Text>
        {this.state.isBusy &&
          <ActivityIndicator></ActivityIndicator>
        }
        <View style={styles.buttonsContainer}>
          <View style={styles.button}>
            <Button title="Register for pushes" onPress={this.onRegisterButtonPress.bind(this)} disabled={this.state.isBusy} />
          </View>
          <View style={styles.button}>
            <Button title="Deregister" onPress={this.onDeregisterButtonPress.bind(this)} disabled={this.state.isBusy} />
          </View>
        </View>
      </View>
    );
  }

  async onRegisterButtonPress() {
    if (!this.state.registerToken || !this.state.registeredOS) {
      Alert.alert("The push notifications token wasn't received.");
      return;
    }

    try {
      this.setState({ isBusy: true });
      const registerApiUrl = "https://push-demo-api-alstrakh.azurewebsites.net/api/notifications/installations";
      const deviceId = DeviceInfo.getUniqueId();
      const pnPlatform = this.state.registeredOS == "ios" ? "apns" : "fcm";
      const pnToken = this.state.registerToken;
      const pnGenericTemplate = this.state.registeredOS == "ios" ? "{\"aps\":{\"alert\":\"$(alertMessage)\"}, \"action\": \"$(alertAction)\"}" : "{\"data\":{\"message\":\"$(alertMessage)\", \"action\":\"$(alertAction)\"}}";
      const pnSilentTemplate = this.state.registeredOS == "ios" ? "{\"aps\":{\"content-available\":1, \"apns-priority\": 5, \"sound\":\"\", \"badge\": 0}, \"message\": \"$(silentMessage)\", \"action\": \"$(silentAction)\"}" : "{\"data\":{\"message\":\"$(silentMessage)\", \"action\":\"$(silentAction)\", \"silent\":\"true\"}}"
      const apiKey = '123-456';
      const result = await fetch(registerApiUrl, {
        method: 'PUT',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
          'apiKey': apiKey
        },
        body: JSON.stringify({
          installationId: deviceId,
          platform: pnPlatform,
          pushChannel: pnToken,
          tags: [],
          templates: {
            genericTemplate: {
              body: pnGenericTemplate
            },
            silentTemplate: {
              body: pnSilentTemplate
            }
          }
        })
      });
      var status = result.status == 200 ? `Registered for ${this.state.registeredOS} push notifications` : `Registration error ${result.status}: ${result.statusText}`;
      this.setState({ status: status });
    }
    finally {
      this.setState({ isBusy: false });
    }
  }

  async onDeregisterButtonPress() {
    if (!this.notificationService)
      return;

    try {
      this.setState({ isBusy: true });
      const deviceId = DeviceInfo.getUniqueId();
      const registerApiUrl = `https://push-demo-api-alstrakh.azurewebsites.net/api/notifications/installations/${deviceId}`;
      const apiKey = '123-456';
      const result = await fetch(registerApiUrl, {
        method: 'DELETE',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
          'apiKey': apiKey
        }
      });
      var status = result.status == 200 ? `Deregistered from push notifications` : `Deregistration error ${result.status}: ${result.statusText}`;
      this.setState({ status: status });
    }
    finally {
      this.setState({ isBusy: false });
    }
  }

  async onRegister(token: any) {
    this.setState({ registerToken: token.token, registeredOS: token.os, status: `The push notifications token has been received.` });
  }

  onNotification(notification: any) {
    this.setState({ status: `Received a push notification...` });
    Alert.alert(notification.data.action, notification.data.message);
  }

  handlePerm(permissions: any) {
    console.log('Push notification handle permissions request has been received.');
  }
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: "center",
    backgroundColor: "azure",
  },
  title: {
    fontSize: 40,
    textAlign: "center",
    margin: 50,
  },
  subtitle: {
    fontSize: 18,
    textAlign: "center",
    margin: 10,
    color: "#3e3e3e",
  },
  status: {
    fontSize: 18,
    textAlign: "center",
    margin: 15,
    color: "black",
    backgroundColor: "yellow",
  },
  button: {
    marginBottom: 10,
  },
  buttonsContainer: {
    flex: 1,
    justifyContent: 'flex-end',
    marginBottom: 50
  }
});

export default App;

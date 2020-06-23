/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * Generated with the TypeScript template
 * https://github.com/react-native-community/react-native-template-typescript
 *
 * @format
 */

import React, { Component } from 'react';
import {
  SafeAreaView,
  StyleSheet,
  ScrollView,
  View,
  Text,
  StatusBar,
  Alert,
  Button,
} from 'react-native';

import {
  Header,
  LearnMoreLinks,
  Colors,
  DebugInstructions,
  ReloadInstructions,
} from 'react-native/Libraries/NewAppScreen';
import DemoNotificationService from './DemoNotificationService';

declare const global: { HermesInternal: null | {} };

class App extends Component {
  private notificationService: DemoNotificationService;

  constructor(props) {
    super(props);
    this.state = {};
    this.notificationService = new DemoNotificationService(
      this.onRegister.bind(this),
      this.onNotification.bind(this),
    );
  }

  render() {
    return (
      <>
        <StatusBar barStyle="dark-content" />
        <SafeAreaView>
          <ScrollView
            contentInsetAdjustmentBehavior="automatic"
            style={styles.scrollView}>
            <Header />
            <Text>PushDemo is a React Native application which registers for push notifications with Azure Web API backend and subscribes to receive push updates</Text>
            <Button title="Register for pushes" onPress={this.onRegisterButtonPress.bind(this)} />
            <Button title="Deregister" onPress={this.onDeregisterButtonPress.bind(this)} />
          </ScrollView>
        </SafeAreaView>
      </>
    );
  }

  async onRegisterButtonPress() {
    if (!this.state.registerToken || !this.state.registeredOS) {
      Alert.alert("The push notifications token wasn't received.");
      return;
    }

    // TODO: get device id
    const registerApiUrl = "https://push-demo-api-alstrakh.azurewebsites.net/api/notifications/installations";
    const deviceId = this.state.registeredOS == "ios" ? "A556CF39-8A55-4F7E-9DE3-E5863FAAF8CC" : "A556CF39-8A55-4F7E-9DE3-E5863FAAF8BB";
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

    console.log(result);
    // TODO: check response code
    Alert.alert("Registered", "The app has been successfully registered for push notifications");
  }

  async onDeregisterButtonPress() {
    if (!this.notificationService)
      return;

    const deviceId = this.state.registeredOS == "ios" ? "A556CF39-8A55-4F7E-9DE3-E5863FAAF8CC" : "A556CF39-8A55-4F7E-9DE3-E5863FAAF8BB";
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

    console.log(result);
    // TODO: check response code
    Alert.alert("Deregistered", "The app has been successfully deregistered from push notifications");
  }

  async onRegister(token) {
    console.log(`The push notifications token has been received for ${token.os}.`);
    this.setState({ registerToken: token.token, registeredOS: token.os });
    Alert.alert("Token received", "The push notifications token has been received.");
  }

  onNotification(notification) {
    console.log(`Push notification has been received for ${this.state.registeredOS}.`);
    Alert.alert(notification.data.action, notification.data.message);
  }

  handlePerm(permissions) {
    console.log('Push notification handle permissions request has been received.');
  }
};

const styles = StyleSheet.create({
  scrollView: {
    backgroundColor: Colors.lighter,
  },
  engine: {
    position: 'absolute',
    right: 0,
  },
  body: {
    backgroundColor: Colors.white,
  },
  sectionContainer: {
    marginTop: 32,
    paddingHorizontal: 24,
  },
  sectionTitle: {
    fontSize: 24,
    fontWeight: '600',
    color: Colors.black,
  },
  sectionDescription: {
    marginTop: 8,
    fontSize: 18,
    fontWeight: '400',
    color: Colors.dark,
  },
  highlight: {
    fontWeight: '700',
  },
  footer: {
    color: Colors.dark,
    fontSize: 12,
    fontWeight: '600',
    padding: 4,
    paddingRight: 12,
    textAlign: 'right',
  },
});

export default App;

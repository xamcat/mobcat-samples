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
} from 'react-native';

import {
  Header,
  LearnMoreLinks,
  Colors,
  DebugInstructions,
  ReloadInstructions,
} from 'react-native/Libraries/NewAppScreen';
import DemoNotificationService from './DemoNotificationService';

declare const global: {HermesInternal: null | {}};

class App extends Component {
  private notificationService: DemoNotificationService;

  constructor(props) {
    console.log('!!! App.ctr');
    super(props);
    this.state = {};

    this.notificationService = new DemoNotificationService(
      this.onRegister.bind(this),
      this.onNotification.bind(this),
    );

    console.log('!!! App.ctr completed');
  }

  render(){
    return (
      <>
        <StatusBar barStyle="dark-content" />
        <SafeAreaView>
          <ScrollView
            contentInsetAdjustmentBehavior="automatic"
            style={styles.scrollView}>
            <Header />
            <Text>PushDemo is a React Native application which registers for push notifications with Azure Web API backend and subscribes to receive push updates</Text>
          </ScrollView>
        </SafeAreaView>
      </>
    );
  }

  async onRegister(token) {
    console.log('!!! App.onRegister');
    Alert.alert("Registered", `The app has been successfully registered for push notifications: ${token.os}=${token.token}`);
    this.setState({registerToken: token.token, fcmRegistered: true});

    const registerApiUrl = "https://push-demo-api-alstrakh.azurewebsites.net/api/notifications/installations";
    const deviceId = "A556CF39-8A55-4F7E-9DE3-E5863FAAF8BB";
    const pnPlatform = token.os == "ios" ? "apns" : "fcm";
    const pnGenericTemplate = token.os == "ios" ? "{\"aps\":{\"alert\":\"$(alertMessage)\"}, \"action\": \"$(alertAction)\"}" : "{\"data\":{\"message\":\"$(alertMessage)\", \"action\":\"$(alertAction)\"}}";
    const pnSilentTemplate = token.os == "ios" ? "{\"aps\":{\"content-available\":1, \"apns-priority\": 5, \"sound\":\"\", \"badge\": 0}, \"message\": \"$(silentMessage)\", \"action\": \"$(silentAction)\"}": "{\"data\":{\"message\":\"$(silentMessage)\", \"action\":\"$(silentAction)\", \"silent\":\"true\"}}"
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
        pushChannel: token.token,
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
  }

  onNotification(notification) {
    console.log('!!! App.onNotification');
    Alert.alert(notification.title, notification.message);
  }

  handlePerm(permissions) {
    console.log('!!! App.handlePerm');
    Alert.alert('Permissions', JSON.stringify(perms));
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

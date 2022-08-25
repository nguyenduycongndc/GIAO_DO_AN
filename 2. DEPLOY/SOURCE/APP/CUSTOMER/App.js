/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow
 */

import React, { Component } from "react";
import { Provider } from "react-redux";
import { StatusBar } from "react-native";
import store from "@app/redux/store";
import codePush from "react-native-code-push";
import AppContainer from "@screen/auth/AppContainer";
import { colors } from "@theme";
import reactotron from "reactotron-react-native";

import * as Sentry from "@sentry/react-native";

Sentry.init({
  dsn: "http://da8ad126b20c403b8d177f3ffd0e5584@18.141.210.230:9000/9",
  release: "carrect-customer@1.1.0",
  debug: __DEV__,
  enableAutoSessionTracking: true,
  // Sessions close after app is 10 seconds in the background.
  sessionTrackingIntervalMillis: 10000,
  environment: "prod",
  ignoreErrors: [
    // Random plugins/extensions
    "top.GLOBALS",
    // See: http://blog.errorception.com/2012/03/tale-of-unfindable-js-error.html
    "originalCreateNotification",
    "canvas.contentDocument",
    "MyApp_RemoveAllHighlights",
    "http://tt.epicplay.com",
    "Can't find variable: ZiteReader",
    "jigsaw is not defined",
    "ComboSearch is not defined",
    "http://loading.retry.widdit.com/",
    "atomicFindClose",
    // Facebook borked
    "fb_xd_fragment",
    // ISP "optimizing" proxy - `Cache-Control: no-transform` seems to
    // reduce this. (thanks @acdha)
    // See http://stackoverflow.com/questions/4113268
    "bmi_SafeAddOnload",
    "EBCallBackMessageReceived",
    // See http://toolbar.conduit.com/Developer/HtmlAndGadget/Methods/JSInjection.aspx
    "conduitPage"
  ],
  denyUrls: [
    // Facebook flakiness
    /graph\.facebook\.com/i,
    // Facebook blocked
    /connect\.facebook\.net\/en_US\/all\.js/i,
    // Woopra flakiness
    /eatdifferent\.com\.woopra-ns\.com/i,
    /static\.woopra\.com\/js\/woopra\.js/i,
    // Chrome extensions
    /extensions\//i,
    /^chrome:\/\//i,
    // Other plugins
    /127\.0\.0\.1:4001\/isrunning/i, // Cacaoweb
    /webappstoolbarba\.texthelp\.com\//i,
    /metrics\.itunes\.apple\.com\.edgesuite\.net\//i
  ]
});

class App extends Component {
  render() {
    console.disableYellowBox = true;
    return (
      <Provider store={store}>
        <StatusBar backgroundColor={colors.primaryDark} />
        <AppContainer />
      </Provider>
    );
  }

  codePushStatusDidChange(status) {
    console.log("Codepush status : ", status);
  }

  codePushDownloadDidProgress(progress) {
    console.log(
      progress.receivedBytes + " of " + progress.totalBytes + " received."
    );
  }
}

let codePushOptions = {
  checkFrequency: codePush.CheckFrequency.MANUAL
  // installMode: codePush.InstallMode.IMMEDIATE
  // rollbackRetryOptions: {
  //   delayInHours: 12,
  //   maxRetryAttempts: 1,
  // },
};

//appcenter codepush release-react -a Apps-Windsoft/CAR_RECT_CUSTOMER_ANDROID -d prod
//appcenter codepush release-react -a Apps-Windsoft/CAR_RECT_CUSTOMER_IOS -d prod

MyApp = codePush(codePushOptions)(App);

export default MyApp;

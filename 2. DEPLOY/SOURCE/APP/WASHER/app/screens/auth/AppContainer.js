import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  SafeAreaView,
  StatusBar,
  AppState,
  Platform,
  PermissionsAndroid,
  Linking,
  Clipboard
} from "react-native";
import { connect } from "react-redux";
import AppNavigator from "../../navigation/AppNavigator";
import NavigationUtil from "../../navigation/NavigationUtil";
import Reactotron from "reactotron-react-native";
import OneSignal from "react-native-onesignal"; // Import package from node modules
import DropdownAlert from "react-native-dropdownalert";
import {
  locationChange,
  getListOrderUpcommingService,
  getNotification,
  getUserInfo
} from "@action/index";
import {
  SCREEN_ROUTER_AUTH,
  SCREEN_ROUTER_WASHER,
  ASYNC_STORAGE,
  ORDER_STATUS,
  TYPE_NOTIFICATION,
  RESULT_VN_PAY
} from "@app/constants/Constants";
import reactotron from "reactotron-react-native";
import { getNotificationData, dismissAllNotification } from "@app/utils/Notify";
import AsyncStorage from "@react-native-community/async-storage";
// import ConnectivityManager from "react-native-connectivity-status";
import GPSState from "react-native-gps-state";
import Geolocation from "react-native-geolocation-service";
import R from "@app/assets/R";
import { updateLocation, updateDeviceID } from "@api";
import { showMessages, showConfirm } from "@app/components/Alert";

class AppContainer extends Component {
  constructor(properties) {
    super(properties);
    OneSignal.init("ac4b681b-0885-400d-8fe4-ec83a8f67547");
    // 0 = None, 1 = InAppAlert, 2 = Notification
    OneSignal.addEventListener("received", this.onReceived);
    OneSignal.addEventListener("opened", this.onOpened);
    OneSignal.addEventListener("ids", this.onIds);
    OneSignal.inFocusDisplaying(2);
    OneSignal.configure();
  }

  componentWillUnmount() {
    OneSignal.removeEventListener("received");
    OneSignal.removeEventListener("opened");
    OneSignal.removeEventListener("ids");
    Linking.removeEventListener("url", this.handleOpenURL);
  }

  updateDeviceID = async () => {
    const token = await AsyncStorage.getItem("token");
    if (token)
      OneSignal.getPermissionSubscriptionState(res => {
        updateDeviceID(res.userId);
      });
  };

  componentDidMount() {
    // hanlde trang thai gps thay doi
    // Linking.getInitialURL().then(url => {
    //   this.navigate(url);
    // });
    Linking.addEventListener("url", this.handleOpenURL);
    this.handleDeepLinkingRequests();
    this.requestLocationPermission();
    GPSState.addListener(status => {
      // console.log(status, GPSState.AUTHORIZED);
      switch (status) {
        case GPSState.NOT_DETERMINED:
        case GPSState.RESTRICTED:
        case GPSState.DENIED:
          this.props.locationChange({
            gpsState: false,
            lati: null,
            longi: null
          });
          break;

        case GPSState.AUTHORIZED_ALWAYS:
          //TODO do something amazing with you app
          break;

        case GPSState.AUTHORIZED_WHENINUSE:
          //TODO do something amazing with you app
          break;
        default:
          break;
      }
    });

    this.updateDeviceID();
  }

  handleDeepLinkingRequests = () => {
    Linking.getInitialURL()
      .then(url => {
        if (url) this.navigate(url);
      })
      .catch(error => console.log(error));
  };

  handleOpenURL = event => {
    this.navigate(event.url);
  };

  navigate = url => {
    const isInScreenRecharge =
      NavigationUtil.getCurrentRoute(NavigationUtil.getNavigator().state.nav) ==
      SCREEN_ROUTER_WASHER.RECHARGE;
    try {
      const route = url.replace(/.*?:\/\//g, "");
      const id = route.match(/\/([^\/]+)\/?$/)[1];
      const routeName = route.split("/")[0];
      if (routeName == RESULT_VN_PAY.SUCCESS && isInScreenRecharge) {
        this.props.getUserInfo();
        NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ACCOUNT);
        showMessages(R.strings().notice, R.strings().recharge_success);
      } else if (routeName == RESULT_VN_PAY.FAILED && isInScreenRecharge) {
        NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ACCOUNT);
        showMessages(R.strings().notice, R.strings().recharge_failed);
      }
    } catch (error) {
      const routeName = url
        .substr(url.indexOf("//") + 2, url.length)
        .split("/")[0];
      if (routeName == RESULT_VN_PAY.SUCCESS && isInScreenRecharge) {
        this.props.getUserInfo();
        NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ACCOUNT);
        showMessages(R.strings().notice, R.strings().recharge_success);
      } else if (routeName == RESULT_VN_PAY.FAILED && isInScreenRecharge) {
        NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ACCOUNT);
        showMessages(R.strings().notice, R.strings().recharge_failed);
      }
    }
  };

  // checkLocationPermission = async () => {
  //   // kiem tra trang thai gps hien tai
  //   const locationServicesAvailable = await ConnectivityManager.areLocationServicesEnabled()
  //   if (!locationServicesAvailable) {
  //     this.props.locationChange({
  //       gpsState: status,
  //       lati: null,
  //       longi: null
  //     });
  //   } else {
  //     // Kiem tra quyen lay vi tri qua gps neu da bat
  //     const locationPermission = await ConnectivityManager.isLocationPermissionGranted()
  //     switch (locationPermission) {
  //       case "Location.Permission.Denied":
  //         alert(R.strings().gps_permissions_warning)
  //         break;
  //       case "Location.Permission.Granted.Always":
  //         // ...
  //         break;
  //       case "Location.Permission.Granted.WhenInUse":
  //         // ...
  //         break;
  //       default:
  //       // ...
  //     }
  //   }

  // }

  componentWillMount() {
    this.requestLocationPermission();
  }

  requestLocationPermission = async () => {
    try {
      let granted = true;
      if (Platform.OS != "ios") {
        granted = await PermissionsAndroid.request(
          PermissionsAndroid.PERMISSIONS.ACCESS_FINE_LOCATION,
          {
            title: "CarRect",
            message: R.strings().grandPermission
          }
        );
      }

      if (Platform.OS === "ios" ? true : PermissionsAndroid.RESULTS.GRANTED) {
        Geolocation.getCurrentPosition(
          position => {
            // console.log("getCurrentPosition", position);
            if (position.coords.latitude) {
              const payload = {
                gpsState: true,
                lati: position.coords.latitude,
                longi: position.coords.longitude
              };
              this._updateLocation(payload);
            }
          },
          error => {
            // See error code charts below.
            console.log(error.code, error.message);
          },
          { enableHighAccuracy: true, timeout: 15000, maximumAge: 10000 }
        );
        Geolocation.watchPosition(
          position => {
            // console.log("watchPosition", position);
            if (position.coords.latitude) {
              const payload = {
                gpsState: true,
                lati: position.coords.latitude,
                longi: position.coords.longitude
              };
              this._updateLocation(payload);
            }
          },
          error => {
            // See error code charts below.
            console.log(error.code, error.message);
          },
          { enableHighAccuracy: true, timeout: 15000, maximumAge: 10000 }
        );
      } else {
        console.log("location permission denied");
      }
    } catch (err) {
      console.warn(err);
    }
  };

  _updateLocation = async payload => {
    const token = await AsyncStorage.getItem("token");
    const { locationState } = this.props;
    if (
      !locationState.lastUpdate ||
      Math.round(Date.now() / 1000) - locationState.lastUpdate / 1000 >= 120
    ) {
      this.props.locationChange(payload);
      if (token) updateLocation(payload);
    }
  };

  onReceived = notification => {
    const dataNoti = notification.payload.additionalData;
    reactotron.log("dataNoti", dataNoti);
    this.props.getNotification();
    if (dataNoti.type == TYPE_NOTIFICATION.REVIEWS) {
      NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER_DETAIL, {
        orderID: dataNoti.orderServiceID
      });
    }
    if (dataNoti.type == TYPE_NOTIFICATION.TRANSACTION_WALLET) {
      this.props.getUserInfo();
    }
    if (dataNoti.type == ORDER_STATUS.REJECTED) {
      this.props.getListOrderUpcommingService({
        status: ORDER_STATUS.CONFIRMED,
        page: 1
      });
    }
    const checkTimeCountDown =
      dataNoti.timeWait -
        parseInt(new Date().getTime() / 1000) +
        dataNoti.timeSend >
      0;
    if (
      AppState.currentState != "inactive" &&
      !NavigationUtil.isNavigatorNull() &&
      dataNoti.type == ORDER_STATUS.CONFIRMED &&
      checkTimeCountDown
    ) {
      if (Platform.OS != "ios") {
        getNotificationData(async data => {
          if (data)
            await AsyncStorage.setItem(
              ASYNC_STORAGE.NOTIFY_DATA,
              JSON.stringify(JSON.parse(data).a)
            );
          else
            await AsyncStorage.setItem(
              ASYNC_STORAGE.NOTIFY_DATA,
              JSON.stringify(dataNoti)
            );
        });
      }
      if (__DEV__) alert(JSON.stringify(notification));
      NavigationUtil.navigate(
        SCREEN_ROUTER_WASHER.ORDER_INCOMMING,
        {
          data: dataNoti
        },
        `orderID ${dataNoti.orderServiceID}`
      );
    }
  };

  onOpened = openResult => {
    const dataNoti = openResult.notification.payload.additionalData;
    if (dataNoti.type == TYPE_NOTIFICATION.REVIEWS) {
      NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER_DETAIL, {
        orderID: dataNoti.orderServiceID
      });
    }
    if (
      AppState.currentState != "active" &&
      Platform.OS == "ios" &&
      dataNoti.type == TYPE_NOTIFICATION.BOOKING
    )
      NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER_INCOMMING, {
        data: dataNoti
      });
    if (dataNoti.type == TYPE_NOTIFICATION.VN_PAY_SUCCESS) {
      this.props.getUserInfo();
      if (
        NavigationUtil.getCurrentRoute(
          NavigationUtil.getNavigator().state.nav
        ) == SCREEN_ROUTER_WASHER.RECHARGE
      ) {
        showMessages(
          R.strings().notice,
          R.strings().recharge_success,
          NavigationUtil.goBack
        );
      }
    }

    if (dataNoti.type == TYPE_NOTIFICATION.VN_PAY_FAILED) {
      if (
        NavigationUtil.getCurrentRoute(
          NavigationUtil.getNavigator().state.nav
        ) == SCREEN_ROUTER_WASHER.RECHARGE
      ) {
        showMessages(
          R.strings().notice,
          R.strings().recharge_failed,
          NavigationUtil.goBack
        );
      } else showMessages(R.strings().notice, R.strings().recharge_failed);
    }
  };

  onIds(device) {
    AsyncStorage.getItem("token", (err, token) => {
      if (token && device.userId != null && device.userId != "undefine")
        updateDeviceID(device.userId);
    });
  }

  render() {
    return (
      <>
        <AppNavigator
          ref={navigatorRef =>
            NavigationUtil.setTopLevelNavigator(navigatorRef)
          }
        />
      </>
    );
  }
}

const mapStateToProps = state => ({
  userState: state.userReducer,
  homeState: state.homeReducer,
  locationState: state.locationReducer
});

const mapDispatchToProps = {
  locationChange,
  getListOrderUpcommingService,
  getNotification,
  getUserInfo
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AppContainer);

const styles = StyleSheet.create({});

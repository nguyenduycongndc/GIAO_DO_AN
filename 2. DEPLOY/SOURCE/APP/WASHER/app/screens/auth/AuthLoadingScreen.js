import R from "@app/assets/R";
import {
  SCREEN_ROUTER_WASHER,
  SCREEN_ROUTER_AUTH,
  ASYNC_STORAGE,
  SCREEN_ROUTER_INTRO
} from "@app/constants/Constants";
import i18 from "@i18";
import AsyncStorage from "@react-native-community/async-storage";
import React, { Component } from "react";
import { Image, ImageBackground, Platform, Text, View } from "react-native";
import NavigationUtil from "../../navigation/NavigationUtil";
import {
  getNotificationData,
  dismissAllNotification,
  clearNotificationData
} from "@app/utils/Notify";
import Loading from "@app/components/Loading";
import codePush from "react-native-code-push";
import reactotron from "reactotron-react-native";
import { connect } from "react-redux";
import { getUserInfo } from "@app/redux/actions";
import * as Progress from "react-native-progress";
import theme from "@app/constants/Theme";

class AuthLoadingScreen extends Component {
  state = {
    update: false,
    progress: {
      receivedBytes: 0,
      totalBytes: 1
    },
    isNeedUpdate: false
  };
  async componentDidMount() {
    if (__DEV__) this.bootstrapAsync();
    else
      setTimeout(() => {
        // this._checkUpdate();
        // codePush.getUpdateMetadata().then(update => {
        //   if (update && update.isFirstRun) this._checkUpdate();
        //   else this.bootstrapAsync();
        // });
        this._checkUpdate();
      }, 200);
  }

  async bootstrapAsync() {
    // Cập nhật ngôn ngữ
    let lang = "vi";
    try {
      lang = await AsyncStorage.getItem("lang");
      if (lang == null) {
        await AsyncStorage.setItem("lang", "vi");
        lang = "vi";
      }
    } catch (error) {}
    i18.locale = lang;
    // const intro = await AsyncStorage.getItem(ASYNC_STORAGE.INTRO);
    // if (!intro) {
    //   await AsyncStorage.setItem(ASYNC_STORAGE.INTRO, "true");
    //   setTimeout(() => {
    //     NavigationUtil.navigate(SCREEN_ROUTER_INTRO.STEP1);
    //   }, 200);
    // } else
    setTimeout(() => {
      // NavigationUtil.navigate(SCREEN_ROUTER_AUTH.LOGIN);
      if (!this.state.update) {
        this.handleNavigator();
        if (Platform.OS != "ios") {
          this.checkNotifyData();
        }
      }
    }, 200);
  }

  async handleNavigator() {
    const token = await AsyncStorage.getItem("token");
    if (token) {
      NavigationUtil.navigate(SCREEN_ROUTER_WASHER.MAIN);
      // this.props.getUserInfo();
    } else NavigationUtil.navigate(SCREEN_ROUTER_AUTH.LOGIN);
  }

  checkNotifyData = () => {
    getNotificationData(async data => {
      var dataNoti = null;
      const res = await AsyncStorage.getItem(ASYNC_STORAGE.NOTIFY_DATA);
      if (data) dataNoti = JSON.parse(data).a;
      else if (res) dataNoti = JSON.parse(res);
      if (
        dataNoti &&
        dataNoti.timeWait -
          parseInt(new Date().getTime() / 1000) +
          dataNoti.timeSend >
          0
      ) {
        NavigationUtil.navigate(
          SCREEN_ROUTER_WASHER.ORDER_INCOMMING,
          {
            data: dataNoti
          },
          `orderID ${dataNoti.orderServiceID}`
        );
      } else {
        dismissAllNotification();
        clearNotificationData();
        await AsyncStorage.setItem(ASYNC_STORAGE.NOTIFY_DATA, "");
      }
    });
  };

  async _checkUpdate() {
    this.setState(
      {
        ...this.state,
        update: true
      },
      async () => {
        codePush
          .checkForUpdate()
          .then(update => {
            this.setState({
              ...this.state,
              update: false
            });
            if (!update) {
              this.bootstrapAsync();
            } else {
              // codePush.notifyAppReady();
              codePush.sync(
                {
                  updateDialog: null,
                  installMode: codePush.InstallMode.IMMEDIATE
                },
                status => {
                  // reactotron.log(status);
                  if (
                    status == codePush.SyncStatus.DOWNLOADING_PACKAGE ||
                    status == codePush.SyncStatus.CHECKING_FOR_UPDATE ||
                    status == codePush.SyncStatus.SYNC_IN_PROGRESS ||
                    status == codePush.SyncStatus.INSTALLING_UPDATE
                  ) {
                    this.setState({
                      ...this.state,
                      update: true
                    });
                  } else {
                    this.setState({
                      ...this.state,
                      update: false
                    });
                  }
                  if (status == codePush.SyncStatus.UPDATE_INSTALLED) {
                    codePush.allowRestart();
                  }
                },
                progress => {
                  this.setState({
                    progress: progress,
                    isNeedUpdate: true,
                    update: false
                  });
                  // reactotron.log(progress);
                }
              );
            }
          })
          .catch(err => {
            console.log("error", error);
            codePush.allowRestart();
          });
      }
    );
    codePush.notifyAppReady();
  }

  render() {
    const { progress, isNeedUpdate, update } = this.state;
    return (
      <ImageBackground
        source={R.images.img_splash}
        resizeMode="cover"
        style={{
          width: width,
          height: height,
          alignItems: "center"
        }}
      >
        {isNeedUpdate ? (
          <View
            style={{
              position: "absolute",
              top: height * 0.5,
              alignSelf: "center"
            }}
          >
            <Progress.Bar
              progress={progress.receivedBytes / progress.totalBytes}
              height={height * 0.018}
              width={width * 0.8}
              color={theme.colors.primary}
              style={{
                borderWidth: 1.5,
                borderColor: theme.colors.borderTopColor,
                backgroundColor: theme.colors.borderTopColor,
                borderRadius: 10
              }}
            />
            <Text
              style={{
                textAlign: "center",
                fontFamily: R.fonts.quicksand_semi_bold,
                fontSize: 14,
                marginVertical: 10,
                color: theme.colors.primary
              }}
              children={`${R.strings().sycning_data} ${Math.round(
                (progress.receivedBytes / progress.totalBytes) * 100
              )}%`}
            />
          </View>
        ) : (
          <Loading />
        )}
      </ImageBackground>
    );
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {
  getUserInfo
};
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AuthLoadingScreen);

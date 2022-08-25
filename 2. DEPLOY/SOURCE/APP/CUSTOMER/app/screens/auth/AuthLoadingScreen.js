import R from "@app/assets/R";
import { SCREEN_ROUTER_CUSTOMER, SCREEN_ROUTER_INTRO } from "@constant";
import i18 from "@i18";
import AsyncStorage from "@react-native-community/async-storage";
import React, { Component } from "react";
import {
  DeviceEventEmitter,
  ImageBackground,
  Text,
  View,
  Platform
} from "react-native";
import NavigationUtil from "../../navigation/NavigationUtil";
import { ASYNC_STORAGE } from "@constant";
import Loading from "@app/components/Loading";
import codePush from "react-native-code-push";
import { changeLanguage } from "@action_language";
import { connect } from "react-redux";
import * as Progress from "react-native-progress";

import reactotron from "reactotron-react-native";
import { colors } from "@app/constants/Theme";
import { showMessages, showMessagesAlert } from "@app/components/Alert";

class AuthLoadingScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      update: false,
      progress: {
        receivedBytes: 0,
        totalBytes: 1
      },
      isNeedUpdate: false
    };
  }

  forceNav = true;

  async componentDidMount() {
    if (__DEV__) this.bootstrapAsync();
    else
      setTimeout(() => {
        this._checkUpdate();
      }, 200);
    // codePush.clearUpdates();

    setTimeout(() => {
      if (this.forceNav) {
        this.bootstrapAsync();
      }
    }, 6000);
  }

  async bootstrapAsync() {
    if (Platform.OS != "ios") {
      DeviceEventEmitter.addListener("SEND_URI", async url => {
        const SUCCESS = "success",
          FAILED = "failed";
        setTimeout(() => {
          try {
            const route = url.replace(/.*?:\/\//g, "");
            const state = route.split("/")[0];
            var id = route.split("/")[1];
            if (id == parseInt(id)) {
              if (state == SUCCESS)
                NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.SEARCH_WASHER, {
                  orderServiceID: id
                });
              else
                showMessages(
                  R.strings().notif_tab_cus,
                  R.strings().pay_failed,
                  () => this.props.getUserInfoAction()
                );
            } else {
              if (state == SUCCESS)
                NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.SEARCH_WASHER, {
                  comboID: id
                });
              else
                showMessages(
                  R.strings().notif_tab_cus,
                  R.strings().pay_failed,
                  () => this.props.getUserInfoAction()
                );
            }
          } catch (error) {
            console.log(error);
          }
          DeviceEventEmitter.removeAllListeners("SEND_URI");
        }, 1000);
      });
    }
    let lang = "vi";

    try {
      lang = await AsyncStorage.getItem(ASYNC_STORAGE.LANG);
      if (lang == null) {
        this.props.changeLanguage({ locale: (lang = "vi"), isIntro: true });
        await AsyncStorage.setItem(ASYNC_STORAGE.LANG, "vi");
        lang = "vi";
      }
    } catch (error) {
      reactotron.log("error", error);
    }
    i18.locale = lang;
    const intro = await AsyncStorage.getItem(ASYNC_STORAGE.INTRO);
    if (!intro) {
      setTimeout(() => {
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.INTRO);
      }, 1000);
      return;
    }

    // setTimeout(async () => {
    const isLogin = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
    if (isLogin) NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.BOOKING);
    else NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.HOME);
    // }, 1000);
  }
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
            this.forceNav = false;
            this.setState({
              ...this.state,
              update: false
            });
            if (!update) {
              this.forceNav = false;
              this.bootstrapAsync();
            } else {
              codePush.notifyAppReady();
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
                    progress,
                    isNeedUpdate: true,
                    update: false
                  });
                }
              );
            }
          })
          .catch(err => {
            this.forceNav = false;
            console.log(err.toString());
            codePush.allowRestart();
            this.bootstrapAsync();
          });
      }
    );
    codePush.notifyAppReady();
  }

  render() {
    const { progress, isNeedUpdate, update } = this.state;

    return (
      <ImageBackground
        style={{ width: width, height: height }}
        resizeMode="cover"
        source={R.images.splash}
      >
        {isNeedUpdate && (
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
              color={colors.primary}
              style={{
                borderWidth: 1.5,
                borderColor: colors.white,
                backgroundColor: colors.grayDivide,
                borderRadius: 10
              }}
            />
            <Text
              style={{
                textAlign: "center",
                fontFamily: R.fonts.quicksand_semi_bold,
                fontSize: 14,
                marginVertical: 10,
                color: colors.primary
              }}
              children={`${R.strings().sycning_data} ${Math.round(
                (progress.receivedBytes / progress.totalBytes) * 100
              )}%`}
            />
          </View>
        )}
        {!isNeedUpdate && <Loading />}
      </ImageBackground>
    );
  }
}
const mapStateToProps = state => ({});

const mapDispatchToProps = {
  changeLanguage
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AuthLoadingScreen);

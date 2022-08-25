import React, { Component } from "react";
import {
  TextInput,
  Text,
  Image,
  StyleSheet,
  TouchableOpacity,
  View,
  Alert,
  Platform,
  StatusBar
} from "react-native";
import AsyncStorage from "@react-native-community/async-storage";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import {
  SCREEN_ROUTER_AUTH,
  SCREEN_ROUTER_WASHER,
  TYPE_ROLE,
  RELEASE_CHANNEL,
  CHANNEL
} from "@app/constants/Constants";
import Card from "@app/components/Card";
import { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import reactotron from "reactotron-react-native";
import { ASYNC_STORAGE, REG_PHONE } from "@app/constants/Constants";
import CallApiHelper from "@app/utils/CallApiHelper";
import * as API from "@app/constants/Api";
import OneSignal from "react-native-onesignal";
import FastImage from "react-native-fast-image";
import ProgressFastImage from "@app/components/ProgressFastImage";
import { showMessages } from "@app/components/Alert";
class LoginScreen extends Component {
  static navigationOptions = {
    header: null
  };
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    this.state = {
      dialogLoading: false,
      payload: {
        Phone: navigation.getParam("Phone"),
        PassWord: "",
        deviceID: "",
        role: TYPE_ROLE.WASHER
      }
    };
  }

  async setToken(token) {
    await AsyncStorage.setItem(ASYNC_STORAGE.TOKEN, token);
    NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH_LOADING);
  }
  componentDidMount() {
    // this.setDeviceID();
  }

  // setDeviceID = async () => {

  //   OneSignal.getPermissionSubscriptionState(status => {
  //     console.log(status.userId);
  //     this.setState({
  //       ...this.state,
  //       payload: {
  //         ...this.state.payload,
  //         deviceID: status.userId
  //       }
  //     });
  //     // reactotron.log(status.userId)
  //     alert(JSON.stringify(status))
  //   });
  // }

  requestLogin = async () => {
    OneSignal.getPermissionSubscriptionState(res => {
      this.setState(
        {
          ...this.state,
          dialogLoading: true,
          payload: {
            ...this.state.payload,
            deviceID: res.userId ? res.userId : "unknow"
          }
        },
        () => {
          CallApiHelper(
            API.requestLogin,
            this.state.payload,
            this,
            async res => {
              if (res.result.role === TYPE_ROLE.WASHER) {
                await AsyncStorage.setItem(
                  ASYNC_STORAGE.TOKEN,
                  res.result.token
                );
                NavigationUtil.navigate(SCREEN_ROUTER_WASHER.MAIN);
              } else {
                this.setState({ ...this.state, dialogLoading: false }, () =>
                  showMessages(R.strings().notice, R.strings().login_fail)
                );
              }
            },
            error => {
              console.log("error", error);
              this.setState({
                ...this.state,
                dialogLoading: false
              });
            }
          );
        }
      );
    });
  };

  // login = () => NavigationUtil.navigate(SCREEN_ROUTER_AUTH.OTP);
  register = () => NavigationUtil.navigate(SCREEN_ROUTER_AUTH.PHONE);
  onChangeTextPassword = password =>
    this.setState({ payload: { ...this.state.payload, PassWord: password } });
  onChangeTextPhoneNumber = phone_number =>
    this.setState({ payload: { ...this.state.payload, Phone: phone_number } });
  render() {
    const { PassWord, Phone } = this.state.payload;
    return (
      <ScreenComponent
        dialogLoading={this.state.dialogLoading}
        isSafeArea={false}
        renderView={
          <>
            {/* <Image
              source={R.images.bg_car_rect}
              resizeMode="center"
            /> */}
            <ProgressFastImage
              uri={R.images.bg_car_rect}
              resizeMode={FastImage.resizeMode.cover}
              style={styles.img_bg}
            />
            <Card style={styles.root_view}>
              <Text style={styles.text_login}>Đăng nhập</Text>
              <TextInput
                value={Phone}
                clearButtonMode="always"
                keyboardType="phone-pad"
                onChangeText={this.onChangeTextPhoneNumber}
                style={styles.textinput_phone_number}
                placeholder={R.strings().phone_number}
              />
              <TextInput
                secureTextEntry
                clearButtonMode="always"
                onChangeText={this.onChangeTextPassword}
                value={PassWord}
                style={styles.textinput_phone_number}
                placeholder={R.strings().password}
              />
              <View
                style={{
                  flexDirection: "row",
                  justifyContent: "space-between"
                }}
              >
                <TouchableOpacity
                  onPress={() =>
                    NavigationUtil.navigate(SCREEN_ROUTER_AUTH.REGISTER)
                  }
                  disabled={RELEASE_CHANNEL == CHANNEL.FAKE_APPLE}
                  children={
                    <Text style={[styles.text_forget_password]}>
                      {RELEASE_CHANNEL == CHANNEL.FAKE_APPLE
                        ? R.strings().register
                        : ""}
                    </Text>
                  }
                />
                <TouchableOpacity
                  onPress={() =>
                    NavigationUtil.navigate(SCREEN_ROUTER_AUTH.FORGOT_PASS)
                  }
                  // style={{ alignItems: "flex-end" }}
                  children={
                    <Text style={styles.text_forget_password}>
                      {R.strings().forget_password}
                    </Text>
                  }
                />
              </View>
              <TouchableOpacity
                onPress={() => {
                  if (!REG_PHONE.test(Phone)) {
                    showMessages(R.strings().notice, R.strings().phone_invalid);
                  } else this.requestLogin();
                }}
                disabled={!Phone || !PassWord}
                children={
                  <Text style={styles.login_button}>{R.strings().login}</Text>
                }
              />
            </Card>
            {/* <View
              style={styles.root_register}
              children={
                <>
                  <Text
                    style={styles.text_register}
                    children={R.strings().dont_have_an_account}
                  />
                  <TouchableOpacity
                    onPress={this.register}
                    children={
                      <Text
                        style={styles.register}
                        children={R.strings().register}
                      />
                    }
                  />
                </>
              }
            /> */}
          </>
        }
      />
    );
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(LoginScreen);

const styles = StyleSheet.create({
  root_register: {
    alignSelf: "center",
    flexDirection: "row",
    marginTop: 25
  },
  text_register: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 12,
    color: colors.gray
  },
  register: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 12,
    color: colors.orange_text,
    textDecorationLine: "underline"
  },
  text_forget_password: {
    color: colors.primary,
    textAlign: "right",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12,
    borderColor: colors.primary,
    textDecorationLine: "underline",
    marginVertical: 10
  },
  textinput_phone_number: {
    padding: 15,
    backgroundColor: colors.text_input,
    borderRadius: 5,
    // width: "95%",
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    marginVertical: 5,
    color: "black"
  },
  login_button: {
    padding: 15,
    backgroundColor: colors.primary,
    borderRadius: 5,
    color: colors.white,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 13,
    textAlign: "center",
    overflow: "hidden",
    marginVertical: 10
  },
  text_login: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: colors.primary,
    textAlign: "center",
    marginVertical: 20
  },
  root_view: {
    justifyContent: "center",
    borderRadius: 10,
    shadowOffset: {
      height: 10,
      width: 10
    },
    shadowRadius: 10,
    marginTop: width / 2.5
  },
  img_bg: {
    position: "absolute",
    width: width,
    height: width / 1.55
  }
});

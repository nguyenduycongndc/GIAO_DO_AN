import React, { Component } from "react";
import {
  TextInput,
  Text,
  Image,
  StyleSheet,
  TouchableOpacity,
  View
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { SCREEN_ROUTER_AUTH, SCREEN_ROUTER_CUSTOMER } from "@constant";
import Card from "@app/components/Card";
import { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { CloseButton } from "@app/components/RNHeader";
import CallApiHelper from "../../utils/CallApiHelper";
import * as API from "@api";
import AsyncStorage from "@react-native-community/async-storage";
import { ASYNC_STORAGE } from "@constant";
import { requestLogin } from "@action/";
import OneSignal from "react-native-onesignal";
import reactotron from "@app/reactotron/ReactotronConfig";
import FastImage from "@app/components/FastImage";

export class LoginScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const newsID = navigation.getParam("newsID", {});
    this.state = {
      isLoading: false,
      error: "",
      getUserInfo: "",
      payload: {
        Phone: "",
        PassWord: "",
        deviceID: ""
      }
    };
  }

  componentDidMount() {
    this.setDeviceID();
  }

  setDeviceID() {
    OneSignal.getPermissionSubscriptionState(status => {
      this.setState({
        payload: {
          ...this.state.payload,
          deviceID: status.userId
        }
      });
      // alert(status.userId)
    });
  }

  // login = () => NavigationUtil.navigate(SCREEN_ROUTER_AUTH.OTP);
  // register = () => NavigationUtil.navigate(SCREEN_ROUTER_AUTH.REGISTER);
  onChangeTextPhoneNumber = phone =>
    this.setState({
      payload: {
        ...this.state.payload,
        Phone: phone
      }
    });
  onChangeTextPassword = password =>
    this.setState({
      payload: {
        ...this.state.payload,
        PassWord: password
      }
    });
  render() {
    const { loginState } = this.props;
    const { PassWord, Phone, isLoading } = this.state.payload;
    const valid = PassWord.trim() != "" && Phone.trim() != "";
    return (
      <ScreenComponent
        dialogLoading={isLoading || loginState.isLoading}
        isSafeAre={false}
        renderView={
          <>
            <FastImage
              source={R.images.bg_car_rect}
              resizeMode="contain"
              style={styles.img_bg}
            />
            <CloseButton style={styles.back_button} />
            <Card style={styles.root_view}>
              <Text style={styles.text_login}>{R.strings().login}</Text>
              <TextInput
                keyboardType={"number-pad"}
                value={Phone}
                maxLength={256}
                clearButtonMode="always"
                onChangeText={this.onChangeTextPhoneNumber}
                style={styles.textinput_phone_number}
                placeholder={R.strings().number_phone}
                placeholderTextColor={colors.gray}
              />

              <TextInput
                secureTextEntry
                maxLength={256}
                clearButtonMode="always"
                onChangeText={this.onChangeTextPassword}
                value={PassWord}
                style={styles.textinput_phone_number}
                placeholderTextColor={colors.gray}
                placeholder={R.strings().password}
              />
              <TouchableOpacity
                onPress={() =>
                  NavigationUtil.navigate(SCREEN_ROUTER_AUTH.FORGOT_PASS)
                }
                children={
                  <Text style={styles.text_forget_password}>
                    {R.strings().forget_password}
                  </Text>
                }
              />
              <TouchableOpacity
                disabled={!valid}
                onPress={() => this.props.requestLogin(this.state.payload)}
                children={
                  <Text
                    style={[
                      styles.login_button,
                      {
                        backgroundColor: valid ? colors.primary : colors.gray
                      }
                    ]}
                  >
                    {R.strings().login}
                  </Text>
                }
              />
            </Card>
            <View
              style={styles.root_register}
              children={
                <>
                  <Text
                    style={styles.text_register}
                    children={R.strings().dont_have_an_account}
                  />
                  <TouchableOpacity
                    onPress={() =>
                      NavigationUtil.navigate(SCREEN_ROUTER_AUTH.REGISTER)
                    }
                    children={
                      <Text
                        style={styles.register}
                        children={R.strings().register}
                      />
                    }
                  />
                </>
              }
            />
          </>
        }
      />
    );
  }
}

const mapStateToProps = state => ({
  loginState: state.userReducer
});

const mapDispatchToProps = {
  requestLogin
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(LoginScreen);

const styles = StyleSheet.create({
  root_register: {
    alignSelf: "center",
    flexDirection: "row",
    alignItems: "center",
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
    marginLeft: 5,
    color: colors.signInButton,
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
    backgroundColor: colors.backgroundInput,
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
    textTransform: "uppercase",
    marginVertical: 10
  },
  text_login: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: colors.primary,
    textAlign: "center",
    marginVertical: 40
  },
  root_view: {
    justifyContent: "center",
    borderRadius: 10,
    shadowOffset: {
      height: 10,
      width: 10
    },
    shadowRadius: 10,
    marginTop: width / 2.5,
    paddingBottom: 37
  },
  img_bg: {
    position: "absolute",
    width: width,
    height: width / 1.5179
  },
  back_button: {
    position: "absolute",
    top: width / 8,
    right: 5,
    width: 50,
    height: 50,
    alignItems: "center",
    justifyContent: "flex-end"
  }
});

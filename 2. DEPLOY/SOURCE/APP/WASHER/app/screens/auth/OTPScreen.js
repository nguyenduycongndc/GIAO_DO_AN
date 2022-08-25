import React, { Component } from "react";
import {
  View,
  Text,
  Image,
  TouchableOpacity,
  StyleSheet,
  TextInput,
  Platform
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import Card from "@app/components/Card";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import { BackButton } from "@app/components/RNHeader";
import reactotron from "reactotron-react-native";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_AUTH, ASYNC_STORAGE, TYPE_GET_OTP } from "@constant";
import * as API from "@api";
import CallApiHelper from "../../utils/CallApiHelper";
import AsyncStorage from "@react-native-community/async-storage";
import { getUserInfoAction } from "@action";
import { connect } from "react-redux";
import { showMessages, showConfirm } from "@app/components/Alert";
import ProgressFastImage from "@app/components/ProgressFastImage";
import FastImage from "react-native-fast-image";

export class OTPScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const Phone = navigation.getParam("Phone", {});
    const type = navigation.getParam("type", {});
    this.state = {
      isLoading: false,
      error: "",
      type: type,
      phone: Phone,
      hasKeySupport: Platform.OS == "ios",
      otp: ["", "", "", "", "", ""]
    };
  }

  refOtp = [];
  setToken = async token => {
    var otpStr = "";
    for (let i = 0; i < this.state.otp.length; i++) {
      otpStr = otpStr + this.state.otp[i];
    }
    NavigationUtil.navigate(SCREEN_ROUTER_AUTH.UPDATE_PASSWORD, {
      token: token,
      phone: this.state.phone,
      type: this.state.type,
      otp: otpStr
    });
  };

  getOTPByPhone() {
    var payload = {
      phone: this.state.phone,
      type: this.state.type
    };
    CallApiHelper(API.getOTPByPhone, payload, this, res => {
      this.setState(
        {
          otp: ["", "", "", "", "", ""]
        },
        () =>
          showMessages(
            R.strings().notif_tab_cus,
            R.strings().OTP_code_will_be_sent_to_the_phone_number +
              payload.phone
          )
      );
    });
  }

  requestConfirmOTP() {
    var otpStr = "";
    for (let i = 0; i < this.state.otp.length; i++) {
      otpStr = otpStr + this.state.otp[i];
    }
    let payload = {
      Phone: this.state.phone,
      OTP: otpStr
    };
    CallApiHelper(API.requestConfirmOTP, payload, this, res => {
      this.setToken(res.result);
    });
  }

  onKeyReallyPressed = (key, index) => {
    // do something
    var otp = this.state.otp;
    if (key != "Backspace") otp[index] = key;
    else otp[index] = "";
    this.setState({ otp });
    if (this.state.otp[index].length == 0 && key == "Backspace" && index > 0)
      this.refOtp[index - 1].focus();
    if (this.state.otp[index].length > 0 && key != "Backspace" && index < 5)
      this.refOtp[index + 1].focus();
    if (this.state.otp[index].length > 0 && key != "Backspace" && index == 5)
      this.refOtp[index].blur();
  };

  onChangeText = (text, index) => {
    if (Platform.OS === "ios") return;
    if (!this.state.hasKeySupport) this.onKeyReallyPressed(text, index);
  };

  onKeyPressInput = (key, index) => {
    this.onKeyReallyPressed(key, index);
    if (Platform.OS === "ios") return;
    if (!this.state.hasKeySupport && !isNaN(key))
      this.setState({ hasKeySupport: true });
  };

  renderOtpInput = (value, index) => (
    <TextInput
      style={[
        styles.otp_input,
        { borderBottomWidth: this.state.otp[index] ? 0 : 1 }
      ]}
      keyboardType={Platform.select({ ios: "number-pad", android: "numeric" })}
      onChangeText={text => this.onChangeText(text, index)}
      onKeyPress={e => this.onKeyPressInput(e.nativeEvent.key, index)}
      ref={ref => (this.refOtp[index] = ref)}
      value={value}
      maxLength={1}
    />
  );
  render() {
    const { otp } = this.state;
    const valid = otp.filter(elem => elem.length > 0).length == 6;
    return (
      <ScreenComponent
        isSafeArea={false}
        renderView={
          <>
            <ProgressFastImage
              uri={R.images.bg_car_rect}
              resizeMode={FastImage.resizeMode.cover}
              style={styles.img_bg}
            />
            <Text
              children={R.strings().please_enter_the_OTP_code_you_receive}
              style={styles.text_otp}
            />
            <BackButton style={styles.back_button} />
            <View
              style={styles.root_otp_input}
              children={otp.map((value, index) =>
                this.renderOtpInput(value, index)
              )}
            />
            <TouchableOpacity
              disabled={!valid}
              onPress={() => this.requestConfirmOTP()}
              children={
                <Text
                  style={[
                    styles.confirm_button,
                    { backgroundColor: valid ? colors.primary : colors.gray }
                  ]}
                  children={R.strings().continue}
                />
              }
            />
            <TouchableOpacity
              onPress={() => this.getOTPByPhone()}
              style={styles.root_re_send}
              children={
                <>
                  <Image source={R.images.ic_resend} style={styles.ic_resend} />
                  <Text
                    style={styles.re_send_otp}
                    children={R.strings().resend_otp_code}
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

const styles = StyleSheet.create({
  ic_resend: { width: 20, height: 20 },
  re_send_otp: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 12,
    color: colors.gray,
    textAlign: "center",
    alignSelf: "center",
    marginHorizontal: 5
  },
  root_re_send: {
    flexDirection: "row",
    marginTop: 20,
    alignSelf: "center"
  },
  root_otp_input: {
    flexDirection: "row",
    alignSelf: "center",
    marginTop: 12
  },
  confirm_button: {
    color: colors.white,
    padding: 10,
    width: "80%",
    alignSelf: "center",
    borderRadius: 5,
    overflow: "hidden",
    textAlign: "center",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 13,
    marginTop: 50
  },
  back_button: {
    position: "absolute",
    top: 30,
    width: 50,
    height: 50,
    alignItems: "center",
    justifyContent: "center"
  },
  img_bg: {
    position: "absolute",
    width: width,
    height: width / 1.5179
  },
  text_otp: {
    marginTop: width / 1.55 + 12,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginHorizontal: 25,
    textAlign: "center"
  },
  otp_input: {
    width: width / 16,
    marginHorizontal: 10,
    textAlign: "center",
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 30,
    color: "black"
  }
});

const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfoAction
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(OTPScreen);

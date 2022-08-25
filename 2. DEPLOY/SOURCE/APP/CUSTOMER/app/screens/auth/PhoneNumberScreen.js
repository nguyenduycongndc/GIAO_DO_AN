import React, { Component } from "react";
import {
  View,
  Text,
  Image,
  TouchableOpacity,
  StyleSheet,
  TextInput
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import { BackButton } from "@app/components/RNHeader";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_AUTH } from "@constant";
import FastImage from "@app/components/FastImage";
class OTPScreen extends Component {
  initState = {
    phone_number: ""
  };
  refOtp = [];
  state = this.initState;
  onConfirm = () => NavigationUtil.navigate(SCREEN_ROUTER_AUTH.OTP);
  onChangeText = phone_number => this.setState({ phone_number });
  render() {
    const { phone_number } = this.state;
    const valid = phone_number.length > 0;
    return (
      <ScreenComponent
        isSafeArea={false}
        renderView={
          <>
            <FastImage
              source={R.images.bg_car_rect}
              resizeMode="center"
              style={styles.img_bg}
            />
            <BackButton style={styles.back_button} />
            <Text
              children="Hệ thống sẽ gửi mã OTP tới số điện thoại bạn đã đăng ký.  Vui lòng nhập mã OTP bạn nhận được!"
              style={styles.text_otp}
            />
            <TextInput
              value={phone_number}
              clearButtonMode="always"
              maxLength={15}
              onChangeText={this.onChangeText}
              style={styles.textinput_phone_number}
              placeholder={R.strings().phone_number}
              placeholderTextColor={colors.gray}
            />
            <TouchableOpacity
              onPress={this.onConfirm}
              disabled={!valid}
              children={
                <Text
                  style={[
                    styles.confirm_button,
                    { backgroundColor: !valid ? colors.gray : colors.primary }
                  ]}
                  children="TIẾP TỤC"
                />
              }
            />
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
)(OTPScreen);
const styles = StyleSheet.create({
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
    top: width / 8,
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
    marginTop: width / 1.55,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginHorizontal: 25,
    textAlign: "center"
  },
  textinput_phone_number: {
    padding: 15,
    backgroundColor: colors.text_input,
    borderRadius: 5,
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    marginVertical: 5,
    marginHorizontal: 20,
    marginTop: 90,
    color: "black"
  }
});

import React, { Component } from "react";
import {
  View,
  Text,
  Image,
  TouchableOpacity,
  StyleSheet,
  TextInput,
  Platform,
  ScrollView
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import Card from "@app/components/Card";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import { BackButton } from "@app/components/RNHeader";
import reactotron from "reactotron-react-native";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  SCREEN_ROUTER_AUTH,
  ASYNC_STORAGE,
  TYPE_GET_OTP,
  REG_PHONE,
  REG_EMAIL
} from "@constant";
import * as API from "@api";
import CallApiHelper from "../../utils/CallApiHelper";
import AsyncStorage from "@react-native-community/async-storage";
import { getUserInfoAction } from "@action";
import { connect } from "react-redux";
import { showMessages, showConfirm } from "@app/components/Alert";
import TextInputInfo from "@app/components/TextInputInfo";
import ButtonPrimary from "@app/components/ButtonPrimary";
import ProgressFastImage from "@app/components/ProgressFastImage";
import FastImage from "react-native-fast-image";

const HEIGHT_IMAGE_BACKGROUND = width / 1.5179;

class RegisterScreen extends Component {
  constructor(props) {
    super(props);

    this.state = {
      Phone: "",
      Password: "",
      Email: "",
      Name: "",
      ConfirmPassword: "",
      isLoading: false
    };
  }

  handleChangeText = key => value => {
    this.setState({ [key]: value });
  };

  validateRegister() {
    const { Phone, Password, Email, Name, ConfirmPassword } = this.state;
    const isRegister = true;

    if (Password != ConfirmPassword) {
      isRegister = false;
      showMessages(R.strings().notice, R.strings().password_not_match);
      return isRegister;
    }

    if (!REG_PHONE.test(Phone)) {
      isRegister = false;
      showMessages(R.strings().notice, R.strings().phone_invalid);
      return isRegister;
    }

    if (!REG_EMAIL.test(Email)) {
      isRegister = false;
      showMessages(R.strings().notice, R.strings().email_invalid);
      return isRegister;
    }

    return isRegister;
  }

  onRegister = async () => {
    if (!this.validateRegister()) return;
    this.setState({ isLoading: true }, async () => {
      try {
        const res = await API.registerWasher(this.state);
        showMessages(
          R.strings().notice,
          "Đăng ký thành công. Vui lòng đăng nhập để tiếp tục sử dụng ứng dụng",
          NavigationUtil.goBack
        );
      } catch (error) {
        console.log("error", error);
        this.setState({ isLoading: false });
      }
    });
  };
  render() {
    const {
      Phone,
      Password,
      Email,
      Name,
      ConfirmPassword,
      isLoading
    } = this.state;
    let isDisableButton =
      !Phone || !Password || !Email || !Name || !ConfirmPassword;
    return (
      <ScreenComponent
        isSafeArea={false}
        dialogLoading={isLoading}
        renderView={
          <>
            {/* <Image
              source={R.images.bg_car_rect}
              resizeMode="contain"
              style={styles.img_bg}
            /> */}
            <ProgressFastImage
              uri={R.images.bg_car_rect}
              resizeMode={FastImage.resizeMode.cover}
              style={styles.img_bg}
            />
            <BackButton style={styles.back_button} />
            <ScrollView
              style={styles.cardContainer}
              showsVerticalScrollIndicator={false}
            >
              <TextInputInfo
                label={R.strings().phone_number}
                value={Phone}
                require
                keyboardType="phone-pad"
                onChangeText={this.handleChangeText("Phone")}
              />
              <TextInputInfo
                label={R.strings().full_name}
                value={Name}
                require
                onChangeText={this.handleChangeText("Name")}
              />
              <TextInputInfo
                label={R.strings().email}
                value={Email}
                require
                onChangeText={this.handleChangeText("Email")}
              />
              <TextInputInfo
                label={R.strings().password}
                value={Password}
                require
                secureTextEntry={true}
                onChangeText={this.handleChangeText("Password")}
              />

              <TextInputInfo
                label={R.strings().confirm_password}
                value={ConfirmPassword}
                require
                secureTextEntry={true}
                onChangeText={this.handleChangeText("ConfirmPassword")}
              />

              <ButtonPrimary
                text={R.strings().register}
                disabled={isDisableButton}
                style={{
                  marginTop: 20,
                  marginBottom: 10,
                  backgroundColor: isDisableButton
                    ? colors.gray
                    : colors.primary
                }}
                onPress={this.onRegister}
              />
            </ScrollView>
          </>
        }
      />
    );
  }
}

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
  container: {},
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
  cardContainer: {
    marginTop: HEIGHT_IMAGE_BACKGROUND / 1.5,
    marginHorizontal: 8,
    borderWidth: 0.2,
    borderRadius: 5,
    backgroundColor: "white",
    shadowOffset: { width: 0, height: 5 },
    shadowColor: "#8B8B8B",
    shadowOpacity: 0.2,
    shadowRadius: 6,
    elevation: Platform.OS == "android" ? 3 : 0,
    borderColor: "white",
    paddingHorizontal: 14,
    marginBottom: 10
  }
});

export default RegisterScreen;

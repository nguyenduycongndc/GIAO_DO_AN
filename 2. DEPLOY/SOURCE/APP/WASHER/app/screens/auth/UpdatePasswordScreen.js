import R from "@app/assets/R";
import Button from "@app/components/Button";
import ScreenComponent from "@app/components/ScreenComponent";
import theme from "@app/constants/Theme";
import React, { Component } from "react";
import {
  StyleSheet,
  Text,
  TextInput,
  View,
  ScrollView,
  Dimensions,
  KeyboardAvoidingView,
  Keyboard,
  Image,
  Platform
} from "react-native";
import Dropdown from "@app/components/ModalDropdown";
import DatePicker from "react-native-datepicker";
// import DropdownMenu from 'react-native-dropdown-menu';
import { showMessages, showConfirm } from "@app/components/Alert";
// import DropdownMenu from 'react-native-dropdown-menu';
import * as API from "@api";
import CallApiHelper from "../../utils/CallApiHelper";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  SCREEN_ROUTER_AUTH,
  ASYNC_STORAGE,
  SCREEN_ROUTER_CUSTOMER,
  TYPE_GET_OTP,
  SCREEN_ROUTER_WASHER,
  TYPE_ROLE
} from "@constant";
import AsyncStorage from "@react-native-community/async-storage";
import { getUserInfoAction } from "@action";
import { connect } from "react-redux";
import TextInputInfo from "@app/components/TextInputInfo";
import TextInfo from "@app/components/TextInfo";
import reactotron from "reactotron-react-native";
import { BackButton } from "@app/components/RNHeader";
import Loading from "@app/components/Loading";
import ButtonPrimary from "@app/components/ButtonPrimary";
import OneSignal from "react-native-onesignal";
import ProgressFastImage from "@app/components/ProgressFastImage";
import FastImage from "react-native-fast-image";
var d = new Date();
const HEIGHT_IMAGE_BACKGROUND = width / 1.5179;

var day = ("0" + d.getDate()).slice(-2);
var month = ("0" + (d.getMonth() + 1)).slice(-2);
var year = d.getFullYear();

var dateStr = day + "/" + month + "/" + year;

var data = [
  {
    value: R.strings().man
  },
  {
    value: R.strings().woman
  }
];

var mailFormat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

class UpdatePasswordScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    this.state = {
      payload: {
        Phone: navigation.getParam("phone"),
        pass: "",
        confirm_pass: "",
        deviceID: "",
        OTP: navigation.getParam("otp")
      },
      dialogLoading: false
    };
  }

  handleChangeText = (key, subKey) => value => {
    let obj = null;
    if (subKey) {
      obj = this.state[key];
      obj[subKey] = value;
    }
    this.setState({ [key]: obj || value });
  };

  onResetPassword = async () => {
    const { Phone, pass, confirm_pass } = this.state.payload;
    if (pass != confirm_pass) {
      showMessages(R.strings().notice, R.strings().password_not_match);
      return;
    }
    OneSignal.getPermissionSubscriptionState(res => {
      this.setState({ dialogLoading: true }, async () => {
        try {
          const vi = await AsyncStorage.getItem(ASYNC_STORAGE.LANG);
          const params = {
            method: "POST",
            headers: {
              Accept: "application/json",
              "Content-Type": "application/json",
              token: this.props.navigation.getParam("token"),
              lang: vi
            },
            body: JSON.stringify({
              pass: this.state.payload.pass,
              OTP: this.state.payload.OTP,
              deviceID: res.userId || "unknow"
            })
          };
          const response = await fetch(
            "http://118.27.192.110/api/Service/UpdatePassword",
            params
          );
          const json = await response.json();
          if (json.status == 1) {
            if (json.result.role === TYPE_ROLE.WASHER) {
              await AsyncStorage.setItem(
                ASYNC_STORAGE.TOKEN,
                json.result.token
              );
              NavigationUtil.navigate(SCREEN_ROUTER_WASHER.MAIN);
            } else {
              this.setState({ ...this.state, dialogLoading: false }, () =>
                showMessages(R.strings().notice, R.strings().login_fail)
              );
            }
          } else {
            this.setState({ dialogLoading: false }, () => {
              showMessages(R.strings().notice, json.message);
            });
          }
        } catch (error) {
          console.log("error", error);
          this.setState({ dialogLoading: false });
        }
      });
    });
  };

  render() {
    const { dialogLoading } = this.state;
    const { Phone, pass, confirm_pass } = this.state.payload;
    let disableButton = !confirm_pass || !pass;
    return (
      <ScreenComponent
        isSafeArea={false}
        dialogLoading={dialogLoading}
        renderView={
          <>
            <ProgressFastImage
              uri={R.images.bg_car_rect}
              resizeMode={FastImage.resizeMode.cover}
              style={styles.img_bg}
            />
            <BackButton style={styles.back_button} />

            <KeyboardAvoidingView
              style={{ flex: 1 }}
              enable
              behavior={Platform.OS == "ios" ? "padding" : undefined}
              keyboardVerticalOffset={80}
            >
              <ScrollView
                style={{
                  flex: 1,
                  marginTop: HEIGHT_IMAGE_BACKGROUND / 1.3,
                  backgroundColor: "white"
                }}
              >
                <TextInputInfo
                  label={R.strings().phone_number}
                  value={Phone}
                  editable={false}
                  keyboardType="phone-pad"
                  onChangeText={this.handleChangeText("payload", "Phone")}
                />
                <TextInputInfo
                  label={R.strings().new_password}
                  value={pass}
                  secureTextEntry={true}
                  onChangeText={this.handleChangeText("payload", "pass")}
                />
                <TextInputInfo
                  label={R.strings().confirm_password}
                  value={confirm_pass}
                  secureTextEntry={true}
                  onChangeText={this.handleChangeText(
                    "payload",
                    "confirm_pass"
                  )}
                />
                <ButtonPrimary
                  text={R.strings().login}
                  disabled={disableButton}
                  style={{
                    marginTop: 20,
                    marginBottom: 10,
                    backgroundColor: disableButton
                      ? theme.colors.gray
                      : theme.colors.primary
                  }}
                  onPress={this.onResetPassword}
                />
              </ScrollView>
            </KeyboardAvoidingView>
          </>
        }
      />
    );
  }

  _checkPassword() {
    if (!this.state.payload.passWord) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().please_select_full_info
      );
    } else if (this.state.confirm_password != this.state.payload.passWord) {
      showMessages(R.strings().notif_tab_cus, R.strings().password_not_match);
    } else return this.updateUserInfo();
  }

  checkValidate() {
    const { payload } = this.state;
    if (!payload.name.trim() || !payload.email.trim()) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().please_select_full_info
      );
    } else if (!payload.email.match(mailFormat)) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().you_have_entered_an_invalid_email_address
      );
    } else return this._checkPassword();
  }
}

const styles = StyleSheet.create({
  confirm_button: {
    color: theme.colors.white,
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

const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfoAction
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(UpdatePasswordScreen);

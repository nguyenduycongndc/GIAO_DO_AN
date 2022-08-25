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
import { SCREEN_ROUTER_AUTH, TYPE_GET_OTP } from "@constant";
import Card from "@app/components/Card";
import { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { CheckBox } from "react-native-elements";
import Icon from "@component/Icon";
import * as API from "@api";
import CallApiHelper from "../../utils/CallApiHelper";
import OneSignal from "react-native-onesignal";
import { checkPhoneNumber } from "../../utils/Validation";
import { showMessages, showConfirm } from "@app/components/Alert";
import { CloseButton, BackButton } from "@app/components/RNHeader";
import FastImage from "@app/components/FastImage";
import analytics from '@react-native-firebase/analytics';

export class RegisterScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      checked: false,
      isLoading: false,
      error: "",
      payload: {
        Phone: "",
        deviceID: ""
      }
    };
  }

  componentDidMount() {
    this.setDeviceID();
  }

  setToken = async token => {
    await AsyncStorage.setItem(ASYNC_STORAGE.TOKEN, token.toString());
    NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.MAIN);
  };

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

  requestRegister() {
    CallApiHelper(API.requestRegister, this.state.payload, this, async res => {
      analytics().logEvent('sign_up_get_otp', this.state.payload)
      NavigationUtil.navigate(SCREEN_ROUTER_AUTH.OTP, {
        Phone: res.result,
        type: TYPE_GET_OTP.REGISTER
      });
    });
  }

  onChangeTextPhoneNumber = Phone =>
    this.setState({
      payload: {
        ...this.state.payload,
        Phone
      }
    });

  CheckBox = () => this.setState({ checked: !this.state.checked });

  render() {
    const { Phone } = this.state.payload;
    const valid = Phone.trim() != "";
    return (
      <ScreenComponent
        dialogLoading={this.state.isLoading}
        isSafeAre={false}
        renderView={
          <>
            <FastImage
              source={R.images.bg_car_rect}
              resizeMode="contain"
              style={styles.img_bg}
            />
            <BackButton style={styles.back_button} />
            <Card style={styles.root_view}>
              <Text style={styles.text_login}>{R.strings().register}</Text>
              <TextInput
                value={Phone}
                clearButtonMode="always"
                maxLength={15}
                keyboardType={"number-pad"}
                onChangeText={this.onChangeTextPhoneNumber}
                style={styles.textinput_phone_number}
                placeholderTextColor={colors.gray}
                placeholder={R.strings().number_phone}
              />
              <TouchableOpacity
                disabled={!valid}
                onPress={() => {
                  this._checkNumberPhone();
                }}
                children={
                  <Text
                    style={[
                      styles.login_button,
                      {
                        backgroundColor: valid ? colors.primary : colors.gray
                      }
                    ]}
                  >
                    {R.strings().register}
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
                    children={R.strings().already_have_an_account}
                  />
                  <TouchableOpacity
                    onPress={() => {
                      NavigationUtil.navigate(SCREEN_ROUTER_AUTH.LOGIN);
                    }}
                    children={
                      <Text
                        style={styles.register}
                        children={R.strings().login}
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
  _checkNumberPhone() {
    if (!checkPhoneNumber(this.state.payload.Phone)) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().you_have_entered_an_invalid_phone_number_address
      );
    } else this.requestRegister();
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(RegisterScreen);

const styles = StyleSheet.create({
  back_button: {
    position: "absolute",
    top: width / 15,
    width: 50,
    height: 50,
    alignItems: "center",
    justifyContent: "flex-end"
  },
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
    paddingBottom: 51,
    shadowRadius: 10,
    marginTop: width / 2.5
  },
  img_bg: {
    position: "absolute",
    width: width,
    height: width / 1.5179
  }
});

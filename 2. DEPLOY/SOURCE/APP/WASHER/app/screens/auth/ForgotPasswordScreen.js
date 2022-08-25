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
  Image
} from "react-native";
import { TYPE_GET_OTP, SCREEN_ROUTER_AUTH, REG_PHONE } from "@constant";
import * as API from "@api";
import CallApiHelper from "../../utils/CallApiHelper";
import AsyncStorage from "@react-native-community/async-storage";
import OneSignal from "react-native-onesignal";
import reactotron from "reactotron-react-native";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { showMessages } from "@app/components/Alert";
import { BackButton } from "@app/components/RNHeader";
import ProgressFastImage from "@app/components/ProgressFastImage";
import FastImage from "react-native-fast-image";

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

export default class ForgotPasswordScreen extends Component {
  static navigationOptions = {
    header: null
  };
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const item = navigation.getParam("item", {});
    this.state = {
      isLoading: false,
      isLoadingToken: true,
      dialogLoading: false,
      error: "",
      payload: {
        phone: "",
        type: TYPE_GET_OTP.FORGOT_PASS
      }
    };
  }

  getOTPByPhone() {
    const { payload } = this.state;
    if (!payload.phone) {
      showMessages(
        R.strings().notice,
        R.strings().please_enter_your_phone_number
      );
      return;
    }
    if (!REG_PHONE.test(payload.phone)) {
      showMessages(R.strings().notice, R.strings().phone_invalid);
      return;
    }
    this.setState({ dialogLoading: true }, () => {
      CallApiHelper(
        API.getOTPByPhone,
        this.state.payload,
        this,
        res => {
          this.setState({ dialogLoading: false }, () => {
            NavigationUtil.navigate(SCREEN_ROUTER_AUTH.OTP, {
              Phone: this.state.payload.phone,
              type: TYPE_GET_OTP.FORGOT_PASS
            });
          });
        },
        error => {
          this.setState({ dialogLoading: false });
        }
      );
    });
  }

  render() {
    const { dialogLoading } = this.state;
    return (
      <ScreenComponent
        back
        isSafeArea={false}
        dialogLoading={dialogLoading}
        // titleHeader={R.strings().forget_password}
        renderView={
          <>
            <ScrollView
              showsVerticalScrollIndicator={true}
              style={{
                backgroundColor: theme.colors.white,
                flex: 1
              }}
            >
              <ProgressFastImage
                uri={R.images.bg_car_rect}
                resizeMode={FastImage.resizeMode.cover}
                style={styles.img_bg}
              />
              <BackButton style={styles.back_button} />
              <View style={{ paddingHorizontal: 15, alignItems: "center" }}>
                <Text
                  style={{
                    paddingVertical: 10,
                    fontFamily: R.fonts.quicksand_semi_bold,
                    fontSize: 14,
                    textAlign: "center",
                    marginBottom: 10
                  }}
                >
                  {R.strings().please_enter_your_phone_number}
                </Text>
                <View style={styles.text_input}>
                  <TextInput
                    style={{ width: "100%", color: "black" }}
                    placeholder={R.strings().number_phone}
                    keyboardType="phone-pad"
                    maxLength={12}
                    onChangeText={text => {
                      this.setState({
                        payload: {
                          ...this.state.payload,
                          phone: text
                        }
                      });
                    }}
                  />
                </View>
                <Button
                  uppercase
                  title={R.strings().confirm}
                  backgroundColor={theme.colors.primary}
                  colorText={theme.colors.white}
                  width={"70%"}
                  buttonStyle={{ marginTop: 20, height: 46 }}
                  action={() => this.getOTPByPhone()}
                />
              </View>
            </ScrollView>
          </>
        }
      />
    );
  }
  _renderTextInput(label, value) {
    return (
      <View>
        <Text
          style={{
            fontFamily: R.fonts.quicksand_medium,
            fontSize: 14,
            paddingVertical: 10,
            marginRight: 5
          }}
        >
          {label}
        </Text>
        <View style={styles.text_input}>
          <TextInput value={value} style={{ width: "80%", color: "black" }} />
        </View>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  text_label: {
    marginHorizontal: 10,
    marginTop: 15,
    marginBottom: 5,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  text_input: {
    height: 46,
    backgroundColor: "#F3F3F3",
    borderRadius: 5,
    paddingHorizontal: 10,
    marginBottom: 15,
    flexDirection: "row",
    alignItems: "center",
    flex: 1
  },
  InputDate: {
    width: "100%"
  },
  img_bg: {
    width: width,
    height: height / 3,
    resizeMode: "cover"
  },
  back_button: {
    position: "absolute",
    top: 30,
    width: 50,
    height: 50,
    alignItems: "center",
    justifyContent: "center"
  }
});

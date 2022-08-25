import R from "@app/assets/R";
import Button from "@app/components/Button";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@app/constants/Theme";
import React, { Component } from "react";
import {
  StyleSheet,
  Text,
  TextInput,
  View,
  ScrollView,
  Dimensions
} from "react-native";
import { TYPE_GET_OTP, SCREEN_ROUTER_AUTH } from "@constant";
import * as API from "@api";
import CallApiHelper from "../../utils/CallApiHelper";
import AsyncStorage from "@react-native-community/async-storage";
import OneSignal from "react-native-onesignal";
import reactotron from "reactotron-react-native";
import NavigationUtil from "@app/navigation/NavigationUtil";

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

export default class ForgotPasswordScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const item = navigation.getParam("item", {});
    this.state = {
      isLoading: false,
      isLoadingToken: true,
      error: "",
      payload: {
        phone: "",
        type: TYPE_GET_OTP.FORGOT_PASS
      }
    };
  }

  getOTPByPhone = () => {
    CallApiHelper(
      API.getOTPByPhone,
      this.state.payload,
      this,
      res => {
        NavigationUtil.navigate(SCREEN_ROUTER_AUTH.OTP, {
          Phone: this.state.payload.phone,
          type: TYPE_GET_OTP.FORGOT_PASS
        });
      },
      error => reactotron.log(error, "check error")
    );
  };

  render() {
    return (
      <ScreenComponent
        back
        titleHeader={R.strings().forget_password}
        renderView={
          <>
            <ScrollView
              showsVerticalScrollIndicator={true}
              style={{
                backgroundColor: theme.colors.white,
                flex: 1,
                paddingHorizontal: 15,
                paddingTop: 15
              }}
            >
              <Text
                style={{
                  paddingVertical: 40,
                  textAlign: "center",
                  paddingHorizontal: 15,
                  fontFamily: R.quicksand_medium,
                  fontSize: 14
                }}
              >
                {R.strings().please_enter_your_phone_number}
              </Text>
              <View style={styles.text_input}>
                <TextInput
                  maxLength={256}
                  style={{ width: "80%", color: "black" }}
                  placeholder={R.strings().number_phone}
                  placeholderTextColor={colors.gray}
                  autoFocus={true}
                  keyboardType="number-pad"
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
                width={"100%"}
                buttonStyle={{ marginTop: 20 }}
                action={() => this.getOTPByPhone()}
              />
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
          <TextInput
            value={value}
            style={{ width: "80%", color: "black" }}
            maxLength={256}
          />
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
    backgroundColor: theme.colors.backgroundInput,
    borderRadius: 5,
    paddingHorizontal: 10,
    marginBottom: 15,
    flexDirection: "row",
    alignItems: "center",
    flex: 1
  },
  InputDate: {
    width: "100%"
  }
});

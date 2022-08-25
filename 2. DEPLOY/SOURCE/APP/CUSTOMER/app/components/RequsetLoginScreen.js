import React, { Component } from "react";
import { Text, View, TouchableOpacity, Image } from "react-native";
import { SCREEN_ROUTER_AUTH } from "@constant";
import NavigationUtil from "../navigation/NavigationUtil";
import theme from "@theme";
import R from "@app/assets/R";
import FastImage from "./FastImage";

export default class RequsetLoginScreen extends Component {
  render() {
    return (
      <View
        style={{
          flex: 1,
          justifyContent: "center",
          alignItems: "center"
        }}
      >
        <FastImage
          style={{
            width: 80,
            height: 80
            // tintColor:Theme.colors.yellow
          }}
          source={require("../assets/images/locked.png")}
        />

        <Text
          style={{
            color: "#000",
            opacity: 0.7,
            paddingVertical: 15,
            marginTop: 5,
            textAlign: "center"
          }}
        >
          {R.strings().please_login_to_continue}
        </Text>

        <TouchableOpacity
          style={{
            borderWidth: 1,
            borderColor: theme.colors.active,
            padding: 10,
            width: width * 0.8,
            alignSelf: "center"
          }}
          onPress={() => {
            NavigationUtil.navigate("Login");
          }}
        >
          <Text
            style={{
              color: theme.colors.active,
              textAlign: "center",
              fontSize: 16
            }}
          >
            ĐĂNG NHẬP/ĐĂNG KÍ TÀI KHOẢN
          </Text>
        </TouchableOpacity>
      </View>
    );
  }
}

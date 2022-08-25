import React, { Component } from "react";
import { View } from "react-native";
import { BarIndicator } from "react-native-indicators";
import theme from "@app/constants/Theme";
import { StyleSheet } from "react-native";
import R from "@app/assets/R";
export default class Loading extends Component {
  render() {
    return (
      <View style={{ flex: 1, justifyContent: "center", alignItems: "center" }}>
        <BarIndicator color={theme.colors.primary} />
      </View>
    );
  }
}

const styles = StyleSheet.create({
  textLoading: {
    fontFamily: R.fonts.roboto_regular,
    fontSize: 13
  }
});

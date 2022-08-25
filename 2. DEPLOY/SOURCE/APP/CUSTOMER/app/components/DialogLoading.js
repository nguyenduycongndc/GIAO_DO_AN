import React from "react";
import { StyleSheet, Text, View } from "react-native";
import { BarIndicator } from "react-native-indicators";
import R from "@app/assets/R";
import theme from "@app/constants/Theme";

const DialogLoading = () => {
  return (
    <View
      style={{
        position: "absolute",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        justifyContent: "center",
        alignItems: "center",
        backgroundColor: "rgba(0, 0, 0, 0.6)",
        elevation: Platform.OS == "android" ? 4 : 0
      }}
    >
      <View
        style={{
          height: 140,
          backgroundColor: "white",
          padding: 30,
          borderRadius: 10
        }}
      >
        <BarIndicator color={theme.colors.indicator} />
        <Text
          style={{
            color: theme.colors.indicator
          }}
        >
          {R.strings().loading}
        </Text>
      </View>
    </View>
  );
};

export default DialogLoading;

const styles = StyleSheet.create({});

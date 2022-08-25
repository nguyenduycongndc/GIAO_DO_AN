import React from "react";
import { StyleSheet, Text, Dimensions, TouchableOpacity } from "react-native";
import { colors } from "@app/constants/Theme";
const { width, height } = Dimensions.get("screen");
import R from "@app/assets/R";
interface Props {
  action?;
}
export default (props: Props) => {
  return (
    <TouchableOpacity
      style={{
        backgroundColor: colors.primary,
        width: width,
        height: height / 15
      }}
      onPress={props.action}
      children={
        <Text
          style={{
            fontFamily: R.fonts.quicksand_bold,
            fontSize: 16,
            color: colors.white,
            paddingVertical: 10,
            textAlign: "center"
          }}
          children={R.strings().order_service}
        />
      }
    />
  );
};

const styles = StyleSheet.create({});

import React, { Component } from "react";
import { Text, StyleSheet, View, TouchableOpacity, Image } from "react-native";
import R from "@app/assets/R";
import theme from "@theme";
import Icon from "./Icon";
import I18n from "@i18";
import C from "@app/constants/Constants";

export default class Button extends Component {
  render() {
    const {
      title,
      borderColor,
      backgroundColor,
      colorText,
      back,
      forward,
      width = 170,
      action,
      borderWidth = 0.25,
      icon,
      imageSource,
      styleImg,
      uppercase,
      buttonStyle,
      disabled,
      onLayout,
      ...props
    } = this.props;
    return (
      <TouchableOpacity
        onLayout={onLayout}
        disabled={disabled}
        onPress={action}
        style={[
          styles.button,
          {
            borderWidth: borderColor ? 0.25 : 0,
            borderColor: borderColor ? borderColor : "",
            backgroundColor: backgroundColor,
            flexDirection: "row",
            alignItems: "center",
            justifyContent: "space-around",
            width: width,
            paddingHorizontal: "5%",
            paddingVertical: 7
          },
          buttonStyle
        ]}
        {...this.props}
      >
        {icon}
        {back && (
          <Icon.Ionicons
            name="ios-arrow-back"
            size={15}
            color={theme.colors.white}
          />
        )}
        <View style={{ flexDirection: "row", alignItems: "center" }}>
          {imageSource && <Image source={imageSource} style={styleImg} />}
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              textTransform: uppercase ? "uppercase" : null,
              color: colorText ? colorText : ""
            }}
          >
            {title}
          </Text>
        </View>
        {forward && (
          <Icon.Ionicons
            name="ios-arrow-forward"
            size={15}
            color={theme.colors.white}
          />
        )}
      </TouchableOpacity>
    );
  }
}

const styles = StyleSheet.create({
  button: {
    borderRadius: 5,
    marginVertical: 13
  }
});

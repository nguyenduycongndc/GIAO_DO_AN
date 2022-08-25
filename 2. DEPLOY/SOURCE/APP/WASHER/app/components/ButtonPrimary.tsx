import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  StyleProp,
  TextStyle,
  TouchableOpacity,
  ViewStyle
} from "react-native";
import { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
interface Props {
  text: string;
  onPress: () => void;
  style?: StyleProp<TextStyle>;
  styleButton?: StyleProp<ViewStyle>;
}
export default class ButtonPrimary extends Component<Props> {
  render() {
    const { onPress, text, style, styleButton, ...props } = this.props;
    return (
      <TouchableOpacity
        {...this.props}
        onPress={onPress}
        style={styleButton}
        children={
          <Text
            style={[styles.button_withdrawal, style]}
            children={text.toUpperCase()}
          />
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  button_withdrawal: {
    backgroundColor: colors.primary,
    color: colors.white,
    borderColor: colors.primary,
    borderRadius: 5,
    marginHorizontal: 10,
    textAlign: "center",
    paddingVertical: 15,
    marginTop: 5,
    overflow: "hidden",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 13
  }
});

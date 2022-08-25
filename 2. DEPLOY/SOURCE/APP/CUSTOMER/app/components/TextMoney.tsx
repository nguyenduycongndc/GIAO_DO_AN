import React, { Component } from "react";
import { Text, StyleSheet, View, TextStyle, StyleProp } from "react-native";
import NumberFormat from "react-number-format";
import { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
interface Props {
  typePay: string;
  value: number;
  style: StyleProp<TextStyle>;
}
export default class TextMoney extends Component<Props> {
  render() {
    const { typePay = "", value, style } = this.props;
    return (
      <View
        children={
          <>
            <NumberFormat
              value={value}
              displayType="text"
              thousandSeparator
              suffix="đ"
              renderText={value => (
                <Text
                  style={[styles.number_account_balance, style]}
                  children={`${value}${typePay}`}
                />
              )}
            />
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  number_account_balance: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  }
});

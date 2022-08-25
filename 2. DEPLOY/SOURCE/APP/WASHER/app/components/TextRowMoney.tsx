import React, { Component } from "react";
import { Text, StyleSheet, View } from "react-native";
import NumberFormat from "react-number-format";
import { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
interface Props {
  label: string;
  value: number;
}
export default class TextRowMoney extends Component<Props> {
  render() {
    const { label, value } = this.props;
    return (
      <View
        style={styles.root_account_balance}
        children={
          <>
            <Text style={styles.text_account_balance} children={label} />
            <NumberFormat
              value={value}
              displayType="text"
              thousandSeparator
              suffix="đ"
              renderText={value => (
                <Text style={styles.number_account_balance} children={value} />
              )}
            />
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  root_account_balance: {
    flexDirection: "row",
    backgroundColor: colors.white,
    padding: 15,
    borderWidth: 0.5,
    borderColor: colors.grayBorder,
    marginTop: 10
  },
  text_account_balance: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  number_account_balance: {
    textAlign: "right",
    flex: 1,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  }
});

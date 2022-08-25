import React, { Component } from "react";
import { Text, StyleSheet, View } from "react-native";
import NumberFormat from "react-number-format";
import { TextInput } from "react-native";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import reactotron from "@app/reactotron/ReactotronConfig";
interface Props {
  label: string;
  value: number;
  onChangeText: (text: string) => void;
}
export default class TextInputMoney extends Component<Props> {
  render() {
    const { label, value, onChangeText } = this.props;
    return (
      <>
        <Text children={label} style={styles.text_withdrawal} />
        <NumberFormat
          value={value || ""}
          displayType="text"
          thousandSeparator
          renderText={value => (
            <View style={styles.root_view}>
              <TextInput
                style={styles.number_withdrawal}
                value={value || ""}
                autoFocus
                onChangeText={onChangeText}
                keyboardType="number-pad"
                maxLength={15}
              />
              <Text style={styles.text_vnd} children=" đ" />
            </View>
          )}
        />
      </>
    );
  }
}

const styles = StyleSheet.create({
  root_view: {
    flexDirection: "row",
    backgroundColor: colors.white,
    paddingHorizontal: 10,
    marginHorizontal: 10,
    borderRadius: 5,
    borderWidth: 0.2,
    marginTop: 10
  },
  text_withdrawal: {
    marginHorizontal: 10,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginTop: 20
  },
  number_withdrawal: {
    textAlign: "left",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    width: "90%",
    paddingVertical: 15,
    color: "black"
  },
  text_vnd: {
    textAlign: "right",
    flex: 1,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    alignSelf: "center"
  }
});

import React, { Component } from "react";
import { Text, StyleSheet, View, Platform } from "react-native";
import { TextInput } from "react-native";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
interface Props {
  label: string;
  value: string;
  placeholder: string;
  onChangeText: (text: string) => void;
  endComponent?: JSX.Element;
  secureTextEntry?: boolean;
}
export default class TextInputInfo extends Component<Props> {
  render() {
    const {
      label,
      value,
      onChangeText,
      placeholder,
      endComponent,
      secureTextEntry
    } = this.props;
    return (
      <>
        <Text children={label} style={styles.text_withdrawal} />
        <View
          style={{
            flexDirection: endComponent ? "row" : "column"
          }}
          children={
            <>
              <TextInput
                style={[styles.text_value, { flex: endComponent && 1 }]}
                value={value}
                placeholder={placeholder}
                placeholderTextColor={colors.gray}
                onChangeText={onChangeText}
                maxLength={256}
                secureTextEntry={secureTextEntry}
              />
              {endComponent}
            </>
          }
        />
      </>
    );
  }
}

const styles = StyleSheet.create({
  text_withdrawal: {
    // marginHorizontal: 10,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginTop: 20
  },
  text_value: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    flexDirection: "row",
    backgroundColor: colors.backgroundInput,
    paddingVertical: Platform.OS == "android" ? 10 : 15,
    paddingHorizontal: 10,
    color: "black",
    borderRadius: 5,

    // borderWidth: 0.3,
    marginTop: 10
  },
  text_vnd: {
    textAlign: "right",
    flex: 1,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  }
});

import React, { Component } from "react";
import { Text, StyleSheet, View } from "react-native";
import NumberFormat from "react-number-format";
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
  editable?: boolean;
  require?: boolean;
}
export default class TextInputInfo extends Component<Props> {
  render() {
    const {
      label,
      value,
      onChangeText,
      placeholder,
      endComponent,
      secureTextEntry,
      editable = true,
      require = false
    } = this.props;
    return (
      <>
        <View style={{ flexDirection: "row" }}>
          <Text
            children={label}
            style={[styles.text_withdrawal, { marginLeft: 10, marginRight: 4 }]}
          />
          {require && (
            <Text style={[styles.text_withdrawal, { color: colors.red }]}>
              (*)
            </Text>
          )}
        </View>
        <View
          style={{
            flexDirection: endComponent ? "row" : "column"
          }}
          children={
            <>
              <TextInput
                style={[
                  styles.text_value,
                  {
                    flex: endComponent && 1,
                    backgroundColor: editable ? "white" : "#ECECEC",
                    color: "black"
                  }
                ]}
                value={value}
                placeholder={placeholder}
                onChangeText={onChangeText}
                maxLength={256}
                secureTextEntry={secureTextEntry}
                editable={editable}
                {...this.props}
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
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginTop: 10
  },
  text_value: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    flexDirection: "row",
    backgroundColor: colors.white,
    paddingVertical: 10,
    paddingHorizontal: 10,
    marginHorizontal: 10,
    borderRadius: 5,
    borderWidth: 0.5,
    marginTop: 10
  },
  text_vnd: {
    textAlign: "right",
    flex: 1,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  }
});

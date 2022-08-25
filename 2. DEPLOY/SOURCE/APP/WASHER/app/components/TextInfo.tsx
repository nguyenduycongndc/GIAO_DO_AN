import React, { Component } from "react";
import { Text, StyleSheet, View } from "react-native";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
interface Props {
  label: string;
  value: string;
  endComponent?: JSX.Element;
}
export default class TextInfo extends Component<Props> {
  render() {
    const { label, value, endComponent } = this.props;
    return (
      <>
        <Text style={styles.text_label} children={label} />
        <View
          style={{
            flexDirection: endComponent ? "row" : "column"
          }}
          children={
            <>
              <Text style={styles.text_input} children={value} />
              {endComponent}
            </>
          }
        />
      </>
    );
  }
}

const styles = StyleSheet.create({
  text_label: {
    marginHorizontal: 10,
    marginTop: 10,
    marginBottom: 5,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  text_input: {
    borderRadius: 5,
    marginTop: 5,
    paddingHorizontal: 10,
    paddingVertical: 15,
    backgroundColor: "#ECECEC",
    margin: 10,
    overflow: "hidden",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    flex: 1
  }
});

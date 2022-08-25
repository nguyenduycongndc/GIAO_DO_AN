import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Platform,
  StyleProp,
  ViewStyle
} from "react-native";
import { colors } from "@app/constants/Theme";
interface Props {
  style?: StyleProp<ViewStyle>;
}
export default class Card extends Component<Props> {
  render() {
    const { style, children, ...props } = this.props;
    return <View style={[styles.container, style]}>{children}</View>;
  }
}

const styles = StyleSheet.create({
  container: {
    marginHorizontal: 8,
    borderWidth: 0.2,
    borderRadius: 5,
    backgroundColor: "white",
    shadowOffset: { width: 0, height: 5 },
    shadowColor: colors.black,
    shadowOpacity: 0.4,
    shadowRadius: 6,
    elevation: Platform.OS == "android" ? 3 : 0,
    borderColor: "white",
    paddingHorizontal: 14
    // marginTop : 5
  }
});

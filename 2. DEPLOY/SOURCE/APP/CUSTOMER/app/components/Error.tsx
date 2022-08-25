import React, { Component } from "react";
import { View, Text, TouchableOpacity, Image, Dimensions } from "react-native";
import R from "@app/assets/R";
import { StyleSheet } from "react-native";
import theme from "@app/constants/Theme";
import FastImage from "@app/components/FastImage";

interface Props {
  reload: () => void;
}
const { width, height } = Dimensions.get("window");
export default class Error extends Component<Props> {
  render() {
    const { reload } = this.props;
    return (
      <View style={{ flex: 1, justifyContent: "center", alignItems: "center" }}>
        <FastImage source={R.images.image_error} style={styles.image} />
        <Text style={styles.description}>{R.strings().error_happend}</Text>
        <TouchableOpacity style={styles.button} onPress={reload}>
          <Text style={styles.textReload}>{R.strings().try_agian}</Text>
        </TouchableOpacity>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  image: {
    width: width / 2,
    height: width / 2,
    resizeMode: "contain"
  },
  button: {
    backgroundColor: theme.colors.primary,
    paddingHorizontal: "10%",
    paddingVertical: 12,
    borderRadius: 50
  },
  textReload: {
    fontFamily: R.fonts.roboto_medium,
    fontSize: 14,
    color: "white"
  },
  description: {
    fontFamily: R.fonts.roboto_medium,
    fontSize: 16,
    color: theme.colors.gray,
    marginTop: 8,
    marginBottom: "10%"
  }
});

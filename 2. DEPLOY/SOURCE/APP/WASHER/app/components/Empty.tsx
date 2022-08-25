import React, { Component } from "react";
import { Text, StyleSheet, View, Dimensions } from "react-native";
import FastImage, { FastImageSource } from "react-native-fast-image";
import theme from "@app/constants/Theme";
import R from "@app/assets/R";
import reactotron from "reactotron-react-native";
const { width, height } = Dimensions.get("window");
interface EmptyProps {
  header?: JSX.Element;
  sourceImage?: FastImageSource | number;
  description?: string;
  marginTop?: number;
}

export default class Empty extends Component<EmptyProps> {
  state = {
    marginTop: height / 5
  };
  render() {
    const { header, sourceImage, description, marginTop } = this.props;
    return (
      <View
        onLayout={event => {
          const result = header
            ? event.nativeEvent.layout.height / 2
            : event.nativeEvent.layout.height;
          this.setState({ marginTop: result });
        }}
        style={{
          marginTop: marginTop ? marginTop : this.state.marginTop,
          alignItems: "center",
          backgroundColor: theme.colors.lightGrey,
          justifyContent: "center"
        }}
      >
        <FastImage
          source={sourceImage || R.images.empty_procressing_order}
          style={styles.imageEmpty}
          resizeMode={FastImage.resizeMode.contain}
        />
        <Text style={styles.textEmpty}>
          {description || "Không có xe nào đang được rửa"}
        </Text>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  imageEmpty: {
    width: width / 2,
    height: width / 2
  },
  textEmpty: {
    fontFamily: R.fonts.roboto_medium,
    fontSize: 16,
    color: theme.colors.gray,
    marginTop: 10
  }
});

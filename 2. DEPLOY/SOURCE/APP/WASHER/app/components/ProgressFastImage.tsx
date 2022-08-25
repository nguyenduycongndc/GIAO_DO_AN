import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  GestureResponderEvent,
  TouchableOpacity,
  View,
  ViewStyle
} from "react-native";
import { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
import FastImage, { FastImageProps } from "react-native-fast-image";
import * as Progress from "react-native-progress";
import { createImageProgress } from "react-native-image-progress";
const FImage = createImageProgress(FastImage);
interface Props {
  uri?: string;
  style?: ViewStyle;
  resizeMode?: FastImageProps;
  touch?: boolean;
  onPress?: () => void;
}

export default class ProgressFastImage extends Component<Props> {
  render() {
    const { uri, resizeMode, style, touch, onPress } = this.props;
    return (
      <TouchableOpacity
        disabled={!onPress}
        style={style}
        onPress={onPress}
        children={
          <FImage
            style={style}
            source={uri}
            resizeMode={resizeMode}
            indicator={Progress.Pie}
            indicatorProps={{
              size: 20,
              borderWidth: 0,
              color: "rgba(60,14,101, 1)",
              unfilledColor: "rgba(60,14,101, 0.2)"
            }}
          />
        }
      />
    );
  }
}

const styles = StyleSheet.create({});

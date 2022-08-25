import React, { Component } from "react";
import {
  StyleSheet,
  View,
  ActivityIndicator,
  Dimensions,
  Image,
  TouchableOpacity,
  StyleProp,
  ImageStyle
} from "react-native";
import FastImage, { FastImageSource } from "react-native-fast-image";
import R from "@app/assets/R";
import reactotron from "reactotron-react-native";

interface Props {
  /**
   *Bắt buộc:  Uri ảnh hoặc uri local image asset
   */
  source: FastImageSource | number;
  /**
   * Style ảnh. Nếu không có thì không hiển thị gì đâu
   */
  style: StyleProp<ImageStyle>;
  resizeMode?: "contain" | "cover" | "stretch" | "center";
}

interface State {
  isLoading: boolean;
  error: Object;
  height: number;
  width: number;
}

export default class LoadableImage extends Component<Props, State> {
  constructor(props) {
    super(props);
    const { source, style } = this.props;
    this.state = {
      isLoading: false,
      error: null,
      height: null,
      width: null
    };
  }
  UNSAFE_componentWillMount() {
    const heightScreen = Dimensions.get("screen").height;
    const widthScreen = Dimensions.get("screen").width;
    if (this.props.source.uri) {
      Image.getSize(
        this.props.source.uri,
        (width, height) => {
          this.setState({
            height: height * (widthScreen / width),
            width: widthScreen
          });
        },
        err => this.setState({ error: err, isLoading: false })
      );
    }
  }
  render() {
    const { isLoading, height, error, width } = this.state;
    const { source, style, resizeMode, ...props } = this.props;
    if (error)
      return (
        <TouchableOpacity
          onPress={() => this.setState({ isLoading: true, error: null })}
          style={[
            style,
            {
              justifyContent: "center",
              alignItems: "center",
              backgroundColor: "#F1EEEE"
            }
          ]}
        >
          <FastImage
            source={R.images.ic_refresh}
            style={{ width: 30, height: 30, tintColor: "white" }}
            resizeMode={FastImage.resizeMode.contain}
          />
        </TouchableOpacity>
      );
    return (
      <>
        {isLoading && source.uri && (
          <View
            style={[
              style,
              {
                justifyContent: "center",
                alignItems: "center",
                backgroundColor: "#F1EEEE"
              }
            ]}
          >
            <ActivityIndicator />
          </View>
        )}
        {source.uri && (
          <FastImage
            {...this.props}
            source={{
              ...source,
              priority: FastImage.priority.low,
              cache: FastImage.cacheControl.immutable
            }}
            style={isLoading ? {} : style || { flex: 1 }}
            onLoadStart={() => {
              this.setState({ isLoading: true, error: null });
            }}
            onLoadEnd={() => {
              this.setState({ isLoading: false });
            }}
            resizeMode={resizeMode || FastImage.resizeMode.cover}
            onError={() => {
              this.setState({ error: "Co loi xay ra", isLoading: false });
            }}
          />
        )}
        {!source.uri && (
          <FastImage
            {...this.props}
            style={style || { height: height, width: width }}
          />
        )}
      </>
    );
  }
}

const styles = StyleSheet.create({});

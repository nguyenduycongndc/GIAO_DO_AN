import React, { Component } from "react";
import { Text, StyleSheet, View, TouchableOpacity, Image } from "react-native";
import { RNCamera } from "react-native-camera";
import R from "@app/assets/R";
import theme from "@app/constants/Theme";
import reactotron from "reactotron-react-native";
import { RowImageLable } from "@app/components/FormRow";
import ImageResizer from "react-native-image-resizer";
import { BarIndicator } from "react-native-indicators";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { BackButton } from "@app/components/RNHeader";

export default class CameraScreen extends Component {
  constructor(props) {
    super(props);
    const totalImageRequire = this.props.navigation.getParam(
      "totalImageRequire"
    );
    this.state = {
      totalImageRequire: totalImageRequire,
      listImageTakePicture: [],
      data: null,
      isLoading: false
    };
  }

  takePicture = async () => {
    if (this.camera) {
      const options = { quality: 0.5, base64: true };
      const data = await this.camera.takePictureAsync(options);
      this.setState({
        ...this.state,
        data: data
      });
    }
  };

  resizeImage = async () => {
    this.setState({ isLoading: true });
    const { data, listImageTakePicture } = this.state;
    const maxWidth = theme.dimension.width;
    const maxHeight = theme.dimension.height;
    var actualWidth = data.width,
      actualHeight = data.height;
    var imgRatio = actualWidth / actualHeight;
    var maxRatio = maxHeight / maxHeight;
    if (actualHeight > maxHeight || actualWidth > maxWidth) {
      if (imgRatio < maxRatio) {
        imgRatio = maxHeight / actualHeight;
        actualWidth = parseInt(imgRatio * actualWidth);
        actualHeight = parseInt(maxHeight);
      } else if (imgRatio > maxRatio) {
        imgRatio = maxWidth / actualWidth;
        actualHeight = parseInt(imgRatio * actualHeight);
        actualWidth = parseInt(maxWidth);
      } else {
        actualHeight = maxHeight;
        actualWidth = maxWidth;
      }
    }
    var uri = data.uri;
    var a = uri.replace("file:", "");
    try {
      const response = await ImageResizer.createResizedImage(
        a,
        actualWidth,
        actualHeight,
        "JPEG",
        75,
        0,
        ""
      );
      listImageTakePicture.unshift(
        Platform.OS == "android"
          ? response.uri
          : response.uri.replace("file://", "")
      );
      this.props.navigation.state.params.onGoBack(response.uri);
      this.setState(
        {
          ...this.state,
          listImageTakePicture: listImageTakePicture,
          data: null,
          isLoading: false
        },
        NavigationUtil.goBack
      );
    } catch (error) {
      listImageTakePicture.unshift(uri);
      this.props.navigation.state.params.onGoBack(uri);
      this.setState(
        {
          ...this.state,
          listImageTakePicture: listImageTakePicture,
          data: null,
          isLoading: false
        },
        NavigationUtil.goBack
      );
    }
  };

  render() {
    const { totalImageRequire, listImageTakePicture, isLoading } = this.state;
    return (
      <View style={styles.container}>
        {isLoading && <BarIndicator />}
        <RNCamera
          ref={ref => {
            this.camera = ref;
          }}
          style={styles.preview}
          type={RNCamera.Constants.Type.back}
          flashMode={RNCamera.Constants.FlashMode.auto}
          androidCameraPermissionOptions={{
            title: "Permission to use camera",
            message: "We need your permission to use your camera",
            buttonPositive: "Ok",
            buttonNegative: "Cancel"
          }}
          androidRecordAudioPermissionOptions={{
            title: "Permission to use audio recording",
            message: "We need your permission to use your audio",
            buttonPositive: "Ok",
            buttonNegative: "Cancel"
          }}
          onGoogleVisionBarcodesDetected={({ barcodes }) => {
            console.log(barcodes);
          }}
        />
        {this.previewImage()}
        {!this.state.data && (
          <View
            style={{
              flex: 0,
              justifyContent: "center",
              alignItems: "center"
            }}
          >
            <Text
              style={{
                marginTop: 10,
                fontFamily: R.fonts.quicksand_bold,
                fontSize: 16,
                color: theme.colors.white
              }}
            >
              {listImageTakePicture.length}/{totalImageRequire}
            </Text>
            <TouchableOpacity onPress={this.takePicture} style={styles.capture}>
              <View
                style={{ backgroundColor: "white", flex: 1, borderRadius: 60 }}
              />
            </TouchableOpacity>
          </View>
        )}
        <BackButton style={styles.back} />
      </View>
    );
  }

  previewImage = () => {
    const { totalImageRequire, listImageTakePicture } = this.state;
    if (!this.state.data) return;
    return (
      <View
        style={{
          position: "absolute",
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          justifyContent: "center",
          alignItems: "center",
          backgroundColor: "black",
          elevation: Platform.OS == "android" ? 2 : 0
        }}
      >
        <Image
          style={{
            flex: 1,
            width: "100%",
            height: undefined,
            backgroundColor: "black",
            resizeMode: "contain"
          }}
          source={{
            uri: this.state.data.uri
          }}
        />

        <View
          style={{
            flex: 0,
            justifyContent: "center",
            alignItems: "center",
            backgroundColor: theme.colors.white,
            paddingTop: 10
          }}
        >
          {/* <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 16,
              color: theme.colors.textColor
            }}
          >
            {listImageTakePicture.length}/{totalImageRequire}
          </Text> */}
          <View
            style={{
              flex: 0,
              flexDirection: "row",
              width: theme.dimension.width,
              alignItems: "center",
              justifyContent: "center"
            }}
          >
            {listImageTakePicture.length + 1 == totalImageRequire ? (
              <TouchableOpacity style={styles.done} onPress={this.resizeImage}>
                <RowImageLable
                  disableTouch={true}
                  lable={"Xong"}
                  size={14}
                  position="left"
                  textColor={theme.colors.nameText}
                  textStyle={{ fontFamily: R.fonts.quicksand_bold }}
                  image={
                    <Image
                      style={{ width: 20, height: 20, marginRight: 5 }}
                      source={R.images.ic_continue}
                    />
                  }
                />
              </TouchableOpacity>
            ) : (
              <View style={{ flexDirection: "row" }}>
                <TouchableOpacity
                  style={styles.continue}
                  onPress={() => {
                    this.setState({
                      ...this.state,
                      data: null
                    });
                  }}
                >
                  <RowImageLable
                    disableTouch={true}
                    lable={R.strings().capture_again}
                    size={14}
                    position="left"
                    textColor={theme.colors.nameText}
                    textStyle={{ fontFamily: R.fonts.quicksand_bold }}
                    image={
                      <Image
                        style={{ width: 20, height: 20, marginRight: 5 }}
                        source={R.images.ic_capture_again}
                      />
                    }
                  />
                </TouchableOpacity>
                <TouchableOpacity
                  style={styles.continue}
                  onPress={this.resizeImage}
                >
                  <RowImageLable
                    disableTouch={true}
                    lable={R.strings().done}
                    size={14}
                    position="right"
                    textColor={theme.colors.nameText}
                    textStyle={{ fontFamily: R.fonts.quicksand_bold }}
                    image={
                      <Image
                        style={{ width: 20, height: 20, marginLeft: 5 }}
                        source={R.images.ic_continue}
                      />
                    }
                  />
                </TouchableOpacity>
              </View>
            )}
          </View>
        </View>
      </View>
    );
  };
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    flexDirection: "column",
    backgroundColor: "black"
  },
  preview: {
    flex: 1,
    justifyContent: "flex-end",
    alignItems: "center"
  },
  capture: {
    width: 60,
    height: 60,
    flex: 0,
    borderRadius: 60,
    padding: 3,
    alignSelf: "center",
    margin: 20,
    borderWidth: 1,
    borderColor: theme.colors.white
  },
  continue: {
    flex: 1,
    height: 40,
    borderRadius: 60,
    padding: 3,
    marginBottom: 25,
    margin: 15,
    borderWidth: 1,
    alignItems: "center",
    justifyContent: "center"
  },
  captureAgain: {
    flex: 1,
    height: 30,
    borderRadius: 60,
    padding: 3,
    alignSelf: "center",
    margin: 20,
    borderWidth: 1
  },
  done: {
    height: 30,
    borderRadius: 60,
    padding: 3,
    alignSelf: "center",
    margin: 20,
    borderWidth: 1,
    paddingHorizontal: 12,
    paddingVertical: 4
  },
  back: {
    position: "absolute",
    top: 40,
    left: 10
  }
});

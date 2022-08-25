import React, { Component } from "react";
import { StyleSheet, Text, TouchableOpacity, View, Image } from "react-native";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { RNCamera } from "react-native-camera";
import theme from "@app/constants/Theme";
import Icon from "@app/components/Icon";
import R from "@R";
import CallApiHelper from "@app/utils/CallApiHelper";
import { RowImageLable } from "@app/components/FormRow";
import * as API from "@app/constants/Api";
import NavigationUtil from "@app/navigation/NavigationUtil";
import reactotron from "reactotron-react-native";
import { updateProcessingImage } from "@app/redux/actions";
import { BarIndicator } from "react-native-indicators";
import ImageResizer from "react-native-image-resizer";
import { RESIZE_IMAGE } from "@app/constants/Constants";

class CameraScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
      data: this.props.navigation.getParam("data", {}),
      uri: null,
      image: null
    };
  }

  render() {
    const {
      isBefore,
      listImageRequire,
      index,
      orderServiceID,
      isLoading
    } = this.state.data;

    return (
      <View style={styles.container}>
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
            {listImageRequire[index].name} -{" "}
            {isBefore ? R.strings().before : R.strings().after}
          </Text>
          <TouchableOpacity onPress={this.takePicture} style={styles.capture}>
            <View
              style={{ backgroundColor: "white", flex: 1, borderRadius: 60 }}
            />
          </TouchableOpacity>
        </View>
        {this.previewImage()}

        <TouchableOpacity
          style={{
            position: "absolute",
            top: 40,
            left: 20
          }}
          onPress={() => {
            NavigationUtil.goBack();
          }}
        >
          <Image
            style={{
              width: width / 10,
              height: width / 10,
              minWidth: 40,
              minHeight: 40
            }}
            source={R.images.ic_delete_image}
          />
        </TouchableOpacity>

        {this.state.isLoading && (
          <View
            style={{
              position: "absolute",
              top: 0,
              left: 0,
              right: 0,
              bottom: 0,
              justifyContent: "center",
              alignItems: "center",
              backgroundColor: "rgba(0, 0, 0, 0.6)",
              elevation: Platform.OS == "android" ? 4 : 0
            }}
          >
            <View
              style={{
                height: 140,
                backgroundColor: "white",
                padding: 30,
                borderRadius: 10
              }}
            >
              <BarIndicator color={theme.colors.indicator} />
              <Text
                style={{
                  color: theme.colors.indicator
                }}
              >
                Vui lòng đợi..
              </Text>
            </View>
          </View>
        )}
      </View>
    );
  }

  previewImage = () => {
    const {
      isBefore,
      listImageRequire,
      index,
      orderServiceID
    } = this.state.data;
    return (
      this.state.uri && (
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
              uri: this.state.uri
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
            <Text
              style={{
                fontFamily: R.fonts.quicksand_bold,
                fontSize: 16,
                color: theme.colors.textColor
              }}
            >
              {" "}
              {listImageRequire[index].name} -{" "}
              {isBefore ? R.strings().before : R.strings().after}
            </Text>
            <View
              style={{
                flex: 0,
                flexDirection: "row",
                width: theme.dimension.width
              }}
            >
              <TouchableOpacity
                style={styles.continue}
                onPress={() => {
                  this.setState({
                    ...this.state,
                    uri: null
                  });
                }}
              >
                <RowImageLable
                  disableTouch={true}
                  lable={"Chụp lại"}
                  size={14}
                  position="left"
                  textColor={theme.colors.nameText}
                  textStyle={{ fontFamily: R.fonts.quicksand_bold }}
                  image={
                    <Image
                      style={{ width: 20, height: 20, marginRight: 5 }}
                      source={require("../../assets/images/ic_capture_again.png")}
                    />
                  }
                />
              </TouchableOpacity>
              <TouchableOpacity
                style={styles.continue}
                onPress={() => {
                  this.setState({ isLoading: true }, this.resizeImage);
                }}
              >
                <RowImageLable
                  disableTouch={true}
                  lable={"Tiếp tục"}
                  size={14}
                  position="right"
                  textColor={theme.colors.nameText}
                  textStyle={{ fontFamily: R.fonts.quicksand_bold }}
                  image={
                    <Image
                      style={{ width: 20, height: 20, marginLeft: 5 }}
                      source={require("../../assets/images/ic_continue.png")}
                    />
                  }
                />
              </TouchableOpacity>
            </View>
          </View>
        </View>
      )
    );
  };

  takePicture = async () => {
    if (this.camera) {
      const options = {
        quality: 0.5,
        base64: true,
        width: RESIZE_IMAGE.WIDTH,
        height: RESIZE_IMAGE.HEIGHT
      };
      const data = await this.camera.takePictureAsync(options);
      const { uri, width, height } = data;
      const cropData = {
        offset: { x: 0, y: 0 },
        size: { width, height },
        displaySize: { width: RESIZE_IMAGE.WIDTH, height: RESIZE_IMAGE.HEIGHT }
      };
      // ImageEditor.cropImage(uri, cropData, resizedImage => {
      //   reactotron.log("resizedImage", resizedImage);
      // }).catch(error => {
      //   reactotron.log("error", error);
      // });
      this.setState({
        ...this.state,
        image: data,
        uri: data.uri
      });
    }
  };

  nextImage = () => {
    const {
      isBefore,
      listImageRequire,
      index,
      orderServiceID
    } = this.state.data;
    if (index + 1 == listImageRequire.length) {
      NavigationUtil.goBack();
    } else {
      this.setState({
        ...this.state,
        isLoading: false,
        uri: null,
        data: {
          ...this.state.data,
          index: index + 1
        }
      });
    }
  };

  resizeImage = async () => {
    const { image } = this.state;
    const maxWidth = theme.dimension.width;
    const maxHeight = theme.dimension.height;
    var actualWidth = image.width,
      actualHeight = image.height;
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
    var uri = image.uri;
    try {
      const response = await ImageResizer.createResizedImage(
        uri,
        actualWidth,
        actualHeight,
        "JPEG",
        75,
        0,
        ""
      );
      this.uploadImage(response.uri);
    } catch (error) {
      this.uploadImage(uri);
    }
  };

  uploadImage = async uri => {
    const formData = new FormData();
    formData.append("image", {
      name: `images1`,
      type: "image/jpeg",
      filename: "image.png",
      uri: uri
    });

    const {
      isBefore,
      listImageRequire,
      index,
      orderServiceID
    } = this.state.data;
    const payload = {
      data: formData,
      serviceID: orderServiceID,
      imageRequiteID: isBefore
        ? listImageRequire[index].before.imageRequireID
        : listImageRequire[index].after.imageRequireID
    };

    CallApiHelper(
      API.uploadImage,
      payload,
      this,
      res => {
        this.props.updateProcessingImage({
          data: res.result,
          isBefore,
          index
        });
        this.setState(
          {
            ...this.state,
            isLoading: false
          },
          () => {
            // this.nextImage
            NavigationUtil.goBack();
          }
        );
      },
      error => {}
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
  }
});

const mapStateToProps = state => ({
  ProcessingServiceState: state.processingServiceReducer
});

const mapDispatchToProps = {
  updateProcessingImage
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(CameraScreen);

import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  FlatList,
  TouchableOpacity,
  ScrollView,
  TextInput,
  KeyboardAvoidingView,
  Platform,
  Dimensions
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import FastImage from "react-native-fast-image";
import reactotron from "reactotron-react-native";
import ButtonPrimary from "@app/components/ButtonPrimary";
import { SafeAreaView } from "react-native";
import theme from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  ORDER_STATUS,
  SCREEN_ROUTER_WASHER,
  IMAGE_TYPE
} from "@app/constants/Constants";
import CallApiHelper from "@app/utils/CallApiHelper";
import * as API from "@app/constants/Api";
import Loading from "@app/components/Loading";
import { connect } from "react-redux";
import { getListOrderService } from "@app/redux/actions";
import { CheckBox } from "react-native-elements";
import ActionSheet from "react-native-actionsheet";
import ImageResizer from "react-native-image-resizer";
import ImagePicker from "react-native-image-picker";

const MAX_LENGTH_IMAGE_SCRATCH = 5;
const maxWidth = Dimensions.get("screen").width;
const maxHeight = Dimensions.get("screen").height;
class UploadImageScratch extends Component {
  constructor(props) {
    super(props);
    const listImage = this.props.navigation.getParam("listImage");
    const note = this.props.navigation.getParam("note");
    const orderServiceID = this.props.navigation.getParam("orderServiceID");
    const dataSearch = this.props.navigation.getParam("dataSearch");
    this.state = {
      contentHeight: 0,
      note: note || "",
      listImage: listImage || [],
      orderServiceID: orderServiceID,
      isLoadingProgress: false,
      type: IMAGE_TYPE.CAR,
      dataSearch: dataSearch,
      checked: false
    };
  }

  renderListImage() {
    const listImage = [...this.state.listImage];
    if (listImage.length < 5) listImage.push(R.images.ic_add_image);
    return (
      <View style={styles.containerListImage}>
        <FlatList
          style={{ marginHorizontal: 7 }}
          data={listImage}
          numColumns={3}
          renderItem={this.renderItem}
        />
      </View>
    );
  }

  onGoBack = callback => {
    var listImage = [...this.state.listImage];
    if (typeof listImage[listImage.length - 1] == "number")
      listImage.splice(listImage.length - 1, 1);
    this.setState({ listImage: listImage.concat(callback) });
  };

  renderActionSheet() {
    return (
      <ActionSheet
        ref={o => (this.ActionSheet = o)}
        title={R.strings().select_picture}
        options={[R.strings().image_library, "Camera", R.strings().cancel]}
        cancelButtonIndex={2}
        onPress={indexAction => {
          if (indexAction == 0) {
            this.showPickerImage();
          }
          if (indexAction == 1) {
            this.handleNavigateCamera();
          }
        }}
      />
    );
  }

  showPickerImage = () => {
    try {
      const options = {};
      ImagePicker.launchImageLibrary(options, response => {
        if (response.didCancel) {
          // console.log("User cancelled photo picker");
        } else if (response.error) {
          // console.log("ImagePicker Error: ", response.error);
        } else if (response.customButton) {
          // console.log("User tapped custom button: ", response.customButton);
        } else {
          // You can also display the image using data:
          // let source = { uri: 'data:image/jpeg;base64,' + response.data };
          var actualWidth = response.width,
            actualHeight = response.height;
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

          const source = { uri: response.uri };
          this._resizeImage(source.uri, actualWidth, actualHeight);
        }
      });
    } catch (error) {
      console.log("error", error);
    }
  };

  _resizeImage = async (uri, actualWidth, actualHeight) => {
    try {
      const response = await ImageResizer.createResizedImage(
        uri,
        actualWidth,
        actualHeight,
        "JPEG",
        70,
        0,
        ""
      );
      this.onGoBack(response.uri);
    } catch (error) {
      console.log(error);
      this.onGoBack(uri);
    }
  };

  handleNavigateCamera = () => {
    const { listImage } = this.state;
    NavigationUtil.navigate(SCREEN_ROUTER_WASHER.CAMERA_UPLOAD_IMAGE_SCRATCH, {
      totalImageRequire: MAX_LENGTH_IMAGE_SCRATCH - listImage.length,
      onGoBack: this.onGoBack
    });
  };

  showActionSheet = () => {
    this.ActionSheet.show();
  };

  renderItem = ({ item, index }) => {
    const { listImage } = this.state;
    const sourceImage =
      typeof item === "number" ? item : { uri: item.image || item };
    return (
      <TouchableOpacity
        disabled={typeof item === "string" || typeof item === "object"}
        style={[
          {
            marginRight: 5,
            width:
              listImage.length == 2 && typeof item === "number"
                ? width / 3
                : width / 3 - 10
          }
        ]}
        onPress={() => {
          if (typeof item !== "string") this.showActionSheet();
        }}
      >
        <FastImage
          source={sourceImage}
          style={[
            styles.image,
            {
              borderRadius: 3
            }
          ]}
          resizeMode={
            typeof item == "number"
              ? FastImage.resizeMode.stretch
              : FastImage.resizeMode.cover
          }
        />
        {typeof item !== "number" && (
          <TouchableOpacity
            style={styles.containerIconDelete}
            onPress={() => this.deleteImage(item, index)}
          >
            <FastImage
              source={R.images.ic_delete_image}
              style={styles.iconDelete}
              resizeMode={FastImage.resizeMode.contain}
            />
          </TouchableOpacity>
        )}
      </TouchableOpacity>
    );
  };

  deleteImage = async (item, index) => {
    if (typeof item === "object" && item.image.includes("http")) {
      //call api remove image
      this.setState({ isLoadingProgress: true }, async () => {
        try {
          await API.requestDeleteImageCarService({ listID: [item.id] });
          this.props.getListOrderService(this.state.dataSearch);
          var array = [...this.state.listImage];
          array.splice(index, 1);
          this.setState({ listImage: array, isLoadingProgress: false });
        } catch (error) {
          this.setState({ isLoadingProgress: false });
        }
      });
    } else {
      var array = [...this.state.listImage];
      array.splice(index, 1);
      this.setState({ listImage: array });
    }
  };

  renderNote() {
    const { checked } = this.state;
    return (
      <View style={{ flex: 1 }}>
        <CheckBox
          checked={checked}
          title={R.strings().car_no_scratch}
          containerStyle={styles.checkbox}
          textStyle={{ fontSize: 14, fontFamily: R.fonts.quicksand_semi_bold }}
          onPress={() =>
            this.setState({
              checked: !checked,
              note: checked ? "" : R.strings().car_no_scratch
            })
          }
        />
        <TextInput
          style={styles.multiline}
          placeholder={`${R.strings().content}...`}
          autoCapitalize="sentences"
          multiline
          maxLength={300}
          value={this.state.note}
          onChangeText={text => this.setState({ note: text })}
          underlineColorAndroid="transparent"
        />
        <Text
          style={{
            fontFamily: R.fonts.quicksand_light,
            fontSize: 12,
            position: "absolute",
            top: 10,
            right: 10,
            color: "black"
          }}
        >
          {this.state.note.length}/300
        </Text>
      </View>
    );
  }

  checkUploadImage() {
    var i = -1;
    this.state.listImage.map((value, index) => {
      if (typeof value === "string" && !value.includes("http")) {
        i = index;
        return;
      }
    });
    return i;
  }

  uploadImage = async () => {
    const { note, listImage, orderServiceID } = this.state;
    var body = null;
    if (!!listImage.length && this.checkUploadImage() != -1) {
      body = new FormData();
      listImage.map((value, index) => {
        if (typeof value == "string" && !value.includes("http")) {
          body.append(`image${index}`, {
            name: `images${index}`,
            type: "image/jpeg",
            uri: value
          });
        }
      });
    }
    try {
      const res = await API.uploadCarImageService(this.state, body);
      this.props.getListOrderService(this.state.dataSearch);
      this.setState({ isLoadingProgress: false }, NavigationUtil.goBack);
    } catch (error) {
      console.log("error", error);
      this.setState({ isLoadingProgress: false });
    }
  };
  handleContentSizeChange = (contentWidth, contentHeight) => {
    if (this.state.contentHeight === 0) {
      this.setState({ contentHeight });
    }
  };
  render() {
    const { isLoadingProgress } = this.state;
    return (
      <ScreenComponent
        titleHeader={R.strings().image_scratch}
        back
        dialogLoading={this.state.isLoadingProgress}
        renderView={
          <ScrollView
            onContentSizeChange={this.handleContentSizeChange}
            style={{
              flex: 1,
              width: "100%"
            }}
            contentContainerStyle={[
              {
                flex: 1,
                width: "100%"
              },
              { minHeight: this.state.contentHeight }
            ]}
          >
            <KeyboardAvoidingView
              style={{ flex: 1, justifyContent: "center" }}
              enabled
              behavior={Platform.OS == "ios" ? "padding" : undefined}
              keyboardVerticalOffset={Platform.OS === "ios" ? 40 : 0}
            >
              {this.renderListImage()}
              {this.renderNote()}
              {this.renderActionSheet()}
              <ButtonPrimary
                onPress={() =>
                  this.setState({ isLoadingProgress: true }, this.uploadImage)
                }
                text={R.strings().done}
                style={styles.button_complete}
              />
            </KeyboardAvoidingView>
          </ScrollView>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  image: {
    width: width / 3 - 15,
    height: 108,
    marginHorizontal: 5,
    marginVertical: 6,
    borderRadius: 3
  },
  checkbox: {
    backgroundColor: "transparent",
    borderWidth: 0,
    marginBottom: -20,
    marginTop: -4
  },
  iconDelete: {
    width: 26,
    height: 26
  },
  containerIconDelete: {
    position: "absolute",
    top: 5,
    right: 4,
    padding: 4
  },
  containerListImage: {
    width: width,
    backgroundColor: "white",
    paddingVertical: 8
  },
  button_complete: { marginBottom: 20 },
  multiline: {
    height: 144,
    marginTop: 20,
    marginHorizontal: 10,
    borderColor: "#CECACA",
    borderWidth: 0.5,
    borderRadius: 5,
    textAlignVertical: "top",
    backgroundColor: "white",
    paddingLeft: 12,
    color: "black"
  }
});
const mapStateToProps = state => ({});

const mapDispatchToProps = {
  getListOrderService
};
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(UploadImageScratch);

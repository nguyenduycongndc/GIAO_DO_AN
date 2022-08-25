import R from "@app/assets/R";
import Button from "@app/components/Button";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@app/constants/Theme";
import React, { Component } from "react";
import {
  Alert,
  StyleSheet,
  Text,
  TextInput,
  View,
  ScrollView,
  Dimensions,
  TouchableOpacity,
  KeyboardAvoidingView,
  FlatList,
  PermissionsAndroid,
  Platform
} from "react-native";
// import Dropdown from "@app/components/Dropdown";
import Icon from "@component/Icon";
import DatePicker from "react-native-datepicker";
// import DropdownMenu from 'react-native-dropdown-menu';
import * as API from "@api";
import CallApiHelper from "../../../utils/CallApiHelper";
import { Dropdown } from "react-native-material-dropdown";
import ImagePicker from "react-native-image-picker";
import PhotoListView from "@app/components/PhotoListView";
import { getUserInfoAction, getCarDetail } from "@action";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import Spinner from "react-native-loading-spinner-overlay";
import reactotron from "reactotron-react-native";
import { showMessages, showConfirm } from "@app/components/Alert";
import { createImageProgress } from "react-native-image-progress";
// import FastImage from "@app/components/FastImage";
import * as Progress from "react-native-progress";
import ImageResizer from "react-native-image-resizer";
// import imagePickerHelper, { options } from "@app/utils/ImagePickerHelper";
import { convertDate } from "@app/constants/Functions";
import FastImg from "@app/components/FastImage";
import analytics from '@react-native-firebase/analytics';

const maxWidth = Dimensions.get("screen").width;
const maxHeight = Dimensions.get("screen").height;
const REGEX_PLATE_NUMBER = /^[0-9]{2}[A-Za-z]{1}-[0-9]{4,5}$/;
// const Image = createImageProgress(FastImage);

var d = new Date();

var date = d.getDate();
var month = d.getMonth() + 1; // Since getMonth() returns month from 0-11 not 1-12
var year = d.getFullYear();

var listImage = [];

var dateStr = date + "/" + month + "/" + year;

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

export class UpdateCarInfoScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const item = navigation.getParam("item", {});
    const carInfo = navigation.getParam("carInfo", {});
    this.state = {
      carInfo: carInfo,
      token: "",
      isLoading: false,
      error: "",
      modelLoading: false,
      limitImage: "",
      text: "",
      keyListCar: {
        search: "",
        carBrandID: ""
      },
      avatarSource: [],
      getListCarBrand: [],
      getListCarMode: [],
      carBrandID: carInfo.carBrandID,
      districtName: item.districtName,
      payload: {
        carID: carInfo.carID,
        carModel: carInfo.carModel,
        carModelID: carInfo.carModelID,
        carBrand: carInfo.carBrand,
        licensePlates: carInfo.licensePlates,
        carColor: carInfo.carColor,
        // Status: carInfo.status,
        // manufacturingDate: carInfo.manufacturingDate,
        registrationDateStr:
          carInfo.registrationDateStr ||
          convertDate(new Date().toISOString().split("T")[0]),
        VehicleRegistration: carInfo.vehicleRegistration,
        listImage: carInfo.listImage
      }
    };
  }

  componentDidMount() {
    this.getData();
  }

  getData() {
    listImage = [];
    this.getConfig();
    this.GetListCarModeAndBrand({ search: "", carBrandID: "" });
    this.GetListCarModeAndBrand({
      search: "",
      carBrandID: this.state.carBrandID
    });
    this.getListImage();
  }

  getListImage() {
    for (let i = 0; i < this.state.payload.listImage.length; i++) {
      listImage.push({ url: this.state.payload.listImage[i].url });
    }
  }

  getConfig() {
    CallApiHelper(API.getConfig, null, null, res => {
      this.setState({
        limitImage: res["result"][2]["valueConstant"]
      });
    });
  }

  _checkPermission = async () => {
    var check = true;
    if (Platform.OS == "android")
      check = await PermissionsAndroid.check(
        PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE
      );
    // console.log(check);
    if (!check) {
      try {
        granted = await PermissionsAndroid.request(
          PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE
        );
        this._pickerImage();
      } catch (error) {}
    } else {
      this._pickerImage();
    }
  };

  updateCarInfo() {
    CallApiHelper(API.UpdateCar, this.state.payload, this, async () => {
      analytics().logEvent('update_car_success', {
        carBrand: res.result.carBrand,
        platesNumber: res.result.licensePlates,
        carModel: res.result.carModel
      })
      this.updateCarPhoto();
    });
  }

  updateCarPhoto() {
    this.setState({
      modelLoading: true
    });
    const data = new FormData();
    this.state.payload.listImage.forEach((value, index) => {
      data.append(
        `${index}`,
        // name: `image${index + 1}`,
        // // type: `image/jpg`,
        // Platform.OS === "ios" ? value.url.replace("file://", "") : value
        {
          name: `images1`,
          type: "image/jpeg",
          filename: "image.png",
          uri: value.url
        }
      );
    });
    var payload = {
      data: data,
      carID: this.state.payload.carID
    };
    CallApiHelper(
      API.uploadCarImage,
      payload,
      this,
      res => {
        this.setState(
          {
            modelLoading: false
          },
          () => {
            this.props.getCarDetail(this.state.payload.carID);
            this.props.getUserInfoAction();
            NavigationUtil.goBack();
          }
        );
      },
      error =>
        this.state(
          {
            modelLoading: false
          },
          () => console.log(error)
        )
    );
  }

  GetListCarModeAndBrand = keyListCar => {
    CallApiHelper(
      API.GetListCarModeAndBrand,
      keyListCar,
      this,
      async res => {
        if (keyListCar.carBrandID == "") {
          this.setState({
            getListCarBrand: res.result.listCarBrand
            // getListCarMode: res.result.listCarMode,
          });
        } else if (this.state.carBrandID != keyListCar.carBrandID) {
          analytics().logEvent('select_car_brand', res.result.listCarBrand[0])
          this.setState({
            modelLoading: false,
            getListCarMode: res.result.listCarMode,
            payload: {
              ...this.state.payload,
              carModel: res.result.listCarMode[0].name,
              carModelID: res.result.listCarMode[0].carModelID
            }
          });
        } else {
          this.setState({
            modelLoading: false,
            getListCarMode: res.result.listCarMode
          });
        }
      },
      err => {
        setTimeout(() => this.GetListCarModeAndBrand(keyListCar), 1000);
      }
    );
  };

  renderDropdown = (label, data, value, onChangeText) => (
    <View>
      <View style={{ flexDirection: "row", alignItems: "center" }}>
        <Text
          style={{
            fontFamily: R.fonts.quicksand_medium,
            fontSize: 14,
            paddingVertical: 10
          }}
        >
          {label}
        </Text>
        {(label == R.strings().model || label == R.strings().brand) && (
          <Text style={{ color: theme.colors.red }}> (*)</Text>
        )}
      </View>
      <View style={styles.text_input}>
        <Dropdown
          inputContainerStyle={{
            borderBottomColor: "transparent",
            // justifyContent: "center",
            fontSize: 14
          }}
          fontSize={14}
          data={data}
          value={value}
          containerStyle={{ width: "100%", marginRight: 5 }}
          baseColor={theme.colors.nameText}
          dropdownOffset={{ top: 15, left: 0 }}
          itemTextStyle={{ fontFamily: R.fonts.quicksand_medium, fontSize: 14 }}
          labelExtractor={item =>
            label == R.strings().brand ? item.carBrandName : item.name
          }
          keyExtractor={item =>
            label == R.strings().brand ? item.carBrandID : item.carModelID
          }
          onChangeText={onChangeText}
        />
      </View>
    </View>
  );

  render() {
    const {
      carInfo,
      isLoading,
      error,
      getListCarBrand,
      payload,
      modelLoading
    } = this.state;
    return (
      <ScreenComponent
        back
        dialogLoading={modelLoading}
        isLoading={isLoading && modelLoading == false}
        isError={error}
        titleHeader={R.strings().update_car_info}
        renderView={
          <>
            <KeyboardAvoidingView
              keyboardVerticalOffset={80}
              behavior={Platform.OS == "ios" ? "padding" : undefined}
              style={{ flex: 1 }}
            >
              <ScrollView
                keyboardShouldPersistTaps="handled"
                showsVerticalScrollIndicator={false}
                style={{ flex: 1, paddingHorizontal: 8 }}
              >
                <Text
                  style={{
                    fontSize: 14,
                    fontFamily: R.fonts.quicksand_bold,
                    color: theme.colors.darkBlue,
                    paddingVertical: 10
                  }}
                >
                  {R.strings().car_info}
                </Text>
                {this._renderTextInput(
                  R.strings().license_plate,
                  payload.licensePlates,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        licensePlates: text
                      }
                    });
                  }
                )}
                {this.renderDropdown(
                  R.strings().brand,
                  this.state.getListCarBrand,
                  this.state.payload.carBrand,
                  (value, index, data) => {
                    this.setState(
                      {
                        modelLoading: true,
                        keyListCar: {
                          ...this.state.keyListCar,
                          carBrandID: data[index].carBrandID
                        },
                        payload: {
                          ...this.state.payload,
                          carBrand: data[index].carBrandName
                        }
                      },
                      () =>
                        this.GetListCarModeAndBrand({
                          search: "",
                          carBrandID: data[index].carBrandID
                        })
                    );
                  }
                )}
                {this.renderDropdown(
                  R.strings().model,
                  this.state.getListCarMode,
                  this.state.payload.carModel,
                  async (value, index, data) => {
                    analytics().logEvent('select_car_mode', {
                      car: data[index].name
                    })
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        carModel: data[index].name,
                        carBrandID: data[index].carBrandID,
                        carModelID: data[index].carModelID
                      }
                    });
                  }
                )}
              {this._renderTextInput(
                  R.strings().color_car,
                  payload.carColor,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        carColor: text
                      }
                    });
                  },
                  false
                )}

                {/* {this._renderTextInput(
                  R.strings().registration_date,
                  payload.registrationDateStr,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        registrationDateStr: text
                      }
                    });
                  }
                )}

                {this._renderTextInput(
                  R.strings().registration,
                  payload.VehicleRegistration,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        VehicleRegistration: text
                      }
                    });
                  }
                )} */}
                <View style={{ flexDirection: "row", alignItems: "center" }}>
                  <Text
                    style={{
                      fontSize: 14,
                      fontFamily: R.fonts.quicksand_bold,
                      color: theme.colors.darkBlue,
                      paddingVertical: 10
                    }}
                  >
                    {R.strings().car_photo}
                  </Text>
                  {/* <Text style={{ color: theme.colors.red }}> (*)</Text> */}
                </View>

                <PhotoListView
                  data={listImage}
                  editable={true}
                  vertical={false}
                  onAddPress={() => this._checkPickImage()}
                  onDeletePress={this._deleteImage}
                />
                <Button
                  action={() => {
                    const {
                      carColor,
                      carModelID,
                      licensePlates,
                      listImage
                    } = this.state.payload;
                    // if (!carColor)
                    //   return showMessages(
                    //     R.strings().notif_tab_cus,
                    //     R.strings().input_color
                    //   );
                    if (!carModelID)
                      return showMessages(
                        R.strings().notif_tab_cus,
                        R.strings().input_car_modal
                      );
                    if (!licensePlates)
                      return showMessages(
                        R.strings().notif_tab_cus,
                        R.strings().input_license_plates
                      );

                    // if (listImage.length == 0)
                    //   return showMessages(
                    //     R.strings().notif_tab_cus,
                    //     R.strings().input_image
                    //   );

                    this.updateCarInfo();
                  }}
                  title={R.strings().update}
                  backgroundColor={theme.colors.primary}
                  colorText={theme.colors.white}
                  width={"100%"}
                  buttonStyle={{ marginTop: 50 }}
                />
              </ScrollView>
            </KeyboardAvoidingView>
          </>
        }
      />
    );
  }

  _checkPickImage() {
    if (listImage.length == this.state.limitImage) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().exceeded_number_of_photos
      );
    } else
      return (Platform.OS = "android"
        ? this._checkPermission()
        : this._pickerImage());
  }

  _pickerImage() {
    const options = {
      title: R.strings().select,
      cancelButtonTitle: R.strings().cancel,
      chooseFromLibraryButtonTitle: R.strings().from_library,
      takePhotoButtonTitle: R.strings().take_photo,
      storageOptions: {
        skipBackup: true,
        path: "images"
      },
      tintColor: colors.black
    };
    ImagePicker.showImagePicker(options, response => {
      if (response.didCancel) {
        console.log("User cancelled image picker");
      } else if (response.error) {
        console.log("ImagePicker Error: ", response.error);
      } else if (response.customButton) {
        console.log("User tapped custom button: ", response.customButton);
      } else {
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
  }

  _resizeImage = async (uri, actualWidth, actualHeight) => {
    try {
      const response = await ImageResizer.createResizedImage(
        uri,
        actualWidth,
        actualHeight,
        "PNG",
        70,
        0,
        ""
      );
      listImage.unshift({ url: response.uri });
      this.setState({
        payload: {
          ...this.state.payload,
          listImage: listImage
        }
      });
    } catch (error) {
      console.log(error);
      listImage.unshift({ url: uri });
      this.setState({
        payload: {
          ...this.state.payload,
          listImage: listImage
        }
      });
    }
  };

  _deleteImage = ({ photo, index }) => {
    var restImage = listImage.filter(value => {
      return value != listImage[index];
    });
    listImage = restImage;
    this.setState({
      payload: {
        ...this.state.payload,
        listImage: listImage
      }
    });
  };

  _renderTextInput(label, value, onChangeText, isRequire = true) {
    return (
      <View>
        <View style={{ flexDirection: "row", alignItems: "center" }}>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              paddingVertical: 10
            }}
          >
            {label}
          </Text>
          {isRequire && <Text style={{ color: theme.colors.red }}> (*)</Text>}
        </View>
        <View style={styles.text_input}>
          {label == R.strings().registration_date ? (
            <DatePicker
              style={styles.InputDate}
              date={this.state.payload.registrationDateStr}
              mode="date"
              format="DD/MM/YYYY"
              minDate="11/11/1980"
              maxDate={dateStr}
              confirmBtnText={R.strings().confirm}
              cancelBtnText={R.strings().cancel}
              androidMode="spinner"
              customStyles={{
                dateInput: {
                  borderWidth: 0,
                  marginLeft: -windowWidth / 1.6
                }
              }}
              onDateChange={date => {
                this.setState({
                  payload: {
                    ...this.state.payload,
                    registrationDateStr: date
                  }
                });
              }}
            />
          ) : (
            <>
              <TextInput
                value={value}
                style={{ width: "95%", color: "black", paddingVertical: 10 }}
                maxLength={30}
                placeholderTextColor={colors.gray}
                placeholder={R.strings().enter + " " + label.toLowerCase()}
                onChangeText={onChangeText}
                autoCapitalize={
                  label == R.strings().license_plate ? "characters" : "none"
                }
              />
            </>
          )}
        </View>
        {label == R.strings().license_plate && !REGEX_PLATE_NUMBER.test(value) && (
          <Text
            style={{
              fontSize: 13,
              fontStyle: "italic",
              color: theme.colors.primary,
              marginTop: -2,
              marginLeft: 2
            }}
            children={R.strings().format_plate}
          />
        )}
      </View>
    );
  }
}

const styles = StyleSheet.create({
  text_label: {
    marginHorizontal: 10,
    marginTop: 15,
    marginBottom: 5,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  text_input: {
    height: 46,
    backgroundColor: theme.colors.backgroundInput,
    borderRadius: 5,
    paddingHorizontal: 10,
    marginBottom: 15,
    flexDirection: "row",
    alignItems: "center",
    flex: 1
  },
  InputDate: {
    width: "100%",
    marginLeft: Platform.OS == "android" ? 10 : 0
  },
  selectImg: {
    padding: 10,
    alignItems: "center",
    justifyContent: "center",
    borderRadius: 5,
    borderWidth: 1,
    width: 112,
    height: 112,
    borderColor: theme.colors.grayBorder,
    marginRight: 12,
    borderStyle: "dashed"
  },
  imgSelected: {
    width: 112,
    height: 112,
    borderRadius: 5
  },
  spinnerTextStyle: {
    color: "#FFF"
  }
});

const mapStateToProps = state => ({
  lang: state.lang,
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfoAction,
  getCarDetail
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(UpdateCarInfoScreen);

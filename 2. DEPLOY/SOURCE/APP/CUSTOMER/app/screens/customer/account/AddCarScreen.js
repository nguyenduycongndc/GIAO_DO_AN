import R from "@app/assets/R";
import Button from "@app/components/Button";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@app/constants/Theme";
import React, { Component } from "react";
import {
  StyleSheet,
  Text,
  TextInput,
  View,
  ScrollView,
  Dimensions,
  KeyboardAvoidingView,
  PermissionsAndroid,
  Platform,
  Linking,
  TouchableOpacity
} from "react-native";
import DatePicker from "react-native-datepicker";
import * as API from "@api";
import CallApiHelper from "../../../utils/CallApiHelper";
import PhotoListView from "@app/components/PhotoListView";
import { Dropdown } from "react-native-material-dropdown";
import { getUserInfoAction } from "@action";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { showMessages, showConfirm } from "@app/components/Alert";
import { createImageProgress } from "react-native-image-progress";
import FastImage from "@app/components/FastImage";
import Modal from "react-native-modal";
import { SCREEN_ROUTER_CUSTOMER } from "@constant";
import ImagePickerHelper from "@app/utils/ImagePickerHelper";
import reactotron from "reactotron-react-native";
import ModalAlert from "@app/components/ModalAlert";
import { requestAddCar } from "@api";
import analytics from '@react-native-firebase/analytics';

const maxWidth = Dimensions.get("screen").width;
const maxHeight = Dimensions.get("screen").height;

const Image = createImageProgress(FastImage);

var d = new Date();

var day = ("0" + d.getDate()).slice(-2);
var month = ("0" + (d.getMonth() + 1)).slice(-2);
var year = d.getFullYear();

var dateStr = day + "/" + month + "/" + year;

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;
const MAX_SIZE_UPLOAD_IMAGE = 5;
const REGEX_PLATE_NUMBER = /^[0-9]{2}[A-Za-z]{1}-[0-9]{4,5}$/;
let isSubmit = false;
export class AddCarScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      carID: "",
      error: "",
      modelLoading: false,
      keyListCar: {
        search: "",
        carBrandID: ""
      },
      avatarSource: [],
      getListCarBrand: [],
      limitImage: "",
      isModalVisible: false,
      isShowRequestCarInfo: false,
      getListCarMode: [],
      carModel: "",
      carBrand: "",
      textRequestCarInfo: "",
      payload: {
        carModelID: "",
        licensePlates: "",
        carColor: "",
        registrationDateStr: dateStr,
        VehicleRegistration: "",
        listImage: []
      }
    };
  }

  componentDidMount = () => this.getData();

  getData = () => {
    this.getConfig();
    this.GetListCarModeAndBrand({ search: "", carBrandID: "" });
  };

  getConfig = () => {
    CallApiHelper(API.getConfig, null, null, res =>
      this.setState({
        limitImage: res["result"][2]["valueConstant"]
      })
    );
  };

  requestPerAndroid = async () => {
    const isGrand = await PermissionsAndroid.request(
      PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE
    );
    if (isGrand != "granted") {
      this.requestPerAndroid();
      return;
    }
    this._pickerImage();
  };

  toggleModal = () =>
    this.setState({
      isModalVisible: !this.state.isModalVisible
    });

  checkValidateValue = payload => {
    if (!payload.licensePlates.trim()) {
      showMessages(
        R.strings().notif_tab_cus,
        `${R.strings().please_enter} ${R.strings().license_plate}`
      );
      return false;
    }
    if (!REGEX_PLATE_NUMBER.test(payload.licensePlates)) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().incorrect_format_plate
      );
      return;
    }
    if (!this.state.carBrand) {
      showMessages(
        R.strings().notif_tab_cus,
        `${R.strings().please_enter} ${R.strings().brand}`
      );
      return false;
    }
    if (!payload.carModelID) {
      showMessages(
        R.strings().notif_tab_cus,
        `${R.strings().please_enter} ${R.strings().model}`
      );
      return false;
    }
    // if (!payload.carColor.trim()) {
    //   showMessages(
    //     R.strings().notif_tab_cus,
    //     `${R.strings().please_enter} ${R.strings().color_car}`
    //   );
    //   return false;
    // }
    // if (!payload.listImage.length) {
    //   showMessages(
    //     R.strings().notif_tab_cus,
    //     `${R.strings().please_enter} ${R.strings().car_photo}`
    //   );
    //   return false;
    // }
    return true;
  };

  addCar = () => {
    const body = JSON.parse(JSON.stringify(this.state.payload));
    if (!this.checkValidateValue(body)) return;
    delete body.listImage;
    CallApiHelper(API.addCar, body, this, async res => {
      analytics().logEvent('add_car_success', {
        carBrand: res.result.carBrand,
        platesNumber: res.result.licensePlates,
        carModel: res.result.carModel
      })
      this.setState(
        {
          carID: res.result.carID
        },
        () => this.updateCarPhoto(res.result.carID)
      );
    });
  };

  updateCarPhoto = carID => {
    this.setState({
      modelLoading: true
    });
    const data = new FormData();
    this.state.payload.listImage.forEach((value, index) => {
      data.append(`${index}`, {
        name: `image${index}`,
        type: "image/jpeg",
        uri: value.url
      });
    });
    var payload = {
      data,
      carID
    };
    CallApiHelper(
      API.uploadCarImage,
      payload,
      this,
      () => {
        this.setState(
          {
            modelLoading: false
          },
          () => {
            this.props.getUserInfoAction();
            if (this.props.navigation.state.params?.onGoBack)
              this.props.navigation.state.params.onGoBack(carID);
            NavigationUtil.goBack();
          }
        );
      },
      error => {
        this.setState({
          modelLoading: false
        });
      }
    );
  };

  GetListCarModeAndBrand = keyListCar => {
    CallApiHelper(
      API.GetListCarModeAndBrand,
      keyListCar,
      this,
      async res => {
        if (keyListCar.carBrandID == "") {
          this.setState({
            getListCarBrand: res.result.listCarBrand
          });
        } else {
          analytics().logEvent('select_car_brand', res.result.listCarBrand[0])
          this.setState({
            modelLoading: false,
            getListCarMode: [{ name: R.strings().other }].concat(
              res.result.listCarMode
            ),
            carModel: res.result.listCarMode[0].name,
            payload: {
              ...this.state.payload,
              carModelID: res.result.listCarMode[0].carModelID
            }
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
          placeholderTextColor={colors.gray}
          placeholder={R.strings().select + " " + label.toLowerCase()}
        />
      </View>
    </View>
  );

  showRequestCarInfo = () => this.setState({ isShowRequestCarInfo: true });
  dismissRequestCarInfo = () => this.setState({ isShowRequestCarInfo: false });
  clearRequestCar = () => this.setState({ textRequestCarInfo: "" });

  render() {
    const {
      error,
      payload,
      modelLoading,
      isShowRequestCarInfo,
      textRequestCarInfo,
      isLoading
    } = this.state;
    return (
      <ScreenComponent
        back
        dialogLoading={modelLoading || isLoading}
        isError={error}
        reload={() => this.getData()}
        titleHeader={R.strings().add_car}
        rightComponent={
          <TouchableOpacity
            onPress={this.showRequestCarInfo}
            children={
              <FastImage
                source={R.images.ic_info}
                style={{ width: 30, height: 30 }}
              />
            }
          />
        }
        renderView={
          <>
            <ModalAlert
              isVisible={isShowRequestCarInfo}
              onClose={() => {
                isSubmit = false;
                this.dismissRequestCarInfo();
                this.clearRequestCar();
              }}
              onSubmit={() => {
                isSubmit = true;
                this.dismissRequestCarInfo();
              }}
              onModalHide={() => {
                if (isSubmit)
                  CallApiHelper(
                    requestAddCar,
                    textRequestCarInfo,
                    this,
                    res => {
                      showMessages(
                        R.strings().notif_tab_cus,
                        res.message,
                        () => {
                          this.clearRequestCar();
                        }
                      );
                    }
                  );
              }}
              validSubmit={!!textRequestCarInfo}
              title={R.strings().request_add_car}
              contentView={
                <>
                  <Text
                    style={styles.title_reuest_car}
                    children={R.strings().note_request_add_car}
                  />
                  <TextInput
                    placeholderTextColor={colors.gray}
                    multiline
                    placeholder={R.strings().placeholder_request_add_car}
                    onChangeText={textRequestCarInfo =>
                      this.setState({ textRequestCarInfo })
                    }
                    value={textRequestCarInfo}
                    maxLength={256}
                    style={styles.text_input_request_car}
                  />
                </>
              }
            />

            <KeyboardAvoidingView
              keyboardVerticalOffset={80}
              behavior={Platform.OS == "ios" ? "padding" : undefined}
              style={{ flex: 1 }}
            >
              <ScrollView
                showsVerticalScrollIndicator={false}
                keyboardShouldPersistTaps="handled"
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
                  this.state.carBrand,
                  (value, index, data) => {
                    this.setState(
                      {
                        modelLoading: true,
                        keyListCar: {
                          ...this.state.keyListCar,
                          carBrandID: data[index].carBrandID
                        },
                        carBrand: data[index].carBrandName
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
                  this.state.carModel,
                  async (value, index, data) => {
                    if (data[index].name == R.strings().other) {
                      setTimeout(() => {
                        this.setState({
                          isModalVisible: true
                        });
                      }, 500);
                    } else {
                      analytics().logEvent('select_car_mode', {
                        car:data[index].name
                      })
                      this.setState({
                        carModel: data[index].name,
                        payload: {
                          ...this.state.payload,
                          carModelID: data[index].carModelID
                        }
                      });
                    }
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
                )} */}
                {/* {this._renderTextInput(
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
                  data={payload.listImage}
                  editable
                  vertical={false}
                  onAddPress={() => this._checkPickImage()}
                  onDeletePress={this._deleteImage}
                />
                <Button
                  action={this.addCar}
                  title={R.strings().add_car}
                  backgroundColor={theme.colors.primary}
                  colorText={theme.colors.white}
                  width={"100%"}
                  buttonStyle={{ marginTop: 30 }}
                />
              </ScrollView>
              {/* {this._requestAddCar()} */}
            </KeyboardAvoidingView>
          </>
        }
      />
    );
  }

  _renderImage = ({ index, item }) => (
    <Image source={{ uri: item.uri }} style={styles.imgSelected} />
  );

  _checkPickImage() {
    if (this.state.payload.listImage.length == MAX_SIZE_UPLOAD_IMAGE) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().exceeded_number_of_photos
      );
    } else
      return Platform.OS == "android"
        ? this.requestPerAndroid()
        : this._pickerImage();
  }

  _pickerImage = () => {
    ImagePickerHelper(res => {
      var { listImage } = this.state.payload;
      if (!listImage) listImage = [];
      listImage.unshift({ url: res });
      this.setState({
        payload: {
          ...this.state.payload,
          listImage
        }
      });
    });
  };
  _deleteImage = ({ item, index }) => {
    var { listImage } = this.state.payload;
    listImage.splice(index, 1);
    this.setState({
      payload: {
        ...this.state.payload,
        listImage
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

  _requestAddCar = () => {
    return (
      <Modal
        isVisible={this.state.isModalVisible}
        style={{ width: maxWidth, style: 1 }}
      >
        <View style={styles.modalOther}>
          <Button
            action={() => Linking.openURL("tel:0965630621")}
            width={maxWidth * 0.8}
            title={R.strings().call_center}
            borderWidth={0.5}
            borderColor={theme.colors.gray}
            colorText={theme.colors.darkBlue}
          />
          <Button
            action={() => {
              this.toggleModal();
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.FEEDBACK);
            }}
            width={maxWidth * 0.8}
            title={R.strings().additional_vehicle_system_requirements}
            borderWidth={0.5}
            borderColor={theme.colors.gray}
            colorText={theme.colors.darkBlue}
          />
          <Button
            action={this.toggleModal}
            width={maxWidth * 0.8}
            title={R.strings().exit}
            colorText={theme.colors.nameText}
          />
        </View>
      </Modal>
    );
  };
}

const styles = StyleSheet.create({
  text_input_request_car: {
    padding: 10,
    borderRadius: 10,
    borderWidth: 0.5,
    height: 100,
    textAlignVertical: "top",
    fontFamily: R.fonts.quicksand_regular,
    fontSize: 14
  },
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
  },
  modalOther: {
    width: maxWidth * 0.9,
    borderRadius: 5,
    backgroundColor: theme.colors.white,
    alignItems: "center",
    paddingTop: 20,
    paddingBottom: 0
  },
  title_reuest_car: {
    fontFamily: R.fonts.quicksand_medium,
    marginVertical: 10,
    fontSize: 14
  }
});
const mapStateToProps = state => ({
  lang: state.lang,
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfoAction
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AddCarScreen);

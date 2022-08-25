import React, { Component, useState } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  Keyboard,
  StyleSheet,
  Image,
  Dimensions,
  FlatList,
  TextInput,
  PermissionsAndroid,
  Platform,
  ActivityIndicator,
} from "react-native";
import { connect } from "react-redux";
import R from "@R";
import theme, { colors } from "@theme";
import MapView, {
  Marker,
  PROVIDER_GOOGLE,
} from "react-native-maps";
import { Divider } from "react-native-elements";
import {
  TYPE_INFO,
  SCREEN_ROUTER_CUSTOMER,
  ASYNC_STORAGE,
  TYPE_NAVIGATION,
  REDUCER_CUSTOM
} from "@constant";
import Icon from "@component/Icon";
import Button from "@component/Button";
import Modal from "react-native-modal";
import Picker from "react-native-wheel-picker";
import NavigationUtil from "@app/navigation/NavigationUtil";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import ScreenComponent from "@app/components/ScreenComponent";
import AsyncStorage from "@react-native-community/async-storage";
import {
  getUserInfoAction,
  sendLocationSelect,
  clearLocationSelect,
  setState,
  sendBookingNow
} from "@action/index";
import Geolocation from "react-native-geolocation-service";
import reactotron from "reactotron-react-native";
import PhotoListView from "@app/components/PhotoListView";
import ImagePickerHelper from "@app/utils/ImagePickerHelper";
import callAPI from "../../../utils/CallApiHelper";
import { showMessages } from "@app/components/Alert";
import FastImage from "@app/components/FastImage";
import FastImg from "@app/components/FastImage";
import { getCurrentTimeString } from "@app/constants/Functions";
const PickerItem = Picker.Item;
var date = [];
var todayString = "";
var tomorrowString = "";
var time = [];
const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;
const MAX_SIZE_CAR = 5;
const initState = {
  isShowMoreDetail: false,
  shiftHour: 90,
  isLogin: false,
  carID: "",
  isLoading: false,
  error: null,
  date: 0,
  time: 0,
  marker: {
    latitude: 21.002383,
    longitude: 105.795718
  },
  isLoadingNearBy: false,
  isModalVisible: false,
  timeSelect: [],
  address: "",
  code: "",
  value: R.strings().indoor,
  washerInfo: "",
  carInfo: {},
  orderService: {
    additionService: [],
    mainService: null,
    PaymentType: "",
    carID: "",
    isInDoor: 1,
    BookingDateInput: "",
    placeID: "",
    couponCode: "",
    comboID: 0,
    UsePoint: "",
    isBookingNow: 0,
    agentCode: "",
    note: "",
    CustomerAddress: ""
  },
  imagesMoreDetail: "",
  noteMoreDetail: "",
  reset: () => { },
  isFocusScreen: true
};

class TabBookingScreen extends Component {
  constructor(props) {
    super(props);
    const isBookingNow = props.navigation.getParam("isBookingNow", 0);
    this.state = {
      ...initState,
      orderService: {
        ...initState.orderService,
        isBookingNow
      },
      reset: this.reset,
      toggleModal: this.toggleModal,
      isVisibleFullOptions: true
    };
    props.setState(SCREEN_ROUTER_CUSTOMER.BOOKING, this.state);
  }

  reset = () => {
    console.log("reset data");
    const { carID, carInfo } = this.state;
    this.setState(
      {
        ...initState,
        orderService: {
          ...initState.orderService,
          carID
        },
        carInfo
      },
      this.getShiftTime
    );
    this.props.sendBookingNow(0);
  };

  getData = () => {
    this.getShiftTime();
    this.getToken();
  };
  componentDidMount() {
    this.getData();
  }

  getToken = async () => {
    const token = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
    if (token) {
      this.setState(
        {
          isLogin: true
        },
        () => {
          this.props.getUserInfoAction();
          this.gotToMyLocation();
          this.props.clearLocationSelect();
        }
      );
    }
  };

  componentWillMount() {
    if (Platform.OS == "android") this.requestPerAndroid();
    const { navigation } = this.props;
    navigation.addListener("willFocus", async () => {
      await this.setState({
        isFocusScreen: true
      });
    });
    navigation.addListener("willBlur", async () => {
      await this.setState({
        isFocusScreen: false
      });
    });
  }

  requestPerAndroid = async () => {
    const data = await PermissionsAndroid.requestMultiple([
      "android.permission.ACCESS_FINE_LOCATION",
      PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE,
      "android.permission.CAMERA"
    ]);
    const isGrand = Object.keys(data).map(k => data[k] == "granted");
    if (!isGrand.every(Boolean)) this.requestPerAndroid();
  };

  checkArea(place_id, locationName) {
    CallApiHelper(API.checkArea, place_id, null, res => {
      this.setState({
        address: locationName
      });
    });
  }

  gotToMyLocation = () => {
    Geolocation.getCurrentPosition(
      ({ coords }) => {
        if (coords) this.onPressLocationSearch(coords);
      },
      error => { },
      // showMessages(
      //   R.strings().notif_tab_cus,
      //   R.strings().please_turn_on_device_location
      // )
      {
        enableHighAccuracy: true,
        timeout: 15000,
        maximumAge: 0,
        forceRequestLocation: true
      }
    );
  };

  checkAgentByCode() {
    CallApiHelper(
      API.checkAgentByCode,
      this.state.code.trim(),
      this,
      res => {
        this.setState({
          washerInfo: res.result,
          orderService: {
            ...this.state.orderService,
            agentCode: res.result.code
          }
        });
      },
      error => {
        this.setState({
          washerInfo: ""
        });
      }
    );
  }

  onPressLocationSearch = coords => {
    this.setState(
      {
        marker: coords
      },
      () => this.getNearBy(coords)
    );
  };

  getNearBy = async loc => {
    this.setState({
      isLoadingNearBy: true
    });
    var res = await API.nearBySearch(loc);
    var { vicinity, name, place_id } = res.data.results[0];
    var locationName = vicinity.length > name.length ? vicinity : name;
    this.setState(
      {
        isLoadingNearBy: false
      },
      () => this.props.sendLocationSelect(place_id, locationName)
    );
  };

  getShiftTime = openSelectTime => {
    CallApiHelper(API.getShiftTime, null, this, res => {
      this.setState(
        {
          timeSelect: res.result
        },
        () => this.getConfig(openSelectTime)
      );
    });
  };
  getConfig = openSelectTime => {
    
    const excute = () => {
      this.getDate();
      this.getTime();
      this.onDatePickerSelect(0, openSelectTime);
    };

    if (this.props?.setDialogLoading)
      this.props.setDialogLoading(false, excute);
    else excute();
  };

  getDate = () => {
    date = this.state.timeSelect.map(elem => elem.date)
  };
  getTime = () => {
    time = this.state.timeSelect[this.state.date].listShift
  };
  _pickImageMoreDetail = () => {
    ImagePickerHelper(res => {
      var { imagesMoreDetail } = this.state;
      if (!imagesMoreDetail) imagesMoreDetail = [];
      imagesMoreDetail.unshift({ url: res });
      this.setState({ imagesMoreDetail });
    });
  };
  _deleteImageMoreDetail = ({ item, index }) => {
    var { imagesMoreDetail } = this.state;
    imagesMoreDetail.splice(index, 1);
    this.setState({ imagesMoreDetail });
  };
  _MoreDetail = () => {
    return (
      <TouchableOpacity
        style={styles.root_more_detail}
        onPress={() => {
          this.setState({
            isShowMoreDetail: !this.state.isShowMoreDetail
          });
        }}
        children={
          <>
            <Text
              children={R.strings().more_detail}
              style={styles.text_more_detail}
            />
            <Icon.Ionicons
              style={styles.ic_arrow}
              name={
                this.state.isShowMoreDetail
                  ? "ios-arrow-down"
                  : "ios-arrow-forward"
              }
              size={20}
              color={theme.colors.grayBorder}
            />
          </>
        }
      />
    );
  };
  onTimePickerSelect = index => {
    this.setState({
      time: index,
      orderService: {
        ...this.state.orderService,
        BookingDateInput: time[index] + " " + date[this.state.date]
      }
    });
  };

  onDatePickerSelect = (index, openSelectTime) => {
    time = this.state.timeSelect[index].listShift
    this.setState(
      {
        date: index,
        orderService: {
          ...this.state.orderService,
          BookingDateInput: time[this.state.time] + " " + date[index]
        }
      },
      () => {
        // if (index == 0) this.getTime();
        // else if (index * prevIndex == 0) this.getTime();
        if (openSelectTime)
          this.setState({
            isModalVisible: true
          });
      }
    );
  };

  toggleModal = () => {
    if (!this.state.isModalVisible) {
      this.getShiftTime(true);
      return;
    }
    this.setState({
      isModalVisible: !this.state.isModalVisible
    });
  };

  render() {
    this.props.setState(SCREEN_ROUTER_CUSTOMER.BOOKING, this.state);
    const {
      isDialogLoading,
      isLoading,
      error,
      imagesMoreDetail,
      isFocusScreen,
      isVisibleFullOptions,
      orderService
    } = this.state;
    const { UserInfoState } = this.props;
    const listCar = UserInfoState?.data?.listCar || [];
    if (orderService.carID == "" && listCar.length > 0) {
      const { length } = listCar;
      if (length > 0) {
        this.onSelectCar({
          item: listCar[0],
          index: 0
        });
      }
    }
    const isBookingNow =
      this.props.typeBookingNow.type == TYPE_NAVIGATION.BOOKING_NOW;
    const coordinate = {
      latitude:
        this.props.selectLocation.lat != "" &&
          this.props.selectLocation.lat != null
          ? this.props.selectLocation.lat
          : this.state.marker.latitude,
      longitude:
        this.props.selectLocation.lon != "" &&
          this.props.selectLocation.lon != null
          ? this.props.selectLocation.lon
          : this.state.marker.longitude
    };
    return (
      <ScreenComponent
        dialogLoading={isDialogLoading || isLoading || UserInfoState.isLoading}
        isError={error || UserInfoState.error}
        reload={this.getData}
        isLogin={false}
        renderView={
          <View style={{ flex: 1, backgroundColor: theme.colors.defaultBg }}>
            {isFocusScreen && (
              <MapView
                ref={ref => {
                  this.map = ref;
                }}
                onTouchStart={Keyboard.dismiss}
                showsUserLocation
                provider={PROVIDER_GOOGLE} // remove if not using Google Maps
                style={[
                  styles.map,
                  {
                    marginTop:
                      this.props.typeBookingNow.type ==
                        TYPE_NAVIGATION.BOOKING_NOW
                        ? -windowHeight / 2
                        : -windowHeight / 3
                  }
                ]}
                region={{
                  ...coordinate,
                  latitudeDelta: 0.01,
                  longitudeDelta: 0.01
                }}
              >
                <Marker coordinate={coordinate} />
              </MapView>
            )}

            <View style={styles.bookInfo}>
              <TouchableOpacity
                style={{
                  alignSelf: "flex-end",
                  marginHorizontal: 15,
                  width: "90%"
                }}
                onPress={() =>
                  this.setState({
                    isVisibleFullOptions: !this.state.isVisibleFullOptions
                  })
                }
                children={
                  <Icon.Ionicons
                    name={
                      isVisibleFullOptions ? "ios-arrow-down" : "ios-arrow-up"
                    }
                    size={25}
                    style={{
                      alignSelf: "flex-end"
                    }}
                    color={theme.colors.black}
                  />
                }
              />
              {isVisibleFullOptions && (
                <View
                  style={{
                    flexDirection: "row",
                    width: "90%",
                    justifyContent: "space-between",
                    alignItems: "center",
                    marginTop: -5,
                    marginBottom: -10
                  }}
                  children={
                    <>
                      <Text
                        children={R.strings().assign_washer}
                        style={{
                          fontFamily: R.fonts.quicksand_medium,
                          fontSize: 13,
                          textAlignVertical: "center",
                          alignSelf: "center"
                        }}
                      />
                      <TouchableOpacity
                        style={{ alignSelf: "center" }}
                        onPress={() => {
                          const { orderService } = this.state;
                          this.setState(
                            {
                              orderService: {
                                ...orderService,
                                isBookingNow: isBookingNow ? 0 : 1,
                                agentCode: "",
                                BookingDateInput: !isBookingNow
                                  ? getCurrentTimeString()
                                  : time[this.state.time] +
                                  " " +
                                  date[this.state.date]
                              },
                              code: ""
                            },
                            () => {
                              this.props.sendBookingNow(
                                isBookingNow ? 0 : TYPE_NAVIGATION.BOOKING_NOW
                              );
                            }
                          );
                        }}
                        children={
                          <Image
                            source={R.images[isBookingNow ? "ic_on" : "ic_off"]}
                            style={{
                              width: width / 8
                            }}
                            resizeMode="contain"
                          />
                        }
                      />
                    </>
                  }
                />
              )}

              {!!isBookingNow && isVisibleFullOptions && this._assignWasher()}
              {this._selectItem(
                () =>
                  NavigationUtil.navigate(
                    SCREEN_ROUTER_CUSTOMER.UPDATE_USER_INFO,
                    { item: UserInfoState.data }
                  ),
                1,
                TYPE_INFO.NAME_PHONE,
                <FastImg
                  source={
                    UserInfoState?.data?.urlAvatar
                      ? { uri: UserInfoState?.data?.urlAvatar }
                      : R.images.avatarDemo
                  }
                  style={styles.ic_avatar}
                />,
                UserInfoState?.data?.name,
                UserInfoState?.data?.phone
              )}
              {isVisibleFullOptions && (
                <>
                  {this._selectItem(
                    () =>
                      NavigationUtil.navigate(
                        SCREEN_ROUTER_CUSTOMER.SELECT_LOCATION
                      ),
                    1,
                    TYPE_INFO.ADDRESS,
                    <Icon.Entypo
                      name="location-pin"
                      size={30}
                      color={theme.colors.primary}
                    />,
                    this.props.selectLocation.name != ""
                      ? this.props.selectLocation.name
                      : this.state.address != ""
                        ? this.state.address
                        : R.strings().tap_for_select_position
                  )}
                  {!isBookingNow &&
                    this._selectItem(
                      this.toggleModal,
                      1,
                      TYPE_INFO.TIME,
                      <Icon.Ionicons
                        name="md-time"
                        size={25}
                        color={theme.colors.primary}
                        style={{ marginHorizontal: 5 }}
                      />,
                      time[this.state.time] && date[this.state.date]
                        ? time[this.state.time] +
                        ", " +
                        (date[this.state.date] == todayString
                          ? R.strings().today
                          : date[this.state.date] == tomorrowString
                            ? R.strings().tomorrow
                            : date[this.state.date])
                        : ""
                    )}
                  <View
                    style={{
                      width: "90%",
                      marginVertical: 7,
                      flexDirection: "row"
                    }}
                  >
                    {this._RadioButton()}
                    {this._MoreDetail()}
                  </View>
                  {this.state.isShowMoreDetail && (
                    <>
                      <TextInput
                        style={{
                          borderWidth: 0.5,
                          borderRadius: 5,
                          textAlignVertical: "top",
                          width: "90%",
                          padding: 10,
                          height: height / 12,
                          maxHeight: height / 12,
                          color: "black"
                        }}
                        value={this.state.noteMoreDetail}
                        onChangeText={noteMoreDetail =>
                          this.setState({ noteMoreDetail })
                        }
                        placeholderTextColor={colors.gray}
                        placeholder={R.strings().infomation_about_park}
                        multiline
                        maxLength={256}
                      // numberOfLines={5}
                      />
                      <Text
                        children={R.strings().add_park_image}
                        style={{
                          fontFamily: R.fonts.quicksand_medium,
                          fontSize: 10,
                          color: colors.gray,
                          marginTop: 5
                        }}
                      />
                      <PhotoListView
                        style={{
                          width: width / 4,
                          height: width / 4
                        }}
                        styleAddImage={{
                          margin: 10
                        }}
                        styleImage={{ marginTop: 10 }}
                        data={imagesMoreDetail}
                        editable
                        vertical={false}
                        onAddPress={this._pickImageMoreDetail}
                        onDeletePress={this._deleteImageMoreDetail}
                      />
                    </>
                  )}
                  <View
                    style={[styles.selectCarView, { flexDirection: "column" }]}
                  >
                    <View style={{ flexDirection: "row" }}>
                      {this._selectImg()}
                      <FlatList
                        showsHorizontalScrollIndicator={false}
                        horizontal
                        data={UserInfoState?.data?.listCar || []}
                        keyExtractor={(item, index) => index.toString()}
                        renderItem={this._carItem}
                      />
                    </View>
                    {!listCar.length && (
                      <TouchableOpacity onPress={this.navigateAddCar}>
                        <Text
                          style={{
                            fontSize: 13,
                            fontFamily: R.fonts.quicksand_medium,
                            fontStyle: "italic",
                            marginTop: 6,
                            color: colors.primaryDark
                          }}
                          children={R.strings().require_add_car}
                        />
                      </TouchableOpacity>
                    )}
                  </View>
                </>
              )}
              {this._DateTimePicker()}
              <Button
                action={() => {
                  if (this.props.UserInfoState.data.listCar.length == 0) {
                    showMessages(
                      R.strings().notif_tab_cus,
                      R.strings().require_add_car,
                      this.navigateAddCar
                    );
                    return;
                  }
                  let param = () => ({
                    orderService: this.state.orderService,
                    carInfo: this.state.carInfo,
                    itemService: this.state.itemService
                  });
                  let navigate = () => {
                    CallApiHelper(
                      API.checkCreateOrder,
                      this.state.orderService,
                      this,
                      res => {
                        NavigationUtil.navigate(
                          SCREEN_ROUTER_CUSTOMER.SELECT_PACKAGE,
                          param()
                        );
                      },
                      null,
                      null,
                      "isDialogLoading"
                    );
                  };

                  this.setState(
                    {
                      orderService: {
                        ...this.state.orderService,
                        placeID: this.props.selectLocation.location
                          ? this.props.selectLocation.location
                          : this.state.orderService.placeID,
                        UsePoint: UserInfoState.data?.point,
                        CustomerAddress: this.props.selectLocation.name
                      }
                    },
                    navigate
                  );
                }}
                width={"90%"}
                title={R.strings().next}
                backgroundColor={theme.colors.primary}
                colorText={theme.colors.white}
              />
            </View>
          </View>
        }
      />
    );
  }

  _assignWasher = () => {
    return (
      <View
        style={{
          // width: "90%",
          paddingTop: 10,
          marginHorizontal: 3
        }}
      >
        <View
          style={{
            flexDirection: "row",
            justifyContent: "space-between",
            alignItems: "center",
            marginVertical: 5
          }}
        >
          <TextInput
            maxLength={30}
            placeholderTextColor={colors.gray}
            placeholder={R.strings().input_washer_code}
            style={styles.inputWasherCode}
            value={this.state.code}
            onChangeText={text => {
              this.setState({
                code: text
              });
              if (this.AgentCodeTimeOut) clearTimeout(this.AgentCodeTimeOut);
              this.AgentCodeTimeOut = setTimeout(() => {
                if (text.trim() != "") this.checkAgentByCode();
              }, 1000);
            }}
          />
          <TouchableOpacity
            onPress={() =>
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.QR, this)
            }
            children={
              !this.state.isLoading ? (
                <FastImage
                  source={R.images.ic_qr_code}
                  style={{ height: 32, width: 32 }}
                />
              ) : (
                  <ActivityIndicator size="large" />
                )
            }
          />
        </View>
        {this.state.code == "" ? (
          <View />
        ) : this.state.washerInfo == "" ? (
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 10,
              color: theme.colors.red
            }}
          >
            {R.strings().not_found_washer}
          </Text>
        ) : (
              this._washerInfo(
                () => { },
                1,
                <FastImage
                  source={
                    this.state.washerInfo.url
                      ? { uri: this.state.washerInfo.url }
                      : R.images.avatarDemo
                  }
                  style={styles.ic_avatar}
                />,
                this.state.washerInfo.name,
                this.state.washerInfo.code
              )
            )}
        <View
          style={{
            height: 0.5,
            backgroundColor: theme.colors.grayBorder
          }}
        />
      </View>
    );
  };

  _selectItem(action, marginTop, type, image, title, phoneNumber) {
    return (
      <>
        <TouchableOpacity
          onPress={action}
          style={[styles.selectItem, { marginTop: marginTop }]}
        >
          <View style={{ flexDirection: "row" }}>
            {image}
            <View
              style={{
                marginLeft: 11,
                justifyContent: "space-around",
                marginVertical: 5
              }}
            >
              <Text
                style={{
                  width: 250,
                  fontFamily: R.fonts.quicksand_bold,
                  fontSize: 13,
                  color:
                    type == TYPE_INFO.NAME_PHONE
                      ? theme.colors.nameText
                      : "black",
                  alignSelf: "center"
                }}
              >
                {title || R.strings().not_update_yet}
              </Text>
              {phoneNumber && (
                <Text
                  style={{
                    fontFamily: R.fonts.quicksand_bold,
                    fontSize: 13,
                    color:
                      type == TYPE_INFO.NAME_PHONE
                        ? theme.colors.nameText
                        : "black"
                  }}
                >
                  {phoneNumber}
                </Text>
              )}
            </View>
          </View>
          <Icon.Ionicons
            name="ios-arrow-forward"
            size={20}
            color={theme.colors.grayBorder}
          />
        </TouchableOpacity>
        <View
          style={{
            width: "90%",
            height: 0.5,
            backgroundColor: theme.colors.grayBorder
          }}
        />
      </>
    );
  }
  _washerInfo(action, marginTop, image, title, code) {
    return (
      <>
        <TouchableOpacity
          onPress={action}
          style={[styles.selectItem, { marginTop: marginTop }]}
        >
          <View style={{ flexDirection: "row" }}>
            {image}
            <View
              style={{
                marginLeft: 11,
                justifyContent: "space-around",
                marginVertical: 5
              }}
            >
              <Text
                style={{
                  width: 250,
                  fontFamily: R.fonts.quicksand_bold,
                  fontSize: 13,
                  color: theme.colors.nameText
                }}
              >
                {title}
              </Text>
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_bold,
                  fontSize: 13,
                  color: theme.colors.nameText
                }}
              >
                {code}
              </Text>
            </View>
          </View>
        </TouchableOpacity>
        <View
          style={{
            width: "100%",
            height: 0.5,
            backgroundColor: theme.colors.grayBorder
          }}
        />
      </>
    );
  }

  addCarSuccess = carID => {
    this.setState({
      ...this.state,
      orderService: { ...this.state.orderService, carID }
    });
  };

  navigateAddCar = () => {
    const { data } = this.props.UserInfoState;
    if (data?.listCar?.length == MAX_SIZE_CAR) {
      showMessages(
        R.strings().notif_tab_cus,
        `${R.strings().limit_car} ${MAX_SIZE_CAR}`
      );
      return;
    }
    NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.ADD_CAR, {
      onGoBack: this.addCarSuccess
    });
  };

  _selectImg = () => {
    return (
      <TouchableOpacity onPress={this.navigateAddCar} style={styles.selectImg}>
        <Icon.AntDesign name="plus" size={19} color={theme.colors.grayPlus} />
      </TouchableOpacity>
    );
  };

  onSelectCar = ({ item, index }) => {
    this.setState({
      carInfo: item,
      orderService: {
        ...this.state.orderService,
        carID: item.carID
      }
    });
  };

  _carItem = ({ item, index }) => {
    const { carID } = this.state.orderService;
    const img =
      item?.listImage.length > 0
        ? { uri: item.listImage[0].url }
        : R.images.carDemo;
    return (
      <TouchableOpacity
        onPress={() => this.onSelectCar({ item, index })}
        style={[
          styles.selectCar,
          {
            backgroundColor:
              item.carID == carID ? theme.colors.primary : theme.colors.white
          }
        ]}
      >
        <FastImage
          source={img}
          style={{ width: 31, height: 31 }}
          resizeMode="contain"
        />
        <View style={{ paddingHorizontal: 5 }}>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 13,
              color:
                item.carID == carID ? theme.colors.white : theme.colors.black
            }}
          >
            {item.carBrand}
          </Text>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 11,
              color:
                item.carID == carID ? theme.colors.white : theme.colors.black
            }}
          >
            {item.licensePlates}
          </Text>
        </View>
      </TouchableOpacity>
    );
  };

  reOrder = () => {
    this.props.setState(SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER, {
      orderService: {
        ...this.props.orderHistory?.orderService,
        BookingDateInput: this.state.orderService.BookingDateInput,
        placeID: this.props.selectLocation.location
          ? this.props.selectLocation.location
          : this.state.orderService.placeID,
        CustomerAddress: this.props.selectLocation.name
      }
    });
    const { carID } = this.props.orderHistory?.orderService;
    const checkCreateOrderPayload = {
      carID,
      BookingDateInput: this.state.orderService.BookingDateInput,
      isBookingNow: 0,
      agentCode: "",
      couponCode: "",
      placeID: this.props.selectLocation.location
        ? this.props.selectLocation.location
        : this.state.orderService.placeID,
      CustomerAddress: this.props.selectLocation.name
    };
    callAPI(
      API.checkCreateOrder,
      checkCreateOrderPayload,
      null,
      res => {
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.SELECT_PACKAGE);
      },
      err =>
        this.props.setState(SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER, {
          reOrder: false
        })
    );
  };
  _DateTimePicker() {
    return (
      <View style={{ flex: 1, width: windowWidth }}>
        <Modal
          isVisible={this.state.isModalVisible}
          style={{ width: windowWidth }}
        >
          <View style={styles.dateTimePickerView}>
            <View style={styles.dateTimePickerTitle}>
              <TouchableOpacity
                onPress={() => {
                  this.props.setState(SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER, {
                    reOrder: false
                  });
                  this.toggleModal();
                }}
              >
                <Text
                  style={{
                    fontFamily: R.fonts.quicksand_medium,
                    fontSize: 16
                  }}
                >
                  {R.strings().cancel}
                </Text>
              </TouchableOpacity>
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_medium,
                  fontSize: 16
                }}
              >
                {R.strings().select_time_to_use}
              </Text>

              <TouchableOpacity
                onPress={() => {
                  this.toggleModal();
                  if (this.props?.changeTimeBooking)
                    this.props.changeTimeBooking();
                  if (this.props?.changeTimeSelectPackage)
                    this.props.changeTimeSelectPackage();
                  if (this.props.orderHistory?.reOrder) {
                    NavigationUtil.navigate(
                      SCREEN_ROUTER_CUSTOMER.SELECT_LOCATION,
                      this.reOrder
                    );
                  }
                }}
              >
                <Text
                  style={{
                    fontFamily: R.fonts.quicksand_medium,
                    fontSize: 16,
                    color: theme.colors.primary
                  }}
                >
                  {R.strings().done}
                </Text>
              </TouchableOpacity>
            </View>
            <View
              style={{
                position: "absolute",
                width: "100%",
                marginTop: 50
              }}
            >
              <Divider
                style={styles.divider}
              />
              <View
                style={{
                  flexDirection: "row",
                  marginTop: 20
                }}
              >
                {this._WheelPicker(R.strings().date, date, this.state.date)}
                <Divider
                  style={{
                    backgroundColor: theme.colors.grayBorder,
                    height: windowHeight / 3,
                    width: 0.5
                  }}
                />
                {this._WheelPicker(R.strings().time, time, this.state.time)}
              </View>
            </View>
          </View>
        </Modal>
      </View>
    );
  }
  _WheelPicker = (title, data, selectedItem) => {
    return (
      <View
        style={{
          width: "50%",
          alignItems: "center",
          height: "100%"
        }}
      >
        <Text style={{ fontFamily: R.fonts.quicksand_bold, fontSize: 15 }}>
          {title}
        </Text>
        <Picker
          style={{
            width: "100%",
            height: "100%"
          }}
          selectedValue={selectedItem}
          itemStyle={{ color: theme.colors.black, fontSize: 20 }}
          onValueChange={index => {
            title == R.strings().date
              ?
              this.onDatePickerSelect(index)
              : this.onTimePickerSelect(index);
          }}
        
        >
          {data.map((value, i) => {
            if (value == todayString) value = R.strings().today;
            if (value == tomorrowString) value = R.strings().tomorrow;
            return <PickerItem label={value} value={i} key={"money" + value} />;
          })}
        </Picker>
      </View>
    );
  };
  _RadioButton() {
    const { isInDoor } = this.state.orderService;
    const options = {
      indoor: {
        key: 1,
        text: R.strings().indoor
      },
      outdoor: {
        key: 0,
        text: R.strings().outdoor
      }
    };
    return (
      <View style={styles.container}>
        {Object.values(options).map(item => {
          const { indoor, outdoor } = options;
          return (
            <View key={[item.key]} style={styles.buttonContainer}>
              <TouchableOpacity
                style={{ flexDirection: "row", alignItems: "center", flex: 1 }}
                onPress={() => {
                  if (item.key == indoor.key) {
                    this.setState({
                      value: indoor.text,
                      orderService: {
                        ...this.state.orderService,
                        isInDoor: 1
                      }
                    });
                  } else if (item.key == outdoor.key) {
                    this.setState({
                      value: outdoor.text,
                      orderService: {
                        ...this.state.orderService,
                        isInDoor: 0
                      }
                    });
                  }
                }}
              >
                <View style={styles.circle}>
                  {isInDoor === item.key && (
                    <View style={styles.checkedCircle} />
                  )}
                </View>
                <Text
                  style={{
                    fontFamily: R.fonts.quicksand_bold,
                    fontSize: 13,
                    textAlign: "center",
                    marginLeft: 10
                  }}
                >
                  {item.text}
                </Text>
              </TouchableOpacity>
            </View>
          );
        })}
      </View>
    );
  }
}

const styles = StyleSheet.create({
  divider: {
    backgroundColor: theme.colors.grayBorder,
    height: 0.5,
    width: "100%"
  },
  map: {
    ...StyleSheet.absoluteFillObject,
    flex: 1
  },
  bookInfo: {
    backgroundColor: theme.colors.white,
    marginHorizontal: 10,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 4
    },
    shadowOpacity: 0.3,
    shadowRadius: 4.65,
    // paddingBottom: 10,
    elevation: 8,
    borderRadius: 10,
    position: "absolute",
    bottom: 30,
    alignItems: "center",
    alignSelf: "center",
    width: "95%",
    paddingTop: 10
  },
  selectItem: {
    flexDirection: "row",
    width: "90%",
    alignItems: "center",
    alignContent: "center",
    justifyContent: "space-between",
    paddingVertical: Platform.OS == "ios" ? 10 : 7
  },
  selectCarView: {
    width: "90%",
    paddingVertical: 6,
    paddingHorizontal: 5,
    flexDirection: "row",
    borderTopColor: theme.colors.grayBorder,
    borderTopWidth: 0.5,
    borderBottomColor: theme.colors.grayBorder,
    borderBottomWidth: 0.5
  },
  selectImg: {
    padding: 10,
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: theme.colors.backgroundGray,
    borderRadius: 5,
    borderWidth: 0.5,
    borderColor: theme.colors.grayBorder,
    marginRight: 12
  },
  selectCar: {
    paddingHorizontal: 6,
    paddingVertical: 4,
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    borderRadius: 5,
    borderWidth: 0.5,
    borderColor: theme.colors.grayBorder,
    marginRight: 12
  },
  container: {
    flexDirection: "row",
    justifyContent: "flex-start",
    alignItems: "center",
    paddingTop: 12,
    marginBottom: 12,
    marginLeft: 6,
    flex: 8
  },
  buttonContainer: {
    flexDirection: "row",
    justifyContent: "space-around",
    alignItems: "center",
    // marginBottom: 30,
    marginRight: 30,
    flex: 4
  },
  root_more_detail: {
    alignSelf: "center",
    flexDirection: "row",
    flex: 4
  },
  circle: {
    height: 17,
    width: 17,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: theme.colors.gray,
    alignItems: "center",
    justifyContent: "center"
  },

  checkedCircle: {
    width: 11,
    height: 11,
    borderRadius: 10,
    backgroundColor: theme.colors.primary
  },
  dateTimePickerView: {
    backgroundColor: theme.colors.white,
    width: windowWidth,
    height: windowHeight / 2,
    marginTop: windowHeight / 1.5,
    marginLeft: -20,
    borderRadius: 10
  },
  dateTimePickerTitle: {
    flexDirection: "row",
    justifyContent: "space-between",
    position: "absolute",
    width: "100%",
    paddingHorizontal: 10,
    paddingVertical: 15
  },
  inputWasherCode: {
    paddingHorizontal: 10,
    paddingVertical: Platform.OS == "ios" ? 11 : 5,
    backgroundColor: theme.colors.backgroundInput,
    borderColor: theme.colors.gray,
    borderWidth: 0.25,
    width: "85%",
    borderRadius: 5,
    color: "black"
  },

  text_more_detail: {
    textAlign: "right",
    textAlignVertical: "center",
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14
  },
  ic_arrow: {
    flex: 1,
    textAlign: "right"
  },
  toggleBooking: {
    padding: 10,
    backgroundColor: colors.orange,
    color: "white",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16,
    width: width * 0.9,
    borderRadius: 10,
    marginVertical: 10,
    textAlign: "center",
    overflow: "hidden"
  },
  ic_avatar: {
    width: 40,
    height: 40,
    borderRadius: 40,
    overflow: "hidden",
    borderWidth: 0.5,
    borderColor: theme.colors.borderTopColor
  }
});

const mapStateToProps = state => ({
  lang: state.lang,
  UserInfoState: state.userReducer,
  selectLocation: state.selectLocation,
  typeBookingNow: state.bookingNowReducer,
  orderHistory:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER],
  changeTimeSelectPackage:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.SELECT_PACKAGE]
      .changeTime,
  changeTimeBooking:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE]
      .changeTime,
  setDialogLoading:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE]
      .setDialogLoading
});

const mapDispatchToProps = {
  getUserInfoAction,
  sendLocationSelect,
  clearLocationSelect,
  setState,
  sendBookingNow
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TabBookingScreen);

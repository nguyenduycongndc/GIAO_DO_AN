import React, { Component } from "react";
import {
  Text,
  TouchableOpacity,
  Image,
  StyleSheet,
  View,
  ScrollView,
  RefreshControl,
  Platform,
  PermissionsAndroid,
  Dimensions
} from "react-native";
import { connect } from "react-redux";
import R from "@R";
import {
  SCREEN_ROUTER_CUSTOMER,
  SCREEN_ROUTER_AUTH,
  ASYNC_STORAGE
} from "@constant";
import ScreenComponent from "@app/components/ScreenComponent";
import NavigationUtil from "@app/navigation/NavigationUtil";
import Card from "@app/components/Card";
import theme, { colors } from "@app/constants/Theme";
import RateStar from "@app/components/RateStar";
import NumberFormat from "react-number-format";
import { getUserInfoAction } from "@action/";
import AsyncStorage from "@react-native-community/async-storage";
// import { Avatar } from "react-native-elements";
import ImagePicker from "react-native-image-picker";
import * as API from "@app/constants/Api";
import CallApiHelper from "@app/utils/CallApiHelper";
import reactotron from "reactotron-react-native";
import { numberWithCommas } from "@app/constants/Functions";
import Button from "@app/components/Button";
import { showMessages, showConfirm } from "@app/components/Alert";
import store from "@app/redux/store";
import ImagePickerHelper from "@app/utils/ImagePickerHelper";
import FastImage, { Avatar } from "@app/components/FastImage";
import FastImg from "@app/components/FastImage";

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

class TabAccountScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      token: "",
      isLoading: false,
      isLoadingToken: false,
      isLogin: false,
      error: ""
    };
  }

  componentWillMount() {
    this.getToken();
  }

  requestLogout() {
    CallApiHelper(API.requestLogout, "", this, null, null, async () =>
      AsyncStorage.removeItem(ASYNC_STORAGE.TOKEN, () => {
        NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH_LOADING);
        store.dispatch({ type: "reset" });
      })
    );
  }

  refreshData = () => {
    this.getToken();
    this.getData();
  };

  getData = () => this.props.getUserInfoAction();
  getToken = async () => {
    const token = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
    this.setState({
      isLogin: !!token
    });
  };

  onPressQRCode = () => {};
  renderBalance = (label, money) => (
    <View
      style={styles.root_balance}
      children={
        <>
          <Text style={styles.label_balance} children={label} />
          <View
            style={styles.root_money}
            children={
              <NumberFormat
                value={money}
                displayType="text"
                thousandSeparator
                suffix="đ"
                renderText={value => (
                  <Text style={styles.text_money} children={value} />
                )}
              />
            }
          />
        </>
      }
    />
  );

  _checkPermission = async () => {
    var check = true;
    if (Platform.OS == "android")
      check = await PermissionsAndroid.check(
        PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE
      );
    if (!check) {
      try {
        granted = await PermissionsAndroid.request(
          PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE
        );
        this._pickImage();
      } catch (error) {}
    } else {
      this._pickImage();
    }
  };

  refreshControl = () => {
    const { UserInfoState } = this.props;
    return (
      <RefreshControl
        refreshing={UserInfoState.isLoading}
        onRefresh={this.refreshData}
      />
    );
  };
  renderActionBalance = (label, icon, screen) => (
    <TouchableOpacity
      onPress={() => NavigationUtil.navigate(screen)}
      style={{ flex: 1 }}
      children={
        <>
          <FastImage
            source={icon}
            style={styles.ic_img_action}
            resizeMode="contain"
          />
          <Text style={styles.text_action_balance} children={label} />
        </>
      }
    />
  );
  renderOption = (label, icon, action) => (
    <TouchableOpacity
      onPress={action}
      style={styles.root_option}
      children={
        <>
          <Image style={styles.ic_option} source={icon} />
          <Text style={styles.text_option} children={label} />
          <Image source={R.images.ic_right_arrow} style={styles.ic_right_arr} />
        </>
      }
    />
  );
  _pickImage = async () => {
    const options = {};

    ImagePickerHelper(res => {
      const data = new FormData();
      data.append("image", {
        name: `imageAvatar`,
        type: `image/jpg`,
        uri: res
        // Platform.OS === "android"
        //   ? response.uri
        //   : response.uri.replace("file://", "")
      });
      // data.append("type", "image");
      this._updateAvatar(data);
    });
  };
  _updateAvatar = payload => {
    CallApiHelper(API.uploadAvatar, payload, this, this.getData);
  };
  render() {
    const { numberStar, isLoading, isLoadingToken } = this.state;
    const { UserInfoState } = this.props;

    return (
      <ScreenComponent
        isLoading={UserInfoState.isLoading || isLoadingToken}
        dialogLoading={isLoading}
        isSafeAre={false}
        // isError={UserInfoState.error}
        renderView={
          <>
            <ScrollView
              showsVerticalScrollIndicator={false}
              contentContainerStyle={{
                flexGrow: 1,
                paddingBottom: 30,
                backgroundColor: theme.colors.backgroundColor
              }}
              refreshControl={this.refreshControl()}
              children={
                <>
                  <Image source={R.images.bg_car_rect} style={styles.img_bg} />
                  <Card
                    style={styles.root_card}
                    children={
                      this.state.isLogin ? (
                        <>
                          <Avatar
                            source={
                              !UserInfoState.data?.urlAvatar
                                ? R.images.avatarDemo
                                : { uri: UserInfoState.data?.urlAvatar }
                            }
                            style={styles.avatar}
                            onPress={() =>
                              Platform.OS != "ios"
                                ? this._checkPermission()
                                : this._pickImage()
                            }
                          />
                          <View
                            style={styles.root_detail}
                            children={
                              <>
                                <Text
                                  style={styles.text_name}
                                  children={
                                    UserInfoState.data?.name ||
                                    R.strings().not_update_yet
                                  }
                                />
                                <View
                                  style={styles.root_rarting}
                                  children={
                                    <>
                                      <Text
                                        children={
                                          R.strings().accumulated_point +
                                          ": " +
                                          `${numberWithCommas(
                                            UserInfoState.data?.point?.toString() ||
                                              "0"
                                          )} ${R.strings().point}`
                                        }
                                        style={styles.text_rating}
                                      />
                                      <TouchableOpacity
                                        onPress={() =>
                                          NavigationUtil.navigate(
                                            SCREEN_ROUTER_CUSTOMER.RANK_ACOUNT_INFO
                                          )
                                        }
                                        style={{
                                          flex: 1,
                                          flexDirection: "row",
                                          justifyContent: "flex-end"
                                        }}
                                        children={
                                          <>
                                            <FastImg
                                              source={R.images.ic_award}
                                              style={{
                                                width: 15,
                                                height: 15,
                                                marginEnd: 5
                                              }}
                                            />
                                            <Text
                                              style={{
                                                fontFamily:
                                                  R.fonts.quicksand_medium,
                                                color: "#EDD30C"
                                              }}
                                              children="VIP"
                                            />
                                            <FastImg
                                              source={R.images.ic_info_fill}
                                              style={{
                                                width: 15,
                                                height: 15,
                                                marginStart: 5
                                              }}
                                            />
                                          </>
                                        }
                                      />
                                    </>
                                  }
                                />
                              </>
                            }
                          />
                        </>
                      ) : (
                        <>
                          <Avatar
                            // rounded
                            // imageProps={{ defaultSource: R.images.ic_symbol }}
                            source={require("../../../assets/images/avatarBg.png")}
                            style={styles.avatar}
                            // onPress={() => this._pickImage()}
                          />
                          <View
                            style={styles.root_detail}
                            children={
                              <>
                                <Text
                                  style={[styles.text_name, { fontSize: 11 }]}
                                  children={
                                    R.strings().sign_in_for_the_best_experience
                                  }
                                />
                                <View
                                  style={styles.root_rarting}
                                  children={
                                    <>
                                      <Button
                                        action={() =>
                                          NavigationUtil.navigate(
                                            SCREEN_ROUTER_AUTH.LOGIN
                                          )
                                        }
                                        title={R.strings().sign_in}
                                        width="65%"
                                        colorText={theme.colors.white}
                                        backgroundColor={theme.colors.primary}
                                        uppercase
                                        buttonStyle={{
                                          marginVertical: 0,
                                          paddingVertical: 9
                                        }}
                                      />
                                    </>
                                  }
                                />
                              </>
                            }
                          />
                        </>
                      )
                    }
                  />
                  {this.state.isLogin ? (
                    <View
                      style={{
                        backgroundColor: theme.colors.backgroundColor,
                        flex: 1
                      }}
                    >
                      {this.renderOption(
                        R.strings().user_info,
                        R.images.ic_user_option,
                        () => {
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.USER_INFO
                          );
                        }
                      )}
                      {this.renderOption(
                        R.strings().location_info,
                        R.images.ic_location,
                        () => {
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.SAVE_LOCATION
                          );
                        }
                      )}
                      {this.renderOption(
                        R.strings().car_info,
                        R.images.ic_car_info,
                        () => {
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.YOUR_CAR,
                            { listCar: UserInfoState.data?.listCar }
                          );
                        }
                      )}
                      {this.renderOption(
                        R.strings().point_history,
                        R.images.ic_point_history,
                        () => {
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.HISTORY_POINT
                          );
                        }
                      )}
                      {this.renderOption(
                        R.strings().contact_operator,
                        R.images.ic_calling,
                        () =>
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.CONTACT
                          )
                      )}
                      {this.renderOption(
                        R.strings().language,
                        R.images.ic_language,
                        () => {
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.CHANGE_LANGUAGE
                          );
                        }
                      )}
                      {this.renderOption(
                        R.strings().change_password,
                        R.images.ic_lock,
                        () =>
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.CHANGE_PASSWORD
                          )
                      )}
                      {/* {this.renderOption("Q&A", R.images.ic_q_and_a, () =>
                        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.Q_A)
                      )} */}
                      {this.renderOption(
                        R.strings().logout,
                        R.images.ic_logout,
                        () => {
                          showConfirm(
                            R.strings().notif_tab_cus,
                            R.strings().confirm_logout,
                            () => this.requestLogout()
                          );
                        }
                      )}
                    </View>
                  ) : (
                    <View
                      style={{
                        backgroundColor: theme.colors.backgroundColor,
                        flex: 1,
                        paddingBottom: 50
                      }}
                    >
                      {this.renderOption(
                        R.strings().contact_operator,
                        R.images.ic_calling,
                        () =>
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.CONTACT
                          )
                      )}
                      {this.renderOption(
                        R.strings().language,
                        R.images.ic_language,
                        () => {
                          NavigationUtil.navigate(
                            SCREEN_ROUTER_CUSTOMER.CHANGE_LANGUAGE
                          );
                        }
                      )}
                      {/* {this.renderOption("Q&A", R.images.ic_q_and_a, () =>
                        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.Q_A)
                      )} */}
                    </View>
                  )}
                </>
              }
            />
          </>
        }
      />
    );
  }
}

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
)(TabAccountScreen);

const styles = StyleSheet.create({
  ic_right_arr: {
    width: 10,
    height: 10,
    position: "absolute",
    right: 10,
    alignSelf: "center",
    resizeMode: "contain"
  },
  text_option: {
    alignSelf: "center",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginHorizontal: 10
  },
  ic_option: {
    width: 20,
    height: 20,
    tintColor: colors.black,
    marginVertical: 10,
    marginHorizontal: 5,
    resizeMode: "contain"
  },
  root_option: {
    flexDirection: "row",
    backgroundColor: colors.white,
    padding: 5,
    marginVertical: 5,
    marginBottom: 0
  },
  root_view_balance: { flexDirection: "row" },
  root_action_balance: {
    flexDirection: "row",
    margin: 10
  },
  root_balance: {
    borderRadius: 5,
    borderWidth: 0.5,
    flex: 1,
    margin: 5,
    alignItems: "center",
    marginTop: 10,
    backgroundColor: colors.white
  },
  ic_qr_code: {
    width: 50,
    height: 50,
    alignSelf: "center",
    flex: 1
  },
  text_rating: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  root_rarting: {
    flexDirection: "row",
    paddingVertical: 5
  },
  text_name: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16,
    paddingVertical: 5,
    maxWidth: windowWidth * 0.7
  },
  img_bg: {
    position: "absolute",
    width: width,
    height: width / 1.5179
    // height: 220
  },
  root_card: {
    padding: 15,
    borderRadius: 10,
    marginTop: Platform.OS == "android" ? width / 1.9 : width / 2.5,
    flexDirection: "row"
  },
  avatar: {
    width: width / 6,
    height: width / 6,
    borderRadius: width / 10,
    overflow: "hidden",
    alignSelf: "center"
  },
  root_detail: {
    alignSelf: "center",
    paddingHorizontal: 10,
    flex: 1
  },
  label_balance: {
    marginTop: 5,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    padding: 5
  },
  root_money: { borderTopWidth: 0.5, width: "100%", padding: 10 },
  text_money: {
    marginTop: 5,
    textAlign: "center",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  ic_img_action: { width: width / 6, height: width / 6, alignSelf: "center" },
  text_action_balance: {
    textAlign: "center",
    color: colors.darkBlue,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12
  }
});

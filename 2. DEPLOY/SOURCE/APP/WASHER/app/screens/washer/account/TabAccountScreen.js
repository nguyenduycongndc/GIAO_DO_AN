import React, { Component } from "react";
import {
  Text,
  TouchableOpacity,
  Image,
  StyleSheet,
  View,
  ScrollView,
  RefreshControl,
  Alert
} from "react-native";
import { connect } from "react-redux";
import R from "@R";
import {
  SCREEN_ROUTER_WASHER,
  SCREEN_ROUTER_AUTH,
  RELEASE_CHANNEL,
  CHANNEL
} from "@app/constants/Constants";
import ScreenComponent from "@app/components/ScreenComponent";
import NavigationUtil from "@app/navigation/NavigationUtil";
import Card from "@app/components/Card";
import { colors } from "@app/constants/Theme";
import RateStar from "@app/components/RateStar";
import NumberFormat from "react-number-format";
import { getUserInfo, changeAvatar } from "@app/redux/actions";
import AsyncStorage from "@react-native-community/async-storage";
import { Avatar } from "react-native-elements";
import { REDUCER_WASHER } from "@app/constants/Constants";
import reactotron from "reactotron-react-native";
import CallApiHelper from "@app/utils/CallApiHelper";
import * as API from "@app/constants/Api";
import { ASYNC_STORAGE } from "@app/constants/Constants";
import ImagePicker from "react-native-image-picker";
import FastImage from "react-native-fast-image";

class TabAccountScreen extends Component {
  initState = {
    numberStar: 4
  };
  state = this.initState;

  componentDidMount() {
    // this.getData();
  }

  getData() {
    this.props.getUserInfo();
  }
  async getData() {
    const token = await AsyncStorage.getItem("token");
    this.props.getUserInfo();
  }
  requestLogout() {
    CallApiHelper(API.requestLogout, "", this, async res => {
      const store = require("@app/redux/store").default;
      store.dispatch({ type: "reset" });
      await AsyncStorage.setItem(ASYNC_STORAGE.TOKEN, "");
      NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH_LOADING);
    });
  }
  _pickImage = async () => {
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
        const data = new FormData();
        data.append("image", {
          name: `imageAvatar`,
          type: `image/jpg`,
          uri:
            Platform.OS === "android"
              ? response.uri
              : response.uri.replace("file://", "")
        });
        this.props.changeAvatar(data);
      }
    });
  };
  onPressQRCode = () => NavigationUtil.navigate(SCREEN_ROUTER_WASHER.QRCODE);
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

  refreshControl = () => <RefreshControl refreshing />;
  renderActionBalance = (label, icon, screen) => (
    <TouchableOpacity
      onPress={() => {
        if (screen == SCREEN_ROUTER_WASHER.WITHDRAWAL)
          if (this.props.UserInfoState.data.withdrawNote == "")
            NavigationUtil.navigate(screen);
          else
            Alert.alert(
              R.strings().notice,
              this.props.UserInfoState.data.withdrawNote
            );
        else NavigationUtil.navigate(screen);
      }}
      style={{ flex: 1 }}
      children={
        <>
          <Image
            source={icon}
            style={styles.ic_img_action}
            resizeMode="contain"
          />
          <Text style={styles.text_action_balance} children={label} />
        </>
      }
    />
  );
  renderOption = (label, icon, screen, data) => (
    <TouchableOpacity
      onPress={async () => {
        if (R.strings().log_out == label) {
          Alert.alert(R.strings().notice, R.strings().confirm_logout, [
            {
              text: "Cancel",
              onPress: () => {}
            },
            { text: "OK", onPress: () => this.requestLogout() }
          ]);
          // await AsyncStorage.setItem(ASYNC_STORAGE.TOKEN, "");
          // NavigationUtil.navigate(screen, data);
        } else {
          NavigationUtil.navigate(screen, data);
        }
      }}
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
  renderHeader = (user, numberStar) => (
    <Card
      style={styles.root_card}
      children={
        <>
          {/* <Image source={R.images.avatarDemo} style={styles.avatar} /> */}
          <Avatar
            rounded
            source={
              user.urlAvatar ? { uri: user.urlAvatar } : R.images.ic_symbol
            }
            style={styles.avatar}
            overlayContainerStyle={{ backgroundColor: "white" }}
            onPress={() => this._pickImage()}
          />
          <View
            style={styles.root_detail}
            children={
              <>
                <Text
                  style={styles.text_name}
                  children={
                    user.name ? user.name : R.strings().not_already_update
                  }
                />
                <View
                  style={styles.root_rarting}
                  children={
                    <>
                      <Text
                        children={`${R.strings().rate}: `}
                        style={styles.text_rating}
                      />
                      <RateStar
                        readonly={true}
                        numberStar={numberStar}
                        isShowNumber
                        color={colors.gray}
                        size={16}
                      />
                    </>
                  }
                />
              </>
            }
          />
          <TouchableOpacity
            style={{ flex: 1 }}
            onPress={this.onPressQRCode}
            children={
              <Image
                source={R.images.ic_qr_code}
                style={styles.ic_qr_code}
                resizeMode="contain"
              />
            }
          />
        </>
      }
    />
  );
  render() {
    const { numberStar } = this.state;
    const { UserInfoState } = this.props;
    return (
      <ScreenComponent
        dialogLoading={this.state.isLoading}
        isLoading={UserInfoState.isLoading}
        isError={UserInfoState.error}
        reload={() => this.getData()}
        isSafeArea={false}
        renderView={
          <>
            <ScrollView
              style={{ flex: 1 }}
              showsVerticalScrollIndicator={false}
              // refreshControl={this.refreshControl}
              refreshControl={
                <RefreshControl
                  refreshing={UserInfoState.isLoading}
                  onRefresh={() => this.getData()}
                />
              }
              children={
                <>
                  <FastImage
                    source={R.images.bg_car_rect}
                    style={styles.img_bg}
                  />
                  {this.renderHeader(
                    UserInfoState.data,
                    UserInfoState.data.rate
                  )}
                  <View
                    style={styles.root_view_balance}
                    children={
                      <>
                        {this.renderBalance(
                          R.strings().deposit_wallet,
                          UserInfoState.data.point
                        )}
                        {this.renderBalance(
                          R.strings().income_wallet,
                          UserInfoState.data.withdrawPoint
                        )}
                      </>
                    }
                  />
                  <View
                    style={styles.root_action_balance}
                    children={
                      <>
                        {RELEASE_CHANNEL == CHANNEL.PRODUCT &&
                          this.renderActionBalance(
                            R.strings().recharge,
                            R.images.ic_wallet,
                            SCREEN_ROUTER_WASHER.RECHARGE
                          )}
                        {this.renderActionBalance(
                          R.strings().withdrawal,
                          R.images.ic_cloud,
                          SCREEN_ROUTER_WASHER.WITHDRAWAL
                        )}
                        {this.renderActionBalance(
                          R.strings().money_transfer,
                          R.images.ic_rev,
                          SCREEN_ROUTER_WASHER.TRANSFERS
                        )}
                        {this.renderActionBalance(
                          R.strings().history,
                          R.images.ic_time_reduce,
                          SCREEN_ROUTER_WASHER.HISTORY_TRANSFERS
                        )}
                      </>
                    }
                  />
                  {this.renderOption(
                    R.strings().personal_information,
                    R.images.ic_user_option,
                    SCREEN_ROUTER_WASHER.PERSONAL_INFO,
                    { userInfo: UserInfoState.data }
                  )}
                  {this.renderOption(
                    R.strings().status_setting,
                    R.images.ic_setting,
                    SCREEN_ROUTER_WASHER.STATE_CHANGE
                  )}
                  {this.renderOption(
                    R.strings().order,
                    R.images.ic_order_production,
                    SCREEN_ROUTER_WASHER.ORDER_PRODUCTION
                  )}
                  {this.renderOption(
                    R.strings().contact_call_center,
                    R.images.ic_calling,
                    SCREEN_ROUTER_WASHER.CONTACT
                  )}
                  {this.renderOption(
                    R.strings().change_password,
                    R.images.ic_lock,
                    SCREEN_ROUTER_WASHER.CHANGE_PASSWORD
                  )}
                  {this.renderOption(
                    "Q&A",
                    R.images.ic_q_and_a,
                    SCREEN_ROUTER_WASHER.QA_SCREEN
                  )}
                  {this.renderOption(
                    R.strings().log_out,
                    R.images.ic_logout,
                    SCREEN_ROUTER_AUTH.LOGIN
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
  getUserInfo,
  changeAvatar
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
    marginVertical: 5
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
  root_rarting: { flexDirection: "row", paddingVertical: 5 },
  text_name: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16,
    paddingVertical: 5
  },
  img_bg: {
    width: width,
    height: height / 3.5,
    resizeMode: "cover"
  },
  root_card: {
    padding: 15,
    borderRadius: 10,
    flexDirection: "row"
  },
  avatar: {
    width: width / 6,
    height: width / 6,
    borderRadius: width / 10,
    overflow: "hidden",
    borderWidth: 0.3,
    borderColor: colors.black,
    backgroundColor: "white"
  },
  root_detail: {
    alignSelf: "center",
    paddingHorizontal: 10
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

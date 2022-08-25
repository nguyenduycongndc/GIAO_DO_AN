import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  Image,
  Vibration,
  TextInput,
  Platform
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import InfoItem from "@app/components/InfoItem";
import R from "@app/assets/R";
import theme from "@theme";
import I18n from "@i18";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { RowAvatarInfo, RowLableInfo } from "@app/components/FormRow";
import {
  PaymentDetail,
  ServicePack,
  Promotion
} from "@app/components/PaymentDetail";
import ButtonPrimary from "@app/components/ButtonPrimary";
import * as API from "@app/constants/Api";
import CallApiHelper from "@app/utils/CallApiHelper";
import { numberWithCommas } from "@app/constants/Functions";
import reactotron from "reactotron-react-native";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  ORDER_STATUS,
  ASYNC_STORAGE,
  SCREEN_ROUTER_WASHER,
  TYPE_LIST_REASON_CANCEL
} from "@app/constants/Constants";
// import { DropDownHolder } from "@app/screens/auth/AppContainer";
import {
  dismissAllNotification,
  clearNotificationData
} from "@app/utils/Notify";
import AsyncStorage from "@react-native-community/async-storage";
import Sound from "react-native-sound";
import {
  getListOrderUpcommingService,
  navigateTab,
  getListOrderPendingService,
  getListOrderHistoryService
} from "@app/redux/actions";
import {
  InfoCustomer,
  InfoService,
  InfoCar
} from "./component/OrderDetailItem";
import ModalAlert from "@app/components/ModalAlert";
import { showMessages, showConfirm } from "@app/components/Alert";
import FastImage from "react-native-fast-image";
import { NavigationActions, StackActions } from "react-navigation";
const vibrate = [
  0,
  500,
  110,
  500,
  110,
  450,
  110,
  200,
  110,
  170,
  40,
  450,
  110,
  200,
  110,
  170,
  40,
  500
];

var ringtone = null;
export class OrderIncommingScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const data = navigation.getParam("data", {});
    this.state = {
      isLoading: true,
      timer: 30,
      data: data,
      detailOrder: {},
      isBack: false,
      timeCountDown: data ? this.getTimeCountDown(data) : 60,
      error: null,
      listReason: [],
      reason: "",
      isVisible: false,
      dialogLoading: false
    };
  }

  playSound() {
    Sound.setCategory("Playback");
    ringtone = new Sound(
      Platform.OS == "ios" ? "love.wav" : "love.mp3",
      Platform.OS == "ios"
        ? encodeURIComponent(Sound.MAIN_BUNDLE)
        : Sound.MAIN_BUNDLE,
      error => {
        ringtone.play();
      }
    );
  }

  playSoundAndVibarate = async () => {
    Vibration.vibrate(vibrate, true);
    // if (ringtone) ringtone.play();
    this.playSound();
  };

  stopSoundAndVibarate = () => {
    if (ringtone) ringtone.stop();
    Sound.setActive(false);
    Vibration.cancel();
  };

  getTimeCountDown = data =>
    data.timeWait - parseInt(new Date().getTime() / 1000) + data.timeSend;

  componentWillMount() {
    this.clearData();
    // this.playSoundAndVibarate();
  }

  clearData = async () => {
    await AsyncStorage.setItem(ASYNC_STORAGE.NOTIFY_DATA, "");
    dismissAllNotification();
    clearNotificationData();
  };

  async componentDidMount() {
    // await AsyncStorage.getItem(
    //   ASYNC_STORAGE.NOTIFY_DATA,
    //   async () => await AsyncStorage.setItem(ASYNC_STORAGE.NOTIFY_DATA, "")
    // );
    this.clearData();
    this.getOrderDetail();
  }

  componentWillUnmount() {
    reactotron.log("componentWillUnmount", this.state);
    this.stopSoundAndVibarate();
  }

  getOrderDetail = () => {
    const { data } = this.state;
    try {
      this.setState(
        {
          ...this.state,
          isLoading: true
        },
        async () => {
          var res = null;
          if (data.orderServiceID)
            res = await API.getOrderServiceDetail({
              orderServiceID: data.orderServiceID
            });
          else if (data.comboCode)
            res = await API.GetOrderComboService(data.comboCode);
          const listReason = await API.GetContentReason(
            TYPE_LIST_REASON_CANCEL
          );
          this.playSoundAndVibarate();
          this.setState(
            {
              ...this.state,
              isLoading: false,
              detailOrder: res.result,
              listReason: listReason.result
            },
            () => reactotron.log("after set state", this.state)
          );
        }
      );
    } catch (error) {
      reactotron.log("error", error);
      this.setState({
        ...this.state,
        isLoading: false,
        error: error
      });
    }
  };
  setData = data => {
    this.setState({
      ...this.state,
      detailOrder: data
    });
  };

  changeStatus = status => {
    const { data, reason, detailOrder } = this.state;
    CallApiHelper(
      API.changeStatusOrder,
      {
        orderServiceID: detailOrder.orderServiceID,
        status: status,
        reason: reason
      },
      this,
      res => {
        this.stopSoundAndVibarate();
        if (status != ORDER_STATUS.REJECTED) {
          this.props.getListOrderUpcommingService({
            status: ORDER_STATUS.CONFIRMED,
            page: 1
          });
          if (status == ORDER_STATUS.CANCEL_BOOKING_NOW) {
            this.props.getListOrderHistoryService({
              status: "",
              page: 1
            });
          }
        } else {
          this.props.getListOrderPendingService({
            status: ORDER_STATUS.PENDING,
            page: 1
          });
        }
        if (status == ORDER_STATUS.CANCEL_BOOKING_NOW) {
          this.props.navigateTab(3);
        } else if (status == ORDER_STATUS.REJECTED) {
          this.props.navigateTab(1);
        } else this.props.navigateTab(2);

        this.goback();
        if (status == ORDER_STATUS.ORDER_STATUS_CONFIRM)
          NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER);
      },
      error => {
        console.log("error", error);
        this.stopSoundAndVibarate();
        this.goback();
      }
    );
  };

  toggleModal = () => {
    this.setState({ isVisible: !this.state.isVisible });
  };

  renderModalCancel = () => {
    const { listReason, reason, isVisible } = this.state;
    return (
      <ModalAlert
        contentView={
          <>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 12,
                paddingVertical: 10
              }}
              children={R.strings().message_warning_cancel_order}
            />
            {listReason.map(e => (
              <TouchableOpacity
                onPress={() => {
                  this.setState({ reason: e });
                }}
                children={<Text style={styles.item_note} children={e} />}
              />
            ))}
            <TextInput
              placeholder={R.strings().require_note_cancel}
              style={styles.text_input_note_cancel}
              multiline
              value={reason}
              onChangeText={text => this.setState({ reason: text })}
              scrollEnabled={false}
              maxLength={300}
            />
          </>
        }
        textSubmit={R.strings().cancel_order}
        onSubmit={() => {
          this.setState({ isSendRequest: true, isVisible: false });
        }}
        onModalHide={() => {
          if (this.state.isSendRequest)
            this.setState({ dialogLoading: true }, () => {
              this.changeStatus(ORDER_STATUS.CANCEL_BOOKING_NOW);
            });
        }}
        validSubmit={reason.trim().length > 0}
        onClose={this.toggleModal}
        isVisible={isVisible}
      />
    );
  };

  Timeout() {
    setTimeout(() => {
      if (this.state.timeCountDown > 0)
        this.setState({
          timeCountDown: this.getTimeCountDown(this.state.data)
        });
      else this.goback();
      // this.setState({
      //   isBack: true
      // });
    }, 1000);
  }
  setGoback = () => {
    this.setState({
      isBack: true
    });
  };
  goback = () => {
    reactotron.log("goBack", this.state, this.props.navigation, this.props);
    // NavigationUtil.pop(1);
    // this.props.navigation.dismiss();
    // this.props.navigation.pop(1);
    // this.props.navigation
    //   .goBack
    //   // this.props.navigation.state.params.data.orderServiceID
    //   ();
    // this.props.navigation.popToTop();
    // this.props.navigation.pop(1);
    // this.props.navigation.dismiss();
    // this.props.navigation.goBack({
    //   key: this.state.data.orderServiceID.toString()
    // });
    // NavigationUtil.goBack(`orderID ${this.state.data.orderServiceID}`);
    // NavigationUtil.goBack();
    // NavigationUtil.goBack({
    //   key: `orderID ${this.state.data.orderServiceID}`
    // });
    let goBack = NavigationActions.back({
      key: `orderID ${this.state.data.orderServiceID}`
    });
    this.props.navigation.dispatch(goBack);
  };
  render() {
    const { timer, detailOrder, isLoading, dialogLoading } = this.state;
    this.Timeout();
    const { isBookingNow, usePoint, basePrice } = this.state.detailOrder;
    const data = this.state.detailOrder;
    return (
      <ScreenComponent
        back
        dialogLoading={dialogLoading}
        isLoading={isLoading}
        titleHeader={R.strings().order_tab_cus}
        renderView={
          <View
            style={{
              flex: 1
            }}
          >
            <ScrollView
              style={{
                flex: 1,
                backgroundColor: theme.colors.backgroundColor
              }}
            >
              <View style={styles.box_count}>
                {isBookingNow == 1 && (
                  <Text style={[styles.price, { marginTop: 30 }]}>
                    Booking Now
                  </Text>
                )}
                <View
                  style={{
                    flexDirection: "row",
                    marginTop: isBookingNow == 1 ? 10 : 30
                  }}
                  children={
                    !this.state.data.comboCode && (
                      <>
                        <Text
                          style={[
                            styles.price,
                            { color: theme.colors.textColor }
                          ]}
                        >
                          {`${R.strings().accept_order}: `}
                        </Text>
                        <Text
                          style={styles.price}
                          children={`-${numberWithCommas(
                            `${detailOrder.totalPrice}`
                          ) + "đ"}`}
                        />
                      </>
                    )
                  }
                />
                <View
                  style={[
                    styles.view,
                    { alignItems: "center", justifyContent: "center" }
                  ]}
                >
                  <FastImage
                    source={
                      isBookingNow == 1
                        ? R.images.count_down_booking_now
                        : R.images.count_down
                    }
                    style={[styles.count_down, { position: "absolute" }]}
                  />
                  <Text style={styles.text}>{this.state.timeCountDown}</Text>
                </View>
              </View>

              <InfoCustomer item={data} />
              <InfoCar item={data} />
              <InfoService
                item={data}
                combo={this.state.data.comboCode || null}
              />
            </ScrollView>
            <View>
              <RowLableInfo
                lable={R.strings().commission.toUpperCase()}
                title={numberWithCommas(`${detailOrder.commission}`) + "đ"}
                lableStyle={styles.lable_benefit}
                titleStyle={styles.title_benefit}
                borderBottom
              />
              <View style={styles.button_bottom}>
                <ButtonPrimary
                  onPress={() => {
                    if (isBookingNow == 1) {
                      showConfirm(
                        R.strings().notice,
                        R.strings().mess_confirm_cancel_order,
                        this.toggleModal
                      );
                    } else this.changeStatus(ORDER_STATUS.REJECTED);
                  }}
                  text={R.strings().reject}
                  styleButton={{ flex: 1 }}
                  style={{
                    backgroundColor: theme.colors.red,
                    marginHorizontal: 5
                  }}
                />
                <ButtonPrimary
                  onPress={() => this.changeStatus(ORDER_STATUS.CONFIRMED)}
                  text={R.strings().accept}
                  styleButton={{ flex: 1 }}
                  style={{ marginHorizontal: 5 }}
                />
              </View>
            </View>
            {this.renderModalCancel()}
          </View>
        }
      />
    );
  }
}
const styles = StyleSheet.create({
  button_bottom: {
    flexDirection: "row",
    marginVertical: 20
  },
  lable_benefit: {
    color: theme.colors.textColor
  },
  title_benefit: {
    color: theme.colors.red,
    fontFamily: R.fonts.quicksand_bold
  },
  price: {
    fontSize: 14,
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.red
  },
  count_down: {
    width: 100,
    height: 100,
    position: "absolute",
    resizeMode: "center"
  },
  view: {
    width: 100,
    height: 100,
    alignItems: "center",
    justifyContent: "center",
    margin: 15
  },
  text: {
    fontSize: 26,
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.white,
    textAlign: "center",
    textAlignVertical: "center",
    alignSelf: "center"
  },
  box_count: {
    alignItems: "center"
  },
  item_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    padding: 10,
    marginBottom: 10,
    borderRadius: 10,
    borderWidth: 0.5,
    color: theme.colors.primary
  },
  text_input_note_cancel: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    borderRadius: 5,
    borderWidth: 0.5,
    padding: 10,
    textAlignVertical: "top",
    height: 100,
    color: "black"
  }
});

const mapStateToProps = state => ({});

const mapDispatchToProps = {
  getListOrderUpcommingService,
  navigateTab,
  getListOrderPendingService,
  getListOrderHistoryService
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(OrderIncommingScreen);

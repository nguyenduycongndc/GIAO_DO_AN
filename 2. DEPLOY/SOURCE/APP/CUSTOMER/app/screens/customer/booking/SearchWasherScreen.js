import React, { Component } from "react";
import {
  View,
  Text,
  StyleSheet,
  Image,
  BackHandler,
  TextInput,
  Linking,
  Platform,
  TouchableOpacity
} from "react-native";
import { connect } from "react-redux";
import R from "@app/assets/R";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@theme";
import RateStar from "@app/components/RateStar";
import Button from "@app/components/Button";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import ModalMessage from "@app/components/ModalMessage";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_CUSTOMER, DEEP_LINK } from "@constant";
import { getOrder, getUserInfoAction, navigateTab } from "@app/redux/actions";
import { ORDER_TYPE, TYPE_REASON } from "@constant";
import callAPI from "../../../utils/CallApiHelper";
import LottieView from "lottie-react-native";
import ModalAlert from "@app/components/ModalAlert";
import { showMessages, showConfirmAlert } from "@app/components/Alert";
import reactotron from "reactotron-react-native";
import { deepLink } from "@app/constants/Functions";
import FastImage from "@app/components/FastImage";
let isSubmit = false;
export class SearchWasherScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    this.state = {
      isLoading: false,
      error: "",
      isBookingNow: 0,
      agentCode: "",
      washerInfo: "",
      reasonCancel: "",
      orderDetail: {}
    };
  }
  timeOut = null;
  componentDidMount() {
    const { orderServiceID, comboID } = this.props.navigation.state.params;

    if (orderServiceID)
      callAPI(API.getOrderServiceDetail, { orderServiceID }, this, res => {
        const { isBookingNow, agentCode } = res.result;
        this.setState({ orderDetail: res.result }, () => {
          if (isBookingNow == 1) this.checkAgentByCode(agentCode);
        });
      });
    else if (comboID)
      callAPI(API.GetOrderComboSevice, comboID, this, res => {
        this.setState({ orderDetail: res.result });
      });
    this.setTimeout();
    this.getReason();
  }

  clearTimeout = () => {
    clearTimeout(this.timeOut);
  };
  setTimeout = () => {
    this.timeOut = setTimeout(() => {
      showMessages(
        "",
        R.strings().message_search_washer,
        this.onSubmitWatingSearch
      );
    }, 5000);
  };

  componentWillUnmount = () => {
    this.removeEventListener();
    this.clearTimeout();
  };

  removeEventListener = () => {};

  checkAgentByCode = agentCode => {
    CallApiHelper(API.checkAgentByCode, agentCode, this, res =>
      this.setState({
        washerInfo: res.result
      })
    );
  };
  handleBackButtonClick = () => {
    this.onCancel();
    return true;
  };

  onSubmitWatingSearch = () => {
    this.dismiss();
    NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.ORDER);
    NavigationUtil.navigate(
      SCREEN_ROUTER_CUSTOMER.GEN_DETAIl,
      this.state.orderDetail
    );
    this.props.navigateTab(1);
    const { resetData } = this.props.reducer.state.upcomingOrder;
    if (resetData) resetData();
  };
  dismiss = () => {
    const { reset } = this.props.tabBookingScreenState;
    this.props.navigation.dismiss();
    if (reset) reset();
  };

  onCancel = () => {
    const {
      cancelOrder: cancelOrderLeft
    } = this.props.reducer.userReducer.data;
    if (cancelOrderLeft > 0) {
      this.clearTimeout();
      this.setState({ isVisibleCancel: true });
    } else showMessages(R.strings().no_left_turn);
  };
  submitCancel = (orderServiceID, reasonCancel) => {
    callAPI(
      API.changeStatusOrder,
      {
        orderServiceID,
        status: ORDER_TYPE.ORDER_STATUS_CANCEL,
        reason: reasonCancel || this.state.reasonCancel
      },
      this,
      () => {
        this.removeEventListener();
        this.dismiss();
        this.props.getUserInfoAction();
        const { upcomingOrder, historyOrder } = this.props.reducer.state;
        if (upcomingOrder?.resetData) upcomingOrder.resetData();
        if (historyOrder?.resetData) historyOrder.resetData();
      }
    );
  };
  closeCancelModal = () => {
    this.setTimeout();
    this.setState({
      isVisibleCancel: false
    });
  };
  getReason = () => {
    callAPI(API.GetContentReason, TYPE_REASON.CANCEL, this, res =>
      this.setState({
        listRequestService: res.result
      })
    );
  };

  render() {
    const { orderServiceID } = this.props.navigation.state.params;
    const {
      cancelOrder: cancelOrderLeft
    } = this.props.reducer.userReducer.data;
    const {
      isLoading,
      isBookingNow,
      isVisibleCancel,
      reasonCancel
    } = this.state;
    return (
      <ScreenComponent
        isLoading={isLoading}
        titleHeader={R.strings().search_washer}
        renderView={
          <>
            <ModalAlert
              contentView={
                <>
                  <Text
                    style={{
                      fontFamily: R.fonts.quicksand_bold,
                      fontSize: 14,
                      paddingVertical: 10
                    }}
                    children={
                      R.strings().you_have_left +
                      " " +
                      cancelOrderLeft +
                      " " +
                      R.strings().turn
                    }
                  />
                  <Text
                    style={{
                      fontFamily: R.fonts.quicksand_medium,
                      fontSize: 12,
                      paddingVertical: 10
                    }}
                    children={R.strings().message_warning_cancel_order}
                  />
                  {this.state.listRequestService?.map(e => (
                    <TouchableOpacity
                      onPress={() => {
                        this.setState({ reasonCancel: e });
                      }}
                      children={<Text style={styles.item_note} children={e} />}
                    />
                  ))}
                  <TextInput
                    placeholderTextColor={colors.gray}
                    placeholder={R.strings().require_note_cancel}
                    style={styles.text_input_note_cancel}
                    multiline
                    value={reasonCancel}
                    onChangeText={reasonCancel =>
                      this.setState({ reasonCancel })
                    }
                    maxLength={256}
                    scrollEnabled={false}
                  />
                </>
              }
              textSubmit={R.strings().cancel_order}
              onSubmit={() => {
                isSubmit = true;
                this.closeCancelModal();
              }}
              onModalHide={() => {
                if (isSubmit)
                  showConfirmAlert(
                    R.strings().notif_tab_cus,
                    R.strings().warning_cancel_order,
                    () => {
                      this.submitCancel(
                        orderServiceID,
                        this.state.reasonCancel
                      );
                    }
                  );
              }}
              textCancel={R.strings().back}
              validSubmit={reasonCancel.trim().length > 0}
              onClose={() => {
                isSubmit = false;
                this.closeCancelModal();
                this.setState({
                  reasonCancel: ""
                });
              }}
              isVisible={isVisibleCancel}
            />
            {isBookingNow == 1
              ? this._renderIsBookingNow()
              : this._renderSearchingWasher()}
          </>
        }
      />
    );
  }
  _renderIsBookingNow = () => {
    return (
      <View style={styles.view}>
        <View
          style={{
            width: "100%",
            backgroundColor: theme.colors.white,
            marginTop: 17,
            alignItems: "center",
            paddingBottom: 20
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              color: theme.colors.greenLight,
              paddingVertical: 21
            }}
          >
            {this.state.isBookingNow == 0
              ? R.strings().carrect_has_found_washer
              : R.strings().waiting_washer_feedback}
          </Text>
          <FastImage
            style={{
              width: 146,
              height: 146,
              borderRadius: 146 / 2
            }}
            source={
              !!this.state.washerInfo?.url
                ? { uri: this.state.washerInfo.url }
                : R.images.avatarDemo
            }
          />
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 22,
              color: theme.colors.primary,
              paddingVertical: 5
            }}
          >
            {this.state.washerInfo.name}
          </Text>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              color: theme.colors.textColor,
              paddingVertical: 5
            }}
          >
            {this.state.washerInfo.code}
          </Text>
          <RateStar
            numberStar={this.state.washerInfo?.rate || 5}
            size={40}
            readonly
          />
        </View>
        <Button
          width="95%"
          title={R.strings().call}
          imageSource={R.images.ic_call}
          styleImg={{
            width: 28,
            height: 28,
            marginRight: 10,
            tintColor: colors.grayIcCall
          }}
          backgroundColor={theme.colors.white}
          borderColor={theme.colors.gray}
          colorText={theme.colors.gray}
          action={() => Linking.openURL(`tel:${this.state.washerInfo.phone}`)}
        />
        <Button
          action={() => deepLink(DEEP_LINK.ZALO, this.state.washerInfo.phone)}
          width="95%"
          title={R.strings().chat_via_zalo}
          imageSource={R.images.ic_zalo}
          styleImg={{ width: 33, height: 33, marginRight: 13 }}
          backgroundColor={theme.colors.backgroundZalo}
          colorText={theme.colors.white}
          buttonStyle={{ marginTop: 0 }}
        />
        <View
          style={{
            paddingHorizontal: 12,
            paddingVertical: 9,
            width: "100%",
            position: "absolute",
            backgroundColor: theme.colors.white,
            bottom: 0,
            justifyContent: "space-around",
            alignItems: "center"
          }}
        >
          <Button
            width="95%"
            title={R.strings().done}
            backgroundColor={theme.colors.primary}
            colorText={theme.colors.white}
            uppercase
            action={() => this.props.navigation.dismiss()}
          />
        </View>
      </View>
    );
  };
  _renderSearchingWasher() {
    return (
      <View style={styles.view}>
        <View
          style={{
            width: "100%",
            backgroundColor: theme.colors.white,
            justifyContent: "center",
            alignItems: "center",
            paddingVertical: width / 20
          }}
        >
          {Platform.OS == "ios" ? (
            <LottieView
              style={{
                height: width,
                width: width,
                borderRadius: width * 0.5,
                overflow: "hidden",
                overlayColor: "white"
              }}
              source={require("@app/assets/json/search_washer.json")}
              autoPlay
              loop
            />
          ) : (
            <FastImage
              style={{
                height: width,
                width: width,
                borderRadius: width * 0.5,
                overflow: "hidden",
                overlayColor: "white"
              }}
              source={require("@app/assets/gif/search_washer.gif")}
            />
          )}

          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              color: theme.colors.greenLight,
              paddingVertical: 0,
              textAlign: "center",
              paddingHorizontal: 10
            }}
          >
            {R.strings().searching_washer}
          </Text>
        </View>
        <View
          style={{
            paddingHorizontal: 12,
            paddingVertical: 9,
            width: "100%",
            position: "absolute",
            backgroundColor: theme.colors.white,
            bottom: 0,
            justifyContent: "space-around",
            alignItems: "center"
          }}
        >
          <Button
            width="95%"
            title={R.strings().cancel}
            backgroundColor={theme.colors.backgroundColor}
            colorText={theme.colors.gray}
            uppercase
            action={this.onCancel}
          />
        </View>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  view: {
    flex: 1,
    backgroundColor: theme.colors.white,
    alignItems: "center"
  },
  root_buttons: { width: "100%", paddingHorizontal: 10 },
  root_comunicate: {
    flexDirection: "row"
  },
  button: {
    height: "60%",
    alignSelf: "center",
    flex: 1
  },
  img_call: {
    width: 28,
    height: 28,
    marginRight: 10,
    tintColor: colors.grayIcCall
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
  },
  item_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    padding: 10,
    marginBottom: 10,
    borderRadius: 10,
    borderWidth: 0.5,
    color: colors.primary
  }
});

const mapStateToProps = state => ({
  reducer: state,
  tabBookingScreenState: state.state.BookingCustomer
});

const mapDispatchToProps = {
  getOrder,
  getUserInfoAction,
  navigateTab
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(SearchWasherScreen);

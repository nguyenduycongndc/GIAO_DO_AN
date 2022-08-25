import React, { Component } from "react";
import { Text, StyleSheet, View, SafeAreaView, StatusBar } from "react-native";
import { connect } from "react-redux";
import AppNavigator from "../../navigation/AppNavigator";
import NavigationUtil from "@app/navigation/NavigationUtil";
import Reactotron from "reactotron-react-native";
import OneSignal from "react-native-onesignal"; // Import package from node modules
import DropdownAlert from "react-native-dropdownalert";
import { getListOrderService, setState, navigateTab } from "@app/redux/actions";
import { ORDER_TYPE, SCREEN_ROUTER_CUSTOMER, TYPE_NOTI } from "@constant";
import analytics from '@react-native-firebase/analytics';
// import {
//     TYPE_NOTIFICATION_RECEIVE,
//     SCREEN_ROUTER,
//     PER,
//     CUSTOMER_TYPE,
// } from '../../constants/Constant';
var deviceId = null;

class AppContainer extends Component {
  constructor(properties) {
    super(properties);
    OneSignal.init("5a36e238-e0b9-481b-82a6-f496aa9b3682");
    // 0 = None, 1 = InAppAlert, 2 = Notification
    OneSignal.inFocusDisplaying(2);
    OneSignal.addEventListener("received", this.onReceived);
    OneSignal.addEventListener("opened", this.onOpened);
    OneSignal.addEventListener("ids", this.onIds);
  }

  componentWillUnmount() {
    OneSignal.removeEventListener("received", this.onReceived);
    OneSignal.removeEventListener("opened", this.onOpened);
    OneSignal.removeEventListener("ids", this.onIds);
  }

  refreshData = () => {
    const {
      upcomingState,
      processingState,
      historyState,
      getDataDetail
    } = this.props;
    if (upcomingState?.resetData) upcomingState.resetData();
    if (processingState?.resetData) processingState.resetData();
    if (historyState?.resetData) historyState.resetData();
    if (getDataDetail) getDataDetail();
  };

  onReceived = async notification => {
    const dataNoti = notification.payload.additionalData;
    const { OrderCustomerState, NotifCustomerState, navigateTab } = this.props;
    switch (dataNoti.type) {
      case TYPE_NOTI.NOTI_ORDER_STATUS_CONFIRM:
        analytics().logEvent('order_match', {
          id: dataNoti.orderServiceID
        })
        this.refreshData();
        navigateTab(1);
        break;
      case TYPE_NOTI.NOTI_ORDER_STATUS_WASHING:
      case TYPE_NOTI.NOTI_UPLOAD_IMAGE:
        this.refreshData();
        navigateTab(0);
        break;
      case TYPE_NOTI.NOTI_ORDER_STATUS_PROCESS:
        this.refreshData();
        navigateTab(2);
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, {
          orderServiceID: dataNoti.orderServiceID,
          noti: true
        });
        break;
      case TYPE_NOTI.NOTI_ORDER_STATUS_CANCEL:
        this.refreshData();
        if (NotifCustomerState.getNotify) NotifCustomerState.getNotify();
        break;

      case TYPE_NOTI.NOTI_TRANSACTION_SUCCESS:
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.SEARCH_WASHER, {
          orderServiceID: dataNoti.orderServiceID,
          comboID: dataNoti.comboCode
        });
        break;
      case TYPE_NOTI.NOTI_TRANSACTION_FAIL:
        break;
    }
  };

  onOpened = openResult => {
    const data = openResult.notification.payload.additionalData;
    switch (data.type) {
      case TYPE_NOTI.NOTI_ORDER_STATUS_CONFIRM:
      case TYPE_NOTI.NOTI_ORDER_STATUS_WASHING:
        this.refreshData();
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, {
          ...data,
          noti: true
        });
        break;
      case TYPE_NOTI.NOTI_ORDER_STATUS_PROCESS:
        this.refreshData();
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, {
          ...data,
          noti: true
        });
        break;
      case TYPE_NOTI.NOTI_TRANSACTION_SUCCESS:
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, {
          ...data,
          noti: true
        });
        break;
    }
  };

  onIds(device) {
    deviceId = device.userId;
  }

  render() {
    return (
      <>
        <AppNavigator
          ref={navigatorRef =>
            NavigationUtil.setTopLevelNavigator(navigatorRef)
          }
        />
        <DropdownAlert
          // onTap={data => {
          //   let type = data.payload.additionalData.type;
          //   if (!type) return;
          //   // if (type == TYPE_NOTIFICATION_RECEIVE.CHAT) {

          //   // }
          //   // if (type == TYPE_NOTIFICATION_RECEIVE.BOOKING)
          //   //     NavigationUtil.navigate(SCREEN_ROUTER.DETAILS_ORDER_MANAGEMENT, {
          //   //         item: data.payload.additionalData,
          //   //     });
          // }}
          updateStatusBar={false}
          defaultContainer={{
            paddingTop: StatusBar.currentHeight + 10,
            paddingBottom: StatusBar.currentHeight,
            flexDirection: "row",
            paddingLeft: 10
          }}
          ref={ref => DropDownHolder.setDropDown(ref)}
          closeInterval={6000}
        />
      </>
    );
  }
}

export class DropDownHolder {
  static dropDown;
  static setDropDown(dropDown) {
    this.dropDown = dropDown;
  }
  static getDropDown() {
    return this.dropDown;
  }
}

const mapStateToProps = state => ({
  userState: state.userReducer,
  homeState: state.homeReducer,
  processingState: state.state.processingOrder,
  upcomingState: state.state.upcomingOrder,
  historyState: state.state.historyOrder,
  upcomingDetailState: state.state.upcomingDetail,
  navigateTabState: state.NavigateTabReducer,
  NotifCustomerState: state.state.NotifCustomer,
  getDataDetail: state.state[SCREEN_ROUTER_CUSTOMER.GEN_DETAIl].getData
});

const mapDispatchToProps = {
  getListOrderService,
  setState,
  navigateTab
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AppContainer);

const styles = StyleSheet.create({});

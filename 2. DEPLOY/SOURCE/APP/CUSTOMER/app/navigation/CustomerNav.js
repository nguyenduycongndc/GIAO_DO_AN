import React, { useState, useEffect } from "react";
import { createStackNavigator } from "react-navigation-stack";
import { createBottomTabNavigator, BottomTabBar } from "react-navigation-tabs";
import R from "@app/assets/R";
import TabBookingScreen from "@screen_customer/booking/TabBookingScreen";
import TabAccountScreen from "@screen_customer/account/TabAccountScreen";
import TabHomeScreen from "@screen_customer/home/TabHomeScreen";
import TabNotificationScreen from "@screen_customer/notification/TabNotificationScreen";
import TabOrderSreen from "@screen_customer/order/TabOrderSreen";
import theme from "@theme";
import Booking from "@app/navigation/screen/customer/booking";
import Auth from "@app/navigation/screen/auth";
import Customer from "@app/navigation/screen/customer";
import {
  Image,
  Text,
  View,
  UIManager,
  LayoutAnimation,
  ScrollView
} from "react-native";
import FastImage from "@app/components/FastImage";
import { Dimensions } from "react-native";
import {
  SCREEN_ROUTER_CUSTOMER,
  SCREEN_ROUTER_AUTH,
  ASYNC_STORAGE,
  REDUCER_CUSTOM
} from "@constant";
import {
  HIDEN_REQUIRE_LOGIN,
  SHOW_REQUIRE_LOGIN,
  HIDE_MESSAGE,
  HIDE_CONFIRM
} from "@action/type";
import AsyncStorage from "@react-native-community/async-storage";
import NavigationUtil from "./NavigationUtil";
import ImageViewScreen from "@app/screens/customer/order/ImageViewScreen";
import ModalAlert from "@app/components/ModalAlert";
import { connect } from "react-redux";
import PromotionScreen from "@app/screens/customer/home/PromotionScreen";
import ModalMessage from "@app/components/ModalMessage";
const dimension = ({ width, height } = Dimensions.get("window"));
const getTabBarIcon = (navigation, focused, tintColor) => {
  const { routeName } = navigation.state;
  const iconSource = tabbarIcons[routeName];
  const iconSize = 24;
  return (
    <>
      <Image
        source={iconSource}
        fadeDuration={0}
        style={{
          width: iconSize,
          height: iconSize,
          tintColor: tintColor
        }}
        resizeMode="contain"
      />
      {/* {routeName === 'Notification' && <UnReadNoti />} */}
    </>
  );
};

const tabbarIcons = {
  [SCREEN_ROUTER_CUSTOMER.HOME]: R.images.ic_tab_home,
  [SCREEN_ROUTER_CUSTOMER.ORDER]: R.images.ic_car_wash,
  [SCREEN_ROUTER_CUSTOMER.NOTIF]: R.images.ic_noti,
  [SCREEN_ROUTER_CUSTOMER.ACCOUNT]: R.images.ic_user
};

var BottomTabBarRedux = props => {
  const { actionModal, showMessage, showConfirm, isSubmit } = props;
  return (
    <>
      <ModalAlert
        isVisible={props.visiable}
        onClose={() => actionModal(HIDEN_REQUIRE_LOGIN)}
        textSubmit={R.strings().login}
        onSubmit={() => {
          actionModal(HIDEN_REQUIRE_LOGIN);
          NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH);
        }}
        backdrop
        contentView={
          <Text
            style={{
              textAlign: "center",
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              paddingVertical: 15
            }}
            children={R.strings().require_login}
          />
        }
      />
      <ModalMessage
        isVisible={showMessage.visiable}
        onClose={() => {
          actionModal(HIDE_MESSAGE, showMessage);
        }}
        onModalHide={() => {
          const { action } = showMessage;
          if (action) action();
        }}
        textSubmit={R.strings().confirm}
        backdrop
        title={showMessage.title}
        contentView={
          !!showMessage.content && (
            <ScrollView
              style={{ maxHeight: height * 0.6 }}
              showsVerticalScrollIndicator={false}
              children={
                <Text
                  style={{
                    textAlign: "center",
                    fontFamily: R.fonts.quicksand_medium,
                    fontSize: 14,
                    paddingVertical: 15
                  }}
                  children={showMessage.content}
                />
              }
            />
          )
        }
      />
      <ModalAlert
        isVisible={showConfirm.visiable}
        onClose={() => actionModal(HIDE_CONFIRM, showConfirm, false)}
        textSubmit={showConfirm.textConfirm}
        textCancel={showConfirm.textCancel}
        onSubmit={() => {
          actionModal(HIDE_CONFIRM, showConfirm, true);
        }}
        onModalHide={() => {
          if (isSubmit) {
            const { action } = showConfirm;
            if (action) action();
          }
        }}
        backdrop
        title={showConfirm.title}
        contentView={
          !!showConfirm.content && (
            <ScrollView
              style={{ maxHeight: height * 0.6 }}
              showsVerticalScrollIndicator={false}
              children={
                <Text
                  style={{
                    textAlign: "center",
                    fontFamily: R.fonts.quicksand_medium,
                    fontSize: 14,
                    paddingVertical: 15
                  }}
                  children={showConfirm.content}
                />
              }
            />
          )
        }
      />
      <BottomTabBar
        {...props}
        onTabPress={async tab => {
          const token = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
          const { key } = tab.route;
          if (
            key != SCREEN_ROUTER_CUSTOMER.HOME &&
            key != SCREEN_ROUTER_CUSTOMER.ACCOUNT
          )
            if (!token) {
              actionModal(SHOW_REQUIRE_LOGIN);
              return;
            }
          props.onTabPress(tab);
        }}
        style={{
          borderTopColor: theme.colors.borderTopColor,
          backgroundColor: theme.colors.backgroundHeader,
          height: 60
        }}
      />
    </>
  );
};
const Main = createBottomTabNavigator(
  {
    [SCREEN_ROUTER_CUSTOMER.HOME]: {
      screen: TabHomeScreen,
      navigationOptions: () => ({
        tabBarLabel: R.strings().home_tab_cus
      })
    },
    [SCREEN_ROUTER_CUSTOMER.ORDER]: {
      screen: TabOrderSreen,
      navigationOptions: () => ({
        tabBarLabel: R.strings().order_tab_cus
      })
    },
    [SCREEN_ROUTER_CUSTOMER.BOOKING]: {
      screen: TabBookingScreen,
      navigationOptions: {
        tabBarLabel: ({ focused, tintColor }) => {
          return (
            <View style={{ alignItems: "center" }}>
              <Image
                source={R.images.ic_booking}
                style={
                  dimension.width > 365
                    ? {
                        width: 75,
                        height: 75
                        // tintColor: tintColor,
                      }
                    : {
                        width: 65,
                        height: 65
                      }
                }
              />
              <Text
                style={[
                  {
                    color: focused
                      ? theme.colors.primary
                      : theme.colors.inactive,
                    fontSize: 11,
                    fontFamily: R.fonts.quicksand_bold
                  }
                ]}
              >
                {R.strings().schedule}
              </Text>
            </View>
          );
        }
      }
    },
    [SCREEN_ROUTER_CUSTOMER.NOTIF]: {
      screen: TabNotificationScreen,
      navigationOptions: () => ({
        tabBarLabel: R.strings().notif_tab_cus
      })
    },
    [SCREEN_ROUTER_CUSTOMER.ACCOUNT]: {
      screen: TabAccountScreen,
      navigationOptions: () => ({
        tabBarLabel: R.strings().account_tab_cus
      })
    }
  },
  {
    defaultNavigationOptions: ({ navigation }) => ({
      tabBarIcon: ({ focused, tintColor }) =>
        getTabBarIcon(navigation, focused, tintColor)
    }),
    tabBarOptions: {
      activeBackgroundColor: theme.colors.backgroundHeader,
      inactiveBackgroundColor: theme.colors.backgroundHeader,
      inactiveTintColor: theme.colors.inactive,
      activeTintColor: theme.colors.primary,
      labelStyle: {
        fontFamily: R.fonts.quicksand_bold
      }
    },
    tabBarComponent: connect(
      state => ({
        visiable: state[REDUCER_CUSTOM.SHOW_MODAL].visiable,
        showMessage: state[REDUCER_CUSTOM.SHOW_MODAL].showMessage,
        showConfirm: state[REDUCER_CUSTOM.SHOW_MODAL].showConfirm,
        isSubmit: state[REDUCER_CUSTOM.SHOW_MODAL].isSubmit
      }),
      require("@action/")
    )(BottomTabBarRedux)
  }
);

export default createStackNavigator(
  {
    main: createStackNavigator(
      Object.assign(
        {
          [SCREEN_ROUTER_CUSTOMER.MAIN]: Main
        },
        Customer
      ),
      {
        headerMode: "none"
      }
    ),
    booking: createStackNavigator(Booking, { headerMode: "none" }),
    [SCREEN_ROUTER_AUTH.AUTH]: createStackNavigator(Auth, {
      headerMode: "none"
    }),
    [SCREEN_ROUTER_CUSTOMER.PROMOTION]: PromotionScreen,
    [SCREEN_ROUTER_CUSTOMER.IMAGE_VIEWER]: ImageViewScreen
  },
  {
    mode: "modal",
    headerMode: "none"
  }
);

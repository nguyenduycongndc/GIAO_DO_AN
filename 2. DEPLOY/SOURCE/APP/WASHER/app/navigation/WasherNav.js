import React from "react";
import { createStackNavigator } from "react-navigation-stack";
import { createBottomTabNavigator, BottomTabBar } from "react-navigation-tabs";
import R from "@app/assets/R";
import TabOrderScreen from "@screen_washer/order/TabOrderScreen";
import TabNotificationScreen from "@screen_washer/notification/TabNotificationScreen";
import TabAccountScreen from "@screen_washer/account/TabAccountScreen";
import theme from "@theme";
import Washer from "@app/navigation/screen/washer";
import { Image } from "react-native";
const TabBarComponent = props => <BottomTabBar {...props} />;
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import TabProductionScreen from "@app/screens/washer/production/TabProductionScreen";
const getTabBarIcon = (navigation, focused, tintColor) => {
  const { routeName } = navigation.state;
  const iconSource = tabbarIcons[routeName] || "";
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
  [SCREEN_ROUTER_WASHER.ORDER]: R.images.ic_tab_home,
  [SCREEN_ROUTER_WASHER.NOTIF]: R.images.ic_noti,
  [SCREEN_ROUTER_WASHER.ACCOUNT]: R.images.ic_user,
  [SCREEN_ROUTER_WASHER.PRODUCTION]: R.images.ic_production
};

const Main = createBottomTabNavigator(
  {
    [SCREEN_ROUTER_WASHER.ORDER]: {
      screen: TabOrderScreen,
      navigationOptions: () => ({
        tabBarLabel: R.strings().transaction
      })
    },
    [SCREEN_ROUTER_WASHER.PRODUCTION]: {
      screen: TabProductionScreen,
      navigationOptions: () => ({
        tabBarLabel: R.strings().production
      })
    },
    [SCREEN_ROUTER_WASHER.NOTIF]: {
      screen: TabNotificationScreen,
      navigationOptions: () => ({
        tabBarLabel: R.strings().notif_tab_cus
      })
    },
    [SCREEN_ROUTER_WASHER.ACCOUNT]: {
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
    tabBarComponent: props => {
      return (
        <TabBarComponent
          {...props}
          onTabPress={props.onTabPress}
          style={{
            borderTopColor: theme.colors.borderTopColor,
            backgroundColor: theme.colors.backgroundHeader,
            height: 60
            // shadowOpacity: 0.1,
            // shadowRadius: 2
          }}
        />
      );
    }
  }
);

export default createStackNavigator(
  {
    main: createStackNavigator(
      Object.assign({ [SCREEN_ROUTER_WASHER.MAIN]: Main }, Washer),
      {
        headerMode: "none"
      }
    )
  },
  {
    mode: "modal",
    headerMode: "none"
  }
);

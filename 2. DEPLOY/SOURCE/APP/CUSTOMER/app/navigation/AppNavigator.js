import { createAppContainer, createSwitchNavigator } from "react-navigation";
import { SCREEN_ROUTER_AUTH, SCREEN_ROUTER_CUSTOMER } from "@constant";
import CustomerNav from "./CustomerNav";
import Intro from "./screen/intro";
import AuthLoadingScreen from "@screen/auth/AuthLoadingScreen";

export default createAppContainer(
  createSwitchNavigator(
    {
      [SCREEN_ROUTER_AUTH.AUTH_LOADING]: AuthLoadingScreen,
      [SCREEN_ROUTER_CUSTOMER.MAIN]: CustomerNav,
      [SCREEN_ROUTER_CUSTOMER.INTRO]: Intro
    },
    {
      initialRouteName: SCREEN_ROUTER_AUTH.AUTH_LOADING
    }
  )
);

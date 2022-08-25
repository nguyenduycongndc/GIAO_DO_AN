import { createAppContainer, createSwitchNavigator } from "react-navigation";
import {
  SCREEN_ROUTER_AUTH,
  SCREEN_ROUTER_WASHER,
  SCREEN_ROUTER_INTRO
} from "@app/constants/Constants";
import WasherNav from "./WasherNav";
import Auth from "./screen/auth";
import Intro from "./screen/intro";
import AuthLoadingScreen from "@screen/auth/AuthLoadingScreen";

export default createAppContainer(
  createSwitchNavigator(
    {
      [SCREEN_ROUTER_AUTH.AUTH_LOADING]: AuthLoadingScreen,
      [SCREEN_ROUTER_AUTH.AUTH]: Auth,
      [SCREEN_ROUTER_WASHER.MAIN]: WasherNav,
      [SCREEN_ROUTER_INTRO.STEP1]: Intro
    },
    {
      initialRouteName: SCREEN_ROUTER_AUTH.AUTH_LOADING
    }
  )
);

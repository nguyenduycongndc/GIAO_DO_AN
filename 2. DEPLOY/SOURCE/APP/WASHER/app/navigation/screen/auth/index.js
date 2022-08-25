import {
  SCREEN_ROUTER_AUTH,
  SCREEN_ROUTER_WASHER
} from "@app/constants/Constants";
import AuthLoadingScreen from "@screen/auth/AuthLoadingScreen";
import { createStackNavigator } from "react-navigation-stack";
import { createSwitchNavigator } from "react-navigation";
import LoginScreen from "@screen/auth/LoginScreen";
import ForgotPasswordScreen from "@screen/auth/ForgotPasswordScreen";
import OTPScreen from "@screen/auth/OTPScreen";
import UpdatePasswordScreen from "@screen/auth/UpdatePasswordScreen";
import RegisterScreen from "@screen/auth/RegisterScreen";
// const login = createStackNavigator({
//   [SCREEN_ROUTER_AUTH.LOGIN]: LoginScreen,
// });

const stack = {
  // [SCREEN_ROUTER_AUTH.AUTH_LOADING]: AuthLoadingScreen,
  [SCREEN_ROUTER_AUTH.LOGIN]: LoginScreen,
  [SCREEN_ROUTER_AUTH.FORGOT_PASS]: ForgotPasswordScreen,
  [SCREEN_ROUTER_AUTH.OTP]: OTPScreen,
  [SCREEN_ROUTER_AUTH.UPDATE_PASSWORD]: UpdatePasswordScreen,
  [SCREEN_ROUTER_AUTH.REGISTER]: RegisterScreen
};

export default createStackNavigator(stack, {
  header: null,
  headerMode: "none"
});

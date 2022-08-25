import { SCREEN_ROUTER_AUTH } from "@constant";
import LoginScreen from "@screen/auth/LoginScreen";
import RegisterScreen from "@screen/auth/RegisterScreen";
import OTPScreen from "@screen/auth/OTPScreen";
import RegisterUserInfoScreen from "@screen/auth/RegisterUserInfoScreen";
import ForgotPasswordScreen from "@screen/auth/ForgotPasswordScreen";
export default {
  [SCREEN_ROUTER_AUTH.LOGIN]: LoginScreen,
  [SCREEN_ROUTER_AUTH.REGISTER]: RegisterScreen,
  [SCREEN_ROUTER_AUTH.OTP]: OTPScreen,
  [SCREEN_ROUTER_AUTH.REGISTER_INFO]: RegisterUserInfoScreen,
  [SCREEN_ROUTER_AUTH.FORGOT_PASS]: ForgotPasswordScreen
};

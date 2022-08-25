import { SCREEN_ROUTER_AUTH, SCREEN_ROUTER_INTRO } from "@app/constants/Constants";
import IntroStep1Screen from "@screen/intro/IntroStep1Screen";
import IntroStep2Screen from "@screen/intro/IntroStep2Screen";
import IntroStep3Screen from "@screen/intro/IntroStep3Screen";
import { createStackNavigator } from "react-navigation-stack";
const stack = {
  [SCREEN_ROUTER_INTRO.STEP1]: IntroStep1Screen,
  [SCREEN_ROUTER_INTRO.STEP2]: IntroStep2Screen,
  [SCREEN_ROUTER_INTRO.STEP3]: IntroStep3Screen,
};

export default createStackNavigator(stack, {
  headerMode: "none"
});

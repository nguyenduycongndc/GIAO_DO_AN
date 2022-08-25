import { CHANGE_LANGUAGE } from "@action_language";
import I18n from "react-native-i18n";
import NavigationUtil from "@app/navigation/NavigationUtil";
const initialState = {
  lang: null
};

export default function(state = initialState, action) {
  switch (action.type) {
    case CHANGE_LANGUAGE: {
      I18n.locale = action.payload;
      // NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.NOTIF);
      // NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.ACCOUNT);
      return { ...state, lang: action.payload };
    }
    default:
      return state;
  }
}

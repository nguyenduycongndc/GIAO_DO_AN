import { CHANGE_LANGUAGE } from "@action_language";
import I18n from "react-native-i18n";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_CUSTOMER } from "@constant";
import callAPI from "@app/utils/CallApiHelper";
import { ChangeLang } from "@app/constants/Api";
const initialState = {
  lang: "vi"
};

export default function(state = initialState, action) {
  switch (action.type) {
    case CHANGE_LANGUAGE: {
      I18n.locale = action.payload.locale;
      if (!action.payload.isIntro) {
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.NOTIF);
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.ACCOUNT);
        callAPI(ChangeLang, null);
      }
      return { ...state, lang: action.payload.locale };
    }
    default:
      return state;
  }
}

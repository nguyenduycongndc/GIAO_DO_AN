import { combineReducers } from "redux";
import LanguageReducer from "./LanguageReducer";
import UserReducer from "./UserReducer";
import ProcessingServiceReducer from "./ProcessingServiceReducer";
import LocationReducer from "./LocationReducer";
import ListOrderUpcommingService from "./ListOrderUpcommingService";
import ListOrderHistoryService from "./ListOrderHistoryService";
import ListOrderPendingService from "./ListOrderPendingService";
import NavigateTabReducer from "./NavigateTabReducer";
import NotificationReducer from "./NotificationReducer";
import CartReducer from "./CartReducer";

import { REDUCER_WASHER } from "@app/constants/Constants";

const LangReducer = {
  lang: LanguageReducer
};
const REDUCER_WASH = {
  [REDUCER_WASHER.USER]: UserReducer,
  [REDUCER_WASHER.PROCESSING_ORDER_SERVICE]: ProcessingServiceReducer,
  [REDUCER_WASHER.LIST_ORDER_UPCOMMING_SERVICE]: ListOrderUpcommingService,
  [REDUCER_WASHER.LIST_ORDER_HISTORY_SERVICE]: ListOrderHistoryService,
  [REDUCER_WASHER.LIST_ORDER_PENDING_SERVICE]: ListOrderPendingService,
  [REDUCER_WASHER.LOCATION_REDUCER]: LocationReducer,
  [REDUCER_WASHER.NAVIGATE_TAB]: NavigateTabReducer,
  [REDUCER_WASHER.NOTIFICATION]: NotificationReducer,
  [REDUCER_WASHER.CART]: CartReducer
};
const REDUCER = Object.assign(REDUCER_WASH, LangReducer);
export const RESET = "reset";

appReducer = combineReducers(REDUCER);

const initialState = appReducer({}, {});

export default (rootReducer = (state, action) => {
  // if (action.type === RESET) {
  //   state = initialState;
  // }

  return appReducer(state, action);
});

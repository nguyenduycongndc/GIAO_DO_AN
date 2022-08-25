import { combineReducers } from "redux";
import LanguageReducer from "./LanguageReducer";
const LangReducer = {
  lang: LanguageReducer
};
const REDUCER_CUSTOMER = require("./reducer");
const REDUCER = Object.assign(REDUCER_CUSTOMER, LangReducer);
const RESET = "reset";

appReducer = combineReducers(REDUCER);

const initialState = appReducer({}, {});

export default (rootReducer = (state, action) => {
  if (action.type === RESET) {
    state = initialState;
  }

  return appReducer(state, action);
});

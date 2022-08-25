import { STATE } from "@action/type";
import { SCREEN_ROUTER_CUSTOMER } from "@constant";
import reactotron from "reactotron-react-native";
var screen = {};
Object.values(SCREEN_ROUTER_CUSTOMER).forEach(e => {
  Object.assign(screen, {
    [e]: {}
  });
});
const INITIAL_STATE = screen;

export default (state = INITIAL_STATE, action) => {
  switch (action.type) {
    case STATE:
      const key = action.routeName;
      return {
        ...state,
        [key]: Object.assign(state[key], action.payload)
      };

    default:
      return state;
  }
};

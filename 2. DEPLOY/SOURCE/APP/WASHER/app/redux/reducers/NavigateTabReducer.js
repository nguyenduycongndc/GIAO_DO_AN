import { NAVIGATE_TAB } from "@action/type";
import { RESET } from "./index";

const initialState = {
  data: {},
  initialPage: 0
};

export default function(state = initialState, action) {
  switch (action.type) {
    case NAVIGATE_TAB: {
      return {
        ...state,
        initialPage: action.payload
      };
    }
    case RESET: {
      return initialState;
    }
    default:
      return state;
  }
}

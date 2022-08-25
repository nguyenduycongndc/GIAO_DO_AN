import { NAVIGATE_TAB } from "@action/type";

const initialState = {
  initialPage: 1
};

export default function(state = initialState, action) {
  switch (action.type) {
    case NAVIGATE_TAB: {
      return {
        ...state,
        initialPage: action.payload
      };
    }
    default:
      return state;
  }
}

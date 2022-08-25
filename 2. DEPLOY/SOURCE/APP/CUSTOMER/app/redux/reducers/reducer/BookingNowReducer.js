import { SEND_BOOKING_NOW, CLEAR_BOOKING_NOW } from "@action/type";

const INITIAL_STATE = {
  type: ""
};

export default (state = INITIAL_STATE, action) => {
  switch (action.type) {
    case SEND_BOOKING_NOW:
      return {
        ...state,
        type: action.payload.type
      };
    case CLEAR_BOOKING_NOW:
      return INITIAL_STATE;
    default:
      return state;
  }
};

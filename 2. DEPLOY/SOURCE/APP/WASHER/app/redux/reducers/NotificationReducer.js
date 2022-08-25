import {
  GET_NOTIFICATION,
  GET_NOTIFICATION_SUCCESS,
  GET_NOTIFICATION_FAIL
} from "@action/type";
import { RESET } from "./index";

const initialState = {
  data: [],
  isLoading: true,
  error: null
};

export default function(state = initialState, action) {
  switch (action.type) {
    case GET_NOTIFICATION: {
      return { ...state, isLoading: true, error: null };
    }
    case GET_NOTIFICATION_SUCCESS: {
      return { isLoading: false, data: action.payload, error: null };
    }
    case GET_NOTIFICATION_FAIL: {
      return { isLoading: false, error: action.payload };
    }
    case RESET: {
      return initialState;
    }
    default:
      return state;
  }
}

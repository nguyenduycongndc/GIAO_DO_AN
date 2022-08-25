import {
  GET_LIST_ORDER_HISTORY_SERVICE,
  GET_LIST_ORDER_HISTORY_SERVICE_FAIL,
  GET_LIST_ORDER_HISTORY_SERVICE_SUCCESS
} from "../actions/type";
import { RESET } from "./index";
const initialState = {
  data: [],
  isLoading: true,
  isLastPage: false,
  isLoadMore: false,
  error: null
};

export default function(state = initialState, action) {
  switch (action.type) {
    case GET_LIST_ORDER_HISTORY_SERVICE: {
      const data = state.data;
      return {
        ...state,
        error: null,
        isLoading: action.payload.page == 1 || !data.length,
        isLoadMore: data.length > 0 && action.payload.page != 1,
        isLastPage: false
      };
    }
    case GET_LIST_ORDER_HISTORY_SERVICE_SUCCESS: {
      if ((action.body.page && action.body.page == 1) || !action.body.page) {
        state = {
          ...state,
          data: action.payload,
          isLoading: false,
          isLoadMore: false,
          isLastPage: false,
          error: null
        };
      } else if (!!action.payload.length)
        state = {
          ...state,
          data: state.data.concat(action.payload),
          isLoading: false,
          isLoadMore: false,
          isLastPage: false,
          error: null
        };
      if (!action.payload.length)
        state = {
          ...state,
          isLoading: false,
          isLoadMore: false,
          isLastPage: true,
          error: null
        };
      return state;
    }
    case GET_LIST_ORDER_HISTORY_SERVICE_FAIL: {
      return {
        ...state,
        isLoading: false,
        isLoadMore: false,
        error: action.payload
      };
    }
    case RESET: {
      return initialState;
    }
    default:
      return state;
  }
}

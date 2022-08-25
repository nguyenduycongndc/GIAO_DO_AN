import {
  GET_USER,
  GET_USER_SUCCESS,
  GET_USER_FAIL,
  CHANGE_AVATAR,
  CHANGE_AVATAR_SUCCESS,
  CHANGE_AVATAR_FAIL,
  CREATE_BANK,
  CREATE_BANK_FAIL,
  CREATE_BANK_SUCCESS,
  DELETE_BANK,
  DELETE_BANK_FAIL,
  DELETE_BANK_SUCCESS,
  UPDATE_USER_INFO,
  UPDATE_USER_INFO_FAIL,
  UPDATE_USER_INFO_SUCCESS,
  TRANSFER_MONEY,
  TRANSFER_MONEY_FAIL,
  TRANSFER_MONEY_SUCCESS
} from "../actions/type";
import { RESET } from "./index";

const initialState = {
  data: {},
  isLoading: true,
  error: null
};

export default function(state = initialState, action) {
  switch (action.type) {
    case GET_USER:
    case DELETE_BANK:
    case CREATE_BANK:
    case TRANSFER_MONEY:
    case UPDATE_USER_INFO: {
      return { ...state, isLoading: true };
    }
    case GET_USER_SUCCESS:
    case CREATE_BANK_SUCCESS:
    case DELETE_BANK_SUCCESS:
    case UPDATE_USER_INFO_SUCCESS:
    case TRANSFER_MONEY_SUCCESS: {
      return {
        ...state,
        data: action.payload,
        isLoading: false,
        error: null
      };
    }
    case GET_USER_FAIL: {
      return {
        ...state,
        error: action.payload,
        isLoading: false
      };
    }
    case CREATE_BANK_FAIL: {
      return {
        ...state,
        isLoading: false
      };
    }
    case DELETE_BANK_FAIL: {
      return {
        ...state,
        isLoading: false
      };
    }
    case UPDATE_USER_INFO_FAIL: {
      return {
        ...state,
        isLoading: false
      };
    }
    case TRANSFER_MONEY_FAIL: {
      return {
        ...state,
        isLoading: false
      };
    }
    case CHANGE_AVATAR: {
      return { ...state, isLoading: true };
    }
    case CHANGE_AVATAR_SUCCESS: {
      return {
        ...state,
        isLoading: false,
        data: {
          ...state.data,
          urlAvatar: action.payload
        }
      };
    }
    case CHANGE_AVATAR_FAIL: {
      return {
        ...state,
        error: action.payload,
        isLoading: false
      };
    }
    case RESET: {
      return initialState;
    }
    default:
      return state;
  }
}

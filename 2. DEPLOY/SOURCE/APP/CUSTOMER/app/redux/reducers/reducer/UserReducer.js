import {
  GET_USER,
  GET_USER_SUCCESS,
  GET_USER_FAIL,
  CHANGE_AVATAR,
  CHANGE_AVATAR_SUCCESS,
  CHANGE_AVATAR_FAIL,
  REQUEST_LOGIN,
  REQUEST_LOGIN_FAIL,
  REQUEST_LOGIN_SUCCESS
} from "@action/type";
import analytics from '@react-native-firebase/analytics';

const initialState = {
  data: {},
  isLoading: false,
  error: null
};

export default function(state = initialState, action) {
  switch (action.type) {
    case REQUEST_LOGIN:
    case GET_USER: {
      return { ...state, isLoading: true };
    }
    case REQUEST_LOGIN_SUCCESS:
    case GET_USER_SUCCESS: {
      const payload = action.payload
      analytics().logEvent('login_success', {
        id: payload.memberID,
        phone: payload.phone,
        name: payload.phone
      })
      return {
        ...state,
        data: payload,
        isLoading: false,
        error: null
      };
    }
    case REQUEST_LOGIN_FAIL:
    case GET_USER_FAIL: {
      return {
        ...state,
        error: action.payload,
        isLoading: false
      };
    }
    case CHANGE_AVATAR: {
      return { ...state, isLoading: true };
    }
    case CHANGE_AVATAR_SUCCESS: {
      return {
        ...state,
        data: {
          ...state.data,
          result: {
            ...state.data.result,
            urlAvatar: action.payload.result
          }
        },
        isLoading: false,
        error: null
      };
    }
    case CHANGE_AVATAR_FAIL: {
      return {
        ...state,
        error: action.payload,
        isLoading: false
      };
    }
    default:
      return state;
  }
}

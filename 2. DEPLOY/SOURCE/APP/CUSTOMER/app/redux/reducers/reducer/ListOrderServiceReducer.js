import {
  GET_LIST_ORDER_SERVICE,
  GET_LIST_ORDER_SERVICE_FAIL,
  GET_LIST_ORDER_SERVICE_SUCCESS
} from "@app/redux/actions/type";
import { ORDER_TYPE } from "@constant";
import reactotron from "reactotron-react-native";
const initialState = {
  data: [],
  isLoading: true,
  error: null,
  dataProcessing: [],
  dataUpcoming: [],
  dataHistory: [],
  isLoadingProcessing: true,
  isLoadingUpcoming: true,
  isLoadingHistory: true,
  errorProcessing: null,
  errorUpcoming: null,
  errorHistory: null
};

export default function(state = initialState, action) {
  switch (action.type) {
    case GET_LIST_ORDER_SERVICE: {
      switch (action.payload.status) {
        case ORDER_TYPE.ORDER_HISTORY:
        case ORDER_TYPE.ORDER_STATUS_COMPLETE:
        case ORDER_TYPE.ORDER_STATUS_CANCEL:
          return { ...state, isLoadingHistory: true };
        case ORDER_TYPE.ORDER_STATUS_CONFIRM:
          return { ...state, isLoadingUpcoming: true };
        case ORDER_TYPE.ORDER_STATUS_WASHING:
          return { ...state, isLoadingProcessing: true };
      }
      return { ...state, isLoading: true };
    }
    case GET_LIST_ORDER_SERVICE_SUCCESS: {
      switch (action.params.status) {
        case ORDER_TYPE.ORDER_HISTORY:
        case ORDER_TYPE.ORDER_STATUS_COMPLETE:
        case ORDER_TYPE.ORDER_STATUS_CANCEL:
          return {
            ...state,
            isLoadingHistory: false,
            errorHistory: null,
            dataHistory: action.payload
          };
        case ORDER_TYPE.ORDER_STATUS_CONFIRM:
          return {
            ...state,
            isLoadingUpcoming: false,
            errorUpcoming: null,
            dataUpcoming: action.payload
          };
        case ORDER_TYPE.ORDER_STATUS_WASHING:
          return {
            ...state,
            isLoadingProcessing: false,
            errorProcessing: null,
            dataProcessing: action.payload
          };
      }
      return {
        ...state,
        isLoading: false,
        error: null,
        data: action.payload
      };
    }
    case GET_LIST_ORDER_SERVICE_FAIL: {
      switch (action.payload.status) {
        case ORDER_TYPE.ORDER_HISTORY:
        case ORDER_TYPE.ORDER_STATUS_COMPLETE:
        case ORDER_TYPE.ORDER_STATUS_CANCEL:
          return {
            ...state,
            errorHistory: action.payload,
            isLoadingHistory: false
          };
        case ORDER_TYPE.ORDER_STATUS_CONFIRM:
          return {
            ...state,
            errorUpcoming: action.payload,
            isLoadingUpcoming: false
          };
        case ORDER_TYPE.ORDER_STATUS_WASHING:
          return {
            ...state,
            errorProcessing: action.payload,
            isLoadingProcessing: false
          };
      }
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

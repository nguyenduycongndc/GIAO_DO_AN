import {
  GET_LIST_ORDER_SERVICE,
  GET_LIST_ORDER_SERVICE_FAIL,
  GET_LIST_ORDER_SERVICE_SUCCESS,
  UPDATE_PROCESSING_IMAGE
} from "@app/redux/actions/type";
import { ORDER_STATUS } from "@app/constants/Constants";
import { RESET } from "./index";
const initialState = {
  data: {},
  isLoading: true,
  error: null
};
import reactotron from "reactotron-react-native";

const findAndUpdateImage = (payload, state) => {
  let listImageRequireTMP = state.data.listImageRequire;
  const { isBefore, index, data } = payload;
  if (isBefore) {
    listImageRequireTMP[index].before = data;
  } else {
    listImageRequireTMP[index].after = data;
  }

  return listImageRequireTMP;
};

export default function(state = initialState, action) {
  // console.log(action.params);

  switch (action.type) {
    case GET_LIST_ORDER_SERVICE: {
      return { ...state, isLoading: true };
    }
    case GET_LIST_ORDER_SERVICE_SUCCESS: {
      return {
        ...state,
        isLoading: false,
        error: null,
        data: action.payload
      };
    }
    case GET_LIST_ORDER_SERVICE_FAIL: {
      return {
        ...state,
        error: action.payload,
        isLoading: false
      };
    }

    case UPDATE_PROCESSING_IMAGE: {
      return {
        ...state,
        data: {
          ...state.data,
          listImageRequire: findAndUpdateImage(action.payload, state)
        }
      };
    }
    case RESET: {
      return initialState;
    }
    default:
      return state;
  }
}

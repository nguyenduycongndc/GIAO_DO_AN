import {
  GET_CAR_DETAIL,
  GET_CAR_DETAIL_FAIL,
  GET_CAR_DETAIL_SUCCESS
} from "@app/redux/actions/type";

const initialState = {
  data: {},
  listImage: [],
  isLoading: false,
  error: null,
  refreshing: false
};

export default function(state = initialState, action) {
  switch (action.type) {
    case GET_CAR_DETAIL: {
      return {
        ...state,
        isLoading: true,
        refreshing: true,
        error: null
      };
    }
    case GET_CAR_DETAIL_SUCCESS: {
      return {
        ...state,
        isLoading: false,
        refreshing: false,
        error: null,
        data: action.payload,
        listImage: action.payload.listImage.map(image => {
          return image.url;
        })
      };
    }
    case GET_CAR_DETAIL_FAIL: {
      return {
        ...state,
        isLoading: false,
        error: action.payload,
        refreshing: false,
        data: {},
        listImage: []
      };
    }
    default:
      return state;
  }
}

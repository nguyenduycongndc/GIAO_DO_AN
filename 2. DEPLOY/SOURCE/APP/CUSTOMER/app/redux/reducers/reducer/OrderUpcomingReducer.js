import { GET_ORDER_UPCOMING } from "@app/redux/actions/type";
import { fail, success } from "@app/utils/SagaHelper";
const initialState = {
  data: [],
  isLoading: true,
  isLastPage: false,
  isLoadMore: false,
  error: null
};

export default function(state = initialState, action) {
  switch (action.type) {
    case GET_ORDER_UPCOMING: {
      const data = state.data;
      return {
        ...state,
        error: null,
        isLoading: action.payload.page == 1 || !data.length,
        isLoadMore: data.length > 0,
        isLastPage: false
      };
    }
    case success(GET_ORDER_UPCOMING): {
      if (action.params.page == 1) {
        state = {
          ...state,
          data: action.payload,
          isLoading: false,
          isLoadMore: false,
          isLastPage: false,
          error: null
        };
      } else if (action.payload.length > 0) {
        state = {
          ...state,
          data: state.data.concat(action.payload),
          isLoading: false,
          isLoadMore: false,
          isLastPage: false,
          error: null
        };
      }
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
    case fail(GET_ORDER_UPCOMING): {
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

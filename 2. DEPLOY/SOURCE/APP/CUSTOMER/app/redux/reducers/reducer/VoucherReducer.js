import { SEND_VOUCHER, CLEAR_VOUCHER } from "@action/type";

const INITIAL_STATE = {
  list: [],
  code: "",
  discount: 0,
  typeDiscount: 2
};

export default (state = INITIAL_STATE, action) => {
  switch (action.type) {
    case SEND_VOUCHER:
      const {
        code,
        discount,
        list = state.list,
        typeDiscount
      } = action.payload;
      return { ...state, code, discount, list, typeDiscount };
    case CLEAR_VOUCHER:
      return INITIAL_STATE;
    default:
      return state;
  }
};

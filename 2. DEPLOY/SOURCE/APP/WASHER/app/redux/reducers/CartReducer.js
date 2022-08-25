import { UPDATE_CART } from "@action/type";
import callAPI from "@app/utils/CallApiHelper";
import { UpdateCart } from "@app/constants/Api";
const initialState = {
  data: {}
};

export default function(state = initialState, action) {
  switch (action.type) {
    case UPDATE_CART: {
      var { data } = state;
      const { payload, quantity = 1 } = action;
      const { productID } = payload;
      const index = Object.keys(data).indexOf(productID + "");

      if (index != -1) {
        if (data[productID].quantity + quantity <= 0) {
          delete data[productID];
          callAPI(UpdateCart, { productID, type: 0 });
        } else
          data = {
            ...data,
            [productID]: {
              ...data[productID],
              quantity: data[productID].quantity + quantity
            }
          };
      } else {
        data = Object.assign(data, {
          [productID]: {
            ...payload,
            quantity: 1
          }
        });
        callAPI(UpdateCart, { productID, type: 1 });
      }
      return { ...state, data };
    }
    default:
      return state;
  }
}

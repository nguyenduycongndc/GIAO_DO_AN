import { REDUCER_CUSTOM } from "@constant";
module.exports = {
  [REDUCER_CUSTOM.USER]: require("./UserReducer").default,
  [REDUCER_CUSTOM.SELECT_LOCATION]: require("./LocationSelectReducer").default,
  [REDUCER_CUSTOM.LIST_ORDER_SERVICE]: require("./ListOrderServiceReducer")
    .default,
  [REDUCER_CUSTOM.VOUCHER]: require("./VoucherReducer").default,
  [REDUCER_CUSTOM.BOOKING_NOW]: require("./BookingNowReducer").default,
  [REDUCER_CUSTOM.SHOW_MODAL]: require("./ShowModalReducer").default,
  [REDUCER_CUSTOM.CAR_DETAIL]: require("./GetCarDetailReducer").default,
  [REDUCER_CUSTOM.STATE]: require("./StateReducer").default,
  [REDUCER_CUSTOM.ORDER_UPCOMING]: require("./OrderUpcomingReducer").default,
  [REDUCER_CUSTOM.ORDER_HISTORY]: require("./OrderHistoryReducer").default,
  [REDUCER_CUSTOM.ORDER_PROCESSING]: require("./OrderProcessingReducer")
    .default,
  [REDUCER_CUSTOM.NAVIGATE_TAB]: require("./NavigateTabReducer").default
};

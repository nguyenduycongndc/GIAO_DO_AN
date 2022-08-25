const {
  GET_USER,
  GET_LIST_ORDER_SERVICE,
  GET_LIST_ORDER_UPCOMMING_SERVICE,
  GET_LIST_ORDER_HISTORY_SERVICE,
  GET_LIST_ORDER_PENDING_SERVICE,
  UPDATE_PROCESSING_IMAGE,
  CHANGE_AVATAR,
  CREATE_BANK,
  DELETE_BANK,
  UPDATE_USER_INFO,
  TRANSFER_MONEY,
  LOCATION_CHANGE,
  NAVIGATE_TAB,
  GET_NOTIFICATION,
  UPDATE_CART
} = require("./type");
module.exports = {
  locationChange: payload => ({
    type: LOCATION_CHANGE,
    payload: payload
  }),
  getUserInfo: () => ({
    type: GET_USER,
    payload: {}
  }),
  getListOrderService: payload => ({
    type: GET_LIST_ORDER_SERVICE,
    payload: payload
  }),
  getListOrderUpcommingService: payload => ({
    type: GET_LIST_ORDER_UPCOMMING_SERVICE,
    payload: payload
  }),
  getListOrderHistoryService: payload => ({
    type: GET_LIST_ORDER_HISTORY_SERVICE,
    payload: payload
  }),
  getListOrderPendingService: payload => ({
    type: GET_LIST_ORDER_PENDING_SERVICE,
    payload: payload
  }),
  updateProcessingImage: payload => ({
    type: UPDATE_PROCESSING_IMAGE,
    payload: payload
  }),
  changeAvatar: payload => ({
    type: CHANGE_AVATAR,
    payload: payload
  }),

  createBank: payload => ({
    type: CREATE_BANK,
    payload: payload
  }),

  deleteBank: payload => ({
    type: DELETE_BANK,
    payload: payload
  }),
  updateUser: payload => ({
    type: UPDATE_USER_INFO,
    payload: payload
  }),
  transferMoney: payload => ({
    type: TRANSFER_MONEY,
    payload: payload
  }),
  navigateTab: payload => ({
    type: NAVIGATE_TAB,
    payload
  }),
  getNotification: payload => ({
    type: GET_NOTIFICATION,
    payload
  }),
  updateCart: (payload, quantity) => ({
    type: UPDATE_CART,
    payload: {
      ...payload,
      type: quantity < 0 ? 0 : 1
    },
    quantity
  })
};

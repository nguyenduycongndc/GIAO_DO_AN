const {
  GET_USER,
  REQUEST_LOGIN,
  GET_LIST_ORDER_SERVICE,
  SEND_LOCATION_SELECT,
  CLEAR_LOCATION_SELECT,
  SEND_VOUCHER,
  STATE,
  SEND_BOOKING_NOW,
  CLEAR_BOOKING_NOW,
  GET_CAR_DETAIL,
  NAVIGATE_TAB
} = require("./type");
module.exports = {
  getUserInfoAction: callBack => ({
    type: GET_USER,
    payload: {},
    callBack
  }),

  requestLogin: payload => ({
    type: REQUEST_LOGIN,
    payload: payload
  }),

  getListOrderService: payload => ({
    type: GET_LIST_ORDER_SERVICE,
    payload: payload
  }),
  sendLocationSelect: (location, name, lon, lat) => ({
    type: SEND_LOCATION_SELECT,
    payload: {
      location: location,
      name: name,
      lon: lon,
      lat: lat
    }
  }),
  clearLocationSelect: () => ({
    type: CLEAR_LOCATION_SELECT,
    payload: {}
  }),
  sendVoucher: (typeDiscount, code, discount, list) => ({
    type: SEND_VOUCHER,
    payload: {
      code,
      discount,
      list,
      typeDiscount
    }
  }),
  sendBookingNow: type => ({
    type: SEND_BOOKING_NOW,
    payload: {
      type: type
    }
  }),
  actionModal: (type, payload, isSubmit) => ({
    type,
    payload,
    isSubmit
  }),
  getCarDetail: payload => ({
    type: GET_CAR_DETAIL,
    payload: payload
  }),

  setState: (routeName, payload) => ({
    type: STATE,
    routeName,
    payload
  }),
  getOrder: (type, payload) => ({
    type,
    payload
  }),
  navigateTab: payload => ({
    type: NAVIGATE_TAB,
    payload
  })
};

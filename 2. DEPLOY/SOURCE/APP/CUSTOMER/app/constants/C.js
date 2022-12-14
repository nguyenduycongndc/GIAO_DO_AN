const SCREEN_ROUTER_AUTH = require("./screen/ScreenRouterAuth");
const SCREEN_ROUTER_CUSTOMER = require("./screen/ScreenRouteCustomer");
const REDUCER_CUSTOM = require("./reducer");
const ASYNC_STORAGE = require("./asyncStorage");
const SCREEN_ROUTER_INTRO = require("./screen/ScreenRouterIntro");

const CHANNEL = {
  FAKE_APPLE: 0,
  PRODUCT: 1
};

module.exports = {
  SCREEN_ROUTER_INTRO,
  ASYNC_STORAGE,
  SCREEN_ROUTER_AUTH,
  SCREEN_ROUTER_CUSTOMER,
  REDUCER_CUSTOM,
  TYPE_INFO: {
    NAME_PHONE: 1,
    ADDRESS: 2,
    TIME: 3
  },
  TYPE_ORDER: {
    HISTORY: 1,
    UPCOMING: 2,
    PROCESSING: 3
  },
  PAYMENT_METHOD: {
    CASH: 1,
    VNPAY: 2
  },
  BOOKING: {
    HISTORY: 1
  },
  TYPE_SERVICE: {
    MAIN: 1,
    ADDITION: 2
  },
  TYPE_COUPON: {
    UNWORN: 1,
    USED: 2
  },
  TYPE_NAVIGATION: {
    HOME: 1,
    BOOKING: 2,
    BOOKING_NOW: 3
  },
  TYPE_NOTI: {
    NOTI_ORDER_STATUS_WAITING: 1,
    NOTI_ORDER_STATUS_CONFIRM: 2,
    NOTI_ORDER_STATUS_PROCESS: 3,
    NOTI_ORDER_STATUS_WASHING: 4,
    NOTI_ORDER_STATUS_CANCEL: 0,
    NOTI_WALLET_NEED_RECHARGE: 5,
    NOTI_HAVE_NEWS: 6,
    NOTI_TRANSACTION_WALLET: 7,
    NOTI_TRANSACTION_SUCCESS: 8,
    NOTI_TRANSACTION_FAIL: 12,
    NOTI_UPLOAD_IMAGE: 9,
    NOTI_ADMIN_SEND: 10
  },
  ORDER_TYPE: {
    ORDER_STATUS_CANCEL: 0,
    ORDER_STATUS_WAITING: 1,
    ORDER_STATUS_CONFIRM: 2,
    ORDER_STATUS_COMPLETE: 3,
    ORDER_STATUS_WASHING: 5,
    ORDER_STATUS_COMFIRM_WASHING: 6,
    ORDER_STATUS_NO_CONFIRM: 4,
    ORDER_STATUS_SEARCH_WASHER: 9,
    ORDER_HISTORY: null
  },
  DEEP_LINK: {
    GMAIL: 1,
    MESS_FB: 2,
    ZALO: 3,
    FACEBOOK: 4,
    TWITTER: 5,
    INSTA: 6
  },
  UPLOAD_IMAGE_TYPE: {
    CAR: 1,
    PARK: 2
  },
  TYPE_GET_OTP: {
    REGISTER: 1,
    FORGOT_PASS: 2
  },
  TYPE_REASON: {
    CANCEL: 1,
    REQUEST_SERVICE: 3,
    NOTE_FOR_MASTER: 2
  },
  TYPE_DISCOUNT: {
    PERCENT: 1,
    NORMAL: 2
  },
  RELEASE_CHANNEL:
    __DEV__ || Platform.OS !== "ios" ? CHANNEL.PRODUCT : CHANNEL.PRODUCT,
  CHANNEL: CHANNEL,
  VNPAY_MINIUM_MONEY: 10000,
  URL_SERVER: {
    WINDS: "http://winds.hopto.org/",
    RELEASE: "http://118.27.192.110/",
    TEST: "http://winds.hopto.org:8517/"
  }
};

const SCREEN_ROUTER_AUTH = require("./screen/ScreenRouterAuth");
const SCREEN_ROUTER_WASHER = require("./screen/ScreenRouterWasher");
const SCREEN_ROUTER_INTRO = require("./screen/ScreenRouterIntro");
const REDUCER_WASHER = require("./reducer/ReducerWasher");
const ASYNC_STORAGE = require("./asyncStorage");
const { Platform } = require("react-native");

const CHANNEL = {
  FAKE_APPLE: 0,
  PRODUCT: 1
};

module.exports = {
  SCREEN_ROUTER_AUTH,
  SCREEN_ROUTER_WASHER,
  SCREEN_ROUTER_INTRO,
  REDUCER_WASHER,
  ASYNC_STORAGE,
  TYPE_INFO: {
    NAME_PHONE: 1,
    ADDRESS: 2,
    TIME: 3
  },
  PAYMENT_METHOD: {
    CASH: 1,
    VNPAY: 2
  },
  ORDER_STATUS: {
    REJECTED: 0, // KHÔNG NHẬN ĐƠN
    PENDING: 1, // ĐANG ƯU TIÊN CHO MÌNH
    PENDING_ALL: 9, // TẤT CẢ WASHER ĐỀU NHÌN THẤY ĐƠN NÀY
    PENDING_ADMIN: 4, // KHÔNG CÓ AI NHẬN ĐƠN TRONG 1 KHOẢNG THỜI GIAN THÌ BÁO CHO ADMIN
    CONFIRMED: 2, // ĐƠN ĐÃ ĐƯỢC WASHER NHẬN
    COMPLETED: 3, // ĐƠN ĐÃ HOÀN THÀNH
    WASHING: 5, // BẮT ĐẦU RỬA XE SAU KHI CẬP NHẬT ẢNH TRƯỚC KHI RỬA
    START: 6, // NHẤN NÚT BẮT ĐẦU RỬA,
    CANCEL_BOOKING_NOW: -1
  },
  DEEP_LINK: {
    GMAIL: 1,
    MESS_FB: 2,
    ZALO: 3,
    FACEBOOK: 4,
    TWITTER: 5,
    INSTA: 6
  },
  TRANSFER_TYPE: {
    CASH: 2,
    VNPAY: 1
  },
  IMAGE_TYPE: {
    CAR: 1
  },
  WALLET: {
    INCOME: 2,
    DEPOSIT: 1
  },
  TYPE_GET_OTP: {
    REGISTER: 1,
    FORGOT_PASS: 2
  },
  REG_PHONE: /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/,
  REG_EMAIL: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
  HISTORY_WITHDRAW_ITEM_TYPE: {
    RECEIVE_MONEY: 1,
    DEDUCTED: 2
  },
  TYPE_NOTIFICATION: {
    REVIEWS: 11,
    TRANSACTION_WALLET: 7,
    BOOKING: 2,
    VN_PAY_SUCCESS: 8,
    VN_PAY_FAILED: 12
  },
  TYPE_ORDER_OF: {
    MINE: 1,
    THEIR: 0
  },
  TYPE_LIST_REASON_CANCEL: 4,
  TYPE_ROLE: {
    CUSTOMER: 1,
    WASHER: 2
  },
  RELEASE_CHANNEL:
    __DEV__ || Platform.OS !== "ios" ? CHANNEL.PRODUCT : CHANNEL.PRODUCT,
  CHANNEL: CHANNEL,
  RESULT_VN_PAY: {
    SUCCESS: "success",
    FAILED: "failed"
  },
  RESIZE_IMAGE: {
    WIDTH: 720,
    HEIGHT: 1280
  },
  URL_SERVER: {
    WINDS: "http://winds.hopto.org/",
    RELEASE: "http://118.27.192.110/",
    TEST: "http://winds.hopto.org:8517/"
  },
  CHANNEL,
  ORDER_PRODUCTION_STATUS: {
    ALL: 0,
    WAITING: 2,
    CONFIRM: 3
  }
};

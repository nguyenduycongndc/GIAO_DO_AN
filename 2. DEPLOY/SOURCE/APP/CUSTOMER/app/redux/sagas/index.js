const { call } = require("redux-saga/effects");
const {
  GET_USER,
  GET_LIST_ORDER_SERVICE,
  REQUEST_LOGIN,
  GET_CAR_DETAIL,
  GET_ORDER_UPCOMING,
  GET_ORDER_PROCESSING,
  GET_ORDER_HISTORY
} = require("@app/redux/actions/type");
const {
  getListOrderService,
  getUserInfo,
  getCarDetail,
  requestLogin
} = require("@api");
const NavigationUtil = require("@app/navigation/NavigationUtil").default;
const { SCREEN_ROUTER_AUTH, ASYNC_STORAGE } = require("@constant");
const AsyncStorage = require("@react-native-community/async-storage").default;
const SagaHelper = require("@app/utils/SagaHelper").default;

const NetworkSaga = {
  watchGetUser: SagaHelper(GET_USER, getUserInfo),
  watchGetOrderUpcoming: SagaHelper(GET_ORDER_UPCOMING, getListOrderService),
  watchGetOrderHistory: SagaHelper(GET_ORDER_HISTORY, getListOrderService),
  watchGetOrderProcessing: SagaHelper(
    GET_ORDER_PROCESSING,
    getListOrderService
  ),
  watchGetCarDetail: SagaHelper(GET_CAR_DETAIL, getCarDetail),
  watchGetListOrderService: SagaHelper(
    GET_LIST_ORDER_SERVICE,
    getListOrderService
  ),
  watchRequestLogin: SagaHelper(
    REQUEST_LOGIN,
    requestLogin,
    () => {},
    () => {},
    (action, res) =>
      AsyncStorage.setItem(
        ASYNC_STORAGE.TOKEN,
        res.result.token.toString(),
        err => NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH_LOADING)
      )
  )
};

module.exports = function* customerSaga() {
  for (let [key, value] of Object.entries(NetworkSaga)) yield value;
};

const axios = require("axios");
const AsyncStorage = require("@react-native-community/async-storage").default;
const { showMessages } = require("@app/components/Alert");
const NavigationUtil = require("../navigation/NavigationUtil").default;
const analytics = require("@react-native-firebase/analytics").default;
const key_billing = "AIzaSyCmeRtaN1l1mhfL6l7ExCjmi1eVzdh58m4";
const { ASYNC_STORAGE, SCREEN_ROUTER_AUTH, URL_SERVER } = require("@constant");
const { default: reactotron } = require("reactotron-react-native");
const R = require("@R").default;
function createAxios() {
  var axiosInstant = axios.create();
  // axiosInstant.defaults.baseURL = __DEV__
  //   ? "http://winds.hopto.org/"
  //   : "http://118.27.192.110/";
  axiosInstant.defaults.baseURL = URL_SERVER.TEST;
  axiosInstant.defaults.timeout = 20000;
  axiosInstant.defaults.headers = {
    "Content-Type": "application/json"
  };

  axiosInstant.interceptors.request.use(
    async config => {
      config.headers.token = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
      config.headers.lang = await AsyncStorage.getItem(ASYNC_STORAGE.LANG);
      return config;
    },
    error => Promise.reject(error)
  );

  axiosInstant.interceptors.response.use(response => {
    if (response.data && response.data.code == 403) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().require_login_againt,
        () =>
          AsyncStorage.removeItem(ASYNC_STORAGE.TOKEN, () => {
            const store = require("@app/redux/store").default;
            store.dispatch({ type: "reset" });
            NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH_LOADING);
          })
      );
    } else if (response.data && response.data.status != 1)
      showMessages(R.strings().notif_tab_cus, response.data.message);
    return response;
  });
  return axiosInstant;
}

const getAxios = createAxios();

function getApiName(url, baseURL) {
  return url.substr(baseURL.length, url.length);
}

/* Support function */
function handleResult(api) {
  return api
    .then(res => {
      if (res.data.status != 1) {
        analytics().logEvent("api_error", {
          api: getApiName(res.config.url, res.config.baseURL),
          token: res.request._headers.token,
          message: res.data.errorMsg
        });
        return Promise.reject(new Error("Status trả về = " + res.data.status));
      }
      return Promise.resolve(res.data);
    })
    .catch(error => {
      return Promise.reject(new Error(error.message));
    });
}

module.exports = {
  requestLogin: payload =>
    handleResult(
      getAxios.post("api/Service/Login", {
        Phone: payload.Phone,
        PassWord: payload.PassWord,
        deviceID: payload.deviceID || "",
        role: 1
      })
    ),
  requestRegister: payload =>
    handleResult(
      getAxios.post("api/Service/Register", {
        Phone: payload.Phone,
        deviceID: payload.deviceID || ""
      })
    ),
  requestConfirmOTP: payload =>
    handleResult(
      getAxios.post("api/Service/ConfirmOTP", {
        Phone: payload.Phone,
        OTP: payload.OTP || ""
      })
    ),
  createOrder: payload =>
    handleResult(getAxios.post("api/Service/CreateOrderService", payload)),
  addCar: payload => handleResult(getAxios.post("api/Service/AddCar", payload)),
  UpdateCar: payload =>
    handleResult(getAxios.post("api/Service/UpdateCar", payload)),
  updateUserInfo: payload =>
    handleResult(getAxios.post("api/Service/UpdateCustomer", payload)),
  requestHomeData: (deviceID = "") =>
    handleResult(getAxios.get(`api/Service/GetHomeScreen`)),
  requestLogout: () => handleResult(getAxios.get(`api/Service/Logout`)),
  getUserInfo: () => handleResult(getAxios.get(`api/Service/GetUserInfor`)),
  requestNewsData: cateID =>
    handleResult(getAxios.get(`api/Service/GetNews?cateID=${cateID}`)),
  requestAddCar: note =>
    handleResult(getAxios.get(`api/Service/RequestAddCar?note=${note}`)),
  checkArea: placeID =>
    handleResult(getAxios.get(`api/Service/checkArea?placeID=${placeID}`)),
  requestNewsDetail: newsID =>
    handleResult(getAxios.get(`api/Service/GetNewsDetail?newID=${newsID}`)),
  getListCoupon: type =>
    handleResult(getAxios.get(`api/Service/GetListCounpon?Type=${type}`)),
  getPointHistory: FromDate =>
    handleResult(
      getAxios.get(`api/Service/GetPointHistory?FromDate=${FromDate}`)
    ),
  checkAgentByCode: agentCode =>
    handleResult(
      getAxios.get(`api/Service/CheckAgentByCode?agentCode=${agentCode}`)
    ),
  getCarDetail: carID =>
    handleResult(getAxios.get(`api/Service/GetCarDetail?carID=${carID}`)),
  getShiftTime: () =>
    handleResult(getAxios.get(`api/Service/GetListShiftTime`)),
  getQA: () => handleResult(getAxios.get(`api/Service/GetQA`)),
  getNotify: () => handleResult(getAxios.get(`api/Service/GetNotify`)),
  getListService: ({ carID, type }) =>
    handleResult(
      getAxios.get(`api/Service/GetListService?carID=${carID}&type=${type}`)
    ),
  getOTPByPhone: ({ phone, type }) =>
    handleResult(
      getAxios.get(`api/Service/GetOTPByPhone?phone=${phone}&type=${1}`)
    ),
  GetListCarModeAndBrand: ({ search, carBrandID }) =>
    handleResult(
      getAxios.get(
        `api/Service/GetListCarModeAndBrand?search=${search}&carBrandID=${carBrandID}`
      )
    ),

  searchPlaceAutoComplete: keySearch =>
    axios.get(
      `https://maps.googleapis.com/maps/api/place/autocomplete/json?input=${keySearch}&key=${key_billing}&language=vi&components=country:vn`
    ),

  getDetailPlace: idPlace =>
    axios.get(
      `https://maps.googleapis.com/maps/api/place/details/json?placeid=${idPlace}&key=${key_billing}&language=vi`
    ),
  nearBySearch: loc =>
    axios.get(
      `https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=${
      loc.latitude
      },${loc.longitude}&key=${key_billing}&language=vi&rankby=distance`
    ),
  getListOrderService: payload =>
    handleResult(
      getAxios.get(
        `api/Service/GetListOrderService?status=${payload.status}&placeID=${
        payload.placeID
        }&fromDate=${payload.fromDate}&toDate=${payload.toDate}&distance=${
        payload.distance
        }&page=${payload.page}&serviceID=${payload.serviceID ||
        ""}&comboID=${payload.comboID || ""}`
      )
    ),
  getOrderServiceDetail: payload =>
    handleResult(
      getAxios.get(
        `api/Service/GetOrderServiceDetail?orderServiceID=${
        payload.orderServiceID
        }&placeID=${payload.placeID || ""}`
      )
    ),
  uploadAvatar: payload =>
    handleResult(getAxios.post("api/Service/ChangeAvatar", payload)),
  uploadCarImage: payload =>
    handleResult(
      getAxios.post(
        `api/Service/UploadCarImage?carID=${payload.carID}`,
        payload.data
      )
    ),
  getConfig: () => handleResult(getAxios.get("api/Service/GetConfig")),
  changeStatusOrder: payload =>
    handleResult(
      getAxios.get(
        `api/Service/ChangeStatus?orderServiceID=${
        payload.orderServiceID
        }&status=${payload.status}&reason=${payload.reason || ""}`
      )
    ),
  checkCreateOrder: payload =>
    handleResult(
      getAxios.get(
        `api/Service/CheckCreateOrder?carID=${payload.carID ||
        0}&placeID=${payload.placeID ||
        ""}&bookingDateInput=${payload.BookingDateInput ||
        ""}&isBookingNow=${payload.isBookingNow ||
        0}&agentCode=${payload.agentCode ||
        ""}&couponCode=${payload.couponCode || ""}`
      )
    ),
  ChangePass: payload =>
    handleResult(getAxios.post(`api/Service/ChangePass`, payload)),
  GetServiceFilter: () =>
    handleResult(getAxios.get(`api/Service/GetServiceFilter`)),
  UploadCarImageService: ({ orderServiceID, type, note, images }) =>
    handleResult(
      getAxios.post(
        `api/Service/UploadCarImageService?orderServiceID=${orderServiceID}&type=${type}&note=${note}`,
        images
      )
    ),
  CreateReview: payload =>
    handleResult(getAxios.post(`api/Service/CreateReview`, payload)),
  GetListIntro: (type = 1) =>
    handleResult(getAxios.get(`api/Service/GetListIntro?type=${type}`)),
  AddLocation: (payload = { Name: "", PlaceID: "", CustomerAddress: "" }) =>
    handleResult(getAxios.post(`api/Service/AddLocation`, payload)),
  UpdateLocation: (
    payload = { Name: "", PlaceID: "", ID: "", CustomerAddress: "" }
  ) => handleResult(getAxios.post(`api/Service/UpdateLocation`, payload)),
  DeleteLocation: ID =>
    handleResult(getAxios.get(`api/Service/DeleteLocation?ID=${ID}`)),
  GetUrlVNPay: transactionID =>
    handleResult(
      getAxios.get(`api/Service/GetUrlVNPay?transactionID=${transactionID}`)
    ),
  GetContentReason: type =>
    handleResult(getAxios.get(`api/Service/GetContentReason?type=${type}`)),
  DeleteCarCustomer: carID =>
    handleResult(getAxios.get(`api/Service/DeleteCarCustomer?carID=${carID}`)),
  updateDeviceID: deviceID =>
    handleResult(
      getAxios.get(`api/Service/UpdateDeviceID?deviceID=${deviceID}`)
    ),
  GetOrderComboSevice: comboCode =>
    handleResult(
      getAxios.get(`api/Service/GetOrderComboSevice?comboCode=${comboCode}`)
    ),
  ChangeLang: () => handleResult(getAxios.get(`api/Service/ChangeLang`)),
  UpdateRefCode: (code) => handleResult(getAxios.get(`api/Service/UpdateRefCode?refCode=${code}`)),
};

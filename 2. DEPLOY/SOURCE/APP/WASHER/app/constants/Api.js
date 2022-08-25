const axios = require("axios");
// const AsyncStorage = require("react-native").AsyncStorage;
const AsyncStorage = require("@react-native-community/async-storage").default;
const Alert = require("react-native").Alert;
const NavigationUtil = require("../navigation/NavigationUtil").default;
const I18n = require("@i18").default;
const R = require("@R").default;
const key_billing = "AIzaSyCkiN4hoPdWkYCX9FRFvBTAwyjFNfionh8";
const {
  ASYNC_STORAGE,
  SCREEN_ROUTER_AUTH,
  URL_SERVER
} = require("@app/constants/Constants");

function createAxios() {
  // AsyncStorage.setItem("token", "CE17FC77B94A8847B1C7F08F162B2F24");
  // AsyncStorage.clear();
  var axiosInstant = axios.create();
  axiosInstant.defaults.baseURL = URL_SERVER.TEST;
  axiosInstant.defaults.timeout = 20000;
  axiosInstant.defaults.headers = { "Content-Type": "application/json" };

  axiosInstant.interceptors.request.use(
    async config => {
      config.headers.token = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
      config.headers.lang = await AsyncStorage.getItem(ASYNC_STORAGE.LANG);
      // config.headers.lang = "vi"
      return config;
    },
    error => Promise.reject(error)
  );

  axiosInstant.interceptors.response.use(response => {
    if (response.data && response.data.code == 403) {
      setTimeout(() => {
        Alert.alert(R.strings().notice, I18n.t("relogin"));
      }, 100);
      const store = require("@app/redux/store").default;
      store.dispatch({ type: "reset" });
      AsyncStorage.setItem("token", "", () => {
        NavigationUtil.navigate(SCREEN_ROUTER_AUTH.LOGIN);
      });
    } else if (response.data && response.data.status != 1) {
      setTimeout(() => {
        Alert.alert(R.strings().notice, response.data.message);
      }, 100);
    }
    return response;
  });
  return axiosInstant;
}

const getAxios = createAxios();

/* Support function */
function handleResult(api) {
  return api.then(res => {
    if (res.data.status != 1) {
      return Promise.reject(new Error("Co loi xay ra"));
    }
    return Promise.resolve(res.data);
  });
}

module.exports = {
  requestLogin: payload =>
    handleResult(
      getAxios.post("api/Service/Login", {
        Phone: payload.Phone,
        PassWord: payload.PassWord,
        deviceID: payload.deviceID,
        role: payload.role
      })
    ),

  requestHomeData: (deviceID = "") =>
    handleResult(
      getAxios.get(`api/Service/GetHomeScreen?deviceID=${deviceID}`)
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
  getUserInfo: () => handleResult(getAxios.get(`api/Service/GetUserInfor`)),
  getNotification: () => handleResult(getAxios.get(`api/Service/GetNotify`)),
  getOrderServiceDetail: payload =>
    handleResult(
      getAxios.get(
        `api/Service/GetOrderServiceDetail?orderServiceID=${
          payload.orderServiceID
        }`
      )
    ),
  getListOrderService: ({
    status = "",
    placeID = "",
    fromDate = "",
    toDate = "",
    distance = "",
    page = 1,
    serviceID = "",
    comboID = "",
    search = "",
    longi = "",
    lati = ""
  }) => {
    return handleResult(
      getAxios.get(
        `api/Service/GetListOrderService?status=${status}&placeID=${placeID}&fromDate=${fromDate}&toDate=${toDate}&distance=${distance}&page=${page}&serviceID=${serviceID}&comboID=${comboID}&search=${search}&longi=${longi}&lati=${lati}`
      )
    );
  },
  changeStatusOrder: payload =>
    handleResult(
      getAxios.get(
        `api/Service/ChangeStatus?orderServiceID=${
          payload.orderServiceID
        }&status=${payload.status}&reason=${payload.reason}`
      )
    ),
  ChangePass: payload =>
    handleResult(getAxios.post(`api/Service/ChangePass`, payload)),
  uploadImage: payload => {
    return handleResult(
      getAxios.post(
        `api/Service/UploadImage?carID=&orderServiceID=${
          payload.serviceID
        }&imageRequiteID=${payload.imageRequiteID}`,
        payload.data
      )
    );
  },
  getConfig: payload =>
    handleResult(getAxios.get(`api/Service/GetConfig?type=${payload || 1}`)),
  uploadAvatar: payload =>
    handleResult(getAxios.post("api/Service/ChangeAvatar", payload)),
  requestLogout: () => handleResult(getAxios.get(`api/Service/Logout`)),
  getPointHistory: payload =>
    handleResult(
      getAxios.get(
        `api/Service/GetPointHistory?FromDate=${payload.fromDate}&ToDate=${
          payload.toDate
        }&type=${payload.type}`
      )
    ),
  updateUserInfo: payload =>
    handleResult(getAxios.post("api/Service/UpdateCustomer", payload)),
  CreateBank: payload =>
    handleResult(getAxios.post("api/Service/CreateBank", payload)),
  DeleteBank: id =>
    handleResult(getAxios.get(`api/Service/DeleteBank?id=${id}`)),
  getListBank: () => handleResult(getAxios.get(`api/Service/GetListBank`)),
  getWithdraw: payload =>
    handleResult(
      getAxios.get(
        `api/Service/Withdraw?point=${payload.point}&id=${payload.id}`
      )
    ),
  geRechargeWallet: payload =>
    handleResult(
      getAxios.get(
        `api/Service/RechargeWallet?point=${payload.point}&type=${payload.type}`
      )
    ),
  updateLocation: payload =>
    handleResult(
      getAxios.get(
        `api/Service/UpdateLocationAgent?lati=${payload.lati}&longi=${
          payload.longi
        }`
      )
    ),
  updateDeviceID: payload =>
    handleResult(
      getAxios.get(`api/Service/UpdateDeviceID?deviceID=${payload}`)
    ),
  uploadCarImageService: (params, body) =>
    handleResult(
      getAxios.post(
        `api/Service/UploadCarImageService?orderServiceID=${
          params.orderServiceID
        }&type=${params.type}&note=${params.note}`,
        body
      )
    ),
  requestDeleteImageCarService: payload => {
    return handleResult(
      getAxios.post("api/Service/DeleteCarImageService", payload)
    );
  },
  getOTPByPhone: ({ phone, type }) =>
    handleResult(
      getAxios.get(`api/Service/GetOTPByPhone?phone=${phone}&type=${type}`)
    ),
  requestConfirmOTP: payload =>
    handleResult(
      getAxios.post("api/Service/ConfirmOTP", {
        Phone: payload.Phone,
        OTP: payload.OTP || ""
      })
    ),
  requestGetListFilter: () => {
    return handleResult(getAxios.get("api/Service/GetServiceFilter"));
  },
  GetUrlVNPay: transactionID =>
    handleResult(
      getAxios.get(`api/Service/GetUrlVNPay?transactionID=${transactionID}`)
    ),
  getQA: () => handleResult(getAxios.get(`api/Service/GetQA`)),
  GetContentReason: type =>
    handleResult(getAxios.get(`api/Service/GetContentReason?type=${type}`)),
  registerWasher: payload =>
    handleResult(getAxios.post(`api/Service/RegisterWasher`, payload)),
  GetOrderComboService: payload =>
    handleResult(
      getAxios.get(`api/Service/GetOrderComboSevice?comboCode=${payload}`)
    ),
  GetProduct: payload =>
    handleResult(getAxios.get(`api/Service/GetProduct?CateID=${payload}`)),
  GetProductCate: () =>
    handleResult(getAxios.get(`api/Service/GetProductCate`)),
  GetListCart: payload =>
    handleResult(getAxios.get(`api/Service/GetListCart?status=${payload}`)),
  GetOrderDetail: payload =>
    handleResult(getAxios.get(`api/Service/GetOrderDetail?orderID=${payload}`)),
  GetOrders: payload =>
    handleResult(getAxios.get(`api/Service/GetOrders?status=${payload}`)),
  GetCart: () => handleResult(getAxios.get(`api/Service/GetCart`)),
  UpdateCart: ({ productID, type }) =>
    handleResult(
      getAxios.get(`api/Service/UpdateCart?productID=${productID}&type=${type}`)
    ),
  CreateOrders: payload =>
    handleResult(getAxios.post(`api/Service/CreateOrders`, payload)),
  AdditionServiceExtra: payload =>
    handleResult(getAxios.post(`api/Service/AdditionServiceExtra`, payload)),
  GetListService: payload =>
    handleResult(
      getAxios.get(
        `api/Service/GetListService?carID=&type=2&mainServiceID=&orderID=${payload}`
      )
    )
};

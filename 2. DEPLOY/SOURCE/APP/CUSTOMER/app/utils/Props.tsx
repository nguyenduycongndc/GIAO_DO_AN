export default interface Props {
  navigation?: Navigation;
  reducer?: Reducer;
  getUserInfoAction?: any;
  requestLogin?: any;
  getListOrderService?: any;
  sendLocationSelect?: any;
  clearLocationSelect?: any;
  sendVoucher?: any;
  sendBookingNow?: any;
  loginRequireMessage?: any;
  getCarDetail?: any;
  setState?: any;
  getOrder?: any;
  toggleModal?: any;
}
export interface Navigation {
  pop: any;
  popToTop: any;
  push: any;
  replace: any;
  reset: any;
  dismiss: any;
  goBack: any;
  navigate: any;
  setParams: any;
  state: State;
  actions: Actions;
  getParam: any;
  getChildNavigation: any;
  isFocused: any;
  isFirstRouteInParent: any;
  dispatch: any;
  getScreenProps: any;
  dangerouslyGetParent: any;
  addListener: any;
  emit: any;
}
export interface State {
  params: Params;
  routeName: string;
  key: string;
}
export interface Params {
  serviceName?: null;
  listService?: (null)[] | null;
  timeStartOrder: number;
  uri?: null;
  additionService?: null;
  mainService?: null;
  car: Car;
  longi: number;
  lati: number;
  isInDoor: number;
  comboID: number;
  couponCode?: null;
  placeID?: null;
  bookingDateInput?: null;
  isBookingNow: number;
  agentCode?: null;
  firstID?: null;
  comboCode: string;
  orderServiceID: number;
  carID: number;
  paymentType: number;
  code: string;
  listImageRequire?: (null)[] | null;
  totalPrice: number;
  basePrice: number;
  couponPoint: number;
  usePoint: number;
  status: number;
  completionDateStr: string;
  confirmDateStr: string;
  createDateStr: string;
  bookDateStr: string;
  estBookDateStr: string;
  createDate: string;
  confirmDate?: null;
  successDate?: null;
  customerName: string;
  customerPhone: string;
  customerAddress: string;
  agentName: string;
  agentPhone: string;
  agentAvatar: string;
  rating: number;
  note: string;
  distance: number;
  customerID: number;
  agentID?: null;
  bookingDate: string;
  estBookDate: string;
}
export interface Car {
  carID: number;
  carModel: string;
  carModelID: number;
  carBrand: string;
  licensePlates: string;
  manufacturingDate: string;
  registrationDate: string;
  registrationDateStr: string;
  carColor: string;
  listImage?: (Image)[];
  status?: null;
  vehicleRegistration?: null;
}

export interface Image {
  url;
  imageID;
}
export interface Actions {
  pop: string;
  popToTop: string;
  push: string;
  replace: string;
  reset: string;
  dismiss: string;
  goBack: string;
  navigate: string;
  setParams: string;
}

export interface Reducer {
  userReducer: UserReducer;
  selectLocation: SelectLocation;
  listOrderServiceReducer: ListOrderServiceReducer;
  voucherReducer: VoucherReducer;
  bookingNowReducer: BookingNowReducer;
  requireLoginReducer: RequireLoginReducer;
  carDetailReducer: CarDetailReducer;
  state: State;
  orderUpcomingReducer: OrderUpcomingReducer;
  orderHISTORYReducer: OrderHISTORYReducer;
  orderPROCESSINGReducer: OrderPROCESSINGReducer;
  lang: Lang;
}
export interface UserReducer {
  error?: null;
  data: Data;
  isLoading: boolean;
}
export interface Data {
  rate: number;
  withdrawNote?: null;
  longi: number;
  lati: number;
  modifyDate?: null;
  modifyDateSTR: string;
  userID: number;
  role: number;
  token: string;
  agentArea?: null;
  listCar?: (ListCarEntity)[] | null;
  listBank?: null;
  memberID: number;
  isNeedUpdate: number;
  passWord?: null;
  acceptService?: null;
  code?: null;
  wallet?: (WalletEntity)[] | null;
  phone: string;
  name: string;
  dob?: null;
  dobStr: string;
  sex: number;
  email: string;
  provinceName: string;
  districtName: string;
  provinceCode?: null;
  districtCode?: null;
  address: string;
  withdrawPoint: number;
  point: number;
  urlAvatar: string;
  cancelOrder;
}
export interface ListCarEntity {
  carID: number;
  carModel: string;
  carModelID: number;
  carBrand: string;
  carBrandID: number;
  licensePlates: string;
  manufacturingDate: string;
  registrationDate: string;
  registrationDateStr: string;
  carColor: string;
  listImage?: (null)[] | null;
  status: string;
  vehicleRegistration: string;
}
export interface WalletEntity {
  id: number;
  balance: number;
  type: number;
}
export interface SelectLocation {
  location: string;
  name: string;
}
export interface ListOrderServiceReducer {
  data?: (null)[] | null;
  isLoading: boolean;
  error?: null;
  dataProcessing?: (null)[] | null;
  dataUpcoming?: (null)[] | null;
  dataHistory?: (null)[] | null;
  isLoadingProcessing: boolean;
  isLoadingUpcoming: boolean;
  isLoadingHistory: boolean;
  errorProcessing?: null;
  errorUpcoming?: null;
  errorHistory?: null;
}
export interface VoucherReducer {
  code: string;
  discount: string;
}
export interface BookingNowReducer {
  type: string;
}
export interface RequireLoginReducer {
  visiable: boolean;
}
export interface CarDetailReducer {
  data: any;
  listImage?: (null)[] | null;
  isLoading: boolean;
  error?: null;
  refreshing: boolean;
}
export interface State {
  MainCustomer: any;
  HomeCustomer: any;
  NotifCustomer: any;
  BookingCustomer: BookingCustomer;
  AccountCustomer: any;
  OrderCustomer: any;
  selectPackage: any;
  selectAdditionService: any;
  detailPackage: any;
  booking: any;
  searchWasher: any;
  selectLocation: any;
  orderDetail: any;
  news: any;
  newsDetail: any;
  promotion: any;
  withdrawal: any;
  contact: any;
  changePassword: any;
  historyPoint: any;
  userInfo: any;
  changeLanguage: any;
  updateUserInfo: any;
  yourCar: any;
  carDetail: any;
  updateCarInfo: any;
  addCar: any;
  Qa: any;
  coupon: any;
  QR: any;
  ImageViewer: any;
  processingDetail: any;
  completeDetail: any;
  cancelDetail: any;
  upcomingDetail: any;
  upcomingOrder: UpcomingOrder;
  processingOrder: any;
  historyOrder: HistoryOrder;
}
export interface BookingCustomer {
  isShowMoreDetail: boolean;
  shiftHour: number;
  isBookingNow: number;
  isLogin: boolean;
  carID: string;
  isLoading: boolean;
  error?: null;
  date: number;
  time: number;
  marker: Marker;
  isLoadingNearBy: boolean;
  isModalVisible: boolean;
  timeSelect?: (null)[] | null;
  address: string;
  code: string;
  value?: null;
  washerInfo: string;
  carInfo: any;
  orderService: OrderService;
  imagesMoreDetail: string;
  noteMoreDetail: string;
  reset?: null;
  toggleModal?;
}
export interface Marker {
  latitude: number;
  longitude: number;
}
export interface OrderService {
  additionService?: (null)[] | null;
  mainService: number;
  PaymentType: string;
  carID: string;
  isInDoor: string;
  BookingDateInput: string;
  placeID: string;
  couponCode: string;
  comboID: number;
  UsePoint: string;
  isBookingNow: number;
  agentCode: string;
  note: string;
}
export interface UpcomingOrder {
  resetData;
}
export interface ServiceEntity {
  serviceID?: string | number;
  serviceName: string;
  comboID?: string | number;
}
export interface HistoryOrder {
  resetData;
}
export interface OrderUpcomingReducer {
  isLastPage: boolean;
  error?: null;
  data?: (DataEntity)[] | null;
  isLoading: boolean;
  isLoadMore: boolean;
}
export interface DataEntity {
  startDateStr?;
  completionDateStr?;
  serviceCode?;
  bookingDateStr?;
  agentPhone?;
  reasonNote?;
  serviceName: string;
  commission: number;
  carNote: CarNoteOrParkNote;
  reasonCancel?: null;
  parkNote: CarNoteOrParkNote;
  listService?: (ListServiceEntity)[] | null;
  timeStartOrder: number;
  uri: string;
  additionService?: (number)[] | null;
  mainService: number;
  car: Car;
  longi: number;
  lati: number;
  placeID: string;
  isInDoor: number;
  comboID: number;
  couponCode?: null;
  bookingDateInput?: null;
  isBookingNow: number;
  agentCode: string;
  firstID: number;
  comboCode: string;
  orderServiceID: number;
  carID: number;
  paymentType: number;
  code: string;
  listImageRequire?: (ListImageRequireEntity)[] | null;
  totalPrice: number;
  basePrice: number;
  couponPoint: number;
  usePoint: number;
  status: number;
  statusStr: string;
  completionDateStr: string;
  confirmDateStr: string;
  createDateStr: string;
  bookDateStr: string;
  estBookDateStr: string;
  createDate: string;
  confirmDate?: null;
  successDate?: null;
  customerName: string;
  customerPhone: string;
  customerAddress?: null;
  customerAvatar?: null;
  agentName: string;
  agentAvatar: string;
  agentRating: number;
  rating: number;
  noteRate?: null;
  note: string;
  distance: number;
  customerID: number;
  agentID?: null;
  bookingDate: string;
  estBookDate: string;
  noti?;
}
export interface CarNoteOrParkNote {
  note?: string | null;
  listImage?: (ListImageEntity | null)[] | null;
}
export interface ListImageEntity {
  image: string;
  id: number;
}
export interface ListServiceEntity {
  name: string;
  price: number;
  type: number;
}
export interface Car {
  carID: number;
  carModel: string;
  carModelID: number;
  carBrand: string;
  carBrandID: number;
  licensePlates: string;
  manufacturingDate: string;
  registrationDate: string;
  registrationDateStr: string;
  carColor: string;
  listImage?: (null)[] | null;
  status?: null;
  vehicleRegistration?: null;
}
export interface ListImageRequireEntity {
  name: string;
  before: BeforeOrAfter;
  after: BeforeOrAfter;
  order: number;
}
export interface BeforeOrAfter {
  imageRequireID: number;
  url?: null;
  dateStr: string;
  date?: null;
}
export interface OrderHISTORYReducer {
  error?: null;
  status?: null;
  data?: (DataEntity1)[] | null;
  isLoading: boolean;
  isLoadMore: boolean;
  isLastPage: boolean;
}
export interface DataEntity1 {
  serviceName: string;
  carNote: CarNoteOrParkNote1;
  reasonCancel?: null;
  parkNote: CarNoteOrParkNote1;
  listService?: (ListServiceEntity)[] | null;
  timeStartOrder: number;
  uri: string;
  additionService?: null;
  mainService?: null;
  car: Car;
  longi: number;
  lati: number;
  isInDoor: number;
  comboID: number;
  couponCode?: null;
  placeID?: null;
  bookingDateInput?: null;
  isBookingNow: number;
  agentCode?: null;
  firstID: number;
  comboCode: string;
  orderServiceID: number;
  carID: number;
  paymentType: number;
  code: string;
  listImageRequire?: (ListImageRequireEntity1)[] | null;
  totalPrice: number;
  basePrice: number;
  couponPoint: number;
  usePoint: number;
  status: number;
  completionDateStr: string;
  confirmDateStr: string;
  createDateStr: string;
  bookDateStr: string;
  estBookDateStr: string;
  createDate: string;
  confirmDate: string;
  successDate: string;
  customerName: string;
  customerPhone: string;
  customerAddress: string;
  agentName: string;
  agentPhone: string;
  agentAvatar: string;
  rating: number;
  note: string;
  distance: number;
  customerID: number;
  agentID: number;
  bookingDate: string;
  estBookDate: string;
}
export interface CarNoteOrParkNote1 {
  note?: null;
  listImage?: (null)[] | null;
}
export interface ListImageRequireEntity1 {
  name: string;
  before: BeforeOrAfter1;
  after: BeforeOrAfter1;
  order: number;
}
export interface BeforeOrAfter1 {
  imageRequireID: number;
  url: string;
  dateStr: string;
  date?: null;
}
export interface OrderPROCESSINGReducer {
  data?: (null)[] | null;
  isLoading: boolean;
  isLastPage: boolean;
  isLoadMore: boolean;
  error?: null;
}
export interface Lang {
  lang?: null;
}

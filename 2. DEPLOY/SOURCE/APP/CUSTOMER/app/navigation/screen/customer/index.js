import SelectLocationScreen from "@app/screens/customer/booking/SelectLocationScreen";
import { SCREEN_ROUTER_CUSTOMER } from "@constant";
import NewsScreen from "@app/screens/customer/home/NewsScreen";
import NewsDetailScreen from "@app/screens/customer/home/NewsDetailScreen";
import ContactScreen from "@app/screens/customer/account/ContactScreen";
import ChangesPasswordScreen from "@app/screens/customer/account/ChangesPasswordScreen";
import HistoryPointScreen from "@app/screens/customer/account/HistoryPointScreen";
import ChangeLanguageScreen from "@app/screens/customer/account/ChangeLanguageScreen";
import UserInfoScreen from "@app/screens/customer/account/UserInfoScreen";
import UpdateUserInfoScreen from "@app/screens/customer/account/UpdateUserInfoScreen";
import YourCarScreen from "@app/screens/customer/account/YourCarScreen";
import CarDetailScreen from "@app/screens/customer/account/CarDetailScreen";
import UpdateCarInfoScreen from "@app/screens/customer/account/UpdateCarInfoScreen";
import AddCarScreen from "@app/screens/customer/account/AddCarScreen";
import QandAScreen from "@app/screens/customer/account/QandAScreen";
import QRScreen from "@app/screens/customer/booking/QRScreen";
import ProcessingDetail from "@app/screens/customer/order/detail/ProcessingDetail";
import CompleteDetail from "@app/screens/customer/order/detail/CompleteDetail";
import CancelDetail from "@app/screens/customer/order/detail/CancelDetail";
import UpcomingDetail from "@app/screens/customer/order/detail/UpcomingDetail";
import FeedbackScreen from "@app/screens/customer/account/FeedbackScreen";
import SaveLocationScreen from "@app/screens/customer/account/SaveLocationScreen";
import GenDetail from "@app/screens/customer/order/detail/GenDetail";
import RankAcountInfoScreen from "@app/screens/customer/account/RankAcountInfoScreen";

export default {
  [SCREEN_ROUTER_CUSTOMER.SELECT_LOCATION]: SelectLocationScreen,
  [SCREEN_ROUTER_CUSTOMER.NEWS]: NewsScreen,
  [SCREEN_ROUTER_CUSTOMER.NEWS_DETAIL]: NewsDetailScreen,
  [SCREEN_ROUTER_CUSTOMER.CONTACT]: ContactScreen,
  [SCREEN_ROUTER_CUSTOMER.CHANGE_PASSWORD]: ChangesPasswordScreen,
  [SCREEN_ROUTER_CUSTOMER.HISTORY_POINT]: HistoryPointScreen,
  [SCREEN_ROUTER_CUSTOMER.USER_INFO]: UserInfoScreen,
  [SCREEN_ROUTER_CUSTOMER.CHANGE_LANGUAGE]: ChangeLanguageScreen,
  [SCREEN_ROUTER_CUSTOMER.UPDATE_USER_INFO]: UpdateUserInfoScreen,
  [SCREEN_ROUTER_CUSTOMER.YOUR_CAR]: YourCarScreen,
  [SCREEN_ROUTER_CUSTOMER.CAR_DETAIL]: CarDetailScreen,
  [SCREEN_ROUTER_CUSTOMER.UPDATE_CAR_INFO]: UpdateCarInfoScreen,
  [SCREEN_ROUTER_CUSTOMER.ADD_CAR]: AddCarScreen,
  [SCREEN_ROUTER_CUSTOMER.Q_A]: QandAScreen,
  [SCREEN_ROUTER_CUSTOMER.QR]: QRScreen,
  [SCREEN_ROUTER_CUSTOMER.PROCESSING_DETAIL]: ProcessingDetail,
  [SCREEN_ROUTER_CUSTOMER.COMPLETE_DETAIL]: CompleteDetail,
  [SCREEN_ROUTER_CUSTOMER.CANCEL_DETAIl]: CancelDetail,
  [SCREEN_ROUTER_CUSTOMER.UPCOMING_DETAIL]: UpcomingDetail,
  [SCREEN_ROUTER_CUSTOMER.FEEDBACK]: FeedbackScreen,
  [SCREEN_ROUTER_CUSTOMER.SAVE_LOCATION]: SaveLocationScreen,
  [SCREEN_ROUTER_CUSTOMER.GEN_DETAIl]: GenDetail,
  [SCREEN_ROUTER_CUSTOMER.RANK_ACOUNT_INFO]: RankAcountInfoScreen
};

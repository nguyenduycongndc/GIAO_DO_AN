import SelectPackageScreen from "@app/screens/customer/booking/SelectPackageScreen";
import BookingScreen from "@app/screens/customer/booking/BookingScreen";
import SearchWasherScreen from "@app/screens/customer/booking/SearchWasherScreen";
import { SCREEN_ROUTER_CUSTOMER } from "@constant";
export default {
  [SCREEN_ROUTER_CUSTOMER.SELECT_PACKAGE]: SelectPackageScreen,
  [SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE]: BookingScreen,
  [SCREEN_ROUTER_CUSTOMER.SEARCH_WASHER]: SearchWasherScreen
};

import OrderDetailScreen from "@app/screens/washer/order/orderdetail/OrderDetailScreen";
import WithdrawalScreen from "@screen_washer/account/withdrawal/WithdrawalScreen";
import AddBankAccountScreen from "@screen_washer/account/withdrawal/AddBankAccountScreen";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import FeedbackScreen from "@screen_washer/order/orderdetail/FeedbackScreen";
import RNCameraScreen from "@screen_washer/RNCameraScreen";
import TransfersScreen from "@screen_washer/account/TransfersScreen";
import HistoryScreen from "@app/screens/washer/account/HistoryScreen";
import PersonalInfoScreen from "@screen_washer/account/personal/PersonalInfoScreen";
import StateChangeScreen from "@screen_washer/account/StateChangeScreen";
import ContactScreen from "@screen_washer/account/ContactScreen";
import EditBankAccountScreen from "@screen_washer/account/personal/EditBankAccountScreen";
import ChangesPasswordScreen from "@screen_washer/account/ChangesPasswordScreen";
import RechargeScreen from "@screen_washer/account/RechargeScreen";
import EditPersonalInfoScreen from "@screen_washer/account/personal/EditPersonalInfoScreen";
import QRCodeScreen from "@screen_washer/account/qrcode/QRCodeScreen";
import UploadImageScratch from "@app/screens/washer/order/scratch/UploadImageScratchScreen";
import CameraUploadImageScratch from "@app/screens/washer/order/scratch/CameraScreen";
import OrderIncommingScreen from "@app/screens/washer/order/OrderIncommingScreen";
import ImageViewerScreen from "@app/screens/washer/order/orderdetail/ImageViewScreen";
import MapScreen from "@app/screens/washer/order/MapScreen";
import QAScreen from "@app/screens/washer/account/QAScreen";
import TabProductionScreen from "@app/screens/washer/production/TabProductionScreen";
import ProdutionDetailScreen from "@app/screens/washer/production/ProdutionDetailScreen";
import CartScreen from "@app/screens/washer/production/CartScreen";
import OrderProductionScreen from "@app/screens/washer/account/order_production/OrderProductionScreen";
import OrderProductionDetailScreen from "@app/screens/washer/account/order_production/OrderProductionDetailScreen";
import AddAdditionalScreen from "@app/screens/washer/order/AddAdditionalScreen";

export default {
  [SCREEN_ROUTER_WASHER.IMAGE_VIEWER]: ImageViewerScreen,
  [SCREEN_ROUTER_WASHER.ORDER_INCOMMING]: OrderIncommingScreen,
  [SCREEN_ROUTER_WASHER.ORDER_DETAIL]: OrderDetailScreen,
  [SCREEN_ROUTER_WASHER.WITHDRAWAL]: WithdrawalScreen,
  [SCREEN_ROUTER_WASHER.ADD_BANK_ACCOUNT]: AddBankAccountScreen,
  [SCREEN_ROUTER_WASHER.CAMERA_SCREEN]: RNCameraScreen,
  [SCREEN_ROUTER_WASHER.TRANSFERS]: TransfersScreen,
  [SCREEN_ROUTER_WASHER.HISTORY_TRANSFERS]: HistoryScreen,
  [SCREEN_ROUTER_WASHER.PERSONAL_INFO]: PersonalInfoScreen,
  [SCREEN_ROUTER_WASHER.STATE_CHANGE]: StateChangeScreen,
  [SCREEN_ROUTER_WASHER.CONTACT]: ContactScreen,
  [SCREEN_ROUTER_WASHER.EDIT_BANK_ACCOUNT]: EditBankAccountScreen,
  [SCREEN_ROUTER_WASHER.CHANGE_PASSWORD]: ChangesPasswordScreen,
  [SCREEN_ROUTER_WASHER.RECHARGE]: RechargeScreen,
  [SCREEN_ROUTER_WASHER.EDIT_PERSONAL_INFO]: EditPersonalInfoScreen,
  [SCREEN_ROUTER_WASHER.QRCODE]: QRCodeScreen,
  [SCREEN_ROUTER_WASHER.UPLOAD_IMAGE_SCRATCH]: UploadImageScratch,
  [SCREEN_ROUTER_WASHER.CAMERA_UPLOAD_IMAGE_SCRATCH]: CameraUploadImageScratch,
  [SCREEN_ROUTER_WASHER.MAP]: MapScreen,
  [SCREEN_ROUTER_WASHER.QA_SCREEN]: QAScreen,
  [SCREEN_ROUTER_WASHER.PRODUCTION]: TabProductionScreen,
  [SCREEN_ROUTER_WASHER.PRODUCTION_DETAIL]: ProdutionDetailScreen,
  [SCREEN_ROUTER_WASHER.CART_SCREEN]: CartScreen,
  [SCREEN_ROUTER_WASHER.ORDER_PRODUCTION]: OrderProductionScreen,
  [SCREEN_ROUTER_WASHER.ORDER_PRODUCTION_DETAIL]: OrderProductionDetailScreen,
  [SCREEN_ROUTER_WASHER.ADD_ADDITIONAL]: AddAdditionalScreen
};

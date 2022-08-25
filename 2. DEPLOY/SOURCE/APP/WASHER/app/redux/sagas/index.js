import {
  watchGetUser,
  watchGetListOrderService,
  watchGetListOrderUpcommingService,
  watchGetListOrderHistoryService,
  watchGetListOrderPendingService,
  watchChangeAvatar,
  watchCreateBank,
  watchDeleteBank,
  watchUpdateUserInfo,
  watchTransferMoney,
  watchGetNotification
} from "./NetworkSaga";

export default function* washerSaga() {
  yield watchGetUser;
  yield watchGetListOrderService;
  yield watchGetListOrderUpcommingService;
  yield watchGetListOrderHistoryService;
  yield watchGetListOrderPendingService;
  yield watchChangeAvatar;
  yield watchCreateBank;
  yield watchDeleteBank;
  yield watchUpdateUserInfo;
  yield watchTransferMoney;
  yield watchGetNotification;
}

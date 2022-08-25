import { takeEvery } from "redux-saga/effects";
import {
  GET_USER,
  GET_LIST_ORDER_SERVICE,
  GET_LIST_ORDER_UPCOMMING_SERVICE,
  GET_LIST_ORDER_HISTORY_SERVICE,
  GET_LIST_ORDER_PENDING_SERVICE,
  CHANGE_AVATAR,
  CREATE_BANK,
  DELETE_BANK,
  UPDATE_USER_INFO,
  TRANSFER_MONEY,
  GET_NOTIFICATION,
  UPDATE_CART
} from "@action/type";
import {
  getUserInfo,
  getListOrderService,
  uploadAvatar,
  CreateBank,
  DeleteBank,
  updateUserInfo,
  geRechargeWallet,
  getNotification,
  UpdateCart
} from "@api";
import APINetwork from "@app/utils/SagaHelper";

export const watchGetUser = takeEvery(GET_USER, APINetwork, getUserInfo);
export const watchGetListOrderService = takeEvery(
  GET_LIST_ORDER_SERVICE,
  APINetwork,
  getListOrderService
);
export const watchGetListOrderUpcommingService = takeEvery(
  GET_LIST_ORDER_UPCOMMING_SERVICE,
  APINetwork,
  getListOrderService
);

export const watchGetListOrderHistoryService = takeEvery(
  GET_LIST_ORDER_HISTORY_SERVICE,
  APINetwork,
  getListOrderService
);
export const watchGetListOrderPendingService = takeEvery(
  GET_LIST_ORDER_PENDING_SERVICE,
  APINetwork,
  getListOrderService
);
export const watchChangeAvatar = takeEvery(
  CHANGE_AVATAR,
  APINetwork,
  uploadAvatar
);
export const watchDeleteBank = takeEvery(DELETE_BANK, APINetwork, DeleteBank);
export const watchCreateBank = takeEvery(CREATE_BANK, APINetwork, CreateBank);
export const watchUpdateUserInfo = takeEvery(
  UPDATE_USER_INFO,
  APINetwork,
  updateUserInfo
);
export const watchTransferMoney = takeEvery(
  TRANSFER_MONEY,
  APINetwork,
  geRechargeWallet
);

export const watchGetNotification = takeEvery(
  GET_NOTIFICATION,
  APINetwork,
  getNotification
);

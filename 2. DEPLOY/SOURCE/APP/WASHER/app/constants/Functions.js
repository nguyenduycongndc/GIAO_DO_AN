import { Linking, Platform } from "react-native";
import {
  ORDER_STATUS,
  PAYMENT_METHOD,
  DEEP_LINK
} from "@app/constants/Constants";
import R from "@R";
function convertDate(dateString) {
  let tmp = dateString.split("-");
  let date = tmp[2] + "/" + tmp[1] + "/" + tmp[0];
  return date;
}
function numberWithCommas(number) {
  return number.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function setValue(string) {
  return string ? string : R.strings().not_already_update;
}

function renStatus(status) {
  switch (status) {
    case ORDER_STATUS.CONFIRMED:
      return R.strings().received;

    case ORDER_STATUS.WASHING:
    case ORDER_STATUS.START:
      return R.strings().washing;

    case ORDER_STATUS.REJECTED:
      return R.strings().canceled;

    case ORDER_STATUS.COMPLETED:
      return R.strings().complete;

    case ORDER_STATUS.PENDING_ALL:
    case ORDER_STATUS.PENDING_ADMIN:
    case ORDER_STATUS.PENDING:
      return R.strings().waiting;

    default:
      return status ? status : R.strings().not_already_update;
      break;
  }
}
function typePayment(type) {
  switch (type) {
    case PAYMENT_METHOD.CASH:
      return R.strings().cash;
    case PAYMENT_METHOD.CASH:
      return "VNPay";
    default:
      break;
  }
}
function callPhone(phone) {
  return Linking.openURL(`tel:${phone}`);
}
function deepLink(type, id, title, body) {
  switch (type) {
    case DEEP_LINK.GMAIL:
      return Linking.openURL(`mailto:${id}?subject=${title}&body=${body}`);
    case DEEP_LINK.MESS_FB:
      return Linking.openURL(`http://m.me/${id}`);
    case DEEP_LINK.ZALO:
      return Linking.openURL(`https://zalo.me/${id}`);
    case DEEP_LINK.FACEBOOK:
      return Linking.openURL(
        `fb://${Platform.OS == "ios" ? "profile" : "page"}/${id}`
      );
    case DEEP_LINK.TWITTER:
      return Linking.openURL(`twitter://user?screen_name=${id}`);
    case DEEP_LINK.INSTA:
      return Linking.openURL(`instagram://user?username=${id}`);
  }
}

export {
  convertDate,
  numberWithCommas,
  setValue,
  renStatus,
  callPhone,
  typePayment,
  deepLink
};

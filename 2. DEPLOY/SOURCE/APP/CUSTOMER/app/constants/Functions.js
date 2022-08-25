import { Linking, Platform } from "react-native";
import { ORDER_TYPE, DEEP_LINK } from "@app/constants/C";
import R from "@R";
function callPhone(phone) {
  return Linking.openURL(`tel:${phone}`);
}
function convertDate(dateString) {
  let tmp = dateString.split("-");
  let date = tmp[2] + "/" + tmp[1] + "/" + tmp[0];
  return date;
}
function numberWithCommas(number) {
  return number.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function renStatus(status) {
  switch (status) {
    case ORDER_TYPE.ORDER_STATUS_COMPLETE:
      return R.strings().complete;
    case ORDER_TYPE.ORDER_STATUS_CANCEL:
      return R.strings().cancel;
    case ORDER_TYPE.ORDER_STATUS_WASHING:
    case ORDER_TYPE.ORDER_STATUS_COMFIRM_WASHING:
      return R.strings().washing;
    case ORDER_TYPE.ORDER_STATUS_CONFIRM:
      return R.strings().upcoming;
    case ORDER_TYPE.ORDER_STATUS_WAITING:
    case ORDER_TYPE.ORDER_STATUS_SEARCH_WASHER:
    case ORDER_TYPE.ORDER_STATUS_NO_CONFIRM:
      return R.strings().search_washer;
    default:
      return "";
  }
}
function formatMoney(amount) {
  try {
    var decimalCount = 0;
    var decimal = "";
    var thousands = ",";
    decimalCount = Math.abs(decimalCount);
    decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

    const negativeSign = amount < 0 ? "-" : "";

    let i = parseInt(
      (amount = Math.abs(Number(amount) || 0).toFixed(decimalCount))
    ).toString();
    let j = i.length > 3 ? i.length % 3 : 0;

    return (
      negativeSign +
      (j ? i.substr(0, j) + thousands : "") +
      i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) +
      (decimalCount
        ? decimal +
          Math.abs(amount - i)
            .toFixed(decimalCount)
            .slice(2)
        : "")
    );
  } catch (e) {
    console.log(e);
  }
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

function getCurrentTimeString() {
  var dateTime = new Date();
  return `${dateTime.getHours()}:${dateTime.getMinutes() +
    1} ${dateTime.getDate()}/${dateTime.getMonth() +
    1}/${dateTime.getFullYear()}`;
}
export {
  convertDate,
  numberWithCommas,
  renStatus,
  callPhone,
  formatMoney,
  deepLink,
  getCurrentTimeString
};

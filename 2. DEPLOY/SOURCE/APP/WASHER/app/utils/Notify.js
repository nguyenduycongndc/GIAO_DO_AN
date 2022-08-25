const { NativeModules, Platform } = require("react-native");
var getNotificationData, dismissAllNotification, clearNotificationData;
if (Platform.OS == "android") {
  getNotificationData = NativeModules.NativeModule.getNotificationData;
  dismissAllNotification = NativeModules.NativeModule.dismissAllNotification;
  clearNotificationData = NativeModules.NativeModule.dismissAllNotification;
} else
  getNotificationData = dismissAllNotification = clearNotificationData = () => {};

module.exports = {
  /**
   * getNotificationData( notiData => void )
   */
  getNotificationData,
  dismissAllNotification,
  clearNotificationData
};

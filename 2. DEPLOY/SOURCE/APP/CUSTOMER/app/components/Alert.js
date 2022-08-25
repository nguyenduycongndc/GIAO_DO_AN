import React, { Component } from "react";
import R from "@R";
import { Alert } from "react-native";
import store from "@app/redux/store";
import { SHOW_MESSAGE, SHOW_CONFIRM } from "@app/redux/actions/type";

export const showConfirm = (
  title,
  content,
  action,
  textCancel = R.strings().cancel,
  textConfirm = R.strings().confirm
) => {
  // setTimeout(() => {
  //   Alert.alert(
  //     title,
  //     content,
  //     [
  //       {
  //         text: textCancel || R.strings().cancel,
  //         style: "cancel"
  //       },
  //       {
  //         text: textConfirm || R.strings().confirm,
  //         onPress: action
  //       }
  //     ],
  //     { cancelable: false }
  //   );
  // }, 350);
  setTimeout(() => {
    store.dispatch({
      type: SHOW_CONFIRM,
      payload: { title, content, action, textCancel, textConfirm }
    });
  }, 100);
};
export const showConfirmAlert = (
  title,
  content,
  action,
  textCancel = R.strings().cancel,
  textConfirm = R.strings().confirm
) => {
  setTimeout(() => {
    Alert.alert(
      title,
      content,
      [
        {
          text: textCancel || R.strings().cancel,
          style: "cancel"
        },
        {
          text: textConfirm || R.strings().confirm,
          onPress: action
        }
      ],
      { cancelable: false }
    );
  }, 100);
};
export const showMessagesAlert = (title, content, action, textConfirm) => {
  setTimeout(() => {
    Alert.alert(
      title,
      content,
      [
        {
          text: textConfirm || R.strings().confirm,
          onPress: action
        }
      ],
      { cancelable: false }
    );
  }, 100);
};
export const showMessages = (title, content, action) => {
  // setTimeout(() => {
  //   Alert.alert(
  //     title,
  //     content,
  //     [
  //       {
  //         text: "OK",
  //         onPress: action
  //       }
  //     ],
  //     { cancelable: false }
  //   );
  // }, 350);
  setTimeout(() => {
    store.dispatch({ type: SHOW_MESSAGE, payload: { title, content, action } });
  }, 100);
};

import React, { useEffect, useRef } from "react";
import { connect } from "react-redux";
import R from "@app/assets/R";
import Props, { DataEntity } from "@app/utils/Props";
import {
  ORDER_TYPE,
  DEEP_LINK,
  REDUCER_CUSTOM,
  TYPE_REASON,
  SCREEN_ROUTER_CUSTOMER,
  PAYMENT_METHOD
} from "@constant";
import Detail from "./Detail";
import {
  View,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity
} from "react-native";
import theme, { colors } from "@app/constants/Theme";
import Button from "@app/components/Button";
import { callAPIHook } from "@app/utils/CallApiHelper";
import {
  changeStatusOrder,
  getOrderServiceDetail,
  GetContentReason
} from "@app/constants/Api";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { GET_ORDER_UPCOMING } from "@app/redux/actions/type";
import { deepLink, callPhone } from "@app/constants/Functions";
import ModalAlert from "@app/components/ModalAlert";
import { useState } from "react";
import {
  showMessages,
  showConfirm,
  showConfirmAlert
} from "@app/components/Alert";
import Menu, { MenuItem, MenuDivider } from "react-native-material-menu";
import reactotron from "reactotron-react-native";
let isSubmit = false;

export default connect(
  state => ({
    reducer: state
  }),
  require("@action/")
)(props => {
  const { cancelOrder: cancelOrderLeft } = props.reducer.userReducer.data;

  const _renderUpcomingButton = () => {
    const phone = data?.agentPhone;
    return (
      <View style={styles.root_buttons}>
        {isUpcoming && (
          <>
            <Button
              width="100%"
              title={R.strings().call}
              action={() => callPhone(phone)}
              imageSource={R.images.ic_call}
              styleImg={styles.img_call}
              backgroundColor={theme.colors.white}
              borderColor={theme.colors.gray}
              colorText={theme.colors.gray}
              buttonStyle={{ marginBottom: 0 }}
            />
            <Button
              width="100%"
              action={() => deepLink(DEEP_LINK.ZALO, phone)}
              title={R.strings().chat_via_zalo}
              imageSource={R.images.ic_zalo}
              styleImg={{ width: 33, height: 33, marginRight: 13 }}
              backgroundColor={theme.colors.backgroundZalo}
              colorText={theme.colors.white}
              buttonStyle={[styles.button, { marginHorizontal: 5 }]}
            />
          </>
        )}
      </View>
    );
  };
  const cancelOrder = reason => {
    closeModal();
    callAPIHook(
      changeStatusOrder,
      {
        orderServiceID: data?.orderServiceID,
        status: ORDER_TYPE.ORDER_STATUS_CANCEL,
        reason
      },
      useLoading,
      () => {
        NavigationUtil.goBack();
        props.getUserInfoAction();
        props.navigateTab(2);
        const { upcomingOrder, historyOrder } = props.reducer.state;
        if (upcomingOrder?.resetData) upcomingOrder.resetData();
        if (historyOrder?.resetData) historyOrder.resetData();
      }
    );
  };

  const getReasonCancel = () => {
    callAPIHook(GetContentReason, TYPE_REASON.CANCEL, useLoading, res =>
      useListRequestService(res.result)
    );
  };
  useEffect(() => {
    getReasonCancel();
  }, []);
  const closeModal = () => {
    useVisible(false);
  };

  const { data, getData, isLoading, useLoading } = props;
  const [isVisible, useVisible] = useState(false);
  const [isClickMenu, useIsClickMenu] = useState(false);
  const [listRequestService, useListRequestService] = useState([]);
  const [reasonCancel, useReasonCancel] = useState("");
  const isUpcoming = !!data?.agentName;
  const status = isUpcoming ? R.strings().upcoming : R.strings().search_washer;
  const setMenuRef = useRef(null);

  const hideMenu = () => {
    setMenuRef.current.hide();
  };

  useEffect(() => {
    if (isClickMenu) hideMenu();
  }, [isClickMenu]);
  const showMenu = () => {
    setMenuRef.current.show();
  };
  return (
    <>
      <Detail
        {...props}
        data={data}
        getData={getData}
        status={status}
        rightComponent={
          !data.comboID && (
            <View style={styles.root_menu}>
              <Menu
                ref={setMenuRef}
                onHidden={() => {
                  if (isClickMenu) {
                    if (cancelOrderLeft > 0) useVisible(true);
                    else showMessages(R.strings().no_left_turn);
                    useIsClickMenu(false);
                  }
                }}
                button={
                  <Text style={styles.menu} onPress={showMenu}>
                    ...
                  </Text>
                }
              >
                <MenuItem onPress={() => useIsClickMenu(true)}>
                  {R.strings().cancel}
                </MenuItem>
              </Menu>
            </View>
          )
        }
        isLoading={isLoading}
        showStepWashing={false}
        renderButton={_renderUpcomingButton()}
      />
      <ModalAlert
        contentView={
          <>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_bold,
                fontSize: 14,
                paddingVertical: 10
              }}
              children={
                R.strings().you_have_left +
                " " +
                cancelOrderLeft +
                " " +
                R.strings().turn
              }
            />
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 12,
                paddingVertical: 10
              }}
              children={R.strings().message_warning_cancel_order}
            />
            {listRequestService.map(e => (
              <TouchableOpacity
                onPress={() => {
                  useReasonCancel(e);
                }}
                children={<Text style={styles.item_note} children={e} />}
              />
            ))}
            <TextInput
              placeholderTextColor={colors.gray}
              placeholder={R.strings().require_note_cancel}
              style={styles.text_input_note_cancel}
              multiline
              maxLength={256}
              value={reasonCancel}
              onChangeText={useReasonCancel}
              scrollEnabled={false}
            />
          </>
        }
        textSubmit={R.strings().cancel_order}
        onSubmit={() => {
          isSubmit = true;
          closeModal();
        }}
        onModalHide={() => {
          if (isSubmit)
            showConfirmAlert(
              R.strings().notif_tab_cus,
              R.strings().warning_cancel_order,
              () => cancelOrder(reasonCancel.trim())
            );
          useReasonCancel("");
        }}
        textCancel={R.strings().back}
        validSubmit={reasonCancel.trim().length > 0}
        onClose={() => {
          isSubmit = false;
          closeModal();
        }}
        isVisible={isVisible}
      />
    </>
  );
});
const styles = StyleSheet.create({
  root_buttons: { width: "100%", paddingHorizontal: 10 },
  root_comunicate: {
    flexDirection: "row"
  },
  button: {
    alignSelf: "center",
    flex: 1
  },
  img_call: {
    width: 28,
    height: 28,
    marginRight: 10,
    tintColor: colors.grayIcCall
  },
  text_input_note_cancel: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    borderRadius: 5,
    borderWidth: 0.5,
    padding: 10,
    textAlignVertical: "top",
    height: 100,
    color: "black"
  },
  item_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    padding: 10,
    marginBottom: 10,
    borderRadius: 10,
    borderWidth: 0.5,
    color: colors.primary
  },
  menu: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 30,
    paddingRight: 0,
    color: colors.white,
    transform: [{ rotate: "90deg" }]
  },
  root_menu: { flex: 1, alignItems: "center", justifyContent: "center" }
});

import React, { useState } from "react";
import { connect } from "react-redux";
import { View, StyleSheet } from "react-native";
import R from "@app/assets/R";
import Button from "@app/components/Button";
import theme, { colors } from "@app/constants/Theme";
import Props, { DataEntity } from "@app/utils/Props";
import { ORDER_TYPE, SCREEN_ROUTER_CUSTOMER } from "@constant";
import Detail from "./Detail";
import ModalAlertRating from "@app/components/ModalAlertRating";
import { callAPIHook } from "@app/utils/CallApiHelper";
import { getOrderServiceDetail } from "@app/constants/Api";
import NavigationUtil from "@app/navigation/NavigationUtil";
import reactotron from "reactotron-react-native";

export default connect(
  state => ({
    reducer: state
  }),
  require("@action/")
)(props => {
  const [isModalVisible, useModal] = useState(
    props.navigation.state.params.noti
  );
  const { data, getData, isLoading, useLoading } = props;
  const _renderCompleteButton = () => {
    return (
      <View style={styles.root_buttons}>
        {data.noteRate == null && (
          <Button
            width="47%"
            uppercase
            action={toggleModal}
            title={R.strings().rate}
            backgroundColor={theme.colors.backgroundGray}
            borderColor={theme.colors.gray}
            colorText={theme.colors.gray}
          />
        )}
        <Button
          width={data.noteRate == null ? "50%" : "100%"}
          uppercase
          action={reOrder}
          title={R.strings().reset}
          backgroundColor={theme.colors.primary}
          colorText={theme.colors.white}
        />
      </View>
    );
  };

  const reOrder = () => {
    const {
      additionService,
      mainService,
      car,
      isInDoor,
      placeID,
      comboID,
      note
    } = data;
    props.setState(SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER, {
      orderService: {
        additionService,
        mainService,
        carID: car.carID,
        isInDoor,
        placeID,
        comboID,
        isBookingNow: 0,
        agentCode: "",
        note,
        BookingDateInput: "",
        couponCode: "",
        PaymentType: "",
        UsePoint: 0
      },
      reOrder: true
    });
    props.reducer.state.BookingCustomer.toggleModal();
  };
  const toggleModal = () => useModal(!isModalVisible);
  const submitModal = () => {
    NavigationUtil.goBack();
  };
  const renderModal = () => (
    <ModalAlertRating
      {...data}
      item={data}
      isModalVisible={isModalVisible}
      submitModal={submitModal}
      toggleModal={toggleModal}
    />
  );

  return (
    <>
      <Detail
        data={data}
        getData={getData}
        status={R.strings().complete}
        isLoading={isLoading}
        renderButton={_renderCompleteButton()}
      />
      {renderModal()}
    </>
  );
});
const styles = StyleSheet.create({
  root_buttons: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    width: "100%",
    paddingHorizontal: 10,
    paddingVertical: 15
  }
});

import React, { useState } from "react";
import { connect } from "react-redux";
import R from "@app/assets/R";
import Button from "@app/components/Button";
import theme, { colors } from "@app/constants/Theme";
import Props, { DataEntity } from "@app/utils/Props";
import { ORDER_TYPE, SCREEN_ROUTER_CUSTOMER } from "@constant";
import Detail from "./Detail";
import { callAPIHook } from "@app/utils/CallApiHelper";
import { getOrderServiceDetail } from "@app/constants/Api";
import reactotron from "reactotron-react-native";

export default connect(
  state => ({
    reducer: state
  }),
  require("@action/")
)(props => {
  const { data, getData, isLoading } = props;
  const _renderButton = () => {
    return (
      <Button
        width="95%"
        uppercase
        action={reOrder}
        title={R.strings().reset}
        backgroundColor={theme.colors.primary}
        colorText={theme.colors.white}
        buttonStyle={{ paddingVertical: 15, alignSelf: "center" }}
      />
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

  return (
    <Detail
      data={data}
      getData={getData}
      status={R.strings().canceled}
      isLoading={isLoading}
      renderButton={_renderButton()}
      showStepWashing={false}
    />
  );
});

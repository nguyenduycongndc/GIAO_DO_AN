import React, { useState, useEffect } from "react";
import { StyleSheet, Text, View } from "react-native";
import Loading from "@app/components/Loading";
import { ORDER_TYPE, SCREEN_ROUTER_CUSTOMER } from "@app/constants/C";
import CompleteDetail from "./CompleteDetail";
import CancelDetail from "./CancelDetail";
import ProcessingDetail from "./ProcessingDetail";
import UpcomingDetail from "./UpcomingDetail";
import { callAPIHook } from "@app/utils/CallApiHelper";
import { getOrderServiceDetail, GetOrderComboSevice } from "@app/constants/Api";
import { connect } from "react-redux";
import { setState } from "@app/redux/actions";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import reactotron from "reactotron-react-native";

const GenDetail = props => {
  const [data, useData] = useState(props.navigation.state.params);
  const [isLoading, useLoading] = useState(false);
  const getData = () => {
    const { orderServiceID } = data;
    if (!data?.comboCode)
      callAPIHook(getOrderServiceDetail, { orderServiceID }, useLoading, res =>
        useData(res.result)
      );
    else
      callAPIHook(GetOrderComboSevice, data?.comboCode || "", useLoading, res =>
        useData(res.result)
      );
  };

  useEffect(() => {
    props.setState(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, { getData });
    if (data?.noti) getData();
    else if (data.orderServiceID) getData();
  }, []);
  switch (data?.status) {
    case ORDER_TYPE.ORDER_STATUS_COMPLETE:
      return (
        <CompleteDetail
          {...props}
          data={data}
          isLoading={isLoading}
          getData={getData}
          useLoading={useLoading}
        />
      );
    case ORDER_TYPE.ORDER_STATUS_CANCEL:
      return (
        <CancelDetail
          {...props}
          data={data}
          isLoading={isLoading}
          getData={getData}
          useLoading={useLoading}
        />
      );
    case ORDER_TYPE.ORDER_STATUS_WASHING:
    case ORDER_TYPE.ORDER_STATUS_COMFIRM_WASHING:
      return (
        <ProcessingDetail
          {...props}
          data={data}
          isLoading={isLoading}
          getData={getData}
          useLoading={useLoading}
        />
      );
    case ORDER_TYPE.ORDER_STATUS_CONFIRM:
    case ORDER_TYPE.ORDER_STATUS_WAITING:
    case ORDER_TYPE.ORDER_STATUS_NO_CONFIRM:
    case ORDER_TYPE.ORDER_STATUS_SEARCH_WASHER:
      return (
        <UpcomingDetail
          {...props}
          data={data}
          isLoading={isLoading}
          getData={getData}
          useLoading={useLoading}
        />
      );
    default:
      return (
        <ScreenComponent
          titleHeader={R.strings().order_detail}
          back
          renderView={<Loading />}
        />
      );
  }
};

export default connect(
  state => ({ state }),
  require("@action/")
)(GenDetail);

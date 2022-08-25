import React, { useState } from "react";
import { connect } from "react-redux";
import R from "@app/assets/R";
import Detail from "./Detail";

export default connect(
  state => ({
    reducer: state
  }),
  require("@action/")
)(props => {
  const { data, getData, isLoading, useLoading } = props;
  return (
    <Detail
      data={data}
      getData={getData}
      status={R.strings().processing}
      isLoading={isLoading}
    />
  );
});

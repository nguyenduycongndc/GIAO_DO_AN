import React from "react";
import { Text } from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { connect } from "react-redux";
import reactotron from "@app/reactotron/ReactotronConfig";
import NavigationUtil from "@app/navigation/NavigationUtil";
import QRCodeScanner from "react-native-qrcode-scanner";

const onReadCode = (event, props) => {
  const tabBooking = props.navigation.state.params;
  tabBooking.setState(
    {
      code: event.data
    },
    () => {
      NavigationUtil.goBack();
      tabBooking.checkAgentByCode();
    }
  );
};
export default connect(
  state => state,
  require("@action")
)(props => {
  return (
    <ScreenComponent
      titleHeader={R.strings().qr_scan}
      back
      renderView={
        <QRCodeScanner
          cameraProps={{ captureAudio: false }}
          onRead={event => onReadCode(event, props)}
        />
      }
    />
  );
});

import React, { Component } from "react";
import { Text, StyleSheet, View, Image, TouchableOpacity } from "react-native";
import theme from "@app/constants/Theme";
import R from "@app/assets/R";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { ORDER_STATUS, SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import reactotron from "reactotron-react-native";

export default class WarningStatus extends Component {
  render() {
    const { data } = this.props;
    if (data.acceptService == 1) return null;
    return (
      <TouchableOpacity
        style={styles.container}
        onPress={() =>
          NavigationUtil.navigate(SCREEN_ROUTER_WASHER.STATE_CHANGE)
        }
        children={
          <>
            <View style={{ flexDirection: "row", alignItems: "center" }}>
              <Image style={styles.ic_warning} source={R.images.ic_warning} />
              <Text
                style={styles.txtWarning}
                children={R.strings().warning_turn_off_status}
              />
            </View>
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  container: {
    paddingVertical: 6,
    paddingHorizontal: 6,
    backgroundColor: "#FAFF51",
    marginTop: 5
  },
  ic_warning: {
    width: 32,
    height: 32,
    resizeMode: "contain"
  },
  txtWarning: {
    fontSize: 14,
    fontFamily: R.fonts.quicksand_medium,
    marginLeft: 5
  }
});

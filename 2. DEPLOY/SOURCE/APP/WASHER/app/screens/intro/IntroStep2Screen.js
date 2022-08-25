import React, { Component } from "react";
import { Text, StyleSheet, View, Image, TouchableOpacity } from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  SCREEN_ROUTER_INTRO,
  SCREEN_ROUTER_AUTH
} from "@app/constants/Constants";
import theme from "@app/constants/Theme";
import { connect } from "react-redux";
export class IntroStep2Screen extends Component {
  render() {
    return (
      <ScreenComponent
        renderView={
          <View style={{ backgroundColor: theme.colors.white, flex: 1 }}>
            <Image
              source={R.images.intro1}
              style={{ width: "100%", resizeMode: "cover" }}
            />
            <View style={styles.bottom}>
              <View style={styles.locationButton}>
                {this.renderButton("Quận Thanh Xuân")}
                {this.renderButton("Quận Nam Từ Liêm")}
              </View>
              <View style={[styles.locationButton, { marginBottom: 60 }]}>
                {this.renderButton("Quận Hà Đông")}
                {this.renderButton("Quận Đống Đa")}
              </View>

              <View style={styles.viewButton}>
                {/* {this.renderBottom(R.strings().skip, () =>
                  NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH)
                )} */}
                <View />
                {this.renderBottom(R.strings().continue, () =>
                  NavigationUtil.navigate(SCREEN_ROUTER_INTRO.STEP3)
                )}
              </View>
            </View>
          </View>
        }
      />
    );
  }
  renderButton(lable) {
    return (
      <TouchableOpacity
        style={styles.location}
        children={
          <Text style={[styles.textButton, { color: theme.colors.black }]}>
            {lable}
          </Text>
        }
      />
    );
  }
  renderBottom(lable, onPress) {
    let isCancel = R.strings().continue != lable;
    return (
      <TouchableOpacity
        onPress={onPress}
        style={[styles.buttonStyle, isCancel ? styles.cancelButton : {}]}
        children={
          <Text
            style={[
              styles.textButton,
              { color: isCancel ? theme.colors.gray : theme.colors.white }
            ]}
          >
            {lable}
          </Text>
        }
      />
    );
  }
}
const styles = StyleSheet.create({
  textStyle: {
    color: theme.colors.active,
    textDecorationLine: "underline"
  },
  viewButton: {
    flexDirection: "row",
    justifyContent: "space-between",
    paddingHorizontal: 15
  },
  textRules: { textAlign: "center", padding: 40 },
  textButton: {
    color: theme.colors.white
  },
  buttonStyle: {
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: colors.primary,
    color: colors.white,
    borderColor: colors.primary,
    borderRadius: 15,
    width: 100,
    height: 30
  },
  cancelButton: {
    backgroundColor: theme.colors.backgroundGray,
    borderWidth: 0.25,
    borderColor: theme.colors.gray
  },
  locationButton: {
    flexDirection: "row",
    alignItems: "center",
    width: "100%",
    justifyContent: "center"
  },
  location: {
    backgroundColor: theme.colors.white,
    borderWidth: 0.25,
    borderColor: theme.colors.gray,
    margin: 5,
    alignItems: "center",
    justifyContent: "center",
    color: colors.white,
    borderRadius: 15,
    width: 150,
    height: 30
  },
  bottom: {
    width: "100%",
    position: "absolute",
    bottom: 20
  }
});
const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(IntroStep2Screen);

import React, { Component } from "react";
import { Text, StyleSheet, View, Image, TouchableOpacity } from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_INTRO } from "@app/constants/Constants";
export default class IntroStep1Screen extends Component {
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
              <Text style={styles.textRules}>
                {"Bằng việc ấn tiếp tục bạn đồng ý với "}
                <Text
                  onPress={() =>
                    NavigationUtil.navigate(SCREEN_ROUTER_INTRO.STEP2)
                  }
                  style={styles.textStyle}
                >
                  điều khoản sử dụng
                </Text>
                {", "}
                <Text
                  onPress={() =>
                    NavigationUtil.navigate(SCREEN_ROUTER_INTRO.STEP2)
                  }
                  style={styles.textStyle}
                >
                  {" "}
                  chính sách bảo mật{" "}
                </Text>
                {" và "}
                <Text
                  onPress={() =>
                    NavigationUtil.navigate(SCREEN_ROUTER_INTRO.STEP2)
                  }
                  style={styles.textStyle}
                >
                  quyền truy cập
                </Text>
                {" của chúng tôi"}
              </Text>
              <View style={styles.viewButton}>
                {/* {this.renderBottom(R.strings().skip, () =>
                  NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH)
                )} */}
                <View />
                {this.renderBottom(R.strings().continue, () =>
                  NavigationUtil.navigate(SCREEN_ROUTER_INTRO.STEP2)
                )}
              </View>
            </View>
          </View>
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
  bottom: {
    width: "100%",
    position: "absolute",
    bottom: 20
  }
});

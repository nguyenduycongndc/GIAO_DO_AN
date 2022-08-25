import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Image,
  TouchableOpacity,
  ScrollView
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  SCREEN_ROUTER_INTRO,
  SCREEN_ROUTER_AUTH
} from "@app/constants/Constants";
import theme from "@app/constants/Theme";
import ButtonPrimary from "@app/components/ButtonPrimary";
export class IntroStep3Screen extends Component {
  render() {
    return (
      <ScreenComponent
        renderView={
          <View style={styles.container}>
            <ScrollView style={{ flex: 1 }}>
              <Image
                source={R.images.imgGif}
                style={{ width: "100%", resizeMode: "contain" }}
              />
              <Text style={styles.textFont}>
                Ưu đãi có thể nhận được khi sử dụng app
              </Text>
              <Text style={{ fontFamily: R.fonts.quicksand_medium }}>
                'Cụ thể, khách hàng sẽ có cơ hội nhận gói “Deal Đủ Đầy” trị giá
                500,000đ bao gồm:{"\n"}
                {"\n"} + 02 mã giảm giá trị giá 50.000 VNĐ áp dụng cho giao dịch
                mua sắm hàng hóa, dịch vụ tại Vinmart, Vinmart~
                {"\n"}
                {"\n"} + từ 100.000 VNĐ sử dụng tính năng “Thanh toán” trên ứng
                dụng VinID qua hình thức thanh toán bằng Ví điện tử VinID Pay. ~
                {"\n"}
                {"\n"}+ 01 mã giảm giá trị giá 50.000 VNĐ áp dụng cho giao dịch
                mua sắm hàng hóa,'
              </Text>
            </ScrollView>
            <View style={[styles.viewButton, styles.bottom]}>
              {/* {this.renderBottom(R.strings().skip, () =>
                NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH)
              )} */}
              <View />
              {this.renderBottom(R.strings().continue, () =>
                NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH)
              )}
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
  container: {
    backgroundColor: theme.colors.white,
    flex: 1,
    // alignItems: "center",
    padding: 10
  },
  textFont: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    alignItems: "center",
    marginBottom: 20
  },
  textStyle: {
    color: theme.colors.active,
    textDecorationLine: "underline"
  },
  viewButton: {
    flexDirection: "row",
    justifyContent: "space-between",
    paddingHorizontal: 15,
    backgroundColor: theme.colors.white
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
    width: theme.dimension.width,
    marginTop: 20
    // position: "absolute",
    // paddingBottom: 20,
    // paddingTop: 20,
    // bottom: 0
  }
});

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(IntroStep3Screen);

import React, { Component } from "react";
import { Text, StyleSheet, View, ScrollView } from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import FastImg from "@app/components/FastImage";
import StepIndicator from "react-native-step-indicator";
import { colors } from "@app/constants/Theme";
import ScrollableTabView, {
  ScrollableTabBar
} from "react-native-scrollable-tab-view";
const text = `Ưu đãi thành viên
  Tích lũy tiêu dùng: Từ 0-999.999
  Tích điểm khi thanh toán tiền mặt: 1%
  Tích điểm khi thanh toán VNpay: 2%
  Ưu đãi chào mừng lên hạng: 50,000đ
  Ưu đãi sinh nhật: 200% tích điểm trong 14 ngày (7 ngày trước và sau sinh nhật)`;
export default class RankAcountInfoScreen extends Component {
  MembershipIncentives = () => (
    <ScrollView
      children={
        <>
          {this.renderItemMembershipIncentives(
            R.images.ic_user_rank,
            "Member",
            "0-999,999",
            text,
            "#2E2E2E"
          )}
          {this.renderItemMembershipIncentives(
            R.images.ic_star,
            "Loyal Member",
            "10,000,000-14,999,999",
            text,
            "#6B6B6B"
          )}
          {this.renderItemMembershipIncentives(
            R.images.ic_award,
            "VIP",
            "15,000,000-24,999,999",
            text,
            "#EDD30C"
          )}
          {this.renderItemMembershipIncentives(
            R.images.ic_award_blue,
            "VVIP",
            "Trên 25,000,000",
            text,
            colors.primary
          )}
        </>
      }
    />
  );
  RankingPolicy = () => (
    <Text
      children={`Tích lũy tiêu dùng được tính từ ngày lập tài khoản
  thành công tới 1 năm sau. Nếu năm sau không tiếp
  tục duy trì mức chi tiêu của hạng đó thì sẽ bị xuống 
  hạng. `}
    />
  );

  renderItemMembershipIncentives = (
    icon,
    title,
    rangePoint,
    content,
    color
  ) => (
    <>
      <View
        style={styles.root_item}
        children={
          <>
            <FastImg source={icon} style={styles.img_user} />
            <Text style={[styles.item_title, { color }]} children={title} />
            <Text style={styles.item_line} children={" | "} />
            <Text style={styles.rangePoint} children={rangePoint} />
          </>
        }
      />
      <Text style={styles.item_content} children={content} />
    </>
  );

  render() {
    return (
      <ScreenComponent
        back
        titleHeader={R.strings().member_info}
        renderView={
          <>
            <View
              style={styles.root_header}
              children={
                <>
                  <View
                    style={styles.root_title}
                    children={
                      <>
                        <FastImg
                          source={R.images.ic_user_rank}
                          style={styles.img_user}
                        />
                        <Text
                          style={styles.title_header}
                          children={"Le Ngoc Anh".toUpperCase()}
                        />
                      </>
                    }
                  />
                  <View
                    style={styles.root_current_point}
                    children={
                      <>
                        <FastImg
                          source={R.images.ic_award}
                          style={styles.ic_award}
                          resizeMode="contain"
                        />
                        <Text style={styles.title_vip} children="VIP" />
                        <Text style={styles.title_line} children=" | " />
                        <Text
                          style={styles.title_current_point}
                          children="20.000.000"
                        />
                      </>
                    }
                  />
                </>
              }
            />
            <StepIndicator
              customStyles={customStyles}
              currentPosition={0.2}
              labels={["Member", "Loyal menber", "VIP", "VVIP"]}
              stepCount={4}
              renderStepIndicator={props => (
                <>
                  <FastImg
                    source={
                      R.images[
                        props.stepStatus == "finished"
                          ? "ic_finished"
                          : "ic_unfinished"
                      ]
                    }
                    style={{ width: 25, height: 25 }}
                    resizeMode="contain"
                  />
                </>
              )}
            />
            <Text
              style={styles.description}
              children="Bạn cần 3,200,000đ để lên thành viên VVIP. Tăng hạng để
              được nhiều ưu đãi hơn"
            />
            <ScrollableTabView
              style={{
                borderColor: colors.border
              }}
              tabBarBackgroundColor={colors.white}
              tabBarPosition="top"
              tabBarActiveTextColor={colors.primary}
              tabBarInactiveTextColor={colors.black}
              tabBarUnderlineStyle={{
                height: 3,
                backgroundColor: colors.primary
              }}
              renderTabBar={() => <ScrollableTabBar />}
              tabBarTextStyle={R.fonts.quicksand_bold}
            >
              <this.MembershipIncentives
                tabLabel={R.strings().membership_incentives}
              />
              <this.RankingPolicy tabLabel={R.strings().ranking_policy} />
            </ScrollableTabView>
          </>
        }
      />
    );
  }
}
const customStyles = {
  stepIndicatorSize: 30,
  currentStepIndicatorSize: 30,
  separatorStrokeWidth: 2,
  currentStepStrokeWidth: 0,
  stepStrokeCurrentColor: "#ffffff",
  stepStrokeWidth: 0,
  stepStrokeFinishedColor: "#ffffff",
  stepStrokeUnFinishedColor: "#ffffff",
  separatorFinishedColor: colors.primary,
  separatorUnFinishedColor: "#aaaaaa",
  stepIndicatorFinishedColor: "#ffffff",
  stepIndicatorUnFinishedColor: "#ffffff",
  stepIndicatorCurrentColor: "#ffffff",
  stepIndicatorLabelFontSize: 13,
  currentStepIndicatorLabelFontSize: 13,
  stepIndicatorLabelCurrentColor: "#fe7013",
  stepIndicatorLabelFinishedColor: "#ffffff",
  stepIndicatorLabelUnFinishedColor: "#aaaaaa",
  labelColor: "#999999",
  labelSize: 14,
  currentStepLabelColor: colors.primary,
  labelFontFamily: R.fonts.quicksand_regular
};
const styles = StyleSheet.create({
  root_current_point: {
    flexDirection: "row"
  },
  root_item: { flexDirection: "row", padding: 10 },
  item_content: {
    paddingHorizontal: 10,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12
  },
  rangePoint: {
    alignSelf: "center",
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: colors.primary
  },
  item_line: {
    alignSelf: "center",
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: "#6B6B6B"
  },
  item_title: {
    alignSelf: "center",
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14
  },
  description: {
    textAlign: "center",
    marginTop: 20,
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 13,
    color: "#939393"
  },
  title_current_point: {
    color: colors.primary,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    alignSelf: "center"
  },
  title_line: {
    color: "#BFBFBF",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    alignSelf: "center"
  },
  title_vip: {
    color: "#EDD30C",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    alignSelf: "center"
  },
  root_header: {
    flexDirection: "row",
    padding: 10,
    marginBottom: 20
  },
  root_title: { flexDirection: "row", flex: 1 },
  img_user: {
    width: 25,
    height: 25,
    marginEnd: 10
  },
  title_header: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    alignSelf: "center"
  },
  ic_award: {
    width: 16,
    height: 16,
    marginHorizontal: 10,
    alignSelf: "center"
  }
});

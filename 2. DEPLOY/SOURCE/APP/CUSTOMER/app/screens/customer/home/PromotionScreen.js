import React, { Component } from "react";
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl
} from "react-native";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import ScrollableTabView, {
  DefaultTabBar,
  ScrollableTabBar
} from "react-native-scrollable-tab-view";
import R from "@app/assets/R";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import theme, { colors } from "@theme";
import I18n from "@i18";
import {
  TYPE_COUPON,
  SCREEN_ROUTER_CUSTOMER,
  TYPE_NAVIGATION,
  TYPE_DISCOUNT
} from "@constant";
import { sendVoucher } from "@action/";
import NavigationUtil from "@app/navigation/NavigationUtil";
import Empty from "@app/components/Empty";
import Loading from "@app/components/Loading";
import { ImageBackground } from "react-native";
import reactotron from "reactotron-react-native";
import { formatMoney } from "@app/constants/Functions";

export class PromotionScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const type = navigation.getParam("type", {});
    this.state = {
      isLoading: false,
      error: "",
      type: type,
      unworn: "",
      used: ""
    };
  }

  componentDidMount() {
    this.getData();
  }

  getData = () => {
    this.getListUnworn();
    this.getListUsed();
  };
  getListUnworn = () => {
    CallApiHelper(API.getListCoupon, TYPE_COUPON.UNWORN, this, res => {
      this.setState({
        unworn: res.result
      });
      const { code, discount } = this.props.voucherReducer;
      this.props.sendVoucher(TYPE_DISCOUNT.NORMAL, code, discount, res.result);
    });
  };

  getListUsed = () => {
    CallApiHelper(API.getListCoupon, TYPE_COUPON.USED, this, res => {
      this.setState({
        used: res.result
      });
    });
  };

  render() {
    const { isLoading, error, unworn, used } = this.state;
    return (
      <ScreenComponent
        back
        titleHeader={R.strings().endow}
        renderView={
          <>
            <ScrollableTabView
              style={{
                borderColor: theme.colors.border
              }}
              tabBarBackgroundColor={theme.colors.white}
              tabBarPosition="top"
              tabBarActiveTextColor={theme.colors.primary}
              tabBarInactiveTextColor={theme.colors.textColor}
              tabBarUnderlineStyle={{
                height: 3,
                backgroundColor: theme.colors.primary
              }}
              renderTabBar={() => <ScrollableTabBar />}
              tabBarTextStyle={R.fonts.quicksand_bold}
              initialPage={0}
            >
              {this._renderTabView(
                R.strings().unworn,
                unworn,
                R.strings().empty_voucher
              )}
              {this._renderTabView(
                R.strings().used,
                used,
                R.strings().empty_voucher_used
              )}
            </ScrollableTabView>
          </>
        }
      />
    );
  }
  _renderTabView = (tabLabel, data, empty) => {
    return (
      <View tabLabel={tabLabel} style={styles.view}>
        {this.state.isLoading ? (
          <Loading />
        ) : (
          <FlatList
            refreshControl={
              <RefreshControl
                refreshing={this.state.isLoading}
                onRefresh={this.getData}
              />
            }
            data={data}
            ListEmptyComponent={
              <Empty description={empty} marginTop={height / 5} />
            }
            keyExtractor={(item, index) => index.toString()}
            renderItem={item => this._renderPromotionItem(item, tabLabel)}
          />
        )}
      </View>
    );
  };
  _renderPromotionItem = ({ item, index }, tabLabel) => {
    return (
      <TouchableOpacity
        onPress={() => {
          this.props.sendVoucher(
            item.typeDiscount,
            item.couponCode,
            item.discount
          );
          if (this.state.type == TYPE_NAVIGATION.HOME) {
            NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.BOOKING);
          } else if (this.state.type == TYPE_NAVIGATION.BOOKING) {
            this.props.navigation.state.params.checkTotalMoney(
              item.discount,
              item.typeDiscount
            );
            NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE);
          }
        }}
        disabled={tabLabel != R.strings().unworn}
        style={{
          paddingHorizontal: 14,
          backgroundColor: colors.white,
          marginVertical: 5,
          paddingVertical: 10
        }}
      >
        <View
          style={{
            flexDirection: "row",
            alignItems: "center",
            justifyContent: "space-between",
            paddingVertical: 9,
            borderBottomColor: theme.colors.gray,
            borderBottomWidth: 0.5
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              color: theme.colors.textColor,
              textTransform: "uppercase"
            }}
          >
            Voucher {item.couponCode}
          </Text>
          {tabLabel == R.strings().unworn && (
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 14,
                color: theme.colors.primary
              }}
            >
              {R.strings().use_now}
            </Text>
          )}
        </View>
        <View style={{ flexDirection: "row" }}>
          <View style={{ paddingTop: 7, flex: 3 }}>
            {!!item.expriceDateStr && (
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_medium,
                  fontSize: 14,
                  marginBottom: 5,
                  color: theme.colors.nameText
                }}
                children={`- ${R.strings().exprice_date} : ${
                  item.expriceDateStr
                }`}
              />
            )}
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 12,
                marginBottom: 5,
                color: theme.colors.nameText
              }}
              children={` - ${item.content}`}
            />
          </View>
          <Text
            style={{
              textAlign: "right",
              flex: 1,
              alignSelf: "center",
              textAlignVertical: "center",
              color: colors.primary,
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 16
            }}
            children={
              item.typeDiscount == TYPE_DISCOUNT.NORMAL
                ? `- ${formatMoney(item?.discount || 0)} đ`
                : `- ${item?.discount} %`
            }
          />
        </View>
      </TouchableOpacity>
    );
  };
}

const styles = StyleSheet.create({
  view: {
    flex: 1,
    backgroundColor: theme.colors.backgroundColor,
    position: "relative"
  }
});

const mapStateToProps = state => ({
  lang: state.lang,
  voucher: state.voucherReducer
});

const mapDispatchToProps = {
  sendVoucher
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(PromotionScreen);

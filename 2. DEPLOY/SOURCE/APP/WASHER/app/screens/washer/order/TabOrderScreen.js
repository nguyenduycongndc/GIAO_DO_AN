import React, { Component } from "react";
import { View, Text } from "react-native";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import R from "@app/assets/R";
import ScreenComponent from "@app/components/ScreenComponent";
import OrderProcessingScreen from "@screen_washer/order/OrderProcessingScreen";
import OrderHistoryScreen from "@screen_washer/order/OrderHistoryScreen";
import OrderUpcomingScreen from "@screen_washer/order/OrderUpcomingScreen";
import OrderPendingScreen from "@screen_washer/order/OrderPendingScreen";
import ScrollableTabView, {
  ScrollableTabBar
} from "react-native-scrollable-tab-view";
import theme from "@theme";
import reactotron from "reactotron-react-native";
import { navigateTab, getUserInfo } from "@app/redux/actions";

class TabOrderScreen extends Component {
  onChangeTab = ({ i, from }) => this.props.navigateTab(i);

  render() {
    return (
      <ScreenComponent
        titleHeader={R.strings().transaction}
        rightComponent={
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              color: this.props.locationState.gpsState
                ? theme.colors.headerTitle
                : theme.colors.red
            }}
          >
            {this.props.locationState.gpsState
              ? R.strings().gps_on
              : R.strings().gps_off}
          </Text>
        }
        renderView={
          <View style={{ flex: 1 }}>
            <ScrollableTabView
              style={{
                borderColor: theme.colors.border
              }}
              ref={tabView => {
                if (tabView != null) {
                  this.tabView = tabView;
                }
              }}
              onChangeTab={this.onChangeTab}
              page={this.props.navigationState.initialPage}
              tabBarBackgroundColor={theme.colors.white}
              tabBarPosition="top"
              tabBarActiveTextColor={theme.colors.primary}
              tabBarInactiveTextColor={theme.colors.primary}
              tabBarUnderlineStyle={{
                height: 3,
                backgroundColor: theme.colors.primary
              }}
              renderTabBar={() => <ScrollableTabBar />}
              tabBarTextStyle={{ fontFamily: R.fonts.quicksand_bold }}
            >
              <OrderProcessingScreen tabLabel={R.strings().processing} />
              <OrderPendingScreen tabLabel={R.strings().pending} />
              <OrderUpcomingScreen tabLabel={R.strings().upcoming} />
              <OrderHistoryScreen tabLabel={R.strings().history} />
            </ScrollableTabView>
          </View>
        }
      />
    );
  }

  componentDidMount() {
    this.props.getUserInfo();
  }
}

const mapStateToProps = state => ({
  lang: state.lang,
  locationState: state.locationReducer,
  navigationState: state.navigateTabReducer
});

const mapDispatchToProps = {
  navigateTab,
  getUserInfo
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TabOrderScreen);

import React, { Component } from "react";
import { View, Text } from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import ScrollableTabView, {
  DefaultTabBar,
  ScrollableTabBar
} from "react-native-scrollable-tab-view";
import ProcessingOrderScreen from "./ProcessingOrderScreen";
import UpcomingOrderScreen from "./UpcomingOrderScreen";
import HistoryOrderScreen from "./HistoryOrderScreen";
import R from "@app/assets/R";
import theme, { colors } from "@theme";
import { SCREEN_ROUTER_CUSTOMER } from "@constant";
import { setState, navigateTab } from "@action/index";
class TabOrderSreen extends Component {
  onChangeTab = ({ i, from }) => this.props.navigateTab(i);

  render() {
    const { initialPage = 1 } = this.props.navigateTabState;

    return (
      <ScreenComponent
        titleHeader={R.strings().order_tab_cus}
        rightComponent={
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 13,
              color: colors.primary,
              backgroundColor: colors.white,
              padding: 5,
              borderRadius: 3,
              overflow: "hidden"
            }}
            children="GPS ON"
          />
        }
        renderView={
          <>
            <ScrollableTabView
              style={{
                borderColor: theme.colors.border
              }}
              tabBarBackgroundColor={theme.colors.white}
              tabBarPosition="top"
              tabBarActiveTextColor={theme.colors.primary}
              tabBarInactiveTextColor={theme.colors.black}
              tabBarUnderlineStyle={{
                height: 3,
                backgroundColor: theme.colors.primary
              }}
              renderTabBar={() => <ScrollableTabBar />}
              tabBarTextStyle={R.fonts.quicksand_bold}
              onChangeTab={this.onChangeTab}
              page={initialPage}
              initialPage={1}
            >
              <ProcessingOrderScreen tabLabel={R.strings().processing} />
              <UpcomingOrderScreen tabLabel={R.strings().upcoming} />
              <HistoryOrderScreen tabLabel={R.strings().history} />
            </ScrollableTabView>
          </>
        }
      />
    );
  }
}

const mapStateToProps = state => ({
  lang: state.lang,
  navigateTabState: state.NavigateTabReducer
});

const mapDispatchToProps = {
  setState,
  navigateTab
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TabOrderSreen);

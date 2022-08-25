import React, { Component, useState, useEffect } from "react";
import {
  View,
  Text,
  RefreshControl,
  FlatList,
  StyleSheet,
  TouchableOpacity
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme, { colors } from "@theme";
import ScrollableTabView, {
  ScrollableTabBar
} from "react-native-scrollable-tab-view";
import Empty from "@app/components/Empty";
import ItemProduction from "@app/components/ItemProduction";
import callAPI, { callAPIHook } from "@app/utils/CallApiHelper";
import { GetOrders } from "@app/constants/Api";
import {
  ORDER_PRODUCTION_STATUS,
  SCREEN_ROUTER_WASHER
} from "@app/constants/Constants";
import LoadableImage from "@app/components/LoadableImage";
import NavigationUtil from "@app/navigation/NavigationUtil";
const TabOrder = props => {
  const [data, useData] = useState([]);
  const [isLoading, useIsLoading] = useState([]);

  useEffect(() => {
    getData();
  }, []);

  const getData = () =>
    callAPIHook(GetOrders, props.status, useIsLoading, res => {
      useData(res.result);
    });
  return (
    <ScreenComponent
      isLoading={isLoading}
      renderView={
        <FlatList
          showsVerticalScrollIndicator={false}
          refreshControl={
            <RefreshControl
              refreshing={false}
              onRefresh={() => {
                getData();
              }}
            />
          }
          ListEmptyComponent={<Empty description={R.strings().notify_emplty} />}
          keyExtractor={(item, index) => index.toString()}
          data={data}
          renderItem={({ item, index }) => ItemOrderProduction(item)}
        />
      }
    />
  );
};

const ItemOrderProduction = item => {
  return (
    <TouchableOpacity
      onPress={() =>
        NavigationUtil.navigate(
          SCREEN_ROUTER_WASHER.ORDER_PRODUCTION_DETAIL,
          item
        )
      }
      style={{
        ...styles.root_item
      }}
      children={
        <>
          <LoadableImage
            source={R.images.ic_zalo}
            style={styles.image}
            resizeMode="contain"
          />
          <View
            style={{ flex: 2 }}
            children={
              <>
                <Text style={styles.title_item} children={item.code} />
                <Text
                  style={styles.content_item}
                  children={`Số lượng: ${item.qty}`}
                />
                <Text style={styles.price_item} children={item.price} />
                <Text
                  style={styles.create_date}
                  children={item.createDateStr}
                />
                <Text style={styles.status} children={item.status} />
              </>
            }
          />
        </>
      }
    />
  );
};

class OrderProductionScreen extends Component {
  render() {
    const lableConfirm =
      R.strings()
        .confirm.toLowerCase()
        .charAt(0)
        .toUpperCase() +
      R.strings()
        .confirm.toLowerCase()
        .slice(1);
    return (
      <ScreenComponent
        titleHeader={R.strings().order}
        back
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
              <TabOrder
                tabLabel="Chờ xác nhận"
                status={ORDER_PRODUCTION_STATUS.WAITING}
              />
              <TabOrder
                tabLabel={lableConfirm}
                status={ORDER_PRODUCTION_STATUS.CONFIRM}
              />
              <TabOrder
                tabLabel={R.strings().history}
                status={ORDER_PRODUCTION_STATUS.ALL}
              />
            </ScrollableTabView>
          </View>
        }
      />
    );
  }
}

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(OrderProductionScreen);
const styles = StyleSheet.create({
  image: {
    flex: 1,
    width: width / 3,
    height: width / 3,
    alignSelf: "center"
  },
  status: {
    color: colors.primary,
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 11,
    position: "absolute",
    top: 0,
    right: 0
  },
  create_date: {
    color: colors.gray,
    fontFamily: R.fonts.quicksand_regular,
    fontSize: 11,
    position: "absolute",
    bottom: 0,
    right: 0
  },
  root_item: {
    flexDirection: "row",
    backgroundColor: colors.white,
    padding: 10,
    borderRadius: 10,
    margin: 10
  },
  title_item: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 18,
    marginHorizontal: 15,
    color: "#444444"
  },
  content_item: {
    maxWidth: "80%",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: "#7B7B7B",
    margin: 15
  },
  price_item: {
    marginHorizontal: 15,
    color: colors.primary,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16,
    position: "absolute",
    bottom: 10
  }
});

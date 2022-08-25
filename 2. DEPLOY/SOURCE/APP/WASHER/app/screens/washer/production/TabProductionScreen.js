import React, { Component, useEffect, useState } from "react";
import {
  View,
  Text,
  FlatList,
  RefreshControl,
  TouchableOpacity,
  Image,
  StyleSheet,
  TextInput
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import ScrollableTabView, {
  ScrollableTabBar
} from "react-native-scrollable-tab-view";
import Empty from "@app/components/Empty";
import FastImage from "react-native-fast-image";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER, REDUCER_WASHER } from "@app/constants/Constants";
import callAPI, { callAPIHook } from "@app/utils/CallApiHelper";
import { GetProductCate, GetProduct, GetCart } from "@app/constants/Api";
import reactotron from "reactotron-react-native";
import { updateCart } from "@app/redux/actions";
import ItemProduction from "@app/components/ItemProduction";
import CartIcon from "@app/components/CartIcon";

class TabProductionScreen extends Component {
  componentWillMount() {
    callAPI(GetProductCate, null, this, res => {
      this.setState({ data: res.result });
    });
    callAPI(GetCart, null, this, res => {
      res.result.map(elem => this.props.updateCart(elem));
    });
  }

  renderContentTabBar = ({ data }) => {
    const [content, useContent] = useState([]);
    const [isLoading, useLoading] = useState([]);
    useEffect(() => {
      getData();
    }, []);
    const getData = () =>
      callAPIHook(GetProduct, data.id, useLoading, res => {
        useContent(res.result);
      });
    return (
      <ScreenComponent
        isLoading={isLoading}
        renderView={
          <View
            style={{ backgroundColor: colors.backgroundColor }}
            children={
              <>
                <FlatList
                  showsVerticalScrollIndicator={false}
                  data={content}
                  ListEmptyComponent={
                    <Empty description={R.strings().notify_emplty} />
                  }
                  refreshControl={
                    <RefreshControl
                      refreshing={false}
                      onRefresh={() => {
                        getData();
                      }}
                    />
                  }
                  keyExtractor={(item, index) => index.toString()}
                  renderItem={({ item, index }) =>
                    ItemProduction(
                      item,
                      <TouchableOpacity
                        onPress={() => {
                          this.props.updateCart(item);
                        }}
                        children={
                          <FastImage
                            source={R.images.ic_add_production}
                            style={styles.img_add_production}
                            resizeMode="contain"
                          />
                        }
                      />
                    )
                  }
                />
              </>
            }
          />
        }
      />
    );
  };

  render() {
    const data = this.state.data || [];
    return (
      <ScreenComponent
        titleHeader={R.strings().production}
        rightComponent={<CartIcon />}
        renderView={
          <>
            <View
              style={{ backgroundColor: colors.primary }}
              children={
                <TextInput
                  placeholder="Tìm kiếm tên sản phẩm"
                  style={styles.search}
                  placeholderTextColor="white"
                  inlineImageLeft={R.images.ic_search}
                />
              }
            />
            <ScrollableTabView
              ref={tabView => {
                if (tabView != null) {
                  this.tabView = tabView;
                }
              }}
              tabBarBackgroundColor={colors.primary}
              tabBarPosition="top"
              tabBarActiveTextColor={"#F7FF11"}
              tabBarInactiveTextColor={"#D4D4D4"}
              tabBarUnderlineStyle={{
                height: 0
              }}
              renderTabBar={() => (
                <ScrollableTabBar style={{ borderWidth: 0 }} />
              )}
              tabBarTextStyle={{ fontFamily: R.fonts.quicksand_bold }}
            >
              {data.map(elem => (
                <this.renderContentTabBar data={elem} tabLabel={elem.name} />
              ))}
            </ScrollableTabView>
          </>
        }
      />
    );
  }
}

const mapStateToProps = state => ({
  cart: state[REDUCER_WASHER.CART]
});

const mapDispatchToProps = {
  updateCart
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TabProductionScreen);

const styles = StyleSheet.create({
  search: {
    fontSize: 12,
    fontFamily: R.fonts.quicksand_medium,
    padding: 15,
    backgroundColor: "rgba(226,226,226,0.5)",
    color: "white",
    borderRadius: 10,
    margin: 10
  },
  background_header: {
    backgroundColor: colors.primary,
    height: width * 0.4
  },
  img_add_production: {
    width: 25,
    height: 25,
    right: -width * 0.35
  }
});

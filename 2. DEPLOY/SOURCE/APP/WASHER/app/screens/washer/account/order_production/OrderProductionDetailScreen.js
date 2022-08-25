import React, { Component } from "react";
import {
  View,
  Text,
  FlatList,
  RefreshControl,
  TouchableOpacity,
  StyleSheet
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import callAPI from "@app/utils/CallApiHelper";
import { GetOrderDetail } from "@app/constants/Api";
import Empty from "@app/components/Empty";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import LoadableImage from "@app/components/LoadableImage";

class OrderProductionDetailScreen extends Component {
  state = {
    data: {}
  };
  componentWillMount() {
    this.getData();
  }

  ItemProduction = item => {
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
              style={{ flex: 2, alignSelf: "center" }}
              children={
                <>
                  <Text style={styles.title_item} children={item.name} />
                  <Text
                    style={styles.qty_item}
                    children={`Số lượng: ${item.qty}`}
                  />
                  <Text style={styles.price_item} children={item.price} />
                </>
              }
            />
          </>
        }
      />
    );
  };
  getData = () => {
    const { orderID } = this.props.navigation.state.params;
    callAPI(GetOrderDetail, orderID, this, res => {
      this.setState({ data: res.result });
    });
  };
  render() {
    const { data } = this.state;
    return (
      <ScreenComponent
        titleHeader="Đơn hàng"
        back
        renderView={
          <>
            <View
              style={styles.root}
              children={
                <>
                  <Text style={styles.text_name} children={data.washerName} />
                  <Text style={styles.text_phone} children={data.washerPhone} />
                </>
              }
            />
            <FlatList
              showsVerticalScrollIndicator={false}
              refreshControl={
                <RefreshControl refreshing={false} onRefresh={this.getData} />
              }
              ListEmptyComponent={
                <Empty description={R.strings().notify_emplty} />
              }
              keyExtractor={(item, index) => index.toString()}
              data={data.listOrderProductDetail}
              renderItem={({ item, index }) => this.ItemProduction(item)}
            />
            <View
              children={
                <>
                  <View
                    style={styles.root_footer}
                    children={
                      <>
                        <Text
                          style={styles.text_sum_price}
                          children={"Tổng tiền"}
                        />
                        <Text style={styles.text_price} children={data.price} />
                      </>
                    }
                  />
                  <TouchableOpacity
                    children={
                      <Text
                        style={styles.cancel_order}
                        children={"Huỷ đơn".toUpperCase()}
                      />
                    }
                  />
                </>
              }
            />
          </>
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
)(OrderProductionDetailScreen);
const styles = StyleSheet.create({
  text_name: {
    marginBottom: 10,
    fontFamily: R.fonts.quicksand_semi_bold,
    fontSize: 16
  },
  text_phone: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: colors.primary
  },
  root_footer: {
    flexDirection: "row",
    justifyContent: "space-between",
    margin: 15
  },
  text_sum_price: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16,
    color: "#444444"
  },
  text_price: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: colors.red
  },
  cancel_order: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16,
    backgroundColor: colors.gray,
    color: colors.white,
    margin: 10,
    padding: 10,
    textAlign: "center",
    borderRadius: 5,
    overflow: "hidden"
  },
  root: {
    margin: 10,
    padding: 10,
    backgroundColor: "white",
    borderRadius: 10
  },
  image: {
    flex: 1,
    width: width / 3,
    height: width / 3,
    alignSelf: "center"
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
    fontSize: 16,
    margin: 5,
    color: "#444444"
  },
  price_item: {
    margin: 5,
    color: colors.primary,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16
  },
  qty_item: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16,
    margin: 5,
    color: colors.gray
  }
});

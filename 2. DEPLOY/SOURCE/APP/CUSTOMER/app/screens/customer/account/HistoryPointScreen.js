import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  FlatList,
  Image,
  RefreshControl
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import NumberFormat from "react-number-format";
import Dropdown from "@app/components/Dropdown";
import AsyncStorage from "@react-native-community/async-storage";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import { numberWithCommas } from "@app/constants/Functions";
import Empty from "@app/components/Empty";
import FastImage from "@app/components/FastImage";

export default class HistoryPointScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
      error: "",
      getPointHistory: [],
      fromDate: ""
    };
  }
  componentDidMount() {
    this.getData();
  }

  getData() {
    CallApiHelper(API.getPointHistory, this.state.fromDate, this, res => {
      this.setState({
        getPointHistory: res.result
      });
    });
  }
  renderItem = ({ item, index }) => (
    <View style={styles.root_item}>
      <View
        style={styles.row_item}
        children={
          <>
            <Text style={styles.text_title} children={item.tittle} />
            <NumberFormat
              value={item.point}
              displayType="text"
              thousandSeparator
              suffix="đ"
              prefix={item.transactionType == 1 ? "+" : "-"}
              renderText={value => (
                <Text
                  style={[
                    styles.text_money,
                    {
                      color:
                        item.transactionType == 1 ? colors.primary : colors.red
                    }
                  ]}
                  children={value}
                />
              )}
            />
          </>
        }
      />

      <Text
        style={styles.text_des}
        children={
          R.strings().balance_money +
          ": " +
          `${numberWithCommas(item.afterBalance.toString())}`
        }
      />
      <View
        style={{
          flexDirection: "row",
          justifyContent: "space-between",
          alignItems: "center"
        }}
      >
        <Text
          style={styles.text_des}
          children={R.strings().trading_code + ": " + item.code}
        />
        <View style={{ flexDirection: "row", alignItems: "center" }}>
          <Text
            style={styles.text_des}
            children={R.strings().time + ": " + item.createDateStr}
          />
          <FastImage
            source={R.images.ic_clock}
            style={{ width: 14, height: 14, marginLeft: 5 }}
          />
        </View>
      </View>
    </View>
  );

  render() {
    const { getPointHistory, isLoading, error } = this.state;
    return (
      <ScreenComponent
        back
        isLoading={isLoading}
        error={error}
        reload={() => this.getData()}
        titleHeader={R.strings().point_history}
        renderView={
          getPointHistory.length == 0 ? (
            <Empty description={R.strings().no_history_point_now} />
          ) : (
            <View
              style={{
                flex: 1,
                backgroundColor: colors.backgroundColor,
                paddingTop: 10
              }}
            >
              <FlatList
                refreshControl={
                  <RefreshControl
                    refreshing={this.state.isLoading}
                    onRefresh={() => this.getData()}
                  />
                }
                style={{ zIndex: -1 }}
                data={getPointHistory}
                renderItem={this.renderItem}
              />
            </View>
          )
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  text_money: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    textAlign: "right",
    flex: 1
  },
  text_title: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    width: "70%"
  },
  text_des: {
    color: colors.gray,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12,
    marginVertical: 2
  },
  dropdown_time: {
    backgroundColor: colors.backgroundColor,
    width: "50%",
    alignSelf: "flex-end"
  },
  wallet_select: {
    backgroundColor: colors.backgroundColor,
    width: "50%",
    paddingStart: 10
  },
  root_dropdown: { flexDirection: "row", paddingVertical: 15 },
  root_item: {
    borderWidth: 0.25,
    borderColor: colors.gray,
    backgroundColor: colors.white,
    marginBottom: 10,
    padding: 10,
    paddingHorizontal: 14
  },
  row_item: { flexDirection: "row", marginVertical: 5 }
});

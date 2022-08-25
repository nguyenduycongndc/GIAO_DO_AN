import React, { Component } from "react";
import { Text, StyleSheet, View, FlatList, RefreshControl } from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import NumberFormat from "react-number-format";
import Dropdown from "@app/components/ModalDropdown";
import * as API from "@app/constants/Api";
import CallApiHelper from "../../../utils/CallApiHelper";
import Empty from "@app/components/Empty";
import { numberWithCommas, convertDate } from "@app/constants/Functions";
import Icon from "@app/components/Icon";
import { RowImageLable } from "@app/components/FormRow";
import { Calendar } from "react-native-calendars";
import { WALLET, HISTORY_WITHDRAW_ITEM_TYPE } from "@app/constants/Constants";
import MarkerDate from "@app/components/MarkerDate";
import reactotron from "reactotron-react-native";
import Loading from "@app/components/Loading";
export default class HistoryScreen extends Component {
  initState = {
    data: [],
    isShowDatePicker: false,
    type: "",
    fromDate: "",
    toDate: "",
    startDay: "",
    endDay: "",
    refreshing: false
  };
  state = this.initState;

  componentDidMount() {
    this.getData();
  }

  getData = () => {
    const { type, fromDate, toDate } = this.state;
    CallApiHelper(
      API.getPointHistory,
      {
        fromDate: fromDate,
        toDate: toDate,
        type: type
      },
      this,
      res => {
        this.setState({
          data: res.result,
          isLoading: false,
          refreshing: false
        });
      },
      error => {
        this.setState({
          isLoading: false,
          refreshing: false,
          error: error
        });
        console.log("error", error);
      }
    );
  };

  renderItem = ({ item, index }) => (
    <View style={styles.root_item}>
      <View>
        <Text
          style={[
            styles.text_money,
            {
              color:
                item.transactionType == HISTORY_WITHDRAW_ITEM_TYPE.RECEIVE_MONEY
                  ? colors.primary
                  : colors.red
            }
          ]}
          children={`${
            item.transactionType == HISTORY_WITHDRAW_ITEM_TYPE.RECEIVE_MONEY
              ? "+"
              : "-"
          }${numberWithCommas(`${item.point}`)}đ`}
        />
      </View>
      <View style={{ flex: 1 }}>
        <Text style={styles.text_title} children={`${item.tittle}`} />
        <Text
          style={styles.text_des}
          children={
            `${R.strings().balance}: ` +
            numberWithCommas(`${item.beforeBalance}`)
          }
        />
        <Text
          style={styles.text_des}
          children={`${R.strings().trade_code}: ` + item.code}
        />
        <Text
          style={styles.text_des}
          children={`${R.strings().time}: ` + item.createDateStr}
        />
      </View>
    </View>
  );
  renderHeader = () => (
    <View
      style={styles.root_dropdown}
      children={
        <>
          <View>
            <Dropdown
              data={[R.strings().income_wallet, R.strings().deposit_wallet]}
              style={styles.wallet_select}
              textStyle={styles.text_dropdown}
              imageStyle={styles.image_dropdown}
              dropDownStyle={styles.dropdownStyle}
              defaultValue={R.strings().select_wallet}
              onSelect={(i, val) => {
                this.onChangeState(
                  "type",
                  val == R.strings().income_wallet
                    ? WALLET.INCOME
                    : WALLET.DEPOSIT
                );
              }}
            />
          </View>
          <View style={styles.lable}>
            <RowImageLable
              disableTouch={false}
              lable={
                this.state.fromDate == ""
                  ? R.strings().select_day
                  : `${R.strings().from_day}: ${this.state.fromDate} - ${
                      this.state.toDate
                    }`
              }
              size={12}
              position="right"
              onPress={() =>
                this.setState({
                  ...this.state,
                  isShowDatePicker: !this.state.isShowDatePicker
                })
              }
              image={
                <Icon.MaterialIcons
                  name="keyboard-arrow-down"
                  size={12}
                  style={{ marginTop: 2 }}
                />
              }
              textColor={colors.black}
            />
          </View>
        </>
      }
    />
  );
  onChangeState = async (key, value) => {
    await this.setState({ [key]: value });
    this.getData();
  };
  _renderCalenderSelect() {
    const { startDay, endDay } = this.state;
    return (
      <View style={{ flex: 1 }}>
        <MarkerDate
          endDay={endDay}
          startDay={startDay}
          onPressDelete={() => this.onPressSetDay()}
          onPressSelect={this.onPressSetDay}
        />
      </View>
    );
  }
  onPressSetDay = (startDay, endDay) => {
    this.setState(
      {
        ...this.state,
        isShowDatePicker: false,
        fromDate: startDay ? convertDate(startDay) : "",
        toDate: endDay ? convertDate(endDay) : "",
        startDay: startDay,
        endDay: endDay,
        page: 1
      },
      () => this.getData()
    );
  };
  render() {
    const { data, isShowDatePicker, refreshing } = this.state;
    return (
      <ScreenComponent
        back
        isLoading={refreshing}
        titleHeader={R.strings().wallet_history}
        renderView={
          <>
            {this.renderHeader()}
            {isShowDatePicker ? (
              this._renderCalenderSelect()
            ) : (
              <>
                {this.state.isLoading ? (
                  <Loading />
                ) : (
                  <FlatList
                    refreshControl={
                      <RefreshControl
                        refreshing={this.state.isLoading}
                        onRefresh={() => {
                          this.setState({ refreshing: true }, this.getData);
                        }}
                      />
                    }
                    ListEmptyComponent={
                      <Empty description={R.strings().history_empty} />
                    }
                    style={{ zIndex: -1 }}
                    data={data}
                    renderItem={this.renderItem}
                    keyExtractor={(item, index) => index.toString()}
                  />
                )}
              </>
            )}
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  lable: { marginTop: 9 },
  text_money: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 18,
    textAlign: "right"
  },
  text_title: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  text_des: {
    color: colors.gray,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12,
    marginVertical: 2
  },
  dropdown_time: {
    backgroundColor: colors.backgroundColor,
    alignSelf: "flex-end",
    paddingVertical: 5
  },
  wallet_select: {
    backgroundColor: colors.backgroundColor,
    paddingStart: 10,
    paddingVertical: 5,
    alignSelf: "center"
  },
  root_dropdown: { flexDirection: "row" },
  root_item: {
    flex: 1,
    flexDirection: "row-reverse",
    borderTopWidth: 0.5,
    borderBottomWidth: 0.5,
    backgroundColor: colors.white,
    marginBottom: 10,
    padding: 10,
    alignItems: "center"
  },
  row_item: { flexDirection: "row", marginVertical: 5 },
  text_dropdown: { fontSize: 12, color: "black" },
  image_dropdown: { top: 12, right: 0 },
  dropdownStyle: {
    marginTop: -20,
    minWidth: 100,
    marginLeft: 10
  },
  calendar: {
    position: "absolute",
    zIndex: 1,
    width: theme.dimension.width,
    top: 30,
    borderBottomWidth: 1
  }
});

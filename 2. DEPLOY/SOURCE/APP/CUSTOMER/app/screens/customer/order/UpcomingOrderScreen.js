import React, { Component } from "react";
import {
  View,
  Text,
  StyleSheet,
  Dimensions,
  FlatList,
  RefreshControl,
  TouchableOpacity,
  Platform
} from "react-native";
import { connect } from "react-redux";
import R from "@app/assets/R";
import theme from "@theme";
import ScreenComponent from "@app/components/ScreenComponent";
import InfoItem from "@app/components/InfoItem";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_CUSTOMER, ORDER_TYPE, REDUCER_CUSTOM } from "@constant";
import { getOrder, setState } from "@app/redux/actions";
import { GET_ORDER_UPCOMING } from "@app/redux/actions/type";
import Dropdown from "@component/ModalDropdown";
import MarkerDate from "@app/components/MarkerDate";
import { convertDate } from "@app/constants/Functions";
import reactotron from "reactotron-react-native";
import Empty from "@app/components/Empty";
import callAPI from "@app/utils/CallApiHelper";
import { GetServiceFilter } from "@api";

const today = new Date().toISOString().split("T")[0];
const enđay = new Date(new Date().getTime() + 2678400000)
  .toISOString()
  .split("T")[0];
const todayStr = convertDate(today);
const toDateStr = convertDate(enđay);
const initState = {
  status: ORDER_TYPE.ORDER_STATUS_CONFIRM,
  placeID: "",
  fromDate: todayStr,
  toDate: toDateStr,
  distance: "",
  page: 1,
  comboID: "",
  serviceID: "",
  isShowDatePicker: false,
  startDay: today,
  endDay: enđay,
  service: []
};
class UpcomingOrderScreen extends Component {
  constructor(props) {
    super(props);
    this.state = initState;
    props.setState(SCREEN_ROUTER_CUSTOMER.UPCOMING_ORDER, {
      resetData: this.resetData
    });
  }

  resetData = () => {
    this.setState({ page: 1 }, this.getData);
  };

  componentWillMount = () => {
    this.getServiceFilter();
    this.getData();
  };

  getServiceFilter = () => {
    callAPI(GetServiceFilter, null, null, res => {
      const service = [
        {
          serviceID: "",
          serviceName: R.strings().all_service,
          comboID: ""
        }
      ].concat(res.result);
      this.setState({
        service
      });
    });
  };

  getData = () => this.props.getOrder(GET_ORDER_UPCOMING, this.state);

  renderEmpty = () => (
    <Empty
      marginTop={height / 6}
      sourceImage={R.images.empty_procressing_order}
      description={R.strings().no_order_in_upcoming}
    />
  );

  removeDays = () => {
    const today = new Date().toISOString().split("T")[0];
    const enđay = new Date(new Date().getTime() + 604800000)
      .toISOString()
      .split("T")[0];
    const todayStr = convertDate(today);
    const toDateStr = convertDate(enđay);

    this.setState(
      {
        fromDate: todayStr,
        toDate: toDateStr,
        startDay: today,
        endDay: enđay
      },
      this.toggleDatePicker
    );
  };

  selectDays = () => {
    this.toggleDatePicker();
    this.getData();
  };

  toggleDatePicker = () =>
    this.setState({
      isShowDatePicker: !this.state.isShowDatePicker
    });
  renderDatePicker = (endDay, startDay) => (
    <MarkerDate
      endDay={endDay || ""}
      startDay={startDay || ""}
      onPressSelect={this.selectDays}
      onPressDelete={this.removeDays}
      onDayPress={date => {
        var dateNum = Date.parse(date.dateString);
        if (dateNum == Date.parse(startDay) || dateNum == Date.parse(endDay)) {
          this.setDay(date.dateString, date.dateString);
        } else if (dateNum < Date.parse(startDay)) {
          this.setDay(date.dateString, endDay);
        } else this.setDay(startDay, date.dateString);
      }}
    />
  );

  renderList = () => {
    const {
      isLoading,
      data,
      isLastPage,
      isLoadMore
    } = this.props.orderUpcoming;
    return (
      <FlatList
        refreshControl={
          <RefreshControl refreshing={isLoading} onRefresh={this.resetData} />
        }
        onMomentumScrollBegin={() =>
          (this.onEndReachedCalledDuringMomentum = false)
        }
        removeClippedSubviews={true}
        showsHorizontalScrollIndicator={false}
        onEndReachedThreshold={0.5}
        onEndReached={() => {
          if (!this.onEndReachedCalledDuringMomentum) {
            if (!isLastPage && !isLoadMore)
              this.setState({ page: this.state.page + 1 }, this.getData);
            this.onEndReachedCalledDuringMomentum = true;
          }
        }}
        ListEmptyComponent={this.renderEmpty}
        contentContainerStyle={{ paddingVertical: 5 }}
        data={data}
        keyExtractor={(item, index) => index.toString()}
        renderItem={({ item, index }) => (
          <InfoItem
            userInfo={item}
            onPress={() =>
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, item)
            }
          />
        )}
      />
    );
  };

  renderView = () => {
    const { isShowDatePicker, endDay, startDay } = this.state;
    return (
      <>
        {isShowDatePicker
          ? this.renderDatePicker(endDay, startDay)
          : this.renderList()}
      </>
    );
  };

  render() {
    const { isLoading, error } = this.props.orderUpcoming;
    return (
      <>
        {this.renderHeaderLable()}
        <ScreenComponent
          reload={this.getData}
          isLoading={isLoading}
          isError={error}
          renderView={<View style={styles.view}>{this.renderView()}</View>}
        />
      </>
    );
  }
  setDay = (startDay, endDay) => {
    this.setState({
      endDay: endDay,
      startDay: startDay,
      fromDate: convertDate(startDay),
      toDate: convertDate(endDay)
    });
  };

  renderFromDateToDateTextHeader = (valfromDate, valToDate) => {
    var fromDate = valfromDate;
    var toDate = valToDate;
    if (valfromDate == todayStr) fromDate = R.strings().today;
    if (valToDate == todayStr) toDate = R.strings().today;
    return (
      <Text
        style={{ textAlign: "right", flex: 1 }}
        children={
          fromDate != toDate ? `${fromDate} - ${toDate}` : `${fromDate}`
        }
      />
    );
  };
  renderHeaderLable = () => {
    const { toDate, fromDate } = this.state;
    const { service } = this.state;
    return (
      <View style={styles.containerHeader}>
        <Dropdown
          defaultValue={R.strings().service}
          data={service.map(e => e.serviceName)}
          style={styles.root_dropdown}
          dropDownStyle={styles.dropDownStyle}
          renderRow={text => (
            <Text style={styles.text_dropdown} children={text} />
          )}
          onSelect={(i, val) => {
            this.setState(
              {
                ...service[i],
                page: 1
              },
              this.getData
            );
          }}
        />

        <TouchableOpacity
          style={{ flex: 1, padding: 10 }}
          onPress={this.toggleDatePicker}
          children={this.renderFromDateToDateTextHeader(fromDate, toDate)}
        />
      </View>
    );
  };
}

const styles = StyleSheet.create({
  view: {
    flex: 1,
    backgroundColor: theme.colors.backgroundColor
  },

  containerHeader: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: theme.colors.defaultBg
  },
  text_dropdown: {
    textAlignVertical: "center",
    paddingHorizontal: 10,
    paddingVertical: 12,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  root_dropdown: {
    alignSelf: "center",
    flex: 1,
    padding: 10
  },
  dropDownStyle: {
    width: "100%",
    marginTop: Platform.OS != "ios" ? -25 : 0
    // borderRadius: 5
    // marginHorizontal: 5
  }
});

const mapStateToProps = state => ({
  orderUpcoming: state[REDUCER_CUSTOM.ORDER_UPCOMING],
  state: state.state[SCREEN_ROUTER_CUSTOMER.UPCOMING_ORDER],
  changePage: state.state[SCREEN_ROUTER_CUSTOMER.ORDER].changePage
});

const mapDispatchToProps = {
  getOrder,
  setState
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(UpcomingOrderScreen);

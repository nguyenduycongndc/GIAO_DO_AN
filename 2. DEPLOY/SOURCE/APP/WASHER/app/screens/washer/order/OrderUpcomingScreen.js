import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  RefreshControl,
  FlatList,
  ActivityIndicator,
  StyleSheet
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import InfoItem from "@app/components/InfoItem";
import R from "@app/assets/R";
import theme from "@theme";
import I18n from "@i18";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { RowImageLable } from "@app/components/FormRow";
import Icon from "@app/components/Icon";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER, ORDER_STATUS } from "@app/constants/Constants";
import { getListOrderUpcommingService } from "@app/redux/actions";
import reactotron from "reactotron-react-native";
import MarkerDate from "@app/components/MarkerDate";
import { numberWithCommas, convertDate } from "@app/constants/Functions";
import Empty from "@app/components/Empty";
import { requestGetListFilter } from "@api";
import Dropdown from "@app/components/ModalDropdown";

const initialState = {
  dataSearch: {
    status: ORDER_STATUS.CONFIRMED,
    placeID: "",
    fromDate: "",
    toDate: "",
    distance: "",
    page: 1,
    serviceID: "",
    comboID: ""
  },
  isShowDatePicker: false,
  startDay: "",
  endDay: "",
  isLoading: true,
  error: null,
  listService: [],
  serviceName: ""
};
export class OrderUpcomingScreen extends Component {
  constructor(props) {
    super(props);
    this.onEndReachedCalledDuringMomentum = true;
    this.state = initialState;
  }
  componentDidMount() {
    this.getData();
    this.getListFilter();
  }

  getListFilter = async () => {
    try {
      const res = await requestGetListFilter();
      this.setState({ isLoading: false, error: null, listService: res.result });
    } catch (error) {
      this.setState({ isLoading: false, error: error });
    }
  };

  getData = () => {
    this.props.getListOrderUpcommingService(this.state.dataSearch);
  };
  onPressSetDay = async (startDay, endDay) => {
    await this.setState({
      ...this.setState,
      dataSearch: {
        ...this.state.dataSearch,
        fromDate: startDay ? convertDate(startDay) : "",
        toDate: endDay ? convertDate(endDay) : ""
      },
      isShowDatePicker: false,
      startDay: startDay,
      endDay: endDay
    });
    this.getData();
  };
  render() {
    const { ListOrderUpcommingServiceState } = this.props;
    const { isShowDatePicker, dataSearch, startDay, endDay } = this.state;
    return (
      <ScreenComponent
        isLoading={ListOrderUpcommingServiceState.isLoading}
        isError={ListOrderUpcommingServiceState.error || this.state.error}
        reload={this.refreshData}
        renderView={
          <>
            {!isShowDatePicker ? (
              <View
                style={{
                  backgroundColor: theme.colors.backgroundColor,
                  flex: 1
                }}
              >
                {this.renderHeaderLable()}
                <FlatList
                  keyExtractor={(item, index) => index.toString()}
                  data={ListOrderUpcommingServiceState.data}
                  style={{ flex: 1 }}
                  refreshControl={
                    <RefreshControl
                      refreshing={ListOrderUpcommingServiceState.isLoading}
                      onRefresh={this.refreshData}
                    />
                  }
                  ListEmptyComponent={
                    <Empty
                      description={R.strings().empty_list}
                      marginTop={height / 5}
                    />
                  }
                  onMomentumScrollBegin={() =>
                    (this.onEndReachedCalledDuringMomentum = false)
                  }
                  onEndReachedThreshold={0.1}
                  onEndReached={() => {
                    if (
                      !this.onEndReachedCalledDuringMomentum &&
                      !ListOrderUpcommingServiceState.isLastPage &&
                      !ListOrderUpcommingServiceState.isLoadMore
                    ) {
                      this.setState(
                        {
                          dataSearch: {
                            ...this.state.dataSearch,
                            page: this.state.dataSearch.page + 1
                          }
                        },
                        () => {
                          this.getData();
                          this.onEndReachedCalledDuringMomentum = true;
                        }
                      );
                    }
                  }}
                  ListFooterComponent={
                    ListOrderUpcommingServiceState.isLoadMore ? (
                      <ActivityIndicator
                        style={{ marginTop: 10, marginBottom: 10 }}
                      />
                    ) : null
                  }
                  renderItem={({ item, index }) => (
                    <InfoItem
                      orderDetail={item}
                      onPress={() => {
                        // NavigationUtil.navigate(
                        //   SCREEN_ROUTER_WASHER.UPCOMING_DETAIL,
                        //   { orderDetail: item }
                        // )
                        NavigationUtil.navigate(
                          SCREEN_ROUTER_WASHER.ORDER_DETAIL,
                          {
                            orderID: item.orderServiceID
                          }
                        );
                      }}
                    />
                  )}
                />
              </View>
            ) : (
              <View style={{ flex: 1 }}>
                {this.renderHeaderLable(R.strings().pending_transaction)}
                <MarkerDate
                  endDay={endDay}
                  startDay={startDay}
                  onPressDelete={() => this.onPressSetDay()}
                  onPressSelect={this.onPressSetDay}
                />
              </View>
            )}
          </>
        }
      />
    );
  }

  refreshData = () => {
    this.setState(
      {
        dataSearch: {
          status: ORDER_STATUS.CONFIRMED,
          placeID: "",
          fromDate: "",
          toDate: "",
          distance: "",
          page: 1,
          serviceID: "",
          comboID: ""
        },
        serviceName: ""
      },
      () => {
        this.getListFilter();
        this.getData();
      }
    );
  };

  renderHeaderLable(Lable) {
    const { dataSearch, listService, serviceName } = this.state;
    return (
      <View
        style={{
          flexDirection: "row",
          alignItems: "center",
          paddingHorizontal: 15,
          paddingVertical: 10,
          backgroundColor: theme.colors.defaultBg,
          justifyContent: "space-between"
        }}
      >
        <Dropdown
          data={listService}
          style={styles.dropdown_select}
          defaultValue={serviceName || R.strings().service}
          renderButtonText={item => <Text>{item.serviceName}</Text>}
          renderRow={(item, index, isSelected) => (
            <View style={{ paddingHorizontal: 6, paddingVertical: 6 }}>
              <Text>{item.serviceName}</Text>
            </View>
          )}
          textStyle={styles.text_dropdown}
          dropDownStyle={styles.dropdownStyle}
          imageStyle={styles.image_dropdown}
          onSelect={(i, val) => {
            this.setState(
              {
                dataSearch: {
                  ...this.state.dataSearch,
                  serviceID: val.serviceID
                },
                serviceName: val.serviceName
              },
              this.getData
            );
            // this.setDataSearch("")
          }}
        />
        {/* <RowImageLable
          disableTouch={false}
          lable={"Trạng thái"}
          size={12}
          position="right"
          image={<Icon.MaterialIcons name="keyboard-arrow-down" size={12} />}
          textColor={theme.colors.nameText}
        /> */}
        <RowImageLable
          disableTouch={false}
          onPress={() =>
            this.setState({
              ...this.setState,
              isShowDatePicker: !this.state.isShowDatePicker
            })
          }
          lable={
            dataSearch.fromDate == ""
              ? R.strings().filter_by_date
              : `${dataSearch.fromDate}-${dataSearch.toDate}`
          }
          size={12}
          position="right"
          image={<Icon.MaterialIcons name="keyboard-arrow-down" size={12} />}
          textColor={theme.colors.nameText}
        />
      </View>
    );
  }
}
const styles = StyleSheet.create({
  dropdown_select: {
    backgroundColor: theme.colors.backgroundColor,
    paddingStart: 0,
    paddingVertical: 0,
    margin: 0,
    alignSelf: "center"
    // minwidth: 80,
  },
  text_dropdown: { fontSize: 12, color: theme.colors.black },
  image_dropdown: { top: 18, left: 80 },
  dropdownStyle: {
    marginTop: -20,
    minWidth: 80
  }
});
const mapStateToProps = state => ({
  ListOrderUpcommingServiceState: state.listOrderUpcommingServiceReducer
});

const mapDispatchToProps = {
  getListOrderUpcommingService
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(OrderUpcomingScreen);

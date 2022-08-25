import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  TextInput,
  Image,
  StyleSheet,
  RefreshControl,
  FlatList,
  ActivityIndicator,
  Platform
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import InfoItem from "@app/components/InfoItem";
import R from "@app/assets/R";
import theme from "@theme";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER, ORDER_STATUS } from "@app/constants/Constants";
import { RowImageLable } from "@app/components/FormRow";
import { getListOrderHistoryService, navigateTab } from "@app/redux/actions";
import reactotron from "reactotron-react-native";
import Icon from "@app/components/Icon";
import MarkerDate from "@app/components/MarkerDate";
import { numberWithCommas, convertDate } from "@app/constants/Functions";
import Dropdown from "@app/components/ModalDropdown";
import Empty from "@app/components/Empty";
import Loading from "@app/components/Loading";
import { requestGetListFilter } from "@api";

const initialState = {
  dataSearch: {
    status: "",
    placeID: "",
    fromDate: "",
    toDate: "",
    distance: "",
    page: 1,
    serviceID: "",
    comboID: "",
    search: ""
  },
  isShowDatePicker: false,
  startDay: "",
  endDay: "",
  isLoading: true,
  error: null,
  listService: [],
  serviceName: "",
  timeout: 0
};

export class OrderHistoryScreen extends Component {
  constructor(props) {
    super(props);
    this.onEndReachedCalledDuringMomentum = true;
    this.state = initialState;
  }
  componentDidMount() {
    this.getData();
    this.getListFilter();
  }
  getData = () => {
    this.props.getListOrderHistoryService(this.state.dataSearch);
  };
  getListFilter = async () => {
    try {
      const res = await requestGetListFilter();
      res.result.unshift({
        serviceID: "",
        serviceName: R.strings().all,
        comboID: ""
      });
      this.setState({
        isLoading: false,
        error: null,
        listService: res.result
      });
    } catch (error) {
      this.setState({ isLoading: false, error: error });
    }
  };
  onPressSetDay = async (startDay, endDay) => {
    await this.setState({
      ...this.setState,
      dataSearch: {
        ...this.state.dataSearch,
        fromDate: startDay ? convertDate(startDay) : "",
        toDate: endDay ? convertDate(endDay) : "",
        page: 1
      },
      isShowDatePicker: false,
      startDay: startDay,
      endDay: endDay
    });
    this.getData();
  };
  setDataSearch = async (key, value) => {
    this.setState(
      {
        ...this.setState,
        dataSearch: {
          ...this.state.dataSearch,
          [key]: value,
          page: 1
        }
      },
      this.getData
    );
  };
  render() {
    const { ListOrderHistoryServiceState } = this.props;
    const { isShowDatePicker, dataSearch, startDay, endDay } = this.state;
    return (
      <ScreenComponent
        isError={ListOrderHistoryServiceState.error}
        reload={this.refreshData}
        renderView={
          <>
            {!isShowDatePicker ? (
              <View
                style={styles.container}
                // style={{ backgroundColor: theme.colors.backgroundColor }}
              >
                {this.renderHeaderLable()}
                {ListOrderHistoryServiceState.isLoading ? (
                  <Loading />
                ) : (
                  <View style={{ flex: 1 }}>
                    <View style={{ zIndex: -1, flex: 1 }}>
                      <FlatList
                        refreshControl={
                          <RefreshControl
                            refreshing={ListOrderHistoryServiceState.isLoading}
                            onRefresh={this.refreshData}
                          />
                        }
                        style={{ flex: 1 }}
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
                            !ListOrderHistoryServiceState.isLastPage &&
                            !ListOrderHistoryServiceState.isLoadMore
                          ) {
                            this.setState(
                              {
                                dataSearch: {
                                  ...this.state.dataSearch,
                                  page: this.state.dataSearch.page + 1
                                }
                              },
                              this.getData
                            );
                            this.onEndReachedCalledDuringMomentum = true;
                          }
                        }}
                        ListFooterComponent={
                          ListOrderHistoryServiceState.isLoadMore ? (
                            <ActivityIndicator
                              style={{ marginTop: 10, marginBottom: 10 }}
                            />
                          ) : null
                        }
                        // contentContainerStyle={{ flex: 1 }}
                        keyExtractor={(item, index) => index.toString()}
                        data={ListOrderHistoryServiceState.data}
                        renderItem={({ item, index }) => (
                          <InfoItem
                            orderDetail={item}
                            onPress={() => {
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
                  </View>
                )}
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
          status: "",
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

  onSearch = text => {
    const { timeout } = this.state;
    if (timeout) clearTimeout(timeout);
    this.setState({
      dataSearch: {
        ...this.state.dataSearch,
        search: text,
        page: 1
      },
      timeout: setTimeout(() => this.getData(), 500)
    });
  };

  renderHeaderLable() {
    const { dataSearch, listService, serviceName } = this.state;

    return (
      <View
        style={{
          flexDirection: "column",
          backgroundColor: theme.colors.defaultBg
        }}
      >
        <View style={styles.searchBox}>
          <Image source={R.images.ic_search} style={styles.imgStyle} />
          <TextInput
            style={styles.textInput}
            placeholder={R.strings().code_name_customer}
            onChangeText={this.onSearch}
            value={dataSearch.search}
          />
        </View>
        <View style={styles.boxHeader}>
          <View style={styles.box_dropdown}>
            <View>
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
                        serviceID: val.serviceID,
                        comboID: val.comboID,
                        page: 1
                      },
                      serviceName: val.serviceName
                    },
                    this.getData
                  );
                  // this.setDataSearch("")
                }}
              />
            </View>
            <View>
              <Dropdown
                data={[
                  R.strings().all,
                  R.strings().complete,
                  R.strings().canceled
                ]}
                style={styles.dropdown_select}
                defaultValue={
                  dataSearch.status == ""
                    ? R.strings().status
                    : dataSearch.status == ORDER_STATUS.COMPLETED
                    ? R.strings().complete
                    : R.strings().canceled
                }
                textStyle={styles.text_dropdown}
                imageStyle={[styles.image_dropdown, { right: 0 }]}
                dropDownStyle={styles.dropdownStyle}
                onSelect={(i, val) => {
                  console.log(val);
                  this.setDataSearch(
                    "status",
                    val == R.strings().complete
                      ? ORDER_STATUS.COMPLETED
                      : ORDER_STATUS.REJECTED
                  );
                }}
              />
            </View>
          </View>
          <RowImageLable
            disableTouch={false}
            onPress={() =>
              this.setState({
                ...this.setState,
                isShowDatePicker: !this.state.isShowDatePicker
              })
            }
            style={{ marginTop: 3 }}
            lable={
              dataSearch.fromDate == ""
                ? R.strings().filter_by_date
                : `${dataSearch.fromDate}-${dataSearch.toDate}`
            }
            size={12}
            position="right"
            image={
              <Icon.MaterialIcons
                style={{ marginTop: 2 }}
                name="keyboard-arrow-down"
                size={12}
              />
            }
            textColor={theme.colors.nameText}
          />
        </View>
      </View>
    );
  }
}

const mapStateToProps = state => ({
  ListOrderHistoryServiceState: state.listOrderHistoryServiceReducer
});

const mapDispatchToProps = {
  getListOrderHistoryService,
  navigateTab
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(OrderHistoryScreen);

const styles = StyleSheet.create({
  box_dropdown: {
    flexDirection: "row",
    flex: 1
  },
  text_dropdown: { fontSize: 12, color: theme.colors.black },
  image_dropdown: { top: 8, right: 0 },
  dropdownStyle: {
    marginTop: -20,
    minWidth: 80
  },
  searchBox: {
    flexDirection: "row",
    backgroundColor: theme.colors.white,
    marginHorizontal: 10,
    marginTop: 10,
    borderRadius: 5,
    borderWidth: 0.25,
    paddingHorizontal: 18,
    paddingVertical: Platform.OS == "ios" ? 6 : 0,
    alignItems: "center"
  },
  container: {
    backgroundColor: theme.colors.backgroundColor,
    flex: 1
  },
  imgStyle: {
    width: 30,
    height: 30,
    marginRight: 5
    // tintColor: "black"
  },
  textInput: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    flex: 1,
    color: "black"
  },
  boxHeader: {
    flexDirection: "row",
    alignItems: "center",
    paddingHorizontal: 15,
    paddingVertical: 10,
    backgroundColor: theme.colors.defaultBg
  },
  dropdown_select: {
    backgroundColor: theme.colors.backgroundColor,
    paddingStart: 0,
    paddingVertical: 0,
    margin: 0,
    alignSelf: "center",
    // minwidth: 80
    minWidth: 80
  }
});

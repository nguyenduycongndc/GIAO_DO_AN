import React, { Component } from "react";
import {
  Image,
  StyleSheet,
  TouchableOpacity,
  View,
  Text,
  ScrollView,
  RefreshControl,
  FlatList,
  ActivityIndicator
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme from "@theme";
import I18n from "@i18";
import { connect } from "react-redux";
import Icon from "@app/components/Icon";
import MarkerDate from "@app/components/MarkerDate";
import { RowImageLable } from "@app/components/FormRow";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER, ORDER_STATUS } from "@app/constants/Constants";
import { getListOrderPendingService } from "@app/redux/actions";
import reactotron from "reactotron-react-native";
import Dropdown from "@app/components/ModalDropdown";
import { numberWithCommas, convertDate } from "@app/constants/Functions";
import Empty from "@app/components/Empty";
import InfoItem from "@app/components/InfoItem";
const initialState = {
  dataSearch: {
    status: ORDER_STATUS.PENDING,
    placeID: "",
    fromDate: "",
    toDate: "",
    distance: R.strings().all,
    page: 1,
    serviceID: "",
    comboID: ""
  },
  isShowDatePicker: false,
  startDay: "",
  endDay: ""
};
export class OrderPendingScreen extends Component {
  constructor(props) {
    super(props);
    this.onEndReachedCalledDuringMomentum = true;
    this.state = initialState;
  }
  componentDidMount() {
    this.getData();
  }
  getData = () => {
    let body = this.state.dataSearch;
    if (Object.keys(this.props.locationState).length != 0) {
      let location = this.props.locationState;
      body = { ...body, longi: location.longi, lati: location.lati };
    }
    this.props.getListOrderPendingService(body);
  };

  render() {
    const { isShowDatePicker, dataSearch, startDay, endDay } = this.state;
    const { ListOrderPendingServiceState } = this.props;
    return (
      <ScreenComponent
        isLoading={ListOrderPendingServiceState.isLoading}
        isError={ListOrderPendingServiceState.error}
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
                {this.renderHeaderLable(R.strings().pending_transaction)}
                {/* <View style={{ flex: 1 }}> */}
                <FlatList
                  refreshControl={
                    <RefreshControl
                      refreshing={ListOrderPendingServiceState.isLoading}
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
                      !ListOrderPendingServiceState.isLastPage &&
                      !ListOrderPendingServiceState.isLoadMore
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
                    ListOrderPendingServiceState.isLoadMore ? (
                      <ActivityIndicator
                        style={{ marginTop: 10, marginBottom: 10 }}
                      />
                    ) : null
                  }
                  keyExtractor={(item, index) => index.toString()}
                  data={ListOrderPendingServiceState.data}
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
                {/* {this.renderItem()}
                {this.renderItem()}
                {this.renderItem()} */}
                {/* </View> */}
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
          status: ORDER_STATUS.PENDING,
          placeID: "",
          fromDate: "",
          toDate: "",
          distance: R.strings().all,
          page: 1,
          serviceID: "",
          comboID: ""
        }
      },
      this.getData
    );
  };

  onPressSetDay = (startDay, endDay) => {
    this.setState(
      {
        ...this.state,
        dataSearch: {
          ...this.state.dataSearch,
          fromDate: startDay ? convertDate(startDay) : "",
          toDate: endDay ? convertDate(endDay) : "",
          page: 1
        },
        isShowDatePicker: false,
        startDay: startDay,
        endDay: endDay,
        page: 1
      },
      this.getData
    );
  };
  setDataSearch = async (key, value) => {
    await this.setState(
      {
        ...this.state,
        page: 1,
        dataSearch: {
          ...this.state.dataSearch,
          [key]: value
        }
      },
      this.getData
    );
  };
  renderHeaderLable(Lable) {
    const { dataSearch } = this.state;
    return (
      <View style={styles.containerHeader}>
        <Text style={styles.lableHeader}>{Lable}</Text>
        <RowImageLable
          disableTouch={false}
          onPress={() =>
            this.setState({
              ...this.state,
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
          image={
            <Icon.MaterialIcons
              style={{ marginTop: 2 }}
              name="keyboard-arrow-down"
              size={12}
            />
          }
          textColor={theme.colors.nameText}
        />
        {/* <RowImageLable
          disableTouch={false}
          lable={"25km"}
          size={12}
          position="right"
          image={<Icon.MaterialIcons name="keyboard-arrow-down" size={12} />}
          textColor={theme.colors.nameText}
        /> */}
        <View>
          <Dropdown
            data={["1 km", "5 km", "25 km", R.strings().all]}
            style={styles.dropdown_select}
            defaultValue={
              dataSearch.distance != R.strings().all
                ? `${dataSearch.distance} km`
                : dataSearch.distance
            }
            textStyle={styles.text_dropdown}
            dropDownStyle={styles.dropdownStyle}
            imageStyle={styles.image_dropdown}
            onSelect={(i, val) =>
              this.setDataSearch("distance", val.replace(" km", ""))
            }
          />
        </View>
      </View>
    );
  }

  renderItem(item) {
    return (
      <TouchableOpacity
        style={styles.touchable}
        onPress={() => {
          // NavigationUtil.navigate(SCREEN_ROUTER_WASHER.PENDING_DETAIL, {
          //   orderServiceID: item.orderServiceID
          // });
          // NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER_DETAIL, {
          //   orderID: item.orderServiceID
          // });

          // test
          NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER_INCOMMING, {
            data: {
              type: 1,
              timeSend: 1588487060,
              timeWait: 18000,
              orderServiceID: 332,
              productID: null,
              newsID: null,
              agentCode: "CM202003-001",
              diviceID: ""
            }
          });
        }}
      >
        <View style={styles.itemContainer}>
          <View>
            <RowImageLable
              lable={"Outsite"}
              image={
                <Image style={styles.imgStyle} source={R.images.ic_outsite} />
              }
            />
            <RowImageLable lable={"Combo"} style={{ marginTop: 5 }} />
          </View>
          <View
            style={{
              width: 0.5,
              height: 38,
              backgroundColor: theme.colors.gray
            }}
          />
          <View>
            <RowImageLable
              lable={item.customerName}
              image={
                <Image
                  style={styles.locationImage}
                  source={R.images.ic_location}
                />
              }
            />
            <RowImageLable
              lable={numberWithCommas(item.totalPrice.toString()) + "đ"}
              image={
                <Image style={styles.tagPrice} source={R.images.ic_tag_price} />
              }
            />
            {/* <RowImageLable
                        lable={"Nguyễn Khánh Toàn"}
                    // position = "right"
                    /> */}
          </View>
        </View>
        <RowImageLable
          lable={item.bookDateStr}
          image={<Image style={styles.time} source={R.images.ic_clock} />}
        />
      </TouchableOpacity>
    );
  }
}
const styles = StyleSheet.create({
  container: {
    flexDirection: "row",
    alignItems: "center",
    paddingHorizontal: 15,
    paddingVertical: 10,
    backgroundColor: theme.colors.inactive
  },
  lableHeader: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    flex: 1
  },
  touchable: {
    padding: 10,
    paddingHorizontal: 20,
    backgroundColor: "white",
    borderBottomWidth: 0.25,
    borderTopWidth: 0.25,
    marginVertical: 2,
    borderColor: theme.colors.gray
  },
  itemContainer: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 5
  },
  imgStyle: {
    // borderRadius: 50,
    width: 16,
    height: 16
  },
  locationImage: {
    width: 18,
    height: 18,
    marginLeft: 5,
    marginRight: 2
  },
  tagPrice: {
    width: 20,
    height: 20,
    marginLeft: 5
  },
  time: {
    borderRadius: 50,
    width: 18,
    height: 18
  },
  containerHeader: {
    flexDirection: "row",
    padding: 5,
    alignItems: "center"
  },
  wallet_select: {
    backgroundColor: theme.colors.backgroundColor,
    paddingStart: 10,
    paddingVertical: 5,
    alignSelf: "center"
  },
  box_dropdown: {
    flexDirection: "row",
    flex: 1
  },
  text_dropdown: { fontSize: 12, color: "black" },
  image_dropdown: { top: 8, right: 2 },
  dropdownStyle: {
    marginTop: -20,
    minWidth: 80
  },
  dropdown_select: {
    backgroundColor: theme.colors.backgroundColor,
    paddingStart: 0,
    paddingVertical: 0,
    margin: 0,
    marginLeft: 10,
    marginBottom: 6,
    alignSelf: "center",
    minWidth: 55
  }
});
const mapStateToProps = state => ({
  ListOrderPendingServiceState: state.listOrderPendingServiceReducer,
  locationState: state.locationReducer
});

const mapDispatchToProps = {
  getListOrderPendingService
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(OrderPendingScreen);

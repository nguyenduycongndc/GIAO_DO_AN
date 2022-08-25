import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  FlatList,
  RefreshControl,
  Animated
} from "react-native";
import { setState } from "@action/index";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_CUSTOMER, REDUCER_CUSTOM } from "@constant";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme from "@theme";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import { TYPE_SERVICE } from "@constant";
import NumberFormat from "react-number-format";
import FooterOrderPackage from "@app/components/FooterOrderPackage";
import FastImage from "@app/components/FastImage";
import analytics from "@react-native-firebase/analytics";
class SelectPackageScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const orderService = navigation.getParam("orderService", {});
    const carInfo = navigation.getParam("carInfo", {});
    this.state = {
      isLoading: false,
      error: null,
      isImageViewVisible: false,
      images: [],
      servicePrice: "",
      listService: [],
      carInfo,
      orderService,
      payload: {
        carID: orderService?.carID || props?.orderHistory?.orderService?.carID,
        type: TYPE_SERVICE.MAIN
      }
    };
    props.setState(SCREEN_ROUTER_CUSTOMER.SELECT_PACKAGE, {
      changeTime: this.changeTime
    });
  }

  componentDidMount() {
    this.getListService();
  }

  changeTime = () => {
    this.setState({
      orderService: {
        ...this.state.orderService,
        BookingDateInput: this.props.BookingDateInput
      }
    });
  };

  getListService = () => {
    CallApiHelper(API.getListService, this.state.payload, this, res => {
      const listService = res.result.listInput.concat(
        this.props.typeBookingNow.type == 0 ? res.result.listInputCombo : []
      );
      this.setState(
        {
          listService
        },
        () => {
          const { itemService } = this.props.tabBookingScreenState;
          if (itemService) {
            this.onSelectPack(
              listService[
                listService.map(e => e.serviceID).indexOf(itemService.serviceID)
              ]
            );
            this.props.setState(SCREEN_ROUTER_CUSTOMER.BOOKING, {
              itemService: null
            });
          }
          if (this.props.orderHistory.reOrder) {
            const { orderService } = this.props.orderHistory;
            this.setState(
              {
                orderService
              },
              () => {
                this.onSelectPack(
                  listService[
                    listService
                      .map(e => e.serviceID)
                      .indexOf(this.props.orderHistory.orderService.mainService)
                  ]
                );
              }
            );
          }
        }
      );
    });
  };

  render() {
    const { isLoading, error, listService, orderService } = this.state;
    return (
      <ScreenComponent
        back
        isLoading={isLoading}
        isError={error}
        titleHeader={R.strings().package}
        renderView={
          <>
            <FlatList
              showsVerticalScrollIndicator={false}
              contentContainerStyle={styles.view}
              data={listService}
              refreshControl={
                <RefreshControl
                  refreshing={isLoading}
                  onRefresh={() => this.getListService()}
                />
              }
              keyExtractor={(item, index) => index.toString()}
              renderItem={this._renderPackage}
            />
          </>
        }
      />
    );
  }

  onSelectPack = item => {
    const param = () => ({
      orderService: this.state.orderService,
      listAddService: item,
      carInfo: this.state.carInfo
    });
    if (item.comboID && item.comboID != 0) {
      this.setState(
        {
          orderService: {
            ...this.state.orderService,
            comboID: item.comboID,
            mainService: null
          }
        },
        () =>
          NavigationUtil.navigate(
            SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE,
            param()
          )
      );
    } else {
      this.setState(
        {
          orderService: {
            ...this.state.orderService,
            mainService: item.serviceID,
            comboID: 0
          }
        },
        () =>
          NavigationUtil.navigate(
            SCREEN_ROUTER_CUSTOMER.BOOKING_SERVICE,
            param()
          )
      );
    }
  };

  // _renderPackage() {
  _renderPackage = ({ item, index }) => {
    const { orderService } = this.state;
    const { data } = this.props.UserInfoState;
    return (
      <TouchableOpacity
        onPress={() => this.onSelectPack(item)}
        style={styles.viewPackage}
      >
        {!!item.mainImage && (
          <FastImage
            style={styles.img_package}
            source={{ uri: item.mainImage }}
            resizeMode="cover"
          >
            <View style={styles.view_in_img}>
              <View
                style={styles.root_cate_name}
                children={
                  <>
                    {!!item.icon && (
                      <FastImage
                        source={{ uri: item.icon }}
                        style={{ width: 60, height: 30, alignSelf: "center" }}
                        resizeMode="contain"
                      />
                    )}
                    <Text
                      style={styles.text_cate_name}
                      children={item.cateName || ""}
                    />
                    {item?.estTime != 0 && (
                      <Text
                        style={styles.text_time_est}
                        children={item.estTime + " " + R.strings().minutes}
                      />
                    )}
                  </>
                }
              />
              <View>
                <NumberFormat
                  value={item.basePrice}
                  displayType="text"
                  thousandSeparator
                  suffix="đ"
                  prefix={item.value > 0 && "+"}
                  renderText={value => (
                    <Text style={styles.txtPrice} children={value} />
                  )}
                />
                <View style={styles.footer}>
                  <View
                    style={{
                      flexDirection: "row",
                      alignItems: "center"
                    }}
                  >
                    <Text style={styles.text_discount_money}>
                      -{item.discount}%
                    </Text>
                    <NumberFormat
                      value={item.price}
                      displayType="text"
                      thousandSeparator
                      suffix="đ"
                      prefix={item.value > 0 && "+"}
                      renderText={value => (
                        <Text
                          style={styles.text_discount_money}
                          children={value}
                        />
                      )}
                    />
                  </View>
                  <TouchableOpacity
                    onPress={async () => {
                      analytics().logEvent("see_detail_image_package", {
                        id: data.memberID,
                        item: "See detail image package",
                        phone: data.phone,
                        name: data.name
                      });
                      NavigationUtil.push(SCREEN_ROUTER_CUSTOMER.IMAGE_VIEWER, {
                        data: item.imageUrl.map(e => ({ url: e })),
                        renderFooter: (
                          <FooterOrderPackage
                            action={() => {
                              NavigationUtil.goBack(true);
                              this.onSelectPack(item);
                            }}
                          />
                        )
                      });
                    }}
                    style={{}}
                  >
                    <TextDetailAnimated />
                  </TouchableOpacity>
                </View>
              </View>
            </View>
          </FastImage>
        )}
      </TouchableOpacity>
    );
  };
}
class TextDetailAnimated extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      changeState: 60
    };
  }

  componentWillMount() {
    this.animatedValue = new Animated.Value(0);
  }
  componentDidMount() {
    Animated.loop(
      Animated.sequence([
        Animated.timing(this.animatedValue, {
          toValue: 60,
          duration: 800
        })
      ])
    ).start();
  }
  render() {
    const interpolateColor = this.animatedValue.interpolate({
      inputRange: [0, 30, 60],
      outputRange: [
        "rgba(0, 137, 206, 0.8)",
        "rgba(248,180,45,0.8)",
        "rgba(0, 137, 206, 0.8)"
      ]
    });

    const fontSize = this.animatedValue.interpolate({
      inputRange: [0, 10, 20, 30, 40, 50, 60],
      outputRange: [14, 14.3, 14.6, 15, 14.6, 14.3, 14]
    });
    const backgroundAnimate = {
      backgroundColor: interpolateColor,
      fontSize
    };

    return (
      <Animated.Text
        style={{ ...styles.text_detail, ...backgroundAnimate }}
        children={`${R.strings().detail}`}
      />
    );
  }
}
const styles = StyleSheet.create({
  view: {
    flexGrow: 1,
    backgroundColor: theme.colors.backgroundColor,
    paddingHorizontal: 14,
    paddingVertical: 10
  },
  img_package: {
    aspectRatio: 352 / 194,
    borderRadius: 10,
    overflow: "hidden"
  },
  view_in_img: {
    overflow: "hidden",
    width: "100%",
    height: "100%",
    paddingHorizontal: 10,
    paddingVertical: 17,
    justifyContent: "flex-end"
  },
  viewPackage: {
    marginBottom: 10,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 2
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,

    elevation: 5
  },
  txtPrice: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 10,
    textDecorationLine: "line-through",
    color: theme.colors.white,
    paddingLeft: 9,
    textShadowColor: "black",
    textShadowOffset: { height: 0, width: 0 },
    textShadowRadius: 3
  },
  footer: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    paddingHorizontal: 9,
    marginTop: 5
  },
  text_cate_name: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 20,
    color: "white",
    backgroundColor: "rgba(0,0,0,0.5)",
    borderRadius: 5,
    padding: 5,
    overflow: "hidden"
  },
  text_time_est: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16,
    color: "white",
    backgroundColor: "rgba(0,0,0,0.5)",
    borderRadius: 5,
    padding: 5,
    paddingVertical: 10,
    marginHorizontal: 10,
    alignSelf: "center",
    overflow: "hidden"
  },
  root_cate_name: {
    position: "absolute",
    top: 15,
    left: 0,
    flexDirection: "row"
  },
  text_detail: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.white,
    textShadowColor: "black",
    textShadowOffset: { height: 0, width: 0 },
    textShadowRadius: 3,
    textAlignVertical: "center",
    overflow: "hidden",
    padding: 5,
    borderRadius: 5
  },
  text_discount_money: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.white,
    borderRadius: 5,
    textAlignVertical: "center",
    padding: 2,
    textShadowColor: "black",
    textShadowOffset: { height: 0, width: 0 },
    textShadowRadius: 3
  }
});

const mapStateToProps = state => ({
  tabBookingScreenState: state.state.BookingCustomer,
  orderHistory:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER],
  typeBookingNow: state.bookingNowReducer,
  BookingDateInput:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.BOOKING].orderService
      .BookingDateInput,
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  setState
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(SelectPackageScreen);

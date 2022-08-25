import R from "@app/assets/R";
import Button from "@app/components/Button";
import ScreenComponent from "@app/components/ScreenComponent";
import AsyncStorage from "@react-native-community/async-storage";
import theme, { colors } from "@theme";
import React, { Component, createRef } from "react";
import {
  StyleSheet,
  Text,
  View,
  ScrollView,
  TouchableOpacity,
  Dimensions,
  FlatList,
  Platform,
  RefreshControl,
  Clipboard
} from "react-native";
import { connect } from "react-redux";
import * as API from "@api";
import {
  getUserInfoAction,
  sendBookingNow,
  actionModal,
  setState
} from "@action";
import { SHOW_REQUIRE_LOGIN } from "@action/type";
import Carousel from "react-native-snap-carousel";
import NavigationUtil from "@app/navigation/NavigationUtil";
import CallApiHelper from "../../../utils/CallApiHelper";
import {
  SCREEN_ROUTER_CUSTOMER,
  SCREEN_ROUTER_AUTH,
  ASYNC_STORAGE,
  TYPE_NAVIGATION
} from "@constant";
import reactotron from "reactotron-react-native";
import { numberWithCommas } from "@app/constants/Functions";
import { Avatar, Header } from "react-native-elements";
import { createImageProgress } from "react-native-image-progress";
import FastImage from "@app/components/FastImage";
import * as Progress from "react-native-progress";
import FooterOrderPackage from "@app/components/FooterOrderPackage";
import callAPI from "@app/utils/CallApiHelper";
import { updateDeviceID } from "@api";
import OneSignal from "react-native-onesignal";
import FastImg from "@app/components/FastImage";
import { showMessagesAlert } from "@app/components/Alert";

const Image = createImageProgress(FastImage);

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

let CurrentSlide = 0;
let IntervalTime = 4000;
class TabHomeScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isLogin: false,
      isLoading: false,
      error: "",
      token: "",
      dataHomeScreen: {},
      listCombo: [],
      listItem: [],
      payload: {
        token: "",
        lang: "vn"
      },
      images: [
        require("../../../assets/images/slideImg1.png"),
        require("../../../assets/images/slideImg2.png"),
        require("../../../assets/images/slideImg3.png")
      ]
    };
  }

  componentDidMount() {
    this.getData();
    const { navigation } = this.props;
    navigation.addListener("willFocus", async () => {
      this._startAutoPlay();
    });
    navigation.addListener("willBlur", async () => {
      this._stopAutoPlay();
    });
  }

  getData() {
    this.getHomeScreen();
    this.getToken();
  }

  componentWillUnmount() {
    this._stopAutoPlay();
  }

  getToken = async () => {
    try {
      const token = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
      if (token != null) {
        OneSignal.getPermissionSubscriptionState(state => {
          // callAPI(updateDeviceID, state.userId, null);
        });
        this.setState(
          {
            isLogin: true
          },
          () => this.props.getUserInfoAction()
        );
      }
    } catch (error) {}
  };
  refreshControl = () => (
    <RefreshControl
      refreshing={this.state.isLoading}
      onRefresh={this.getHomeScreen}
    />
  );

  getHomeScreen = () => {
    CallApiHelper(
      API.requestHomeData,
      this.state.payload,
      this,
      res => {
        this.setState(
          {
            dataHomeScreen: res.result,
            listCombo: res.result.listCombo,
            listItem: res.result.listItem
          },
          () => {
            this._stopAutoPlay();
            this._startAutoPlay();
          }
        );
      },
      error => reactotron.log(error, "check error")
    );
  };

  flatList = createRef();
  // TODO _goToNextPage()
  _goToNextPage = () => {
    try {
      if (CurrentSlide >= this.state.dataHomeScreen.listPromotion.length - 1)
        CurrentSlide = -1;
      if (this.flatList.current != null)
        this.flatList.current.scrollToIndex({
          index: ++CurrentSlide,
          animated: true
        });
    } catch (error) {}
  };
  _startAutoPlay = () => {
    this._timerId = setInterval(this._goToNextPage, IntervalTime);
  };
  _stopAutoPlay = () => {
    if (this._timerId) {
      clearInterval(this._timerId);
      this._timerId = null;
    }
  };

  render() {
    const { isLoading, error, dataHomeScreen } = this.state;
    const { UserInfoState } = this.props;
    return (
      <ScreenComponent
        isLoading={isLoading}
        isError={error}
        header={this._renderHeader()}
        reload={this.getData}
        isSafeAre={false}
        renderView={
          <>
            <FlatList
              style={styles.view}
              showsVerticalScrollIndicator={false}
              refreshControl={this.refreshControl()}
              ListHeaderComponent={
                <View style={{ backgroundColor: colors.backgroundColor }}>
                  <View style={styles.slider_box}>{this._renderSlider()}</View>
                  {this._renderMenu()}
                  {this._renderCarousel()}
                  {this._renderNews()}
                </View>
              }
              contentContainerStyle={{
                paddingBottom: 22
              }}
              numColumns={2}
              data={dataHomeScreen.listNews}
              keyExtractor={(item, index) => index.toString()}
              renderItem={this._renderNewsItem}
            />
          </>
        }
      />
    );
  }
  _renderHeader = () => {
    const { isLogin } = this.state;
    const { UserInfoState } = this.props;
    return (
      <View style={styles.header}>
        {this.state.isLogin == false ? (
          <>
            <Text style={styles.text_welcome_name}>
              {R.strings().sign_in_for_the_best_experience}
            </Text>
            <Button
              buttonStyle={{ paddingVertical: 7 }}
              colorText={theme.colors.white}
              title={R.strings().sign_in}
              uppercase
              width={"40%"}
              backgroundColor={theme.colors.signInButton}
              action={() => NavigationUtil.navigate(SCREEN_ROUTER_AUTH.LOGIN)}
            />
          </>
        ) : (
          <View style={styles.root_header_login}>
            <FastImg
              style={styles.avatar_header}
              source={
                UserInfoState.data.urlAvatar
                  ? { uri: UserInfoState.data.urlAvatar }
                  : R.images.avatarDemo
              }
            />
            <View style={styles.root_text_header}>
              <Text style={styles.text_welcome_name}>
                {R.strings().welcome} {UserInfoState.data.name}
              </Text>
              <Text style={styles.text_point}>
                {UserInfoState.data.point
                  ? R.strings().accumulated_point +
                    ": " +
                    `${numberWithCommas(UserInfoState.data.point.toString())}`
                  : R.strings().accumulated_point + ": 0"}
              </Text>

              <Text
                onPress={() => {
                  Clipboard.setString(UserInfoState.data.phone);
                  showMessagesAlert(
                    R.strings().notif_tab_cus,
                    R.strings().referral_code_copied
                  );
                }}
                style={styles.text_referral_code}
                children={
                  R.strings().referral_code + ": " + UserInfoState.data.phone
                }
              />
            </View>
          </View>
        )}
      </View>
    );
  };
  _renderMenu() {
    return (
      <View style={styles.menu}>
        {this._renderMenuItem("Voucher", R.images.Voucher, () =>
          NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.PROMOTION, {
            type: TYPE_NAVIGATION.HOME
          })
        )}
        {this._renderMenuItem(R.strings().schedule, R.images.ic_symbol, () => {
          this.props.sendBookingNow(0);
          NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.BOOKING);
        })}
        {this._renderMenuItem(
          R.strings().booking_now,
          R.images.ic_book_now,
          () => {
            this.props.sendBookingNow(TYPE_NAVIGATION.BOOKING_NOW);
            NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.BOOKING);
          }
        )}
      </View>
    );
  }
  _renderMenuItem(title, source, action) {
    return (
      <TouchableOpacity
        onPress={async () => {
          const token = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
          if (!token) this.props.actionModal(SHOW_REQUIRE_LOGIN);
          else action();
        }}
        style={{ alignItems: "center", flex: 1 }}
      >
        <View style={[styles.menuItem]}>
          <FastImage
            style={{
              height: 45,
              width: 45
              // margin: 5
            }}
            resizeMode="contain"
            source={source}
          />
        </View>
        <Text
          style={{
            fontFamily: R.fonts.quicksand_medium,
            fontSize: 12,
            color: theme.colors.gray,
            marginTop: 10
          }}
        >
          {title}
        </Text>
      </TouchableOpacity>
    );
  }
  _renderSlider = () => {
    return (
      <View style={{ flex: 1 }}>
        <FlatList
          showsHorizontalScrollIndicator={false}
          style={{
            flex: 1
          }}
          data={this.state.dataHomeScreen.listPromotion}
          keyExtractor={this._keyExtractor.bind(this)}
          renderItem={this._renderItem.bind(this)}
          horizontal={true}
          pagingEnabled
          flatListRef={React.createRef()}
          ref={this.flatList}
        />
      </View>
    );
  };
  _renderItem({ item, index }) {
    return (
      <TouchableOpacity
        style={{ width: width }}
        disabled={true}
        onPress={() => {
          NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.NEWS_DETAIL, {
            newsID: item.newsID
          });
        }}
      >
        <Image source={{ uri: item.urlImage }} style={styles.sliderItems} />
      </TouchableOpacity>
    );
  }
  _keyExtractor(item, index) {
    return index.toString();
  }
  _renderCarousel() {
    const { dataHomeScreen, listCombo, listItem } = this.state;
    const data = listItem.concat(listCombo);
    return (
      <View style={styles.carousel}>
        <Carousel
          data={data}
          renderItem={this._renderCarouselItem}
          itemWidth={width / 3 + 50}
          sliderWidth={width}
          enableMomentum
          autoplay
          contentContainerCustomStyle={{ paddingVertical: 10 }}
          loop
        />
      </View>
    );
  }
  _renderCarouselItem = ({ item: itemService, index }) => {
    if (Array.isArray(itemService) == false)
      return (
        <TouchableOpacity
          style={styles.carouselItem}
          onPress={() => {
            NavigationUtil.push(SCREEN_ROUTER_CUSTOMER.IMAGE_VIEWER, {
              data: itemService.imageUrl.map(e => ({ url: e })),
              renderFooter: (
                <FooterOrderPackage
                  action={() => {
                    if (!this.state.isLogin)
                      this.props.actionModal(SHOW_REQUIRE_LOGIN);
                    else {
                      this.props.setState(SCREEN_ROUTER_CUSTOMER.BOOKING, {
                        itemService
                      });
                      NavigationUtil.goBack(true);
                      NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.BOOKING);
                    }
                  }}
                />
              )
            });
          }}
        >
          <FastImage
            source={{ uri: itemService.thumbnail }}
            style={{ width: 141, height: 185 }}
          />
        </TouchableOpacity>
      );
  };
  _renderNews() {
    return (
      <View style={styles.news}>
        <Text
          style={{
            fontFamily: R.fonts.quicksand_bold,
            fontSize: 15,
            textTransform: "uppercase"
          }}
        >
          {R.strings().news}
        </Text>
        <TouchableOpacity
          onPress={() => NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.NEWS)}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 11
            }}
          >
            {R.strings().show_more}
          </Text>
        </TouchableOpacity>
      </View>
    );
  }
  _renderNewsItem = ({ item, index }) => {
    const { listNews } = this.state.dataHomeScreen;
    return (
      <>
        <TouchableOpacity
          activeOpacity={0.9}
          style={[
            styles.newsItem,
            {
              marginRight:
                listNews.length % 2 != 0 && index === listNews.length - 1
                  ? 8
                  : 4
            }
          ]}
          onPress={() =>
            NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.NEWS_DETAIL, {
              newsID: item.newsID
            })
          }
        >
          <FastImage
            source={{ uri: item.urlImage }}
            style={{ height: 120, borderRadius: 5 }}
            resizeMode="cover"
          />
          <Text
            style={{
              fontFamily: R.fonts.quicksand_semi_bold,
              fontSize: 14,
              paddingHorizontal: 4
            }}
            numberOfLines={3}
          >
            {item.title}
          </Text>
          <View
            style={{
              flexDirection: "row",
              alignItems: "center",
              marginTop: 6,
              marginLeft: 4
            }}
          >
            <FastImage
              style={{
                height: 14,
                width: 14,
                marginRight: 5,
                tintColor: "red"
              }}
              source={R.images.ic_clock}
            />
            <Text
              style={{
                fontFamily: R.fonts.quicksand_regular,
                fontSize: 11
                // color: theme.colors.grayBorder
              }}
            >
              {item.createDateSTR}
            </Text>
          </View>
        </TouchableOpacity>
      </>
    );
  };
}

const styles = StyleSheet.create({
  root_header_login: { flexDirection: "row", paddingBottom: 10 },
  avatar_header: {
    height: 52,
    width: 52,
    borderRadius: 52,
    overflow: "hidden",
    borderWidth: 0.5,
    borderColor: theme.colors.borderTopColor,
    backgroundColor: "white"
  },
  root_text_header: {
    justifyContent: "space-around",
    paddingVertical: 5,
    marginHorizontal: 10
  },
  text_welcome_name: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 13,
    color: theme.colors.white
  },
  text_point: {
    fontFamily: R.fonts.quicksand_light,
    fontSize: 12,
    color: theme.colors.white
  },
  view: {
    flex: 1
  },
  slider_box: {
    backgroundColor: theme.colors.white
  },
  header: {
    paddingHorizontal: 16,
    paddingTop: Platform.OS == "ios" ? 10 : 30,
    backgroundColor: theme.colors.primary,
    width: "100%"
  },
  menu: {
    paddingVertical: 20,
    flexDirection: "row",
    justifyContent: "space-around",
    width: "100%",
    backgroundColor: theme.colors.white,
    marginVertical: 15,
    paddingHorizontal: 15
  },
  menuItem: {
    backgroundColor: theme.colors.white,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 3
    },
    shadowOpacity: 0.29,
    shadowRadius: 4.65,

    elevation: 7,
    borderRadius: 100 / 2,
    justifyContent: "center",
    alignItems: "center",
    padding: 12
  },
  carousel: {
    backgroundColor: theme.colors.white,
    width: "100%",
    paddingVertical: 10
  },
  carouselItem: {
    // height: 185,
    alignSelf: "center",
    // width: 141,
    // borderRadius: 10,
    // borderWidth: 0.5,
    // borderColor: theme.colors.gray,
    shadowColor: "#000",
    shadowOffset: {
      width: 0,
      height: 3
    },
    shadowOpacity: 0.29,
    shadowRadius: 4.65,

    elevation: 7
  },
  news: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginHorizontal: 13,
    marginTop: 10,
    marginBottom: 6
    // backgroundColor: colors.white
  },
  newsItem: {
    marginHorizontal: 4,
    marginVertical: 3,
    flex: 1,
    maxWidth: width / 2,
    backgroundColor: theme.colors.white,
    shadowOffset: { width: 0, height: 5 },
    shadowColor: "#8B8B8B",
    shadowOpacity: 0.2,
    shadowRadius: 6,
    elevation: Platform.OS == "android" ? 3 : 0,
    paddingBottom: 8,
    borderRadius: 5,
    borderColor: theme.colors.borderTopColor,
    borderWidth: 1
  },
  sliderItems: {
    // marginHorizontal: 10,
    // height: 155,
    width: width,
    aspectRatio: 2,
    // borderRadius: 10,
    backgroundColor: theme.colors.primary,
    // overflow: "hidden",
    alignSelf: "center"
  },
  text_referral_code: {
    fontFamily: R.fonts.quicksand_regular,
    fontSize: 13,
    color: theme.colors.white
  }
});

const mapStateToProps = state => ({
  lang: state.lang,
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfoAction,
  sendBookingNow,
  actionModal,
  setState
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TabHomeScreen);

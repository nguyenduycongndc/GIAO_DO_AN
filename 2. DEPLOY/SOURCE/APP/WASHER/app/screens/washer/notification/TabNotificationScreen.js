import React, { Component } from "react";
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  Image,
  RefreshControl,
  TouchableOpacity
} from "react-native";
import { connect } from "react-redux";
import R from "@app/assets/R";
import ScreenComponent from "@app/components/ScreenComponent";
import theme from "@theme";
// import { getNotification } from "@app/constants/Api";
import { getNotification } from "@app/redux/actions";
import CallApiHelper from "@app/utils/CallApiHelper";
import Empty from "@app/components/Empty";
import reactotron from "reactotron-react-native";
import {
  REDUCER_WASHER,
  TYPE_NOTIFICATION,
  SCREEN_ROUTER_WASHER
} from "@app/constants/Constants";
import NavigationUtil from "@app/navigation/NavigationUtil";
const TYPE_NOTI_ENABLE_CLICK = [TYPE_NOTIFICATION.REVIEWS];
class TabNotificationScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      data: {}
    };
  }

  componentDidMount() {
    this.getData();
  }

  getData = () => {
    this.props.getNotification();
  };

  render() {
    const { data, isLoading, error } = this.props.notificationState;
    return (
      <ScreenComponent
        isLoading={isLoading}
        isError={error}
        reload={() => this.getData()}
        titleHeader={R.strings().notif_tab_cus}
        renderView={
          !isLoading &&
          !error && (
            <FlatList
              refreshControl={
                <RefreshControl
                  refreshing={this.state.isLoading}
                  onRefresh={() => this.getData()}
                />
              }
              ListEmptyComponent={
                <Empty description={R.strings().notify_emplty} />
              }
              style={styles.view}
              data={data || []}
              keyExtractor={(item, index) => index.toString()}
              renderItem={({ item, index }) => this._renderNewsItem(item)}
            />
          )
        }
      />
    );
  }
  _renderNewsItem(item) {
    return (
      <TouchableOpacity
        style={styles.root_view}
        disabled={
          !item.orderServiceID || !TYPE_NOTI_ENABLE_CLICK.includes(item.type)
        }
        onPress={() => {
          NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ORDER_DETAIL, {
            orderID: item.orderServiceID
          });
        }}
      >
        <Text numberOfLines={10} style={styles.text_content}>
          <Image
            source={item.icon ? { uri: item.icon } : R.images.notiImgDemo}
            style={styles.img_noti}
          />
          {item.content}
        </Text>
        <View
          style={styles.root_view_time}
          children={
            <>
              <Image
                style={styles.img_time}
                source={require("../../../assets/images/ic_clock.png")}
              />
              <Text style={styles.text_time}>{item.createdDateStr}</Text>
            </>
          }
        />
      </TouchableOpacity>
    );
  }
}

const styles = StyleSheet.create({
  text_content: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.textColor
  },
  img_noti: { height: 21, width: 21 },
  view: {
    flex: 1,
    backgroundColor: theme.colors.backgroundColor,
    paddingTop: 10
  },
  root_view: {
    paddingHorizontal: 13,
    paddingVertical: 12,
    backgroundColor: theme.colors.white,
    marginBottom: 10,
    justifyContent: "space-between"
  },
  text_time: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12,
    color: theme.colors.gray
  },
  root_view_time: {
    flexDirection: "row",
    alignItems: "center",
    marginHorizontal: 3,
    marginTop: 5
  },
  img_time: { height: 14, width: 14, marginRight: 5 }
});

const mapStateToProps = state => ({
  lang: state.lang,
  notificationState: state[REDUCER_WASHER.NOTIFICATION]
});

const mapDispatchToProps = {
  getNotification
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TabNotificationScreen);

import React, { Component } from "react";
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  RefreshControl,
  TouchableOpacity
} from "react-native";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import R from "@app/assets/R";
import ScreenComponent from "@app/components/ScreenComponent";
import theme from "@theme";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import { createImageProgress } from "react-native-image-progress";
import FastImage from "@app/components/FastImage";
import Empty from "@app/components/Empty";
import { setState } from "@action/index";
import { SCREEN_ROUTER_CUSTOMER, TYPE_NOTI } from "@app/constants/C";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { showMessagesAlert } from "@app/components/Alert";
const Image = createImageProgress(FastImage);

const TYPE_NOTIFY_ORDER = [
  TYPE_NOTI.NOTI_ORDER_STATUS_CANCEL,
  TYPE_NOTI.NOTI_ORDER_STATUS_CONFIRM,
  TYPE_NOTI.NOTI_ORDER_STATUS_PROCESS,
  TYPE_NOTI.NOTI_ORDER_STATUS_WAITING,
  TYPE_NOTI.NOTI_ORDER_STATUS_WASHING,
  TYPE_NOTI.NOTI_ADMIN_SEND,
  TYPE_NOTI.NOTI_HAVE_NEWS
];

class TabNotificationScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    this.state = {
      isLoading: false,
      error: "",
      listNotify: []
    };
  }

  componentDidMount() {
    this.getNotify();
    this.props.setState(SCREEN_ROUTER_CUSTOMER.NOTIF, {
      getNotify: this.getNotify
    });
  }

  getNotify() {
    CallApiHelper(API.getNotify, null, this, res => {
      this.setState({
        listNotify: res.result
      });
    });
  }

  render() {
    const { isLoading, error, listNotify } = this.state;
    return (
      <ScreenComponent
        reload={this.getNotify}
        isLoading={isLoading}
        isError={error}
        titleHeader={R.strings().notif_tab_cus}
        renderView={
          listNotify.length == 0 ? (
            <Empty description={R.strings().no_notification_now} />
          ) : (
            <View style={styles.view}>
              <FlatList
                contentContainerStyle={{
                  paddingVertical: 5,
                  flexGrow: 1,
                  backgroundColor: theme.colors.backgroundColor
                }}
                refreshControl={
                  <RefreshControl
                    onRefresh={() => this.getNotify()}
                    refreshing={isLoading}
                  />
                }
                data={listNotify}
                keyExtractor={(item, index) => index.toString()}
                renderItem={this._renderNewsItem}
              />
            </View>
          )
        }
      />
    );
  }
  _renderNewsItem = ({ item, index }) => {
    return (
      <TouchableOpacity
        disabled={!TYPE_NOTIFY_ORDER.includes(item.type)}
        onPress={() => {
          switch (item.type) {
            case TYPE_NOTI.NOTI_ADMIN_SEND:
              showMessagesAlert(
                item.title,
                item.content,
                () => {},
                R.strings().close
              );
              break;
            case TYPE_NOTI.NOTI_HAVE_NEWS:
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.NEWS_DETAIL, item);
            default:
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.GEN_DETAIl, item);
              break;
          }
        }}
      >
        <View
          style={{
            paddingHorizontal: 13,
            paddingVertical: 12,
            backgroundColor: theme.colors.white,
            marginBottom: 10,
            justifyContent: "space-between"
          }}
        >
          <View style={{ marginBottom: 5 }}>
            <Text
              // numberOfLines={5}
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 14,
                color: theme.colors.textColor,
                justifyContentL: "center"
              }}
            >
              <FastImage
                source={{ uri: item.icon }}
                style={{ height: 21, width: 21 }}
              />
              {"   " + item.title}
            </Text>
          </View>
          <View
            style={{
              flexDirection: "row",
              alignItems: "center",
              marginLeft: 3
            }}
          >
            <FastImage
              style={{ height: 14, width: 14, marginRight: 5 }}
              source={require("../../../assets/images/ic_clock.png")}
            />
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 12,
                color: theme.colors.gray
              }}
            >
              {item.createdDateStr}
            </Text>
          </View>
        </View>
      </TouchableOpacity>
    );
  };
}

const styles = StyleSheet.create({
  view: {
    flex: 1,
    backgroundColor: theme.colors.backgroundColor,
    paddingTop: 10
  }
});

const mapStateToProps = state => ({
  lang: state.lang
});

const mapDispatchToProps = {
  setState
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TabNotificationScreen);

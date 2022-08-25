import React, { Component } from "react";
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  RefreshControl,
  StyleSheet,
  Platform
} from "react-native";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import R from "@app/assets/R";
import ScreenComponent from "@app/components/ScreenComponent";
import theme from "@theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_CUSTOMER } from "@constant";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import { createImageProgress } from "react-native-image-progress";
import FastImage from "@app/components/FastImage";
import * as Progress from "react-native-progress";

const Image = createImageProgress(FastImage);

export class NewsScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
      error: "",
      getNewsData: "",
      cateID: null
    };
  }

  componentDidMount() {
    this.getNews();
  }

  // onClickSomething() {
  //   this.carousel.goToPage(3);
  // }
  getNews() {
    CallApiHelper(API.requestNewsData, this.state.cateID, this, res => {
      this.setState({
        getNewsData: res.result
      });
    });
  }

  render() {
    const { isLoading, error, getNewsData } = this.state;
    return (
      <ScreenComponent
        back
        isLoading={isLoading}
        isError={error}
        reload={() => this.getNews()}
        titleHeader={R.strings().news}
        renderView={
          <>
            <View
              style={{ flex: 1, backgroundColor: theme.colors.backgroundColor }}
            >
              <FlatList
                contentContainerStyle={{
                  paddingVertical: 5,
                  flexGrow: 1,
                  backgroundColor: theme.colors.backgroundColor
                }}
                refreshControl={
                  <RefreshControl
                    onRefresh={() => this.getNews()}
                    refreshing={this.state.isLoading}
                  />
                }
                numColumns={2}
                data={getNewsData.listNews}
                keyExtractor={(item, index) => index.toString()}
                // renderItem={({ item, index }) => this._renderItem(item, index)}
                renderItem={this._renderNewsItem}
              />
            </View>
          </>
        }
      />
    );
  }
  _renderNewsItem = ({ item, index }) => {
    const { listNews } = this.state.getNewsData;
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
            style={{
              flex: 1,
              height: 120,
              borderRadius: 5,
              marginHorizontal: 1
            }}
            resizeMode="cover"
          />
          <Text
            style={{
              fontFamily: R.fonts.quicksand_semi_bold,
              fontSize: 14,
              paddingLeft: 6,
              marginTop: 2
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
  }
});

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(NewsScreen);

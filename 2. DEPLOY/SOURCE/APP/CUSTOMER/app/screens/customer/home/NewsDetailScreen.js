import React, { Component } from "react";
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  // Image,
  StyleSheet,
  ScrollView,
  RefreshControl,
  Platform
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
import * as Progress from "react-native-progress";
import HTML from "react-native-render-html";
import * as HTMLRenderers from "@app/components/HTMLRenderers";
import reactotron from "reactotron-react-native";

export class NewsDetailScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const newsID = navigation.getParam("newsID", {});
    this.state = {
      isLoading: false,
      error: "",
      getNewsData: "",
      relateNews: [],
      newsID: newsID
    };
  }

  componentDidMount() {
    this.getNewsDetail();
  }

  getNewsDetail() {
    CallApiHelper(API.requestNewsDetail, this.state.newsID, this, res => {
      this.setState({
        getNewsData: res.result.newDetail,
        relateNews: res.result.listNew
      });
    });
  }

  renderContentNew() {
    const { isLoading, error, getNewsData, relateNews } = this.state;

    return (
      <>
        <View
          style={{
            paddingHorizontal: 4,
            paddingVertical: 10,
            backgroundColor: theme.colors.white,
            marginBottom: 18
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 18,
              color: theme.colors.textColor,
              paddingHorizontal: 10
            }}
          >
            {getNewsData?.title}
          </Text>
          <View
            style={{
              flexDirection: "row",
              alignItems: "center",
              paddingHorizontal: 10,
              paddingVertical: 10
            }}
          >
            <FastImage
              style={{ height: 14, width: 14, marginRight: 5 }}
              source={require("../../../assets/images/ic_clock.png")}
            />
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 11
              }}
            >
              {getNewsData?.createDateSTR}
            </Text>
          </View>
          <FastImage
            source={{ uri: getNewsData?.urlImage }}
            style={{
              width: width - 10,
              height: 200,
              borderRadius: 5,
              marginBottom: 8
            }}
          />
          <HTML
            ignoredStyles={["font-family"]}
            renderers={HTMLRenderers}
            html={`${getNewsData?.description}`}
            imagesMaxWidth={width - 10}
            onLinkPress={(evt, href) => {
              Linking.openURL(href);
            }}
          />
          <View style={{ paddingHorizontal: 5 }}>
            <HTML
              ignoredStyles={["font-family"]}
              renderers={HTMLRenderers}
              html={`${getNewsData?.content}`}
              imagesMaxWidth={width - 10}
              onLinkPress={(evt, href) => {
                Linking.openURL(href);
              }}
            />
          </View>
        </View>
        <Text
          style={{
            fontFamily: R.fonts.quicksand_bold,
            fontSize: 18,
            marginHorizontal: 11,
            marginBottom: 10
          }}
        >
          {R.strings().related_news}
        </Text>
      </>
    );
  }

  render() {
    const { isLoading, error, getNewsData, relateNews } = this.state;
    return (
      <ScreenComponent
        back
        isLoading={isLoading}
        isError={error}
        reload={() => this.getNewsDetail()}
        titleHeader={R.strings().news_detail}
        isSafeArea={false}
        renderView={
          <FlatList
            refreshControl={
              <RefreshControl
                onRefresh={() => this.getNewsDetail()}
                refreshing={this.state.isLoading}
              />
            }
            ListHeaderComponent={this.renderContentNew()}
            numColumns={2}
            data={relateNews}
            keyExtractor={(item, index) => item.newsID}
            renderItem={this._renderNewsItem}
          />
        }
      />
    );
  }
  _renderNewsItem = ({ item, index }) => {
    return (
      <>
        <TouchableOpacity
          style={styles.newsItem}
          onPress={() =>
            this.setState(
              {
                newsID: item.newsID
              },
              () => this.getNewsDetail()
            )
          }
        >
          <FastImage
            source={{ uri: item.urlImage }}
            style={{ flex: 1, height: 120, borderRadius: 5 }}
            resizeMode="cover"
          />
          <Text
            style={{
              fontFamily: R.fonts.quicksand_semi_bold,
              fontSize: 14,
              paddingLeft: 6
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
)(NewsDetailScreen);

import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  FlatList,
  Dimensions
} from "react-native";
import NavigationUtil from "../navigation/NavigationUtil";
import theme from "@app/constants/Theme";
import R from "@app/assets/R";
import PropTypes from "prop-types";
import Icon from "./Icon";
import { SCREEN_ROUTER_CUSTOMER } from "@constant";
import FastImage from "@app/components/FastImage";
const width = Dimensions.get("window").width;
export default class PhotoListView extends React.Component {
  static propTypes = {
    data: PropTypes.arrayOf(
      PropTypes.shape({ url: PropTypes.string, id: PropTypes.any })
    ),
    emptyMessage: PropTypes.string,
    description: PropTypes.string,
    editable: PropTypes.bool,
    vertical: PropTypes.bool,
    thumbnailSize: PropTypes.number,
    onPressImage: PropTypes.func,
    onDeletePress: PropTypes.func,
    onAddPress: PropTypes.func
  };

  constructor(props) {
    super(props);
    this.itemMargin = 2;
    this.clearButtonMargin = this.itemMargin + 2;
    const { thumbnailSize = 160, vertical = false } = this.props;

    const itemSize = thumbnailSize + this.itemMargin * 2 + 2;
    const column = Math.floor(width / itemSize);

    this.itemFlex = (column && 1 / column) || 1;
    this.state = {
      itemSize: itemSize,
      gridColumn: (vertical && column) || undefined,
      itemStyle: (vertical && {
        padding: this.itemMargin,
        flex: this.itemFlex
      }) || {
        padding: this.itemMargin,
        width: itemSize
      }
    };
  }

  render() {
    const {
      data,
      thumbnailSize = 160,
      emptyMessage,
      editable = false,
      vertical = false
    } = this.props;

    if (!editable && (data || []).length == 0 && !!emptyMessage) {
      return (
        <View
          style={{
            backgroundColor: theme.colors.gray3,
            borderColor: theme.colors.gray,
            borderWidth: 1,
            borderRadius: 4,
            padding: 8,
            flex: 1,
            direction: "vertical",
            justifyContent: "center",
            height: thumbnailSize,
            alignItems: "center"
          }}
        >
          <View style={{ flex: 1 }} />
          <Icon.FontAwesome name="image" size={60} color={theme.colors.gray} />

          <View style={{ flex: 1 }}>
            <Text
              style={{
                color: theme.colors.black2,
                textAlign: "center",
                textAlignVertical: "center"
              }}
            >
              {emptyMessage}
            </Text>
          </View>
        </View>
      );
    }
    return (
      <FlatList
        data={data}
        maxToRenderPerBatch={10}
        horizontal={!vertical}
        numColumns={this.state.gridColumn}
        keyExtractor={(item, index) => index.toString()}
        renderItem={this._renderItem}
        showsHorizontalScrollIndicator={false}
        ListHeaderComponent={this._renderListHeaderComponent}
      />
    );
  }

  _renderListHeaderComponent = () => {
    return (
      (this.props.editable && (
        <View
          style={[
            this.state.itemStyle,
            { flex: 1, width: 120 },
            this.props.style,
            this.props.styleAddImage
          ]}
        >
          <TouchableOpacity
            style={[
              {
                padding: 10,
                alignItems: "center",
                justifyContent: "center",
                borderRadius: 5,
                borderWidth: 1,
                width: 112,
                height: 112,
                borderColor: theme.colors.grayBorder,
                marginRight: 0,
                borderStyle: "dashed"
              },
              this.props.style
            ]}
            onPress={this._onPressAddImage}
          >
            <Icon.AntDesign
              size={36}
              name="plus"
              color={theme.colors.grayPlus}
            />
          </TouchableOpacity>
        </View>
      )) || <View />
    );
  };

  _renderItem = ({ item, index }) => {
    return (
      <View
        style={[this.state.itemStyle, this.props.style, this.props.styleImage]}
      >
        <View
          style={{
            // margin: this.itemMargin,
            borderWidth: 0.5,
            borderColor: theme.colors.gray,
            alignItems: "center",
            borderRadius: 4,
            overflow: "hidden",
            marginLeft: 5
          }}
        >
          <TouchableOpacity
            style={{
              alignItems: "center"
            }}
            onPress={() =>
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.IMAGE_VIEWER, {
                data: this.props.data,
                index
              })
            }
          >
            <LoadableImage
              resizeMode="cover"
              source={{ uri: item.url }}
              style={[{ height: 112, width: 203 }, this.props.style]}
            />
          </TouchableOpacity>
        </View>
        {this.props.editable && (
          <TouchableOpacity
            style={{
              position: "absolute",
              right: this.clearButtonMargin,
              top: this.clearButtonMargin,
              borderRadius: 50,
              width: 26,
              height: 26,
              backgroundColor: theme.colors.white
            }}
            onPress={() => this.props.onDeletePress({ photo: item, index })}
          >
            <Icon.AntDesign
              size={26}
              name="closecircle"
              color={theme.colors.primary}
            />
          </TouchableOpacity>
        )}
      </View>
    );
  };

  _onPressAddImage = () => {
    this.props.onAddPress && this.props.onAddPress();
  };
}

class LoadableImage extends React.Component {
  render() {
    return <FastImage {...this.props} />;
  }
}

export class ImageListViewer extends React.Component {
  render() {
    return (
      <Modal
        visible={this.state.imageViewerVisible}
        style={{ margin: 0 }}
        coverScreen={true}
        onRequestClose={() => {}}
      >
        <View style={{ flex: 1 }}>
          <ImageViewer
            style={{ flex: 1 }}
            imageUrls={this.state.images}
            index={this.state.imageViewerIndex}
            enableSwipeDown={true}
            onCancel={this._hideImageViewer}
          />
          <TouchableOpacity
            onPress={this._hideImageViewer}
            style={{ position: "absolute", left: 8, top: 8 }}
          >
            <FastImage
              source={R.images.ic_cancel_circle_black}
              style={{ width: 24, height: 24, margin: 8 }}
            />
          </TouchableOpacity>
        </View>
      </Modal>
    );
  }
  _hideImageViewer = () => {
    this.setState({ imageViewerVisible: false });
  };
}

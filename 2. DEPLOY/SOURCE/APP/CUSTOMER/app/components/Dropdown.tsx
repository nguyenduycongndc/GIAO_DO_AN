"use-strict";
import React, { Component } from "react";
import {
  StyleSheet,
  Text,
  Image,
  Dimensions,
  ViewStyle,
  TouchableOpacity,
  ScrollView,
  View,
  FlatList
} from "react-native";
import R from "@app/assets/R";
import { StyleProp } from "react-native";
import { colors } from "@app/constants/Theme";
import FastImage from "./FastImage";

const { width, height } = Dimensions.get("window");

interface Props {
  onSelect?: (index: number, item: JSON) => void;
  data: any[];
  style?: StyleProp<ViewStyle>;
  styleDropdown?: StyleProp<ViewStyle>;
  defaultValue?: string;
}

interface State {
  show: boolean;
}

export default class Dropdown extends Component<Props> {
  state = {
    show: false,
    select: -1
  };

  renderList = list =>
    list.map((item, index) => (
      <TouchableOpacity
        onPress={() => this.onPress(index, item)}
        style={[
          {
            top: 40 * index - 1,
            position: "absolute",
            backgroundColor: colors.white,
            width: "95%",
            padding: 10,
            marginHorizontal: 10,
            borderWidth: 0.3,
            borderRadius: 5,
            overflow: "hidden"
          },
          this.props.styleDropdown
        ]}
        children={
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14
            }}
            children={item}
          />
        }
      />
    ));
  onPress = (index, item) =>
    this.setState({ show: false, select: index }, () =>
      this.props.onSelect(index, item)
    );
  switch = () => this.setState({ show: !this.state.show });
  render() {
    const { data, style, defaultValue } = this.props;
    const { show, select } = this.state;
    return (
      <View style={style}>
        <TouchableOpacity
          onPress={this.switch}
          style={[styles.dropdownStyle, style]}
          children={
            <>
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_medium,
                  fontSize: 14
                }}
                children={
                  select == -1 ? defaultValue || R.strings().all : data[select]
                }
              />
              <FastImage
                source={R.images.ic_right_arrow}
                style={styles.icArrow}
                resizeMode="center"
              />
            </>
          }
        />
        <View style={{ zIndex: 99 }} children={show && this.renderList(data)} />
      </View>
    );
  }
}

const styles = StyleSheet.create({
  dropdownStyle: {
    borderRadius: 5,
    backgroundColor: "#ECECEC",
    flexDirection: "row",
    zIndex: 1
  },

  icArrow: {
    position: "absolute",
    right: 10,
    transform: [{ rotate: "90deg" }],
    alignSelf: "center",
    width: 10,
    height: 10
  },
  dropdown: {
    marginHorizontal: 10,
    padding: 5,
    position: "absolute"
  }
});

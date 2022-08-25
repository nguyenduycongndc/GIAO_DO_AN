import React, { Component } from "react";
import {
  StyleSheet,
  View,
  Image,
  Dimensions,
  ViewStyle,
  TextStyle
} from "react-native";
import ModalDropdown from "./ModalDropdown";
import R from "@app/assets/R";
import { StyleProp } from "react-native";
import Icon from "@app/components/Icon";

const { width, height } = Dimensions.get("window");

interface Props {
  /**
   * Hàm được gọi khi click item trong Dropdown
   */
  onSelect?: (index: number, item: JSON) => void;
  /**
   * Hiển thị View chứa item trong Dropdown
   */
  renderRow: (item: object, index: number, isSelected: boolean) => void;
  /**
   * Xuất hiện khi click item trong dropdown
   */
  renderButtonText?: (text: any) => void;
  data: any[];
  /**
   * Style của Dropdown
   */
  style?: StyleProp<ViewStyle>;
  defaultValue?: string;
  dropDownStyle: StyleProp<ViewStyle>;
  isShowInBottom?: boolean;
  textStyle?: StyleProp<TextStyle>;
  imageStyle?: StyleProp<ViewStyle>;
}

export default class Dropdown extends Component<Props> {
  render() {
    const {
      onSelect,
      renderRow,
      renderButtonText,
      data,
      style,
      defaultValue,
      dropDownStyle,
      isShowInBottom,
      textStyle,
      imageStyle
    } = this.props;
    //console.log(this.props)
    return (
      <>
        <ModalDropdown
          style={[styles.dropdownStyle, style]}
          isShow={isShowInBottom}
          dropdownStyle={dropDownStyle || null}
          textStyle={[styles.textStyle, textStyle]}
          dropdownTextStyle={textStyle}
          renderButtonText={renderButtonText}
          defaultIndex={-1}
          defaultValue={defaultValue || "Tất cả"}
          animated
          accessible
          onSelect={onSelect}
          renderRow={renderRow}
          options={data}
        />
        <Icon.MaterialIcons
          name="keyboard-arrow-down"
          size={12}
          style={[styles.icArrow, imageStyle]}
        />
        {/* <Image
          source={R.images.ic_down_arrow}
          style={styles.icArrow}
          resizeMode="center"
        /> */}
      </>
    );
  }
}

const styles = StyleSheet.create({
  dropdownStyle: {
    // width: width / 2,
    borderRadius: 5,
    // borderWidth: 0.1,
    // borderColor: '#707070',
    // padding: 10,
    marginTop: 5,
    paddingHorizontal: 10,
    paddingVertical: 15,
    backgroundColor: "#ECECEC"
  },
  textStyle: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    paddingRight: 10
  },
  icArrow: {
    position: "absolute",
    top: 11,
    right: 10,
    width: 17,
    height: 15,
    resizeMode: "center"
  },
  dropdown: {
    // height: 0,
    // position: "absolute",
    // top: 0
    // borderWidth: StyleSheet.hairlineWidth,
    // borderColor: "lightgray",
    // borderRadius: 2,
    // backgroundColor: "white",
    // justifyContent: "center"
  }
});

import React, { Component } from "react";
import {
  StyleSheet,
  View,
  Image,
  Dimensions,
  ViewStyle,
  TouchableOpacity,
  TextStyle
} from "react-native";
import ModalDropdown from "./ModalDropdown";
import R from "@app/assets/R";
import { StyleProp } from "react-native";

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
  textStyle?: StyleProp<TextStyle>;
  isShowInBottom?: false;
  isShowIconArrow?: true;
}

export default class Dropdown extends Component<Props> {
  render() {
    const {
      onSelect,
      renderRow,
      data,
      style,
      defaultValue,
      dropDownStyle,
      isShowInBottom,
      isShowIconArrow,
      textStyle
    } = this.props;
    return (
      <ModalDropdown
        style={[style]}
        isShow={isShowInBottom}
        dropdownStyle={dropDownStyle || null}
        textStyle={[styles.textStyle, textStyle]}
        defaultIndex={-1}
        defaultValue={defaultValue || "Tất cả"}
        animated
        accessible
        onSelect={onSelect}
        renderRow={renderRow}
        options={data}
        isShowIconArrow={isShowIconArrow}
      />
    );
  }
}

const styles = StyleSheet.create({
  textStyle: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    textAlignVertical: "center"
  },

  dropdown: {}
});

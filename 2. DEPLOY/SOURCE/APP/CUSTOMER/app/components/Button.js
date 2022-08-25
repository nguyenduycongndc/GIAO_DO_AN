import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  TouchableOpacity,
  Image,
  ViewPropTypes
} from "react-native";
import R from "@app/assets/R";
import theme from "@theme";
import Icon from "./Icon";
import I18n from "@i18";
import C from "@app/constants/C";
import PropTypes from "prop-types";

const ViewPropTypesStyle = ViewPropTypes
  ? ViewPropTypes.style
  : View.propTypes.style;

const debounce = function(func = () => {}, wait = 1000, immediate = true) {
  let timeout;

  return function executedFunction() {
    const context = this;
    const args = arguments;

    const later = function() {
      timeout = null;
      if (!immediate) func.apply(context, args);
    };

    const callNow = immediate && !timeout;

    clearTimeout(timeout);

    timeout = setTimeout(later, wait);

    if (callNow) func.apply(context, args);
  };
};

export default class Button extends Component {
  static propTypes = {
    action: PropTypes.func,
    waitTime: PropTypes.number,
    children: PropTypes.node,
    activeOpacity: PropTypes.number,
    style: ViewPropTypesStyle
  };

  static defaultProps = {
    action: () => {},
    waitTime: 300,
    children: null,
    style: null,
    activeOpacity: 0.2
  };

  constructor(props) {
    super(props);
    this.onPressDebounce = debounce(props.action, props.waitTime);
  }
  render() {
    const {
      title,
      borderColor,
      backgroundColor,
      colorText,
      back,
      forward,
      width = 170,
      action,
      borderWidth = 0.25,
      icon,
      imageSource,
      styleImg,
      uppercase,
      buttonStyle,
      disabled,
      onLayout
    } = this.props;
    return (
      <TouchableOpacity
        onLayout={onLayout}
        onPress={this.onPressDebounce}
        style={[
          styles.button,
          {
            borderWidth: borderColor ? 0.25 : 0,
            borderColor: borderColor ? borderColor : "",
            backgroundColor: backgroundColor,
            flexDirection: "row",
            alignItems: "center",
            justifyContent: "space-around",
            width: width,
            paddingHorizontal: "5%",
            paddingVertical: 13
          },
          buttonStyle
        ]}
        disabled={disabled}
      >
        {icon}
        {back && (
          <Icon.Ionicons
            name="ios-arrow-back"
            size={15}
            color={theme.colors.white}
          />
        )}
        <View style={{ flexDirection: "row", alignItems: "center" }}>
          {imageSource && <Image source={imageSource} style={styleImg} />}
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              // textTransform: uppercase ? "uppercase" : "",
              color: colorText ? colorText : ""
            }}
          >
            {uppercase ? title.toUpperCase() : title}
          </Text>
        </View>
        {forward && (
          <Icon.Ionicons
            name="ios-arrow-forward"
            size={15}
            color={theme.colors.white}
          />
        )}
      </TouchableOpacity>
    );
  }
}

const styles = StyleSheet.create({
  button: {
    borderRadius: 5,
    marginVertical: 10
  }
});

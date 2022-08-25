import React, { Component } from "react";
import { Text, View, StyleSheet, StyleProp, ViewStyle } from "react-native";
import Icon from "@app/components/Icon";
import theme from "@theme";
import R from "@R";
import { Rating, AirbnbRating } from "react-native-elements";
interface Props {
  numberStar: number;
  size?: number;
  color?: string;
  style?: StyleProp<ViewStyle>;
  isShowNumber?: boolean;
  readonly?: boolean;
}
export default class RateStar extends Component<Props> {
  render() {
    const {
      numberStar,
      size = 20,
      color = theme.colors.orange,
      style,
      isShowNumber,
      readonly
    } = this.props;

    return (
      <View style={[{ flexDirection: "row", alignItems: "center" }, style]}>
        <Rating startingValue={numberStar} imageSize={size} type="star" readonly={readonly} />
        {isShowNumber && (
          <View
            style={{
              width: 0.5,
              height: size,
              backgroundColor: theme.colors.gray,
              marginHorizontal: 5
            }}
          />
        )}
        {isShowNumber && (
          <Text
            style={[
              styles.textInfo,
              {
                fontSize: size
              }
            ]}
          >
            {numberStar}
          </Text>
        )}
      </View>
    );
  }
}

const styles = StyleSheet.create({
  textInfo: {
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.nameText
  }
});

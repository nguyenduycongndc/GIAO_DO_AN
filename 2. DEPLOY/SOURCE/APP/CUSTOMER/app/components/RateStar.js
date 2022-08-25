import React, { Component } from "react";
import { Text, View, StyleSheet } from "react-native";
import Icon from "@app/components/Icon";
import theme from "@theme";
import R from "@R";
import { Rating, AirbnbRating } from "react-native-elements";

export default class RateStar extends Component {
  state = {
    start: this.props.numberStar || 3
  };
  render() {
    const {
      numberStar,
      size = 20,
      color = theme.colors.orange,
      style,
      isShowNumber,
      onFinishRating,
      styleRating,
      readonly,
      rating = false
    } = this.props;

    return (
      <View style={[{ flexDirection: "row", alignItems: "center" }, style]}>
        {rating ? (
          <AirbnbRating
            showRating={false}
            size={size}
            onFinishRating={onFinishRating}
            starStyle={{ marginHorizontal: 10 }}
            count={5}
            defaultRating={5}
          />
        ) : (
          <Rating
            style={styleRating}
            startingValue={numberStar}
            ratingBackgroundColor="white"
            ratingColor="white"
            imageSize={size}
            ratingCount={5}
            readonly={readonly}
            onFinishRating={onFinishRating}
          />
        )}

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

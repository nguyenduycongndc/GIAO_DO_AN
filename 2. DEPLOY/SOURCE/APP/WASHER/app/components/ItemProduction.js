import React from "react";
import { TouchableOpacity, StyleSheet, View, Text } from "react-native";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import FastImage from "react-native-fast-image";

export default (item, renderButton) => {
  return (
    <TouchableOpacity
      onPress={() =>
        NavigationUtil.navigate(SCREEN_ROUTER_WASHER.PRODUCTION_DETAIL, item)
      }
      style={{
        ...styles.root_item
      }}
      children={
        <>
          <FastImage
            source={R.images.ic_zalo}
            style={{
              flex: 1,
              width: width / 3,
              height: width / 3,
              alignSelf: "center"
            }}
            resizeMode="contain"
          />
          <View
            style={{ flex: 2 }}
            children={
              <>
                <Text style={styles.title_item} children={item.name} />
                <Text
                  style={styles.content_item}
                  children={item.description}
                  numberOfLines={3}
                />
                <View
                  style={styles.root_price}
                  children={
                    <>
                      <Text style={styles.price_item} children={item.price} />
                      {renderButton}
                    </>
                  }
                />
              </>
            }
          />
        </>
      }
    />
  );
};
const styles = StyleSheet.create({
  root_item: {
    flexDirection: "row",
    backgroundColor: colors.white,
    padding: 10,
    borderRadius: 10,
    margin: 10
  },
  title_item: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 12,
    marginHorizontal: 10,
    color: "#444444"
  },
  content_item: {
    maxWidth: "80%",
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 9,
    color: "#7B7B7B",
    margin: 15
  },
  price_item: {
    alignSelf: "center",
    marginHorizontal: 15,
    color: colors.primary,
    fontFamily: R.fonts.quicksand_medium
  },

  root_price: {
    flexDirection: "row",
    marginVertical: 5,
    position: "absolute",
    bottom: 10
  }
});

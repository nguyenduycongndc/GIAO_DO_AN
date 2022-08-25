import React, { Component } from "react";
import { View, Text, TouchableOpacity } from "react-native";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER, REDUCER_WASHER } from "@app/constants/Constants";
import R from "@app/assets/R";
import FastImage from "react-native-fast-image";
import { colors } from "@app/constants/Theme";

class CartIcon extends Component {
  render() {
    const listcart = Object.values(this.props.cart.data);
    var count = 0;
    for (let i = 0; i < listcart.length; i++) {
      count += listcart[i].quantity;
    }

    return (
      <TouchableOpacity
        onPress={() =>
          NavigationUtil.navigate(SCREEN_ROUTER_WASHER.CART_SCREEN)
        }
        children={
          <>
            <FastImage
              source={R.images.ic_cart}
              style={{ width: 30, height: 30 }}
            />

            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                color: colors.white,
                backgroundColor: colors.primary,
                borderWidth: 1,
                borderColor: colors.white,
                width: 20,
                height: 20,
                alignSelf: "center",
                textAlign: "center",
                textAlignVertical: "center",
                borderRadius: 10,
                position: "absolute",
                right: -5,
                top: -10
              }}
              children={count > 99 ? ":D" : count}
            />
          </>
        }
      />
    );
  }
}

const mapStateToProps = state => ({
  cart: state[REDUCER_WASHER.CART]
});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(CartIcon);

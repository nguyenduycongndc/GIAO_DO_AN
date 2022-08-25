import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  StyleSheet,
  ScrollView
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import LoadableImage from "@app/components/LoadableImage";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import CartIcon from "@app/components/CartIcon";
import reactotron from "reactotron-react-native";
import { updateCart } from "@app/redux/actions";

class ProdutionDetailScreen extends Component {
  state = {
    data: this.props.navigation.state.params
  };
  render() {
    const { data } = this.state;
    return (
      <ScreenComponent
        back
        titleHeader={data.name || R.strings().detail}
        rightComponent={<CartIcon />}
        renderView={
          <>
            <ScrollView
              children={
                <>
                  <LoadableImage
                    source={R.images.ic_zalo}
                    style={styles.image}
                    resizeMode="contain"
                  />
                  <View
                    style={styles.root_price}
                    children={
                      <>
                        <View
                          style={{ flexDirection: "row" }}
                          children={
                            <>
                              <Text
                                style={styles.text_price}
                                children={data.price}
                              />
                              <Text
                                style={styles.text_base_price}
                                children={data.basePrice}
                              />
                            </>
                          }
                        />
                        <Text
                          style={styles.text_left}
                          children={`Còn lại: ${data.qty}`}
                        />
                      </>
                    }
                  />
                  <Text style={styles.text_name} children={data.name} />
                  <Text
                    style={styles.description}
                    children={data.description}
                  />
                  <TouchableOpacity
                    children={
                      <>
                        {/* <Text
                          style={{ margin: 10, alignSelf: "center" }}
                          children="Đã thêm vào giỏ hàng"
                        /> */}
                      </>
                    }
                  />
                </>
              }
            />
            <View
              style={styles.root_button_footer}
              children={
                <>
                  <TouchableOpacity
                    style={styles.root_button_footer_child}
                    onPress={() => {
                      this.props.updateCart(data, 1);
                    }}
                    children={
                      <Text
                        style={styles.text_footer}
                        children="Thêm vào giỏ hàng"
                      />
                    }
                  />
                  <TouchableOpacity
                    style={{
                      ...styles.root_button_footer_child,
                      backgroundColor: colors.primary
                    }}
                    onPress={() => {
                      this.props.updateCart(data, 1);
                      NavigationUtil.navigate(SCREEN_ROUTER_WASHER.CART_SCREEN);
                    }}
                    children={
                      <Text style={styles.text_footer} children="Mua ngay" />
                    }
                  />
                </>
              }
            />
          </>
        }
      />
    );
  }
}
const mapStateToProps = state => ({});

const mapDispatchToProps = {
  updateCart
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ProdutionDetailScreen);

const button_footer = { flex: 1, padding: 10, borderRadius: 5, margin: 10 };

const styles = StyleSheet.create({
  description: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    padding: 15
  },
  text_name: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16,
    padding: 15
  },
  text_left: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16,
    color: "#19597A"
  },
  text_base_price: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: colors.primary,
    paddingStart: 5
  },
  text_price: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: "#4E4E4E",
    textDecorationLine: "line-through"
  },
  root_price: {
    flexDirection: "row",
    justifyContent: "space-between",
    paddingHorizontal: 15
  },
  image: {
    width: width * 0.8,
    height: width * 0.8,
    alignSelf: "center"
  },
  root_button_footer: {
    flexDirection: "row",
    position: "absolute",
    bottom: 10
  },
  text_footer: {
    textAlign: "center",
    color: colors.white,
    fontSize: 13,
    fontFamily: R.fonts.quicksand_medium
  },
  button_footer,
  root_button_footer_child: {
    ...button_footer,
    marginEnd: 5,
    backgroundColor: colors.red
  }
});

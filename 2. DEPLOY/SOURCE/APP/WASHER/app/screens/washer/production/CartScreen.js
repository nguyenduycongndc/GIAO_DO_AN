import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  RefreshControl,
  FlatList,
  TextInput,
  KeyboardAvoidingView,
  StyleSheet
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
import Empty from "@app/components/Empty";
import { REDUCER_WASHER, SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import { updateCart } from "@app/redux/actions";
import ItemProduction from "@app/components/ItemProduction";
import callAPI from "@app/utils/CallApiHelper";
import { GetCart, CreateOrders } from "@app/constants/Api";
import reactotron from "reactotron-react-native";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { showConfirm } from "@app/components/Alert";

class CartScreen extends Component {
  state = {
    heightButton: 0,
    isLoading: false
  };

  onPressPay = () => {
    showConfirm(
      "Thanh toán",
      `Xác nhận thanh toán${money > 0 ? " " + money : ""}`,
      () =>
        callAPI(
          CreateOrders,
          {
            counponCode: "",
            listProduct: data.map(elem => ({
              productID: elem.productID,
              qty: elem.quantity
            }))
          },
          this,
          res => {
            data.map(elem => {
              this.props.updateCart(elem, -Infinity);
            });
            NavigationUtil.navigate(SCREEN_ROUTER_WASHER.PRODUCTION);
          }
        )
    );
  };

  render() {
    const data = Object.values(this.props.cart.data);
    const { isLoading } = this.state;
    var money = 0;
    data.forEach(element => {
      money += element.price * element.quantity;
    });
    const isValid = data.length > 0;
    return (
      <ScreenComponent
        back
        isLoading={isLoading}
        titleHeader="Giỏ hàng"
        renderView={
          <>
            <FlatList
              showsVerticalScrollIndicator={false}
              refreshControl={
                <RefreshControl refreshing={false} onRefresh={() => {}} />
              }
              ListEmptyComponent={
                <Empty description={R.strings().notify_emplty} />
              }
              keyExtractor={(item, index) => index.toString()}
              data={data}
              renderItem={({ item, index }) =>
                ItemProduction(
                  item,
                  <View
                    style={styles.root_add_or_minus}
                    children={
                      <>
                        <TouchableOpacity
                          onPress={() => {
                            this.props.updateCart(item, -1);
                          }}
                          style={{ paddingHorizontal: 15 }}
                          children={
                            <Text style={styles.text_quanity} children="-" />
                          }
                        />
                        <Text
                          style={styles.text_quanity}
                          children={item.quantity}
                        />
                        <TouchableOpacity
                          onPress={() => {
                            this.props.updateCart(item, 1);
                          }}
                          style={{ paddingHorizontal: 15 }}
                          children={
                            <Text style={styles.text_quanity} children="+" />
                          }
                        />
                      </>
                    }
                  />
                )
              }
            />
            <KeyboardAvoidingView
              style={styles.root_bottom}
              behavior="position"
              keyboardVerticalOffset={this.state.heightButton}
              children={
                <View
                  onLayout={even => {
                    this.setState({
                      heightButton: even.nativeEvent.layout.height - 20
                    });
                  }}
                  style={{ backgroundColor: colors.white }}
                >
                  <View
                    style={styles.root_bottom_content}
                    children={
                      <>
                        <Text
                          style={styles.text_promotion}
                          children={R.strings().promotion}
                        />
                        <TextInput
                          style={styles.text_input_promotion}
                          placeholder={R.strings().promotion}
                        />
                      </>
                    }
                  />
                  <TouchableOpacity
                    style={{
                      ...styles.button_create,
                      backgroundColor: isValid ? colors.primary : colors.gray
                    }}
                    onPress={this.onPressPay}
                    disabled={!isValid}
                    children={
                      <Text
                        style={styles.text_button_create}
                        children={`THANH TOÁN${money > 0 ? " " + money : ""}`}
                      />
                    }
                  />
                </View>
              }
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

const mapDispatchToProps = {
  updateCart
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(CartScreen);

const styles = StyleSheet.create({
  text_quanity: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 16
  },
  text_button_create: { textAlign: "center", color: colors.white },
  button_create: {
    backgroundColor: colors.primary,
    margin: 10,
    padding: 10,
    borderRadius: 5,
    width: "95%",
    alignSelf: "center"
  },
  text_input_promotion: {
    flex: 2,
    backgroundColor: colors.backgroundColor,
    padding: 10,
    borderRadius: 17,
    fontFamily: R.fonts.quicksand_light,
    fontSize: 13
  },
  text_promotion: {
    flex: 1,
    padding: 10,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 16
  },
  root_bottom_content: {
    flexDirection: "row",
    margin: 10,
    backgroundColor: colors.white
  },
  root_bottom: {
    position: "absolute",
    bottom: 10,
    width
  },
  root_add_or_minus: {
    flexDirection: "row",
    borderWidth: 0.5,
    borderRadius: 5,
    position: "absolute",
    right: -width * 0.4
  },
  root_content: {
    maxWidth: (width * 3) / 4 - 10,
    alignSelf: "center",
    padding: 10
  },
  header: {
    height: width * 0.4,
    backgroundColor: colors.primary
  },

  root_item: {
    flexDirection: "row",
    margin: 10,
    padding: 10,
    borderRadius: 10,
    backgroundColor: colors.white
  },
  ic_item: { width: width / 4 - 10, height: width / 4 - 10 }
});

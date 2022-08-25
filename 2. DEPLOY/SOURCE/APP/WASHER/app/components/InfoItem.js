import React, { Component } from "react";
import { Image, StyleSheet, TouchableOpacity, View, Text } from "react-native";
import R from "@app/assets/R";
import theme from "@theme";
import { renStatus, numberWithCommas } from "@app/constants/Functions";
import { PAYMENT_METHOD, ORDER_STATUS } from "@app/constants/Constants";
import reactotron from "reactotron-react-native";
import { connect } from "react-redux";
import { distance } from "@app/utils/Location";
import ProgressFastImage from "./ProgressFastImage";
import FastImage, { FastImageProps } from "react-native-fast-image";
import LoadableImage from "./LoadableImage";

class InfoItem extends Component {
  getIconService() {
    const { orderDetail } = this.props;
    var service = orderDetail.listService.find(
      service => service.serviceID === orderDetail.mainService
    );
    return service;
  }
  render() {
    const { onPress, orderDetail, icon, locationState } = this.props;
    const distanceLocation = distance(
      orderDetail.lati,
      orderDetail.longi,
      locationState.lati,
      locationState.longi
    );
    return (
      <TouchableOpacity style={styles.view} onPress={onPress}>
        <View
          style={{
            flexDirection: "row",
            alignItems: "center"
          }}
        >
          <View
            style={{
              flexDirection: "row",
              alignItems: "center"
            }}
          >
            <FastImage
              resizeMode={FastImage.resizeMode.contain}
              style={{ width: 13, height: 13 }}
              source={{
                uri: this.getIconService()
                  ? this.getIconService().icon
                  : orderDetail.uri
              }}
            />
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 14,
                marginLeft: 5,
                marginRight: 5
              }}
            >
              {orderDetail.serviceName || ""}
              {!!orderDetail.comboCode && (
                <Text
                  style={{
                    fontSize: 14,
                    fontFamily: R.fonts.quicksand_medium
                  }}
                  children=" - Combo"
                />
              )}
            </Text>
            <View
              style={{
                width: 1,
                height: 19,
                backgroundColor: theme.colors.gray
              }}
            />
            <FastImage
              style={{
                borderRadius: 50,
                width: 18,
                height: 18,
                marginHorizontal: 5
                // resizeMode: "center"
              }}
              source={
                orderDetail.customerAvatar
                  ? { uri: orderDetail.customerAvatar }
                  : R.images.ic_user_circle
              }
            />
            <Text
              ellipsizeMode="middle"
              numberOfLines={1}
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 11,
                color: theme.colors.nameText,
                flex: 1
              }}
            >
              {orderDetail?.customerName}
            </Text>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 11,
                color: theme.colors.nameText,
                textAlign: "right",
                flex: 1
              }}
            >
              {orderDetail?.bookDateStr}
            </Text>
          </View>
        </View>
        <View
          style={{
            marginTop: 12,
            flexDirection: "row",
            alignItems: "center"
          }}
        >
          <LoadableImage
            source={
              orderDetail?.car?.listImage &&
              !!orderDetail?.car?.listImage.length
                ? {
                    uri: orderDetail.car.listImage[0].url,
                    priority: FastImage.priority.low
                  }
                : require("../assets/images/carImg.png")
            }
            style={{ width: 79, height: 79, borderRadius: 7 }}
            resizeMode={FastImage.resizeMode.cover}
          />
          <View
            style={{
              marginLeft: 13,
              justifyContent: "space-around",
              height: 75,
              flex: 1,
              marginRight: 0
            }}
          >
            <Text style={{ fontFamily: R.fonts.quicksand_bold, fontSize: 14 }}>
              {orderDetail?.car?.carBrand}/{orderDetail?.car?.licensePlates}
            </Text>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 11,
                color: theme.colors.nameText
              }}
            >
              {orderDetail.customerAddress}
            </Text>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 11,
                color:
                  orderDetail.status == ORDER_STATUS.REJECTED
                    ? theme.colors.red
                    : theme.colors.primary
              }}
            >
              {renStatus(orderDetail.status)}
            </Text>
            <Text
              style={{ fontFamily: R.fonts.quicksand_medium, fontSize: 14 }}
            >
              {numberWithCommas(orderDetail.totalPrice.toString())}đ -{" "}
              {orderDetail.paymentType == PAYMENT_METHOD.CASH
                ? R.strings().cash
                : "VNPAY"}
            </Text>
          </View>
        </View>
        {!!Object.keys(locationState).length &&
          locationState.lati &&
          locationState.longi &&
          orderDetail.lati &&
          orderDetail.longi && (
            <Text style={styles.txtDistance}>
              {distanceLocation && distanceLocation < 100
                ? distanceLocation
                : "> 100"}{" "}
              km
            </Text>
          )}
      </TouchableOpacity>
    );
  }
}

const styles = StyleSheet.create({
  view: {
    width: "100%",
    // height: 127,
    padding: 8,
    backgroundColor: theme.colors.white,
    marginVertical: 10
  },
  txtDistance: {
    fontSize: 12,
    fontFamily: R.fonts.quicksand_regular,
    color: theme.colors.gray,
    position: "absolute",
    right: 10,
    bottom: 10
  }
});
const mapStateToProps = state => ({
  locationState: state.locationReducer
});

const mapDispatchToProps = {};
export default connect(
  mapStateToProps,
  mapDispatchToProps
)(InfoItem);

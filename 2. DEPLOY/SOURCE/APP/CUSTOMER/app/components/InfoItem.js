import React, { Component, PureComponent } from "react";
import { Image, StyleSheet, TouchableOpacity, View, Text } from "react-native";
import R from "@app/assets/R";
import theme, { colors } from "@theme";
import { BOOKING, ORDER_TYPE, PAYMENT_METHOD } from "@constant";
import TextMoney from "@app/components/TextMoney";
import setValue from "@app/utils/SetValue";
import { renStatus } from "@app/constants/Functions";
import RateStar from "./RateStar";
import reactotron from "reactotron-react-native";
import FastImage from "@app/components/FastImage";

export default class InfoItem extends PureComponent {
  getDateTime(dateStr) {
    var result = {
      time: "",
      day: ""
    };
    if (!dateStr) return result;
    var dateTime = dateStr.split(" ");
    if (dateTime.length > 1)
      return (result = {
        time: dateTime[0],
        day: dateTime[1]
      });
    return result;
  }

  render() {
    const { onPressRate, onPressReset, onPress, type, userInfo } = this.props;
    const timeBook =
      type == BOOKING.HISTORY &&
      userInfo.status == ORDER_TYPE.ORDER_STATUS_COMPLETE
        ? this.getDateTime(userInfo?.startDateStr)
        : this.getDateTime(userInfo?.bookDateStr);
    const timeEst =
      type == BOOKING.HISTORY
        ? this.getDateTime(userInfo?.completionDateStr)
        : this.getDateTime(userInfo?.estBookDateStr);
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
            <Image
              style={{ width: 13, height: 13, resizeMode: "contain" }}
              source={R.images.Icon_diamond_medal}
            />
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 14,
                marginLeft: 5,
                marginRight: 10
              }}
            >
              {userInfo?.serviceName || R.strings().service.toUpperCase()}
              {userInfo?.comboID ? " - Combo" : ""}
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
              }}
              source={
                !!userInfo?.agentAvatar
                  ? {
                      uri: userInfo?.agentAvatar
                    }
                  : R.images.avatarDemo
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
              {userInfo?.agentName || R.strings().washer}
            </Text>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 11,
                color: theme.colors.nameText,
                textAlign: "right",
                flex: 1.5
              }}
            >
              {`${timeBook.time} - ${timeEst.time} ${timeBook.day}`}
            </Text>
          </View>
        </View>
        <View
          style={{ marginTop: 12, flexDirection: "row", alignItems: "center" }}
        >
          {!!userInfo?.car?.listImage.length > 0 ? (
            <FastImage
              source={{ uri: userInfo?.car?.listImage[0].url }}
              style={{ width: 85, height: 85, borderRadius: 7 }}
            />
          ) : (
            <Image
              style={{ width: 85, height: 85, borderRadius: 7 }}
              source={R.images.carDemo}
            />
          )}
          {/* <FastImage
            source={
              !!userInfo?.car?.listImage.length > 0
                ? { uri: userInfo?.car?.listImage[0].url }
                : R.images.carDemo
            }
            style={{ width: 85, height: 85, borderRadius: 7 }}
          /> */}
          <View
            style={{
              marginHorizontal: 13,
              justifyContent: "space-around",
              height: 90
            }}
          >
            <Text
              style={{
                fontFamily: R.fonts.quicksand_bold,
                fontSize: 14
              }}
            >
              {userInfo?.car?.carBrand}/{userInfo?.car?.licensePlates}
            </Text>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 11,
                color: theme.colors.nameText,
                paddingEnd: 100
              }}
            >
              {userInfo?.customerAddress}
            </Text>
            <Text
              style={{
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 11,
                color:
                  userInfo?.status == ORDER_TYPE.ORDER_STATUS_CANCEL
                    ? theme.colors.red
                    : theme.colors.primary,
                marginBottom: 3
              }}
            >
              {renStatus(userInfo?.status)}
            </Text>

            <TextMoney
              value={userInfo?.totalPrice}
              typePay={` - ${
                userInfo?.paymentType == PAYMENT_METHOD.CASH
                  ? R.strings().cash
                  : R.strings().vnpay
              }`}
            />
          </View>
        </View>
        {type == BOOKING.HISTORY && (
          <View
            style={{
              flexDirection: "row",
              width: "100%",
              justifyContent: "flex-end"
            }}
          >
            {userInfo.status == ORDER_TYPE.ORDER_STATUS_COMPLETE &&
              (userInfo.noteRate != null
                ? this.renderNoteRate(userInfo?.noteRate, userInfo?.rating)
                : this._renderButton(R.strings().rate, onPressRate))}
            {this._renderButton(R.strings().reset, onPressReset)}
          </View>
        )}
      </TouchableOpacity>
    );
  }

  renderNoteRate = (noteRate, numberStar) => (
    <View
      style={{
        alignSelf: "center",
        marginHorizontal: 5,
        maxWidth: width / 5
      }}
    >
      <RateStar readonly numberStar={numberStar} size={15} />
      <Text
        style={{
          fontFamily: R.fonts.quicksand_medium,
          fontSize: 14,
          color: colors.orange,
          textAlign: "right"
        }}
        // children={"ádasdasdasdasdassdsdasdas"}
        children={noteRate}
        numberOfLines={1}
        minimumFontScale={0.01}
      />
    </View>
  );
  _renderButton(label, action) {
    return (
      <TouchableOpacity
        style={{
          backgroundColor:
            label != R.strings().reset
              ? theme.colors.grayButton
              : theme.colors.white,
          borderRadius: 50,
          borderWidth: 0.5,
          borderColor:
            label != R.strings().reset
              ? theme.colors.gray
              : theme.colors.primary,
          marginLeft: 5,
          marginTop: 5
        }}
        onPress={() => {
          if (action) action(this.props.userInfo);
        }}
      >
        <Text
          style={{
            fontFamily: R.fonts.quicksand_medium,
            fontSize: 14,
            paddingHorizontal: 20,
            paddingVertical: 9,
            color:
              label != R.strings().reset
                ? theme.colors.gray
                : theme.colors.primary
          }}
        >
          {label}
        </Text>
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
    marginBottom: 8
  }
});

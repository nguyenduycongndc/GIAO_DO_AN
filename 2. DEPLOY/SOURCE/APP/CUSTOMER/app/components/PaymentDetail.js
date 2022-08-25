import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  Image,
  TouchableOpacity,
  TextInput,
  Switch,
  Platform
} from "react-native";
import R from "@app/assets/R";
import theme, { colors } from "@theme";
import I18n from "@i18";
import { RowLableInfo, RowLableInfoText } from "@app/components/FormRow";
import Icon from "@app/components/Icon";
import ToggleSwitch from "toggle-switch-react-native";
import { numberWithCommas, formatMoney } from "@app/constants/Functions";
import NumberFormat from "react-number-format";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  PAYMENT_METHOD,
  SCREEN_ROUTER_CUSTOMER,
  TYPE_SERVICE,
  TYPE_DISCOUNT
} from "@constant";
import TextMoney from "./TextMoney";
import reactotron from "reactotron-react-native";
import { CheckBox } from "react-native-elements";
export class PaymentDetail extends Component {
  render() {
    const { totalPrice, usePoint, paymentType } = this.props;
    return (
      <View
        style={{
          flex: 1,
          backgroundColor: theme.colors.white,
          marginTop: 15
        }}
      >
        <View
          style={{
            flexDirection: "row",
            padding: 10,
            paddingHorizontal: 9,
            paddingLeft: 6,
            backgroundColor: theme.colors.defaultBg,
            alignItems: "center"
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              color: theme.colors.textColor,
              flex: 1
            }}
          >
            {R.strings().payment_info}
          </Text>
          {!!paymentType && (
            <View style={styles.buttonPayment}>
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_medium,
                  fontSize: 14,
                  color: theme.colors.organePayment
                }}
              >
                {paymentType == PAYMENT_METHOD.VNPAY
                  ? R.strings().cashed
                  : R.strings().cash}
              </Text>
            </View>
          )}
        </View>
        {/* <RowLableInfo
          lable={R.strings().money_service}
          title="0đ"
          color={theme.colors.textColor}
          borderBottom
        /> */}
        <View
          style={{
            flexDirection: "row",
            padding: 10,
            backgroundColor: theme.colors.white
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              color: theme.colors.textColor,
              flex: 1
            }}
          >
            {R.strings().total_money}
          </Text>
          <TextMoney style={{ color: colors.primary }} value={totalPrice} />
          {/* {!!usePoint && (
            <Text
              style={{
                position: "absolute",
                right: 13,
                bottom: 0,
                fontFamily: R.fonts.quicksand_medium,
                fontSize: 10,
                color: theme.colors.red
              }}
              children={`-${formatMoney(usePoint)}đ`}
            />
          )} */}
        </View>
      </View>
    );
  }
}
export class CarStatusUpdate extends Component {
  render() {
    return (
      <View
        style={{
          flex: 1,
          paddingBottom: 10,
          marginTop: 15
        }}
      >
        <View
          style={{
            flexDirection: "row",
            padding: 10,
            paddingBottom: 0,
            backgroundColor: theme.colors.defaultBg,
            alignItems: "center"
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_bold,
              fontSize: 14,
              color: theme.colors.textColor,
              flex: 1
            }}
          >
            {R.strings().update_car_status}
          </Text>
        </View>
      </View>
    );
  }
}

export class ServicePack extends Component {
  render() {
    const { supportPack = [] } = this.props;
    return (
      <View style={styles.containerServicePack}>
        {supportPack.map((item, index) => (
          <RowLableInfo
            lable={item.name || item.serviceName || item.cateName}
            title={`${item.price || item.servicePrice} đ`}
            color={theme.colors.textColor}
            borderBottom={index != supportPack.length - 1}
          />
        ))}
      </View>
    );
  }
}
export class Promotion extends Component {
  state = {
    usePoint: ""
  };
  render() {
    const {
      edit,
      vatCheck,
      onVatCheck,
      toggleSwitch,
      onPressVoucher,
      yourPoint = 0,
      yourPointHave = 0,
      code,
      priceDiscout = 0,
      baseMoney,
      value = "",
      onChangeTextDiscount,
      resetCouponCode,
      onPressVisitationRequest,
      onPressRequest,
      requestService,
      noteForMaster,
      priceDiscoutType
    } = this.props;
    return (
      <>
        <View style={styles.containerPromotion}>
          <View style={styles.promotion}>
            <Text style={styles.textPromotion}>{R.strings().promotion}</Text>
            {!edit ? (
              <View style={{ flexDirection: "row" }}>
                <Text style={styles.textPrice}>{`${formatMoney(
                  priceDiscout
                )} đ`}</Text>
                {!!code && <Text style={styles.code}>({code})</Text>}
              </View>
            ) : (
              <View>
                <View style={{ flexDirection: "row", alignItems: "center" }}>
                  <TextInput
                    textAlignVertical="center"
                    style={styles.textInput}
                    value={value}
                    maxLength={256}
                    placeholderTextColor={colors.gray}
                    placeholder={R.strings().choose_your_vourcher_or_code}
                    onChangeText={onChangeTextDiscount}
                  />

                  {this.props.value.length > 0 && (
                    <TouchableOpacity
                      onPress={resetCouponCode}
                      style={{ paddingHorizontal: 5 }}
                      children={<Icon.MaterialIcons name="close" size={24} />}
                    />
                  )}
                  <TouchableOpacity
                    onPress={onPressVoucher}
                    style={{ paddingHorizontal: 5 }}
                    children={
                      <Icon.MaterialIcons
                        name="keyboard-arrow-right"
                        size={30}
                      />
                    }
                  />
                </View>
              </View>
            )}
          </View>
          {priceDiscout > 0 && edit && (
            <>
              <View
                style={{
                  height: 1,
                  width: "95%",
                  backgroundColor: colors.grayBorder,
                  alignSelf: "center"
                }}
              />
              <View
                style={{
                  flexDirection: "row",
                  marginHorizontal: 10,
                  marginVertical: 10
                }}
              >
                <Text style={styles.text_discount}>{R.strings().discount}</Text>
                <Text style={styles.text_discount_money}>
                  {priceDiscoutType == TYPE_DISCOUNT.NORMAL
                    ? `- ${formatMoney(priceDiscout)} đ `
                    : `- ${formatMoney(
                        (baseMoney * parseInt(priceDiscout)) / 100
                      )} đ`}
                </Text>
              </View>
            </>
          )}
          <View
            style={{
              marginHorizontal: 9,
              height: 1,
              backgroundColor: theme.colors.grayBorder
            }}
          />

          {yourPointHave + yourPoint != 0 && (
            <>
              <View
                style={{
                  flexDirection: "row",
                  padding: 10,
                  backgroundColor: theme.colors.white,
                  alignItems: "center"
                }}
              >
                <Text
                  style={{
                    fontFamily: R.fonts.quicksand_medium,
                    fontSize: 14,
                    color: theme.colors.textColor,
                    flex: 1
                  }}
                >
                  {R.strings().use_point}
                </Text>
                <TextInput
                  onBlur={e => {
                    const text = e.nativeEvent.text.split(",").join("");
                    this.setState({
                      usePoint:
                        text == 0
                          ? ""
                          : formatMoney(text) +
                            "/" +
                            formatMoney(yourPoint) +
                            " " +
                            R.strings().point
                    });
                  }}
                  onFocus={() => {
                    this.setState({
                      usePoint: this.state.usePoint.split("/")[0]
                    });
                  }}
                  style={[styles.usePoint, { marginRight: edit ? 5 : 0 }]}
                  placeholder={
                    "0/" +
                    formatMoney(yourPoint.toString()) +
                    " " +
                    R.strings().point
                  }
                  keyboardType="number-pad"
                  value={this.state.usePoint}
                  maxLength={10}
                  onChangeText={text => {
                    if (
                      parseInt(text.split(",").join("") || 0) <
                      parseInt(yourPoint)
                    ) {
                      toggleSwitch(text.split(",").join(""));
                      this.setState({
                        usePoint: text && formatMoney(text.split(",").join(""))
                      });
                    } else {
                      toggleSwitch(yourPoint);
                      this.setState({
                        usePoint: formatMoney(yourPoint)
                      });
                    }
                  }}
                />
                {/* {yourPointHave != 0 && isCheckSwitch && (
                  <TextInput
                    style={[
                      styles.usePoint,
                      {
                        marginRight: edit ? 5 : 0,
                        fontSize: 10
                      }
                    ]}
                    value={`(- ${numberWithCommas(yourPointHave.toString())})`}
                  />
                )} */}
                {/* {edit && (
                  <ToggleSwitch
                    isOn={isCheckSwitch}
                    onColor={theme.colors.active}
                    offColor={theme.colors.gray}
                    onToggle={isOn => toggleSwitch(isOn)}
                  />
                )} */}
              </View>
              <View
                style={{
                  marginHorizontal: 9,
                  height: 1,
                  backgroundColor: theme.colors.grayBorder
                }}
              />
            </>
          )}

          {edit ? (
            <>
              <TouchableOpacity
                onPress={onPressRequest}
                style={[
                  styles.promotion,
                  { alignItems: "center", padding: 0, paddingHorizontal: 10 }
                ]}
              >
                <Text style={styles.textPromotion}>{R.strings().request}</Text>
                <Text
                  style={styles.note}
                  children={
                    requestService ||
                    R.strings().select + " " + R.strings().request.toLowerCase()
                  }
                />
              </TouchableOpacity>
              <View
                style={{
                  marginHorizontal: 9,
                  height: 1,
                  backgroundColor: theme.colors.grayBorder
                }}
              />
              <TouchableOpacity
                onPress={onPressVisitationRequest}
                style={[
                  styles.promotion,
                  { alignItems: "center", padding: 5, paddingHorizontal: 10 }
                ]}
              >
                <Text style={styles.textPromotion}>
                  {R.strings().visitation_request}
                </Text>
                <Text
                  style={styles.note}
                  children={
                    noteForMaster || R.strings().choose_your_visit_request
                  }
                />
              </TouchableOpacity>
              <View
                style={{
                  marginHorizontal: 9,
                  height: 1,
                  backgroundColor: theme.colors.grayBorder
                }}
              />
              <View
                style={{
                  backgroundColor: colors.backgroundColor
                }}
                children={
                  <>
                    <CheckBox
                      fontFamily={R.fonts.quicksand_medium}
                      checked={vatCheck}
                      onPress={onVatCheck}
                      containerStyle={{ padding: 0 }}
                      title="Lấy hóa đơn VAT"
                      titleProps={{
                        style: {
                          color: colors.primary,
                          fontFamily: R.fonts.quicksand_medium,
                          fontSize: 14,
                          marginStart: 10
                        }
                      }}
                      wrapperStyle={{
                        backgroundColor: colors.backgroundColor,
                        padding: 5,
                        borderColor: colors.backgroundColor
                      }}
                    />
                    {vatCheck && (
                      <>
                        <Text
                          style={{
                            marginStart: 10,
                            fontFamily: R.fonts.quicksand_bold
                          }}
                          children="Thông tin hóa đơn"
                        />
                        <TextInput
                          style={{
                            margin: 10,
                            backgroundColor: "white",
                            padding: 15,
                            borderRadius: 5,
                            borderWidth: 0.25,
                            fontFamily: R.fonts.quicksand_medium
                          }}
                          placeholder="Tên công ty"
                        />
                        <TextInput
                          style={{
                            margin: 10,
                            backgroundColor: "white",
                            padding: 15,
                            borderRadius: 5,
                            borderWidth: 0.25,
                            fontFamily: R.fonts.quicksand_medium
                          }}
                          placeholder="Mã số thuế"
                        />
                      </>
                    )}
                  </>
                }
              />
            </>
          ) : (
            <>
              <RowLableInfoText
                lable={R.strings().request}
                title={requestService}
                color={theme.colors.textColor}
                borderBottom
              />

              <RowLableInfoText
                lable={R.strings().note}
                title={noteForMaster}
                color={theme.colors.textColor}
              />
            </>
          )}
        </View>
      </>
    );
  }
}

const styles = StyleSheet.create({
  note: {
    width: theme.dimension.width / 1.7,
    fontFamily: R.fonts.quicksand_regular,
    fontSize: 14,
    textAlign: "right",
    padding: 10,
    color: colors.gray
  },
  containerServicePack: {
    flex: 1,
    backgroundColor: theme.colors.white
  },
  pricePack: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.textColor
  },
  packageName: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.textColor,
    flex: 1
  },
  choosePackService: {
    flexDirection: "row",
    padding: 10,
    paddingVertical: 12,
    backgroundColor: theme.colors.white
  },
  switch: {
    marginLeft: 5,
    paddingVertical: 10
  },
  textInput: {
    // flex: 1,
    // paddingHorizontal: 5,
    fontSize: 12,
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.gray,
    textAlign: "center",
    borderRadius: 25,
    borderColor: theme.colors.gray,
    borderWidth: 0.25,
    padding: Platform.OS != "ios" ? 0 : 8,
    width: width / 2
  },
  buttonPayment: {
    borderColor: theme.colors.organePayment,
    borderRadius: 10,
    borderWidth: 1,
    padding: 4,
    paddingHorizontal: 10,
    backgroundColor: theme.colors.white
  },
  containerPromotion: {
    flex: 1,
    backgroundColor: theme.colors.white,
    marginTop: 15
  },
  promotion: {
    flexDirection: "row",
    padding: 10,
    backgroundColor: theme.colors.white,
    alignItems: "center"
  },
  textPromotion: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.textColor,
    flex: 1
  },
  textPrice: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.textColor
  },
  code: {
    position: "absolute",
    right: 0,
    bottom: -10,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 10,
    color: theme.colors.green
  },
  chooseCode: {
    // flexDirection: "row",
    borderRadius: 25,
    borderColor: theme.colors.gray,
    borderWidth: 0.25,
    backgroundColor: theme.colors.backgroundInput,
    width: width / 2,
    paddingHorizontal: 10,
    paddingVertical: 5
  },
  usePoint: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.textColor,
    marginRight: 0
  },
  text_discount: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: colors.primaryDark,
    textAlignVertical: "center",
    flex: 1
  },
  text_discount_money: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: colors.primaryDark,
    textAlign: "right",
    textAlignVertical: "center",
    flex: 1
  }
});

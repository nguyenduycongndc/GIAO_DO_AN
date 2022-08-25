import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  Image,
  TouchableOpacity,
  TextInput,
  Switch
} from "react-native";
import R from "@app/assets/R";
import theme from "@theme";
import I18n from "@i18";
import { RowLableInfo } from "@app/components/FormRow";
import Icon from "@app/components/Icon";
import ToggleSwitch from "toggle-switch-react-native";
import { numberWithCommas, typePayment } from "@app/constants/Functions";
export class PaymentDetail extends Component {
  render() {
    const { paymentType, totalPrice, amountService } = this.props;
    return (
      <View style={styles.container_payment}>
        <View style={styles.box_payment}>
          <Text style={styles.payment_info}>{R.strings().payment_info}</Text>
          <View style={styles.buttonPayment}>
            <Text style={styles.text_payment}>{typePayment(paymentType)}</Text>
          </View>
        </View>
        <RowLableInfo
          lable={R.strings().amount_of_service}
          title={numberWithCommas(`${amountService}đ`)}
          color={theme.colors.textColor}
          borderBottom
        />
        <TouchableOpacity style={styles.btn_total}>
          <Text style={styles.text_total}>{R.strings().total_price}</Text>
          <Text style={styles.text_price}>
            {numberWithCommas(`${totalPrice}đ`)}
          </Text>
          <Text style={styles.reality_pay}>(+850.000đ)</Text>
        </TouchableOpacity>
      </View>
    );
  }
}
export class ServicePack extends Component {
  render() {
    const { edit, onPress, mainPack, supportPack } = this.props;
    return (
      <View style={styles.containerServicePack}>
        {/* <TouchableOpacity
          style={styles.choosePackService}
          onPress={onPress}
          disabled={!edit}
        >
          <Text style={styles.packageName}>
            {mainPack.cateName ? mainPack.cateName : mainPack.serviceName}
          </Text>
          <Text
            style={styles.pricePack}
            children={`${numberWithCommas(
              `${mainPack.price ? mainPack.price : mainPack.servicePrice}`
            )}đ`}
          />
          {edit && <Icon.MaterialIcons name="keyboard-arrow-right" size={18} />}
        </TouchableOpacity>
        <View
          style={{
            marginHorizontal: 9,
            height: 0.5,
            backgroundColor: theme.colors.grayBorder
          }}
        /> */}
        {supportPack.map((item, index) => (
          <RowLableInfo
            lable={item?.name}
            title={`${numberWithCommas(`${item?.price}`)}đ`}
            color={theme.colors.textColor}
            borderBottom={index == supportPack.length - 1 ? false : true}
          />
        ))}
      </View>
    );
  }
}
export class Promotion extends Component {
  render() {
    const { edit, isCheckSwitch, toggleSwitch } = this.props;

    return (
      <View
        style={{
          flex: 1,
          paddingBottom: 10,
          backgroundColor: theme.colors.white,
          marginTop: 15
        }}
      >
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
            Khuyến mãi
          </Text>
          {!edit ? (
            <View style={{ flexDirection: "row" }}>
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_medium,
                  fontSize: 14,
                  color: theme.colors.textColor
                }}
              >
                1500000đ
              </Text>
              <Text
                style={{
                  position: "absolute",
                  right: 0,
                  bottom: -10,
                  fontFamily: R.fonts.quicksand_medium,
                  fontSize: 10,
                  color: theme.colors.green
                }}
              >
                (Gói kim cương)
              </Text>
            </View>
          ) : (
            <TouchableOpacity
              style={{ flexDirection: "row", alignItems: "center" }}
            >
              <View
                style={{
                  flexDirection: "row",
                  borderRadius: 25,
                  backgroundColor: theme.colors.defaultBg,
                  width: 200
                }}
              >
                <Text style={styles.textInput}>Chọn vorcher, hoặc nhập mã</Text>
              </View>
              <Icon.MaterialIcons name="keyboard-arrow-right" size={18} />
            </TouchableOpacity>
          )}
        </View>
        <View
          style={{
            marginHorizontal: 5,
            height: 0.25,
            backgroundColor: theme.colors.gray
          }}
        />
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
            Dùng điểm
          </Text>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              color: theme.colors.textColor,
              marginRight: 5
            }}
          >
            120000đ
          </Text>
          {edit && (
            <ToggleSwitch
              isOn={isCheckSwitch}
              onColor={theme.colors.active}
              offColor={theme.colors.gray}
              size="small"
              onToggle={isOn => toggleSwitch}
            />
          )}
        </View>
        <View
          style={{
            marginHorizontal: 5,
            height: 0.25,
            backgroundColor: theme.colors.gray
          }}
        />
        <RowLableInfo
          lable="Thêm ghi chú"
          title="120000đ"
          color={theme.colors.textColor}
        />
      </View>
    );
  }
}

const styles = StyleSheet.create({
  reality_pay: {
    position: "absolute",
    right: 13,
    bottom: 1,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 10,
    color: theme.colors.red
  },
  text_price: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.primary
  },
  text_total: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.textColor,
    flex: 1
  },
  btn_total: {
    flexDirection: "row",
    padding: 10,
    backgroundColor: theme.colors.white
  },
  text_payment: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: theme.colors.orange
  },
  payment_info: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: theme.colors.textColor,
    flex: 1
  },
  box_payment: {
    flexDirection: "row",
    padding: 10,
    backgroundColor: theme.colors.defaultBg,
    alignItems: "center"
  },
  container_payment: {
    flex: 1,
    paddingBottom: 10,
    backgroundColor: theme.colors.white,
    marginTop: 15
  },
  textInput: {
    flex: 1,
    padding: 5,
    fontSize: 12,
    fontFamily: R.fonts.quicksand_medium
  },
  buttonPayment: {
    borderColor: theme.colors.orange,
    borderRadius: 10,
    borderWidth: 1,
    padding: 4,
    paddingHorizontal: 10,
    backgroundColor: theme.colors.white
  },
  containerServicePack: {
    flex: 1,
    paddingBottom: 5,
    backgroundColor: theme.colors.white
  },
  containerServicePack: {
    flex: 1,
    paddingBottom: 5,
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
  }
});

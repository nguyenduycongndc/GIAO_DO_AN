import React, { Component } from "react";
import { View, Text, NativeModules, StyleSheet } from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import TextInputMoney from "@app/components/TextInputMoney";
import Button from "@app/components/Button";
import theme from "@app/constants/Theme";
import ButtonPrimary from "@app/components/ButtonPrimary";
import CallApiHelper from "@app/utils/CallApiHelper";
import * as API from "@api";
import { TRANSFER_TYPE } from "@app/constants/Constants";
import { showMessages } from "@app/components/Alert";
import reactotron from "reactotron-react-native";
import { Linking } from "react-native";
const { SDKMerchantModule } = NativeModules;
export class RechargeScreen extends Component {
  state = {
    amount: 0,
    isLoadingGetData: true,
    isLoading: false,
    listConfig: null,
    error: null
  };
  onTopUp = () => {
    const { amount, listConfig } = this.state;
    if (
      Number(this.state.amount.replace(/[^0-9\.]+/g, "")) <
      parseInt(listConfig.minPointWithdraw)
    ) {
      showMessages(
        R.strings().notice,
        `${R.strings().top_up_min}: ${this.formatMoney(
          listConfig.minPointWithdraw
        )} đ`
      );
      return;
    }
    const payload = {
      point: Number(this.state.amount.replace(/[^0-9\.]+/g, "")),
      type: TRANSFER_TYPE.VNPAY
    };
    CallApiHelper(API.geRechargeWallet, payload, this, res => {
      CallApiHelper(API.GetUrlVNPay, res.result, this, res => {
        // SDKMerchantModule.openMerchantModule(res.result);
        Linking.openURL(res.result);
      });
    });
  };

  formatMoney(amount, decimalCount = 2, decimal = ".", thousands = ",") {
    try {
      decimalCount = Math.abs(decimalCount);
      decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

      const negativeSign = amount < 0 ? "-" : "";

      let i = parseInt(
        (amount = Math.abs(Number(amount) || 0).toFixed(decimalCount))
      ).toString();
      let j = i.length > 3 ? i.length % 3 : 0;

      let result =
        negativeSign +
        (j ? i.substr(0, j) + thousands : "") +
        i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) +
        (decimalCount
          ? decimal +
            Math.abs(amount - i)
              .toFixed(decimalCount)
              .slice(2)
          : "");
      return result.substr(0, result.indexOf("."));
    } catch (e) {
      console.log(e);
    }
  }
  render() {
    const {
      amount,
      isLoading,
      isLoadingGetData,
      error,
      listConfig
    } = this.state;
    return (
      <ScreenComponent
        titleHeader={R.strings().top_up}
        back
        isError={error}
        isLoading={isLoadingGetData}
        dialogLoading={isLoading}
        renderView={
          !isLoadingGetData && (
            <>
              <TextInputMoney
                value={amount}
                label={R.strings().please_enter_the_amount_to_top_up}
                onChangeText={text =>
                  this.setState({ ...this.setState, amount: text })
                }
              />
              <Text
                style={styles.text_note}
                children={`(*) ${R.strings().top_up_min}: ${this.formatMoney(
                  listConfig.minPointWithdraw
                )} đ`}
              />
              <View
                style={styles.root_button}
                children={
                  <ButtonPrimary
                    disabled={!amount}
                    onPress={this.onTopUp}
                    text={R.strings().confirm}
                  />
                }
              />
            </>
          )
        }
      />
    );
  }

  getData = async () => {
    try {
      const res = await API.getConfig();
      this.setState({ isLoadingGetData: false, listConfig: res.result });
    } catch (error) {
      this.setState({ isLoadingGetData: false, error: error });
    }
  };

  componentDidMount() {
    this.getData();
    // NativeModule.SDKMerchantModule.openMerchantModule("");
  }
}

const styles = StyleSheet.create({
  root_button: {
    position: "absolute",
    bottom: 20,
    width: "95%",
    alignSelf: "center"
  },
  text_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12,
    color: theme.colors.red,
    padding: 10
  }
});

const mapStateToProps = state => ({});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(RechargeScreen);

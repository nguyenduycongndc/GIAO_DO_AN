import React, { Component } from "react";
import { View, Text, StyleSheet, Alert } from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import TextRowMoney from "@app/components/TextRowMoney";
import TextInputMoney from "@app/components/TextInputMoney";
import Button from "@app/components/Button";
import ButtonPrimary from "@app/components/ButtonPrimary";
import ModalAlert from "@app/components/ModalAlert";
import R from "@app/assets/R";
import { transferMoney, getUserInfo } from "@app/redux/actions";
import { TRANSFER_TYPE } from "@app/constants/Constants";
import reactotron from "reactotron-react-native";
import CallApiHelper from "@app/utils/CallApiHelper";
import * as API from "@app/constants/Api";
import { showMessages } from "@app/components/Alert";
import NavigationUtil from "@app/navigation/NavigationUtil";
import theme, { colors } from "@app/constants/Theme";

export class TransfersScreen extends Component {
  initState = {
    modalVisiable: false,
    point: "",
    isLoading: false,
    isLoadingGetData: true,
    listConfig: null,
    error: null
  };
  state = this.initState;
  transfers = () => {
    const { point } = this.state;
    if (point == "") {
      Alert.alert(R.strings().notice, "Vui lòng nhập số điểm cần rút");
    } else if (point <= 0) {
      Alert.alert(R.strings().notice, "Vui lòng nhập số điểm cần rút");
    } else {
      CallApiHelper(
        API.geRechargeWallet,
        {
          point: Number(point.replace(/[^0-9\.]+/g, "")),
          type: TRANSFER_TYPE.CASH
        },
        this,
        res => {
          this.props.getUserInfo();
          NavigationUtil.goBack();
        }
      );
      // this.props.transferMoney();
    }
  };
  closeModal = () => this.setState({ modalVisiable: false });
  showModal = () => this.setState({ modalVisiable: true });
  render() {
    const {
      modalVisiable,
      point,
      isLoading,
      isLoadingGetData,
      error,
      listConfig
    } = this.state;
    const { data } = this.props.UserInfoState;
    return (
      <ScreenComponent
        isLoading={isLoadingGetData || this.props.UserInfoState.isLoading}
        isError={this.props.UserInfoState.error || error}
        back
        dialogLoading={isLoading}
        titleHeader={R.strings().money_transfer}
        renderView={
          !isLoadingGetData && (
            <>
              <ModalAlert
                isVisible={modalVisiable}
                onClose={this.closeModal}
                onModalHide={() => {
                  if (this.state.isSendRequest) this.transfers();
                }}
                onSubmit={() => {
                  this.setState({ modalVisiable: false, isSendRequest: true });
                }}
                title={R.strings().confirm_money_transfer}
              />
              <TextRowMoney
                label={R.strings().income_wallet_balance}
                value={data.withdrawPoint}
              />
              <TextRowMoney
                label={R.strings().deposit_wallet_balance}
                value={data.point}
              />
              <TextInputMoney
                label={R.strings().amount_money_transfer_to_deposit_wallet}
                value={point}
                onChangeText={text =>
                  this.setState({ ...this.setState, point: text })
                }
              />
              <Text
                style={styles.text_note}
                children={`(*) ${R.strings().withdraw_min}: ${this.formatMoney(
                  listConfig.minPointWithdraw
                )} đ`}
              />
              <View
                style={styles.root_button}
                children={
                  <ButtonPrimary
                    onPress={() => {
                      if (
                        Number(this.state.point.replace(/[^0-9\.]+/g, "")) <
                        parseInt(listConfig.minPointWithdraw)
                      ) {
                        showMessages(
                          R.strings().notice,
                          `${R.strings().transfer_min}: ${this.formatMoney(
                            listConfig.minPointWithdraw
                          )} đ`
                        );
                        return;
                      }
                      this.showModal();
                    }}
                    text={R.strings().update}
                  />
                }
              />
            </>
          )
        }
      />
    );
  }

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

  componentDidMount() {
    this.getData();
  }

  getData = async () => {
    try {
      const res = await API.getConfig();
      this.setState({ isLoadingGetData: false, listConfig: res.result });
    } catch (error) {
      this.setState({ isLoadingGetData: false, error: error });
    }
  };
}

const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  transferMoney,
  getUserInfo
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(TransfersScreen);

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
    padding: 10,
    paddingBottom: 0
  }
});

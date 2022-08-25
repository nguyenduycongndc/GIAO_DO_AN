import React, { Component } from "react";
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Alert
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import ButtonPrimary from "@component/ButtonPrimary";
import theme, { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import ModalAlert from "@app/components/ModalAlert";
import TextRowMoney from "@app/components/TextRowMoney";
import TextInputMoney from "@app/components/TextInputMoney";
import BankAccount from "@app/components/BankAccount";
import * as API from "@app/constants/Api";
import CallApiHelper from "@app/utils/CallApiHelper";
import { numberWithCommas } from "@app/constants/Functions";
import reactotron from "reactotron-react-native";
import { showMessages } from "@app/components/Alert";
import { transferMoney, getUserInfo } from "@app/redux/actions";
export class WithdrawalScreen extends Component {
  initState = {
    modelVisiable: false,
    isSendRequest: false,
    select: 0,
    moneyWithdrawal: "",
    isLoading: true,
    dialogLoading: false,
    listConfig: [],
    error: null
  };
  state = this.initState;
  addBankAccount = () =>
    NavigationUtil.navigate(SCREEN_ROUTER_WASHER.ADD_BANK_ACCOUNT);
  selectBank = select => this.setState({ select });
  renderBankAccount = () =>
    this.props.UserInfoState.data.listBank.map((e, i) => (
      <BankAccount
        item={e}
        selected={this.state.select == i}
        onPress={() => this.selectBank(i)}
      />
    ));
  withdrawal = () => {
    const { moneyWithdrawal, listConfig } = this.state;
    if (!this.props.UserInfoState.data.listBank.length) {
      showMessages(R.strings().notice, R.strings().no_bank_selected);
      return;
    }
    if (
      moneyWithdrawal == "" ||
      Number(moneyWithdrawal.replace(/[^0-9\.]+/g, "")) <=
        parseInt(listConfig.minPointWithdraw)
    )
      showMessages(
        R.strings().notice,
        `${R.strings().withdraw_min}: ${this.formatMoney(
          listConfig.minPointWithdraw
        )} đ`
      );
    else this.setState({ modelVisiable: true });
  };
  componentDidMount() {
    this.getData();
  }
  getData = async () => {
    try {
      const res = await API.getConfig();
      this.setState({ isLoading: false, listConfig: res.result });
    } catch (error) {
      this.setState({ isLoading: false, error: error });
    }
  };
  withdrawalMoney = () => {
    const { select, moneyWithdrawal } = this.state;
    const { data } = this.props.UserInfoState;
    CallApiHelper(
      API.getWithdraw,
      {
        point: moneyWithdrawal.replace(/\,/g, ""),
        id: data.listBank[select].id
      },
      this,
      res => {
        this.props.getUserInfo();
        showMessages(
          R.strings().notice,
          R.strings().successful_require_withdrawal,
          NavigationUtil.goBack
        );
      },
      error => {}
    );
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
    const { data } = this.props.UserInfoState;
    const {
      modelVisiable,
      moneyWithdrawal,
      select,
      isLoading,
      error,
      listConfig
    } = this.state;

    return (
      <ScreenComponent
        dialogLoading={this.state.dialogLoading}
        back
        isLoading={isLoading}
        isError={error}
        reload={this.getData}
        titleHeader={R.strings().withdrawal}
        renderView={
          <>
            <ScrollView style={{ flex: 1, marginBottom: 140 }}>
              <ModalAlert
                title={R.strings().withdrawal_confirm}
                isVisible={modelVisiable}
                onModalHide={() => {
                  if (this.state.isSendRequest)
                    this.setState(
                      { isSendRequest: false, dialogLoading: true },
                      this.withdrawalMoney
                    );
                }}
                onSubmit={() => {
                  this.setState({ modelVisiable: false, isSendRequest: true });
                }}
                onClose={() => this.setState({ modelVisiable: false })}
                contentView={
                  <Text
                    children={
                      !!data.listBank.length
                        ? `${
                            R.strings().do_you_confirm_withdraw
                          } ${numberWithCommas(moneyWithdrawal)}đ ${
                            R.strings().to_account
                          } ${data.listBank[select].bankName} - ${
                            data.listBank[select].acount
                          } - ${data.listBank[select].acountOwner} ?`
                        : R.strings().list_bank_empty
                    }
                  />
                }
              />
              <TextRowMoney
                label={R.strings().wallet_balance}
                value={data.withdrawPoint}
              />
              <TextInputMoney
                label={R.strings().withdrawal_money}
                value={moneyWithdrawal}
                onChangeText={moneyWithdrawal =>
                  this.setState({
                    moneyWithdrawal: moneyWithdrawal?.replace(/\,/g, "")
                  })
                }
              />
              <Text
                style={styles.text_note}
                children={`(*) ${R.strings().withdraw_min}: ${this.formatMoney(
                  listConfig.minPointWithdraw
                )} đ`}
              />
              <Text
                children={R.strings().bank_account}
                style={styles.text_bank_accout}
              />
              {this.renderBankAccount()}
              <TouchableOpacity
                onPress={this.addBankAccount}
                children={
                  <Text
                    children={R.strings().other_account}
                    style={styles.text_other_account}
                  />
                }
              />
            </ScrollView>
            <View
              style={styles.root_withdrawal}
              children={
                <>
                  {/* <Text
                    style={styles.text_note}
                    children={R.strings().note_withdraw}
                  /> */}
                  <ButtonPrimary
                    onPress={this.withdrawal}
                    text={R.strings().withdrawal.toUpperCase()}
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

const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfo
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(WithdrawalScreen);
const styles = StyleSheet.create({
  root_withdrawal: {
    // backgroundColor: colors.white,
    position: "absolute",
    width: "100%",
    bottom: 20
  },
  text_other_account: {
    backgroundColor: colors.white,
    color: colors.primary,
    borderColor: colors.primary,
    borderWidth: 0.5,
    borderRadius: 5,
    marginHorizontal: 10,
    textAlign: "center",
    paddingVertical: 15,
    marginTop: 5,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 13
  },
  text_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12,
    color: colors.red,
    padding: 10,
    paddingBottom: 0
  },
  text_bank_accout: {
    marginHorizontal: 10,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginTop: 10
  },
  text_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12,
    color: colors.red,
    padding: 10
  }
});

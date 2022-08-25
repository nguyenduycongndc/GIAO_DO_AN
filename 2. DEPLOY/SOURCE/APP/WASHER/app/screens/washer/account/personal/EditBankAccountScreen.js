import React, { Component } from "react";
import { ScrollView, Text, StyleSheet, View } from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import { colors } from "@app/constants/Theme";
import R from "@app/assets/R";
import ButtonPrimary from "@app/components/ButtonPrimary";
import BankAccount from "@app/components/BankAccount";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import * as API from "@app/constants/Api";
import CallApiHelper from "@app/utils/CallApiHelper";
import { getUserInfo,deleteBank } from "@app/redux/actions";
export class EditBankAccountScreen extends Component {
  initState = {};
  state = this.initState;
  renderBankAccount = () => {
    const { data } = this.props.UserInfoState;
    console.log(data.listBank);
    return (
      <View>
        {data.listBank?.map(item => (
          <BankAccount item={item} onPressDelete={e => this.deleteBank(e.id)} />
        ))}
      </View>
    );
  };
  deleteBank = id => {
    this.props.deleteBank(id)
  };
  render() {
    return (
      <ScreenComponent
        dialogLoading={this.props.UserInfoState.isLoading}
        // isLoading={this.state.isLoading || this.props.UserInfoState.isLoading}
        isError={this.props.UserInfoState.error}
        reload={()=>this.props.getUserInfo()}
        titleHeader={`${
          R.strings().edit
        } ${R.strings().bank_account.toLowerCase()}`}
        back
        renderView={
          <ScrollView>
            <Text
              style={styles.header_text}
              children={R.strings().bank_account.toUpperCase()}
            />
            {this.renderBankAccount()}
            <ButtonPrimary
              onPress={() =>
                NavigationUtil.navigate(
                  SCREEN_ROUTER_WASHER.ADD_BANK_ACCOUNT,
                  this.props.UserInfoState
                )
              }
              text={R.strings().add_account}
              style={styles.button_add_account}
            />
          </ScrollView>
        }
      />
    );
  }
}

const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfo,
  deleteBank
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(EditBankAccountScreen);
const styles = StyleSheet.create({
  header_text: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: colors.primaryDark,
    marginHorizontal: 10,
    marginTop: 20
  },

  button_add_account: {
    backgroundColor: colors.white,
    color: colors.primary,
    borderColor: colors.primary,
    borderWidth: 0.5
  }
});

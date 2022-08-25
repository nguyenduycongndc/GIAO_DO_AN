import React, { Component } from "react";
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  KeyboardAvoidingView,
  ScrollView,
  Alert
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import Dropdown from "@app/components/ModalDropdown";
import { colors } from "@app/constants/Theme";
import ButtonPrimary from "@app/components/ButtonPrimary";
import TextInputInfo from "@app/components/TextInputInfo";
import * as API from "@app/constants/Api";
import CallApiHelper from "@app/utils/CallApiHelper";
import { createBank, getUserInfo } from "@app/redux/actions";
import NavigationUtil from "@app/navigation/NavigationUtil";

export class AddBankAccountScreen extends Component {
  initState = {
    bankID: "",
    acount: "",
    acountOwner: "",
    listBank: []
  };
  state = this.initState;
  componentDidMount() {
    this.getData();
  }
  createBank() {}
  getData() {
    CallApiHelper(
      API.getListBank,
      {},
      this,
      res => {
        this.setState({
          ...this.setState,
          listBank: res.result
        });
      },
      error => {}
    );
  }
  bankName(arr) {
    let str = [];
    arr.forEach(element => {
      str.push(element.bankName);
    });
    return str;
  }
  renderDropdown = (label, list) => (
    <>
      <Text style={styles.text_label} children={label} />
      <View>
        <Dropdown
          data={list}
          style={styles.root_dropdown}
          dropDownStyle={styles.dropDownStyle}
          renderRow={text => (
            <Text style={styles.text_dropdown} children={text} />
          )}
          imageStyle={styles.image_dropdown}
          defaultValue={`${R.strings().select} ${label.toLowerCase()}`}
          onSelect={(index, value) => {
            this.setState({
              ...this.state,
              bankID: this.state.listBank[index]?.bankID
            });
          }}
        />
      </View>
    </>
  );
  renderTextInput = (label, key, maxLength = 30) => (
    <>
      <Text style={styles.text_label} children={label} />
      <TextInput
        style={styles.text_input}
        maxLength={maxLength}
        autoCapitalize="characters"
        placeholder={`${R.strings().select} ${label.toLowerCase()}`}
        onChangeText={text => this.changeState(text, key)}
      />
    </>
  );
  changeState(text, key) {
    this.setState({
      ...this.state,
      [key]: text.toUpperCase()
    });
  }
  createBank = () => {
    const { acount, acountOwner, bankID } = this.state;
    let payload = {
      acount: acount,
      acountOwner: acountOwner,
      bankID: bankID
    };
    if (acount == "" || acountOwner == "" || bankID == "")
      Alert.alert(
        R.strings().notice,
        R.strings().please_complete_all_infomation
      );
    else {
      let res = this.props.createBank(payload);
      if (res) {
        NavigationUtil.goBack();
      }
    }
  };
  render() {
    const { listBank } = this.state;
    console.log(this.state);

    return (
      <ScreenComponent
        dialogLoading={this.props.UserInfoState.isLoading}
        // isLoading={this.state.isLoading}
        isError={this.props.UserInfoState.error}
        reload={() => this.props.getUserInfo()}
        back
        titleHeader={R.strings().add_bank_account}
        renderView={
          <>
            {this.renderDropdown(R.strings().bank, this.bankName(listBank))}
            {/* {this.renderDropdown(R.strings().branch, ["BIDV", "TECH", "TIMO"])} */}
            {this.renderTextInput(R.strings().account_number, "acount", 20)}
            {this.renderTextInput(
              R.strings().account_holder,
              "acountOwner",
              30
            )}
            <ButtonPrimary
              onPress={this.createBank}
              text={R.strings().add_account}
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
  createBank,
  getUserInfo
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(AddBankAccountScreen);
const styles = StyleSheet.create({
  image_dropdown: {
    top: 23,
    right: 20
  },
  text_dropdown: {
    textAlignVertical: "center",
    padding: 10
  },
  root_dropdown: {
    marginHorizontal: 10,
    width: "95%"
  },
  dropDownStyle: {
    width: "90%",
    marginTop: -20,
    borderRadius: 5
  },
  text_label: {
    marginHorizontal: 10,
    marginTop: 15,
    marginBottom: 5,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  text_input: {
    borderRadius: 5,
    marginTop: 5,
    padding: 10,
    backgroundColor: "#ECECEC",
    flexDirection: "row",
    margin: 10,
    color: colors.primaryDark,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    color: "black"
  },
  button_add_bank_account: {
    borderRadius: 5,
    paddingHorizontal: 10,
    paddingVertical: 15,
    margin: 10,
    backgroundColor: colors.primary,
    overflow: "hidden",
    textAlign: "center",
    color: colors.white,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginTop: 30
  }
});

import React, { Component } from "react";
import { StyleSheet, Alert } from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import ButtonPrimary from "@app/components/ButtonPrimary";
import TextInputInfo from "@app/components/TextInputInfo";
import CallApiHelper from "@app/utils/CallApiHelper";
import * as API from "@app/constants/Api";
import { connect } from "react-redux";
import DropdownAlertUtil from '@app/components/DropdownAlertUtil'
import NavigationUtil from "@app/navigation/NavigationUtil";

class ChangesPasswordScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      Phone: this.props.UserInfoState.data.phone,
      newPass: "",
      oldPass: "",
      retypePass: "",
      dialogLoading: false
    };
  }
  changePassWord() {
    const { Phone, newPass, oldPass } = this.state;
    let payload = {
      Phone: Phone,
      newPass: newPass,
      oldPass: oldPass
    };
    CallApiHelper(
      API.ChangePass,
      payload,
      this,
      res => {
        console.log("oke");
        Alert.alert(R.strings().notice, R.strings().successful_change_password);
        NavigationUtil.goBack();
      },
      error => {
        console.log(error);
      }
    );
  }
  render() {
    return (
      <ScreenComponent
        dialogLoading={this.state.isLoading}
        back
        titleHeader={R.strings().confirm_new_password}
        renderView={
          <>
            {this._renderTextInput(R.strings().old_password)}
            {this._renderTextInput(R.strings().new_password)}
            {this._renderTextInput(R.strings().confirm_new_password)}
            <ButtonPrimary
              text={R.strings().update}
              style={styles.button_update}
              onPress={() => this.checkAfterChange()}
            />
          </>
        }
      />
    );
  }
  _renderTextInput(label, placeholer) {
    let key =
      label == R.strings().old_password
        ? "oldPass"
        : label == R.strings().new_password
        ? "newPass"
        : "retypePass";
    return (
      <TextInputInfo
        label={label}
        secureTextEntry={true}
        onChangeText={text => this.setText(text, key)}
      />
    );
  }

  setText(text, key) {
    this.setState({
      ...this.setState,
      [key]: text
    });
  }
  checkAfterChange() {
    const { newPass, retypePass } = this.state;
    if (newPass != retypePass) alert(R.strings().password_not_match);
    else if (newPass.length < 6) alert(R.strings().password_must_be_at_least);
    else this.changePassWord();
  }
}

const styles = StyleSheet.create({
  button_update: { marginTop: 50 }
});
const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ChangesPasswordScreen);

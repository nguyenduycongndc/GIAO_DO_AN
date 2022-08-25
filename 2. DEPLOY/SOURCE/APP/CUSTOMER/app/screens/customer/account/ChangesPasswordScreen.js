import React, { Component } from "react";
import { Text, StyleSheet, View, Image, TextInput, Alert } from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import theme, { colors } from "@app/constants/Theme";
import Button from "@app/components/Button";
import CallApiHelper from "@app/utils/CallApiHelper";
import * as API from "@app/constants/Api";
import { connect } from "react-redux";
import DropdownAlertUtil from "@app/components/DropdownAlertUtil";
import TextInputInfo from "@app/components/TextInputInfo";
import { showMessages } from "@app/components/Alert";

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
    CallApiHelper(API.ChangePass, payload, this, res => {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().change_password_success
      );
    });
  }
  render() {
    return (
      <ScreenComponent
        dialogLoading={this.state.isLoading}
        back
        titleHeader={R.strings().change_password}
        renderView={
          <>
            <View
              style={{
                backgroundColor: theme.colors.white,
                flex: 1,
                paddingHorizontal: 8,
                paddingTop: 15
              }}
            >
              {this._renderTextInput(R.strings().old_password)}
              {this._renderTextInput(R.strings().new_password)}
              {this._renderTextInput(R.strings().confirm_new_password)}
              <Button
                uppercase
                title={R.strings().update}
                backgroundColor={theme.colors.primary}
                colorText={theme.colors.white}
                width={"100%"}
                buttonStyle={{ marginTop: 50 }}
                action={() => this.checkAfterChange()}
              />
            </View>
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
    else if (newPass.length < 6) alert("Mật khẩu tối thiểu phải có 6 ký tự.");
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

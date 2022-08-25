import { getUserInfoAction } from "@action";
// import DropdownMenu from 'react-native-dropdown-menu';
import * as API from "@api";
import R from "@app/assets/R";
import { showMessages } from "@app/components/Alert";
import Button from "@app/components/Button";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import React, { Component } from "react";
import {
  Dimensions,
  Keyboard,
  KeyboardAvoidingView,
  ScrollView,
  StyleSheet,
  Text,
  TextInput,
  View
} from "react-native";
// import Dropdown from "@app/components/Dropdown";
import DatePicker from "react-native-datepicker";
import { Dropdown } from "react-native-material-dropdown";
import { connect } from "react-redux";
import CallApiHelper from "../../../utils/CallApiHelper";
import { SCREEN_ROUTER_CUSTOMER } from "@app/constants/C";

var d = new Date();

var date = d.getDate();
var month = d.getMonth() + 1; // Since getMonth() returns month from 0-11 not 1-12
var year = d.getFullYear();

var dateStr = date + "/" + month + "/" + year;

var mailFormat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

export class UpdateUserInfoScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const item = navigation.getParam("item", {});
    this.state = {
      token: "",
      isLoading: false,
      error: "",
      text: "",
      provinceName: item.provinceName,
      districtName: item.districtName,
      isDialogLoading: false,
      payload: {
        userID: item.userID,
        name: item.name,
        phone: item.phone,
        email: item.email,
        dobStr: item.dobStr,
        address: item.address,
        referralCode: item.referralCode,
        sex: item.sex,
        role: item.role,
        point: item.point
      }
    };
  }

  updateUserInfo() {
    CallApiHelper(API.updateUserInfo, this.state.payload, this, res => {
      if (!!this.state.payload.referralCode)
        CallApiHelper(API.UpdateRefCode, this.state.payload.referralCode, this, () => {
          this.props.getUserInfoAction();
          NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.USER_INFO);
        }, null, null, 'isDialogLoading');
      else {
        this.props.getUserInfoAction();
        NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.USER_INFO);
      }
    }, null, null, 'isDialogLoading');



  }

  renderDropdown = (label, defVal, list) => {
    const value =
      this.state.payload.sex == 1 ? R.strings().man : R.strings().woman;
    const data = [
      {
        value: R.strings().man
      },
      {
        value: R.strings().woman
      }
    ];
    return (
      <View>
        <Text
          style={{
            fontFamily: R.fonts.quicksand_medium,
            fontSize: 14,
            paddingVertical: 10
          }}
          children={label}
        />
        <View style={styles.text_input}>
          <Dropdown
            inputContainerStyle={{
              borderBottomColor: "transparent",
              justifyContent: "center",
              fontSize: 14
            }}
            fontSize={14}
            value={value}
            data={data}
            containerStyle={{ width: "100%", marginRight: 5 }}
            baseColor={theme.colors.nameText}
            dropdownOffset={{ top: 15, left: 0 }}
            itemTextStyle={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14
            }}
            onChangeText={value => {
              if (value == R.strings().man)
                this.setState({
                  payload: {
                    ...this.state.payload,
                    sex: 1
                  }
                });
              else
                this.setState({
                  payload: {
                    ...this.state.payload,
                    sex: 0
                  }
                });
            }}
          />
        </View>
      </View>
    );
  };

  render() {
    const { payload } = this.state;
    return (
      <ScreenComponent
        back
        dialogLoading={this.state.isDialogLoading}
        titleHeader={R.strings().edit_user_info}
        renderView={
          <>
            <KeyboardAvoidingView
              keyboardVerticalOffset={80}
              behavior={Platform.OS == "ios" ? "padding" : undefined}
              style={{ flex: 1 }}
            >
              <ScrollView
                keyboardShouldPersistTaps="never"
                showsVerticalScrollIndicator={false}
                style={{ flex: 1, paddingHorizontal: 8 }}
              >
                {this._renderTextInput(
                  R.strings().full_name,
                  payload.name,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        name: text
                      }
                    });
                  },
                  () => {
                    this.secondTextInput.focus();
                  }
                )}
                {this._renderTextInput(
                  R.strings().date_of_birth,
                  payload.dobStr
                )}
                {this.renderDropdown(
                  R.strings().gender,
                  payload.sex == 1 ? R.strings().man : R.strings().woman
                )}
                {this._renderTextInput(
                  R.strings().referral_code,
                  payload.referralCode,
                  referralCode => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        referralCode
                      }
                    });
                  }
                )}
                {this._renderTextInput(
                  R.strings().number_phone,
                  payload.phone,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        phone: text
                      }
                    });
                  }
                )}
                {this._renderTextInput(
                  "Email",
                  payload.email,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        email: text.replace(/\s/g, "")
                      }
                    });
                  },
                  () => {
                    this.thirdTextInput.focus();
                  },
                  input => {
                    this.secondTextInput = input;
                  }
                )}
                {this._renderTextInput(
                  R.strings().address,
                  payload.address,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        address: text
                      }
                    });
                  },
                  () => {
                    Keyboard.dismiss();
                  },
                  input => {
                    this.thirdTextInput = input;
                  }
                )}
                <Button
                  action={this.checkValidate}
                  title={R.strings().update}
                  backgroundColor={theme.colors.primary}
                  colorText={theme.colors.white}
                  width={"100%"}
                  buttonStyle={{ marginTop: 10 }}
                />
              </ScrollView>
            </KeyboardAvoidingView>
          </>
        }
      />
    );
  }
  _renderTextInput(label, value, onChangeText, onSubmitEditing, ref) {
    return (
      <>
        <View
          style={{
            flexDirection: "row",
            alignItems: "center"
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              paddingVertical: 10
            }}
          >
            {label}
          </Text>
          {(label == R.strings().password ||
            label == R.strings().confirm_password ||
            label == R.strings().full_name ||
            label == "Email") && (
              <Text style={{ color: theme.colors.red }}>(*)</Text>
            )}
        </View>
        <View style={styles.text_input}>
          {label == R.strings().date_of_birth ? (
            <DatePicker
              style={styles.InputDate}
              date={this.state.payload.dobStr}
              mode="date"
              format="DD/MM/YYYY"
              minDate="01/01/1950"
              maxDate={dateStr}
              confirmBtnText={R.strings().confirm}
              cancelBtnText={R.strings().cancel}
              androidMode="spinner"
              customStyles={{
                dateInput: {
                  borderWidth: 0,
                  marginLeft: -windowWidth / 1.6
                }
              }}
              onDateChange={date => {
                this.setState({
                  payload: {
                    ...this.state.payload,
                    dobStr: date
                  }
                });
              }}
            />
          ) : (
              <TextInput
                value={value}
                style={{
                  width: "80%",
                  color: "black",
                  paddingVertical: 10
                }}
                placeholder={R.strings().not_update_yet}
                onChangeText={onChangeText}
                returnKeyType={"next"}
                ref={ref}
                placeholderTextColor={colors.gray}
                maxLength={256}
                onSubmitEditing={onSubmitEditing}
                editable={label != R.strings().number_phone ? true : false}
                blurOnSubmit={false}
              />
            )}
        </View>
      </>
    );
  }

  checkValidate = () => {
    const { payload } = this.state;
    const regexEmail = /^(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;

    if (!payload.name.trim())
      return showMessages(R.strings().notif_tab_cus, R.strings().input_name);
    if (!payload.email.trim())
      return showMessages(R.strings().notif_tab_cus, R.strings().input_email);
    if (!regexEmail.test(payload.email.trim()))
      return showMessages(
        R.strings().notif_tab_cus,
        R.strings().you_have_entered_an_invalid_email_address
      );
    this.updateUserInfo();
  };
}

const styles = StyleSheet.create({
  text_label: {
    marginHorizontal: 10,
    marginTop: 15,
    marginBottom: 5,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  text_input: {
    height: 46,
    backgroundColor: theme.colors.backgroundInput,
    borderRadius: 5,
    paddingHorizontal: 10,
    marginBottom: 15,
    flexDirection: "row",
    alignItems: "center",
    flex: 1
  },
  InputDate: {
    width: "100%",
    marginLeft: Platform.OS === "android" ? 10 : 0
  }
});

const mapStateToProps = state => ({
  lang: state.lang,
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfoAction
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(UpdateUserInfoScreen);

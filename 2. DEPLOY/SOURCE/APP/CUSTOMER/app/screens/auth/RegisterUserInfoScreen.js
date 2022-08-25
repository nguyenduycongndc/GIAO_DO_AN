import R from "@app/assets/R";
import Button from "@app/components/Button";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@app/constants/Theme";
import React, { Component } from "react";
import {
  StyleSheet,
  Text,
  TextInput,
  View,
  ScrollView,
  Dimensions,
  KeyboardAvoidingView,
  Keyboard
} from "react-native";
// import Dropdown from "@app/components/Dropdown";
import DatePicker from "react-native-datepicker";
// import DropdownMenu from 'react-native-dropdown-menu';
import { showMessages, showConfirm } from "@app/components/Alert";
import { Dropdown } from "react-native-material-dropdown";
// import DropdownMenu from 'react-native-dropdown-menu';
import * as API from "@api";
import CallApiHelper from "../../utils/CallApiHelper";
import NavigationUtil from "@app/navigation/NavigationUtil";
import {
  SCREEN_ROUTER_AUTH,
  ASYNC_STORAGE,
  SCREEN_ROUTER_CUSTOMER,
  TYPE_GET_OTP
} from "@constant";
import AsyncStorage from "@react-native-community/async-storage";
import { getUserInfoAction } from "@action";
import { connect } from "react-redux";
import analytics from '@react-native-firebase/analytics';

var d = new Date();

var day = ("0" + d.getDate()).slice(-2);
var month = ("0" + (d.getMonth() + 1)).slice(-2);
var year = d.getFullYear();

var dateStr = day + "/" + month + "/" + year;

var data = [
  {
    value: R.strings().man
  },
  {
    value: R.strings().woman
  }
];

var mailFormat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

export class RegisterUserInfoScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const token = navigation.getParam("token", {});
    const phone = navigation.getParam("phone", {});
    const type = navigation.getParam("type", {});
    this.state = {
      isLoading: false,
      isLoadingToken: true,
      error: "",
      text: "",
      type: type,
      confirm_password: "",
      payload: {
        token: token,
        name: "",
        phone: phone,
        email: "",
        dobStr: dateStr,
        address: "",
        sex: 0,
        passWord: ""
      }
    };
  }

  componentDidMount() {
    this.getToken();
  }

  getToken = async () => {
    try {
      const token = await AsyncStorage.getItem(ASYNC_STORAGE.TOKEN);
      if (token != null) {
        this.setState(
          {
            isLoadingToken: false
          },
          () => {
            this.getUserInfo();
          }
        );
      }
    } catch (error) {
      this.setState({
        isLoadingToken: false
      });
    }
  };

  getUserInfo() {
    CallApiHelper(
      API.getUserInfo,
      {},
      this,
      res => {
        this.setState({
          payload: {
            ...this.state.payload,
            name: res.result.name,
            phone: res.result.phone,
            sex: res.result.sex,
            address: res.result.address,
            email: res.result.email,
            dobStr: res.result.dobStr || dateStr
          }
        });
      },
      error => reactotron.log(error, "check error")
    );
  }

  updateUserInfo() {
    CallApiHelper(
      API.updateUserInfo,
      this.state.payload,
      this,
      async res => {
        analytics().logEvent('sign_up_success', res)
        this.setToken(res.result.token);
      },
      error => reactotron.log(error, "check error")
    );
  }

  setToken = async token => {
    await AsyncStorage.setItem(ASYNC_STORAGE.TOKEN, token.toString());
    NavigationUtil.navigate(SCREEN_ROUTER_AUTH.AUTH_LOADING);
  };

  renderDropdown = (label, defVal, list) => (
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
          value={
            this.state.payload.sex == 1 ? R.strings().man : R.strings().woman
          }
          data={data}
          containerStyle={{ width: "100%", marginRight: 5 }}
          baseColor={theme.colors.nameText}
          dropdownOffset={{ top: 15, left: 0 }}
          itemTextStyle={{ fontFamily: R.fonts.quicksand_medium, fontSize: 14 }}
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

  render() {
    const { payload, type } = this.state;
    const { UserInfoState } = this.props;
    return (
      <ScreenComponent
        back
        isLoading={this.state.isLoading || this.state.isLoadingToken}
        isError={this.state.error}
        reload={() => this.getToken()}
        titleHeader={R.strings().update_user_info}
        renderView={
          <>
            <KeyboardAvoidingView
              keyboardVerticalOffset={80}
              behavior={Platform.OS == "ios" ? "padding" : undefined}
              style={{ flex: 1 }}
            >
              <ScrollView
                showsVerticalScrollIndicator={false}
                keyboardShouldPersistTaps="handled"
                style={{ flex: 1, paddingHorizontal: 8 }}
              >
                {type == TYPE_GET_OTP.REGISTER && (
                  <Text
                    style={{
                      paddingVertical: 10,
                      textAlign: "center",
                      paddingHorizontal: 15,
                      fontFamily: R.quicksand_medium,
                      fontSize: 14
                    }}
                  >
                    {
                      R.strings()
                        .please_complete_the_information_to_complete_the_registration_process
                    }
                  </Text>
                )}
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
                    this.fourthTextInput.focus();
                  },
                  input => {
                    this.thirdTextInput = input;
                  }
                )}
                {this._renderTextInput(
                  R.strings().password,
                  payload.passWord,
                  text => {
                    this.setState({
                      payload: {
                        ...this.state.payload,
                        passWord: text
                      }
                    });
                  },
                  () => {
                    this.fifthTextInput.focus();
                  },
                  input => {
                    this.fourthTextInput = input;
                  }
                )}
                {this._renderTextInput(
                  R.strings().confirm_password,
                  this.state.confirm_password,
                  text => {
                    this.setState({
                      confirm_password: text
                    });
                  },
                  () => {
                    Keyboard.dismiss();
                  },
                  input => {
                    this.fifthTextInput = input;
                  }
                )}
                <Button
                  action={() => this.checkValidate()}
                  title={R.strings().update}
                  backgroundColor={theme.colors.primary}
                  colorText={theme.colors.white}
                  width={"100%"}
                  buttonStyle={{ marginTop: 50 }}
                />
              </ScrollView>
            </KeyboardAvoidingView>
          </>
        }
      />
    );
  }

  _checkPassword() {
    if (!this.state.payload.passWord) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().please_select_full_info
      );
    } else if (this.state.confirm_password != this.state.payload.passWord) {
      showMessages(R.strings().notif_tab_cus, R.strings().password_not_match);
    } else return this.updateUserInfo();
  }

  checkValidate() {
    const { payload } = this.state;
    if (
      !payload.name.trim() ||
      // !payload.email.trim() ||
      !payload.passWord.trim() ||
      !this.state.confirm_password.trim()
    ) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().please_select_full_info
      );
    } else if (!payload.email.match(mailFormat)) {
      showMessages(
        R.strings().notif_tab_cus,
        R.strings().you_have_entered_an_invalid_email_address
      );
    } else return this._checkPassword();
  }

  _renderTextInput(label, value, onChangeText, onSubmitEditing, ref) {
    return (
      <View>
        <View style={{ flexDirection: "row", alignItems: "center" }}>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              paddingVertical: 10,
              marginRight: 5
            }}
          >
            {label}
          </Text>
          {(label == R.strings().password ||
            label == R.strings().confirm_password ||
            label == R.strings().full_name) && (
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
              minDate="11/11/1980"
              maxDate={dateStr}
              confirmBtnText={R.strings().confirm}
              cancelBtnText={R.strings().cancel}
              androidMode="spinner"
              customStyles={{
                // dateIcon: {
                //   position: "absolute",
                //   left: 5,
                //   top: 8,
                //   marginLeft: 0,
                //   marginRight: 15,
                //   borderWidth: 0,
                //   height: 25,
                //   width: 25
                // },
                dateInput: {
                  borderWidth: 0,
                  marginLeft: -windowWidth / 1.6
                }
                // ... You can check the source to find the other keys.
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
              secureTextEntry={
                label == R.strings().password ||
                label == R.strings().confirm_password
                  ? true
                  : false
              }
              value={value}
              placeholder={R.strings().not_update_yet}
              style={{ width: "80%", color: "black", paddingVertical: 10 }}
              onChangeText={onChangeText}
              placeholderTextColor={colors.gray}
              editable={label != R.strings().number_phone ? true : false}
              ref={ref}
              maxLength={256}
              returnKeyType={"next"}
              onSubmitEditing={onSubmitEditing}
              blurOnSubmit={false}
            />
          )}
        </View>
      </View>
    );
  }
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
    marginLeft: Platform.OS == "android" ? 10 : 0
  }
});

const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});

const mapDispatchToProps = {
  getUserInfoAction
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(RegisterUserInfoScreen);

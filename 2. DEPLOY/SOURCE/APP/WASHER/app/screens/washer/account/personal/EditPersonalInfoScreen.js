import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  ScrollView,
  TouchableOpacity,
  Image,
  Alert
} from "react-native";
import ScreenComponent from "@app/components/ScreenComponent";
import TextInfo from "@app/components/TextInfo";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import TextInputInfo from "@app/components/TextInputInfo";
import ButtonPrimary from "@app/components/ButtonPrimary";
import Dropdown from "@app/components/ModalDropdown";
import * as API from "@app/constants/Api";
import reactotron from "reactotron-react-native";
import { updateUser } from "@app/redux/actions";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { ASYNC_STORAGE, REG_PHONE, REG_EMAIL } from "@app/constants/Constants";
import { showMessages } from "@app/components/Alert";

export class EditPersonalInfoScreen extends Component {
  initState = {
    data: this.props.navigation.state.params,
    addressNow: ""
  };
  state = this.initState;
  componentDidMount() {
    const { data } = this.state;
    this.getNearBy({
      latitude: data.lati,
      longitude: data.longi
    });
  }
  async getNearBy(loc) {
    var res = await API.nearBySearch(loc);
    var { vicinity, name, place_id } = res.data.results[0];
    var locationName = vicinity.length > name.length ? vicinity : name;
    this.setState(
      {
        addressNow: locationName
      },
      () => console.log(locationName)
      // this.checkArea(place_id, locationName)
    );
  }
  renderTextInfo = (label, value) => <TextInfo label={label} value={value} />;
  editBankAccount = () =>
    NavigationUtil.navigate(SCREEN_ROUTER_WASHER.EDIT_BANK_ACCOUNT);
  renderHeader = label => (
    <View
      style={styles.root_header}
      children={
        <Text style={styles.header_text} children={label.toUpperCase()} />
      }
    />
  );
  renderDropdown = (label, defVal, list) => (
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
          // isShowInBottom={true}
          imageStyle={styles.image_dropdown}
          defaultValue={defVal}
          onSelect={(index, value) => {
            this.setValue("sex", index);
            console.log(this.state.data.sex, index);
          }}
        />
      </View>
    </>
  );
  setValue = async (key, value) => {
    await this.setState({
      ...this.setState,
      data: {
        ...this.state.data,
        [key]: value.trim()
      }
    });
  };
  renderText = () => (
    <Text style={styles.text_label} children={R.strings().sex} />
  );
  updateUserInfo = () => {
    const { name, phone, email } = this.state.data;
    if (name == "" || phone == "" || email == "") {
      Alert.alert(
        R.strings().notice,
        R.strings().please_complete_all_infomation
      );
    } else if (phone && !REG_PHONE.test(phone)) {
      showMessages(R.strings().notice, R.strings().phone_invalid);
    } else if (email && !REG_EMAIL.test(email)) {
      showMessages(R.strings().notice, R.strings().email_invalid);
    } else {
      let res = this.props.updateUser(this.state.data);
      if (res) {
        Alert.alert(R.strings().notice, R.strings().update_successful);
        NavigationUtil.goBack();
      }
    }
  };
  render() {
    const { data, addressNow } = this.state;
    return (
      <ScreenComponent
        dialogLoading={this.props.UserInfoState.isLoading}
        isError={this.props.UserInfoState.error}
        back
        titleHeader={R.strings().edit_personal_information}
        renderView={
          <ScrollView>
            {this.renderHeader(R.strings().personal_information)}
            {/* <TextInputInfo
              label={R.strings().employee_code}
              value={data.code}
              editable={false}
            /> */}
            <TextInfo label={R.strings().employee_code} value={data.code} />
            <TextInputInfo
              label={R.strings().full_name}
              value={data.name}
              require
              onChangeText={text => this.setValue("name", text)}
            />
            {/* <TextInputInfo label={R.strings().birthday} value={"15/12/12"} /> */}
            {this.renderDropdown(
              R.strings().sex,
              data.sex == 0 ? R.strings().female : R.strings().male,
              [R.strings().female, R.strings().male]
            )}
            <TextInputInfo
              label={R.strings().phone_number}
              value={data.phone}
              editable={false}
              onChangeText={text => this.setValue("phone", text)}
            />
            <TextInputInfo
              label={R.strings().email}
              value={data.email}
              require
              onChangeText={text => this.setValue("email", text)}
            />
            {/* <TextInputInfo
              label={R.strings().present_location}
              value={addressNow}
              editable={false}
            /> */}
            {/* <TextInputInfo
              label={R.strings().registered_partition}
              value={"HH2 Bắc Hà, Tố Hữu, Hà Nội"}
              editable={false}

            /> */}
            <TextInfo label={R.strings().present_location} value={addressNow} />
            <TextInfo
              label={R.strings().registered_partition}
              value={data?.agentArea?.map((item, index) => `${item.name}, `)}
            />
            <ButtonPrimary
              text={R.strings().update}
              style={styles.button_update}
              onPress={this.updateUserInfo}
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
  updateUser
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(EditPersonalInfoScreen);

const styles = StyleSheet.create({
  image_dropdown: {
    top: 23,
    right: 20
  },
  button_update: { marginTop: 30, marginBottom: 30 },
  text_dropdown: {
    textAlignVertical: "center",
    padding: 10
  },
  dropDownStyle: {
    width: "90%",
    borderRadius: 5,
    marginTop: -20
  },
  root_dropdown: {
    marginHorizontal: 10,
    width: "95%",
    backgroundColor: colors.white,
    borderRadius: 5,
    borderWidth: 0.5
  },
  text_label: {
    marginHorizontal: 10,
    marginTop: 15,
    marginBottom: 5,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14
  },
  ic_location: {
    width: 40,
    height: 40
  },
  root_ic_location: {
    alignSelf: "center",
    marginEnd: 15
  },
  item_bank_account: {
    borderRadius: 5,
    borderWidth: 0.5,
    padding: 15,
    marginVertical: 2.5,
    marginHorizontal: 10,
    marginTop: 10
  },
  text_bank_name: {
    color: colors.primary,
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    marginBottom: 10
  },
  text_info_account: {
    color: colors.gray,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    marginVertical: 1
  },
  header_text: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    color: colors.primaryDark
  },
  header_edit_text: {
    // textAlign: "right",
    color: colors.primary,
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    position: "absolute",
    right: 10,
    paddingHorizontal: 10
  },
  root_header: {
    flexDirection: "row",
    marginTop: 20,
    marginHorizontal: 10
  },
  root_header_edit: { flex: 1 }
});

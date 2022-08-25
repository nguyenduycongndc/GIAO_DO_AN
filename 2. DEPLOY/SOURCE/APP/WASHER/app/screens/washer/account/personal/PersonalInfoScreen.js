import React, { Component } from "react";
import {
  View,
  Text,
  ScrollView,
  TouchableOpacity,
  StyleSheet,
  Image
} from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import TextInfo from "@app/components/TextInfo";
import { SCREEN_ROUTER_WASHER } from "@app/constants/Constants";
import R from "@app/assets/R";
import { colors } from "@app/constants/Theme";
import NavigationUtil from "@app/navigation/NavigationUtil";
import BankAccount from "@app/components/BankAccount";
import { setValue } from "@app/constants/Functions";
import * as API from "@app/constants/Api";
export class PersonalInfoScreen extends Component {
  initState = {
    addressNow: ""
  };
  state = this.initState;
  componentDidMount() {
    const { data } = this.props.UserInfoState;
    this.getNearBy({
      latitude: data.lati,
      longitude: data.longi
    });
  }
  renderBankAccount = listBank => listBank?.map(e => <BankAccount item={e} />);
  editBankAccount = () =>
    NavigationUtil.navigate(SCREEN_ROUTER_WASHER.EDIT_BANK_ACCOUNT);
  editPersonalInfo = () =>
    NavigationUtil.navigate(
      SCREEN_ROUTER_WASHER.EDIT_PERSONAL_INFO,
      this.props.UserInfoState.data
    );
  renderHeader = (label, action) => (
    <View
      style={styles.root_header}
      children={
        <>
          <Text style={styles.header_text} children={label.toUpperCase()} />
          <TouchableOpacity
            onPress={action}
            style={styles.root_header_edit}
            children={<Text style={styles.header_edit_text} children="Sửa" />}
          />
        </>
      }
    />
  );
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
  render() {
    const { data } = this.props.UserInfoState;
    console.log(this.state);
    
    return (
      <ScreenComponent
        back
        titleHeader={R.strings().personal_information}
        renderView={
          <ScrollView style={styles.root_view}>
            {this.renderHeader(
              R.strings().personal_information,
              this.editPersonalInfo
            )}
            <TextInfo label={R.strings().employee_code} value={data.code} />
            <TextInfo
              label={R.strings().full_name}
              value={setValue(data.name)}
            />
            {/* <TextInfo label={R.strings().birthday} value={"15/12/12"} /> */}
            <TextInfo
              label={R.strings().sex}
              value={data.sex == 1 ? R.strings().male : R.strings().female}
            />
            <TextInfo
              label={R.strings().phone_number}
              value={setValue(data.phone)}
            />
            <TextInfo label={R.strings().email} value={setValue(data.email)} />
            <TextInfo
              label={R.strings().present_location}
              value={setValue(this.state.address)}
              // endComponent={
              //   <Image
              //     source={R.images.ic_location}
              //     style={styles.ic_location}
              //     resizeMode="contain"
              //   />
              // }
            />
            <TextInfo
              label={R.strings().registered_partition}
              value={data?.agentArea?.map((item, index) => `${item.name}, `)}
            />
            {this.renderHeader(R.strings().bank_account, this.editBankAccount)}
            {this.renderBankAccount(data.listBank)}
          </ScrollView>
        }
      />
    );
  }
}

const mapStateToProps = state => ({
  UserInfoState: state.userReducer
});
const mapDispatchToProps = {};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(PersonalInfoScreen);
const styles = StyleSheet.create({
  root_view: { marginBottom: 30 },
  ic_location: {
    width: 40,
    height: 40,
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

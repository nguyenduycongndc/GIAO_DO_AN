import React, { Component } from "react";
import { View, Text, Image, Switch, StyleSheet } from "react-native";
import { connect } from "react-redux";
import ScreenComponent from "@app/components/ScreenComponent";
import R from "@app/assets/R";
import TextInputInfo from "@app/components/TextInputInfo";
import { colors } from "@app/constants/Theme";
import TextInfo from "@app/components/TextInfo";
import * as API from "@app/constants/Api";
import { updateUser } from "@app/redux/actions";
import { Avatar } from "react-native-elements";

export class StateChangeScreen extends Component {
  initState = {
    isSwitch: this.props.UserInfoState.data.acceptService == 1 ? true : false,
    addressNow: "",
    data: this.props.UserInfoState.data
  };
  state = this.initState;
  componentDidMount() {
    const { data } = this.props.UserInfoState;
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
  switch = async isSwitch => {
    await this.setState({
      isSwitch,
      data: {
        ...this.state.data,
        acceptService: isSwitch == true ? 1 : 0
      }
    });
    this.props.updateUser(this.state.data);
  };
  render() {
    const { isSwitch, addressNow } = this.state;
    const { data } = this.props.UserInfoState;
    console.log(this.state.data.acceptService);

    return (
      <ScreenComponent
        dialogLoading={this.props.UserInfoState.isLoading}
        isError={this.props.UserInfoState.error}
        titleHeader={R.strings().status_setting}
        back
        renderView={
          <>
            {/* <Image source={R.images.avatarDemo} style={styles.avatar} /> */}
            <Avatar
              rounded
              source={
                data.urlAvatar ? { uri: data.urlAvatar } : R.images.ic_symbol
              }
              overlayContainerStyle={{ backgroundColor: "white" }}
              style={styles.avatar}
            />
            <Text style={styles.text_under_avatar} children={data.code} />
            <TextInputInfo
              label={R.strings().my_location}
              value={addressNow}
              editable={false}
            />
            <View
              style={styles.root_row_set_state}
              children={
                <>
                  <Text
                    style={styles.text_set_state}
                    children={
                      !isSwitch
                        ? R.strings().turn_on_status
                        : R.strings().turn_off_status
                    }
                  />
                  <Switch
                    onValueChange={this.switch}
                    style={styles.switch}
                    value={isSwitch}
                  />
                </>
              }
            />
            {!isSwitch && (
              <Text
                children={R.strings().state_note}
                style={styles.text_note}
              />
            )}
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
  updateUser
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(StateChangeScreen);
const styles = StyleSheet.create({
  root_row_set_state: {
    flexDirection: "row",
    paddingHorizontal: 10,
    marginTop: 30
  },
  text_note: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 12,
    color: colors.red,
    padding: 10,
    marginTop: 10
  },
  switch: {
    position: "absolute",
    right: 10,
    alignSelf: "center"
  },
  text_set_state: {
    fontFamily: R.fonts.quicksand_medium,
    fontSize: 14,
    alignSelf: "center"
  },
  text_under_avatar: {
    fontFamily: R.fonts.quicksand_bold,
    fontSize: 14,
    textAlign: "center",
    marginTop: 10
  },
  avatar: {
    width: width / 4,
    height: width / 4,
    alignSelf: "center",
    marginTop: 50
  }
  // avatar: {
  //   width: width / 6,
  //   height: width / 6,
  //   borderRadius: width / 10,
  //   overflow: "hidden",
  //   borderWidth: 0.5,
  //   borderColor: colors.black
  // },
});

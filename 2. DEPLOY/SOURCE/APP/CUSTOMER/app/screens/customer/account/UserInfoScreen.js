import R from "@app/assets/R";
import Button from "@app/components/Button";
import ScreenComponent from "@app/components/ScreenComponent";
import theme from "@app/constants/Theme";
import React, { Component } from "react";
import {
  StyleSheet,
  Text,
  TextInput,
  View,
  ScrollView,
  Image,
  TouchableOpacity,
  RefreshControl
} from "react-native";
import { SCREEN_ROUTER_CUSTOMER, ASYNC_STORAGE } from "@constant";
// import ScreenComponent from "@app/components/ScreenComponent";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { getUserInfoAction } from "@action";
import { connect } from "react-redux";
import FastImage from "@app/components/FastImage";

class UserInfoScreen extends Component {
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    this.getData();
  }

  getData = () => {
    this.props.getUserInfoAction();
  };

  render() {
    const { UserInfoState } = this.props;
    return (
      <ScreenComponent
        back
        isLoading={UserInfoState.isLoading}
        isError={UserInfoState.error}
        titleHeader={R.strings().user_info}
        rightComponent={
          <>
            <TouchableOpacity
              onPress={() =>
                NavigationUtil.navigate(
                  SCREEN_ROUTER_CUSTOMER.UPDATE_USER_INFO,
                  { item: UserInfoState.data }
                )
              }
            >
              <FastImage
                source={R.images.ic_edit}
                style={{ width: 27, height: 27 }}
              />
            </TouchableOpacity>
          </>
        }
        renderView={
          <>
            <ScrollView
              refreshControl={
                <RefreshControl
                  onRefresh={this.getData}
                  refreshing={UserInfoState.isLoading}
                />
              }
              style={{
                backgroundColor: theme.colors.white,
                flex: 1,
                paddingHorizontal: 8,
                paddingTop: 15
              }}
            >
              {this._renderTextInput(
                R.strings().full_name,
                UserInfoState.data.name
              )}
              {this._renderTextInput(
                R.strings().date_of_birth,
                UserInfoState.data.dobStr
              )}
              {this._renderTextInput(
                R.strings().gender,
                UserInfoState.data.sex == 1
                  ? R.strings().man
                  : R.strings().woman
              )}
              {this._renderTextInput(
                R.strings().referral_code,
                UserInfoState.data.referralCode
              )}
              {this._renderTextInput(
                R.strings().number_phone,
                UserInfoState.data.phone
              )}
              {this._renderTextInput("Email", UserInfoState.data.email)}
              {this._renderTextInput(
                R.strings().address,
                UserInfoState.data.address
              )}
            </ScrollView>
          </>
        }
      />
    );
  }
  _renderTextInput = (label, value) => {
    return (
      <View>
        <Text
          style={{
            fontFamily: R.fonts.quicksand_medium,
            fontSize: 14,
            paddingVertical: 10
          }}
        >
          {label}
        </Text>
        <View
          style={{
            height: 46,
            backgroundColor: theme.colors.backgroundInput,
            borderRadius: 5,
            paddingHorizontal: 10,
            marginBottom: 15,
            justifyContent: "center"
          }}
        >
          <Text children={value || R.strings().not_update_yet} />
        </View>
      </View>
    );
  };
}

const styles = StyleSheet.create({});

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
)(UserInfoScreen);

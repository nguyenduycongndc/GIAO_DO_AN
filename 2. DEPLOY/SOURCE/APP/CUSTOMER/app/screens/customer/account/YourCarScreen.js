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
  Dimensions,
  TouchableOpacity,
  ImageBackground
} from "react-native";
// import Dropdown from "@app/components/Dropdown";
import DatePicker from "react-native-datepicker";
// import DropdownMenu from 'react-native-dropdown-menu';
import Icon from "@component/Icon";
import { Dropdown } from "react-native-material-dropdown";
import { FlatList } from "react-native-gesture-handler";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { TYPE_INFO, SCREEN_ROUTER_CUSTOMER } from "@constant";
import { getUserInfoAction } from "@action/";
import { connect } from "react-redux";
import { createImageProgress } from "react-native-image-progress";
import FastImage from "@app/components/FastImage";
import * as Progress from "react-native-progress";
import Empty from "@app/components/Empty";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import { showMessages, showConfirm } from "@app/components/Alert";
import FastImg from "@app/components/FastImage";

const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

const Image = createImageProgress(FastImage);

const ProgressImageBackground = createImageProgress(ImageBackground);
const MAX_SIZE_CAR = 5;

export class YourCarScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const listCar = navigation.getParam("listCar", {});
    this.state = {
      token: "",
      isLoading: false,
      error: "",
      text: "",
      listCar: listCar,
      isDialogLoading: false
    };
  }

  deleteCar = carID => {
    CallApiHelper(
      API.DeleteCarCustomer,
      carID,
      this,
      res => {
        this.props.getUserInfoAction();
      },
      null,
      null,
      "isDialogLoading"
    );
  };

  render() {
    const { UserInfoState } = this.props;
    return (
      <ScreenComponent
        back
        titleHeader={R.strings().your_car}
        isLoading={UserInfoState.isLoading}
        dialogLoading={this.state.isDialogLoading}
        // isError={UserInfoState.error}
        rightComponent={
          <TouchableOpacity
            onPress={() => {
              if (UserInfoState.data.listCar.length == MAX_SIZE_CAR) {
                showMessages(
                  R.strings().notif_tab_cus,
                  `${R.strings().limit_car} ${MAX_SIZE_CAR}`
                );
                return;
              }
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.ADD_CAR);
            }}
          >
            <Icon.AntDesign name="plus" size={25} color={theme.colors.white} />
          </TouchableOpacity>
        }
        renderView={
          UserInfoState.data.listCar.length == 0 ? (
            <Empty description={R.strings().no_car_now} />
          ) : (
            <View
              style={{ flex: 1, backgroundColor: theme.colors.backgroundColor }}
            >
              <Text
                style={{
                  fontFamily: R.fonts.quicksand_light,
                  fontSize: 12,
                  color: theme.colors.black,
                  textAlign: "center"
                }}
                children={R.strings().hold_to_delete_car}
              />
              <FlatList
                contentContainerStyle={{
                  paddingVertical: 5,
                  flexGrow: 1,
                  backgroundColor: theme.colors.backgroundColor
                }}
                data={UserInfoState.data.listCar}
                keyExtractor={(item, index) => index.toString()}
                // renderItem={({ item, index }) => this._renderItem(item, index)}
                renderItem={this._renderCarItem}
              />
            </View>
          )
        }
      />
    );
  }
  _renderCarItem = ({ item, index }) => {
    return (
      <TouchableOpacity
        style={styles.imgItem}
        onPress={() =>
          NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.CAR_DETAIL, {
            carInfo: item
          })
        }
        onLongPress={() =>
          showConfirm(
            R.strings().notif_tab_cus,
            R.strings().confirm_delete_car,
            () => this.deleteCar(item.carID)
          )
        }
      >
        <FastImg
          style={{
            width: windowWidth,
            height: 249,
            overflow: "hidden",
            justifyContent: "flex-end"
          }}
          source={
            item.listImage[0]
              ? { uri: item.listImage[0].url }
              : R.images.empty_car_img
          }
        >
          <View
            style={{
              flexDirection: "row",
              alignItems: "center",
              justifyContent: "center",
              backgroundColor: "rgba(192,192,192,0.9)",
              paddingVertical: 12
            }}
          >
            <Text
              style={{
                fontFamily: R.fonts.quicksand_bold,
                fontSize: 16,
                color: theme.colors.darkBlue,
                marginRight: 10
              }}
            >
              {item.carBrand} {item.carModel} - {item.licensePlates}
            </Text>
            <TouchableOpacity
              onPress={() =>
                NavigationUtil.navigate(
                  SCREEN_ROUTER_CUSTOMER.UPDATE_CAR_INFO,
                  {
                    carInfo: item
                  }
                )
              }
            >
              <FastImage
                source={R.images.ic_edit_blue}
                style={{ height: 19.12, width: 19.62 }}
              />
            </TouchableOpacity>
          </View>
        </FastImg>
      </TouchableOpacity>
    );
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
  imgItem: {
    width: "100%",
    backgroundColor: theme.colors.white,
    alignItems: "center",
    marginBottom: 10,
    borderWidth: 0.25,
    borderColor: theme.colors.gray
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
    width: "100%"
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
)(YourCarScreen);

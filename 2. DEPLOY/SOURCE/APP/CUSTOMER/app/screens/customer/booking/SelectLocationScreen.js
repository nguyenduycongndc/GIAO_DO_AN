import React, { Component } from "react";
import {
  View,
  Text,
  TouchableOpacity,
  TextInput,
  StyleSheet,
  Keyboard,
  FlatList,
  Platform,
  PermissionsAndroid
} from "react-native";
import { connect } from "react-redux";
import R from "@app/assets/R";
import ScreenComponent from "@app/components/ScreenComponent";
import theme, { colors } from "@theme";
import Icon from "@component/Icon";
import {
  getDetailPlace,
  nearBySearch,
  searchPlaceAutoComplete
} from "../../../constants/Api";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { sendLocationSelect, setState } from "@action/";
import CallApiHelper from "../../../utils/CallApiHelper";
import * as API from "@api";
import { showMessages } from "@app/components/Alert";
import callAPI from "../../../utils/CallApiHelper";
import reactotron from "reactotron-react-native";
import Loading from "@app/components/Loading";
import { SCREEN_ROUTER_CUSTOMER, REDUCER_CUSTOM } from "@constant";
var timeOut;

export class SelectLocationScreen extends Component {
  constructor(props) {
    super(props);
    this.state = {
      resSearchPlace: [],
      searchKey: "",
      locationSelect: {},
      marker: {
        latitude: 21.002383,
        longitude: 105.795718
      },
      markerName: "",
      isSelectLocationByMarker: false,
      showMap: true,
      marginBottom: 1,
      isLoadingNearBy: false,
      placeID: ""
    };
  }

  onChangeText = text => {
    if (timeOut) clearTimeout(timeOut);
    timeOut = setTimeout(() => this.search(), 600);
    this.setState({
      searchKey: text
    });
  };

  requestPerAndroid = async () => {
    const isGrand = await PermissionsAndroid.requestPermission(
      "android.permission.ACCESS_FINE_LOCATION"
    );
    if (!isGrand) this.requestPerAndroid();
  };

  componentWillMount() {
    // Keyboard.dismiss;
    if (Platform.OS == "android") this.requestPerAndroid();
  }

  componentDidMount() {
    // this.textSearch.focus();
    // Keyboard.dismiss;
  }

  search = async () => {
    if (this.state.searchKey.trim() != "") {
      callAPI(
        searchPlaceAutoComplete,
        this.state.searchKey.trim(),
        this,
        res => {
          this.setState({
            resSearchPlace: res.data.predictions
          });
        },
        null,
        null,
        "isDialogLoading"
      );
    } else this.setState({ resSearchPlace: [] });
  };

  checkArea = (place_id, description, location) => {
    CallApiHelper(API.checkArea, place_id, this, res => {
      this.props.sendLocationSelect(
        place_id,
        description,
        location.lng,
        location.lat
      );
      NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.BOOKING);
      if (this.props.orderHistory?.reOrder) {
        this.props.navigation.state.params();
      }
    });
  };
  onPressLocationSearch = async (place_id, description) => {
    callAPI(API.getDetailPlace, place_id, this, res => {
      const { location } = res.data.result.geometry;
      this.checkArea(place_id, description, location);
    });
  };

  renderItem = (place_id, description, address) => (
    <View style={styles.root_item}>
      <TouchableOpacity
        style={styles.root_touch}
        onPress={() =>
          this.onPressLocationSearch(place_id, address || description)
        }
      >
        <Icon.Entypo
          name={"location-pin"}
          size={20}
          color={theme.colors.borderColor}
        />
        <Text style={styles.text_des} numberOfLines={2}>
          {description || R.strings().not_update_yet}
        </Text>
      </TouchableOpacity>
      <View style={styles.line} />
    </View>
  );

  removeReOrder = () => {
    NavigationUtil.goBack();
    this.props.setState(SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER, {
      reOrder: false
    });
  };

  render() {
    const {
      isDialogLoading,
      marker,
      searchKey,
      resSearchPlace,
      isLoading
    } = this.state;
    const { data } = this.props.userReducer;
    return (
      <ScreenComponent
        dialogLoading={isLoading}
        back
        onBack={this.removeReOrder}
        titleHeader={R.strings().select_location}
        renderView={
          <>
            <View
              style={{
                flex: 1,
                backgroundColor: theme.colors.backgroundColor,
                alignItems: "center"
              }}
            >
              <View
                style={{
                  flexDirection: "row",
                  alignItems: "center",
                  width: "100%",
                  padding: 10
                }}
              >
                <View style={styles.textInput}>
                  <Icon.FontAwesome
                    name={"search"}
                    size={20}
                    color={theme.colors.borderColor}
                  />
                  <TextInput
                    ref={ref => (this.textSearch = ref)}
                    placeholder={R.strings().enter_your_address}
                    style={{ marginLeft: 10, width: "80%", color: "black" }}
                    value={searchKey}
                    autoFocus={true}
                    maxLength={256}
                    placeholderTextColor={colors.gray}
                    onChangeText={this.onChangeText}
                  />
                </View>
                <TouchableOpacity onPress={this.removeReOrder}>
                  <Text
                    style={{
                      fontFamily: R.fonts.quicksand_bold,
                      fontSize: 14,
                      marginLeft: 15
                    }}
                  >
                    {R.strings().cancel}
                  </Text>
                </TouchableOpacity>
              </View>
              {isDialogLoading && <Loading />}
              <FlatList
                ListHeaderComponent={
                  <>
                    {resSearchPlace.length == 0 &&
                      data?.listLocation.map(e =>
                        this.renderItem(
                          e.placeID,
                          e.name + " : " + e.address,
                          e.address
                        )
                      )}
                  </>
                }
                keyboardShouldPersistTaps={true}
                style={[styles.textItemResSearch]}
                data={resSearchPlace}
                renderItem={({ item }) =>
                  this.renderItem(item.place_id, item.description)
                }
              />
            </View>
          </>
        }
      />
    );
  }
}

const styles = StyleSheet.create({
  line: {
    height: 0.5,
    backgroundColor: theme.colors.grayBorder,
    marginHorizontal: 15
  },
  text_des: {
    width: "95%",
    fontSize: 14,
    fontFamily: R.fonts.quicksand_medium,
    paddingHorizontal: 5
  },
  textInput: {
    width: "85%",
    flexDirection: "row",
    alignItems: "center",
    padding: 13,
    paddingVertical: Platform.OS == "ios" ? 13 : 0,
    backgroundColor: theme.colors.white,
    borderRadius: 50,
    borderWidth: 0.5,
    borderColor: theme.colors.borderColor
  },
  textItemResSearch: {
    textAlignVertical: "center",
    fontFamily: "Roboto-regular",
    fontSize: 14,
    width: "100%",
    paddingVertical: Platform.OS == "android" ? 6 : 8
  },
  root_item: {
    marginTop: 5,
    width: "100%",
    paddingVertical: 5,
    paddingBottom: 5,
    backgroundColor: theme.colors.white
  },
  root_touch: {
    marginBottom: 3,
    paddingHorizontal: 10,
    paddingVertical: 4,
    flexDirection: "row",
    alignItems: "center",
    paddingRight: 5
  }
});

const mapStateToProps = state => ({
  userReducer: state.userReducer,
  orderHistory:
    state[REDUCER_CUSTOM.STATE][SCREEN_ROUTER_CUSTOMER.HISTORY_ORDER]
});

const mapDispatchToProps = {
  sendLocationSelect,
  setState
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(SelectLocationScreen);

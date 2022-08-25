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
  TouchableOpacity
} from "react-native";
import { SliderBox } from "react-native-image-slider-box";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { TYPE_INFO, SCREEN_ROUTER_CUSTOMER } from "@constant";
import { getCarDetail } from "@action/";
import { connect } from "react-redux";
import FastImg from "@app/components/FastImage";
const windowWidth = Dimensions.get("window").width;
const windowHeight = Dimensions.get("window").height;

export class CarDetailScreen extends Component {
  constructor(props) {
    super(props);
    const { navigation } = this.props;
    const carInfo = navigation.getParam("carInfo", {});
    this.state = {
      carInfo: carInfo,
      images: []
    };
  }

  componentDidMount() {
    this.getData();
  }

  async getData() {
    await this.props.getCarDetail(this.state.carInfo.carID);
    this.getListImage();
  }

  getListImage() {
    var listImage = [];
    for (let i = 0; i < this.props.CarDetailState.listImage.length; i++) {
      listImage.push(this.props.CarDetailState.listImage[i].url);
    }
    this.setState({
      images: listImage
    });
  }

  render() {
    const { carInfo, images } = this.state;
    const { CarDetailState } = this.props;
    return (
      <ScreenComponent
        back
        isLoading={CarDetailState.isLoading}
        isError={CarDetailState.error}
        reload={() => this.props.getCarDetail(this.state.carInfo.carID)}
        titleHeader={R.strings().car_info}
        rightComponent={
          <TouchableOpacity
            onPress={() =>
              NavigationUtil.navigate(SCREEN_ROUTER_CUSTOMER.UPDATE_CAR_INFO, {
                carInfo: CarDetailState.data
              })
            }
          >
            <FastImg
              source={R.images.ic_edit}
              style={{ width: 27, height: 27 }}
            />
          </TouchableOpacity>
        }
        renderView={
          <>
            <ScrollView
              showsVerticalScrollIndicator={false}
              contentContainerStyle={{
                flexGrow: 1,
                backgroundColor: theme.colors.backgroundColor,
                paddingVertical: 7,
                paddingBottom: 50
              }}
            >
              <Text
                style={{
                  fontSize: 14,
                  fontFamily: R.fonts.quicksand_bold,
                  color: theme.colors.darkBlue,
                  paddingHorizontal: 9,
                  paddingVertical: 10
                }}
              >
                {R.strings().car_photo}
              </Text>
              {CarDetailState.listImage.length == 0 ? (
                <View style={styles.imgItem}>
                  <FastImg
                    style={{ width: windowWidth, height: 197 }}
                    source={R.images.empty_car_img_no_title}
                  />
                </View>
              ) : (
                <SliderBox
                  images={CarDetailState.listImage}
                  sliderBoxHeight={200}
                  ImageComponentStyle={{
                    width: "100%"
                  }}
                  onCurrentImagePressed={index =>
                    console.warn(`image ${index} pressed`)
                  }
                  dotColor={theme.colors.primary}
                  paginationBoxVerticalPadding={20}
                  sliderBoxHeight={200}
                  autoplay
                  circleLoop
                />
              )}
              <Text
                style={{
                  fontSize: 14,
                  fontFamily: R.fonts.quicksand_bold,
                  color: theme.colors.darkBlue,
                  paddingHorizontal: 9,
                  paddingVertical: 10,
                  marginTop: 15
                }}
              >
                {R.strings().car_info}
              </Text>
              {this._renderCarItem(
                R.strings().license_plate,
                CarDetailState.data.licensePlates
              )}
              {this._renderCarItem(
                R.strings().brand,
                CarDetailState.data.carBrand
              )}
              {this._renderCarItem(
                R.strings().model,
                CarDetailState.data.carModel
              )}
              {this._renderCarItem(
                R.strings().color_car,
                CarDetailState.data.carColor
              )}
              {/* {this._renderCarItem(
                R.strings().year_of_manufacture,
                CarDetailState.data.manufacturingDate
              )} */}
              {this._renderCarItem(
                R.strings().registration_date,
                CarDetailState.data.registrationDateStr
              )}
              {/* {this._renderCarItem(
                R.strings().status,
                CarDetailState.data.status
              )} */}
              {this._renderCarItem(
                R.strings().registration,
                CarDetailState.data.vehicleRegistration
              )}
            </ScrollView>
          </>
        }
      />
    );
  }

  _renderCarItem(label, content) {
    return (
      <View style={{ backgroundColor: theme.colors.white, width: "100%" }}>
        <View
          style={{
            paddingHorizontal: 9,
            flexDirection: "row",
            justifyContent: "space-between",
            alignItems: "center",
            paddingVertical: 15
          }}
        >
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              color: theme.colors.darkBlue,
              width: "25%"
            }}
          >
            {label}
          </Text>
          <Text
            style={{
              fontFamily: R.fonts.quicksand_medium,
              fontSize: 14,
              color: theme.colors.darkBlue,
              width: "70%",
              textAlign: "right"
            }}
          >
            {content || R.strings().not_update_yet}
          </Text>
        </View>
        {label != R.strings().registration && (
          <View
            style={{
              paddingHorizontal: 5,
              height: 1,
              backgroundColor: theme.colors.grayBorder,
              marginHorizontal: 5
            }}
          />
        )}
      </View>
    );
  }
}

const styles = StyleSheet.create({
  imgItem: {
    width: "100%",
    backgroundColor: theme.colors.white,
    alignItems: "center",
    marginBottom: 10,
    borderWidth: 0.25,
    borderColor: theme.colors.gray
  }
});

const mapStateToProps = state => ({
  CarDetailState: state.carDetailReducer
});

const mapDispatchToProps = {
  getCarDetail
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(CarDetailScreen);

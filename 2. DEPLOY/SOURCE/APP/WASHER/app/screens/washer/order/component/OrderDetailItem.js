import React, { Component } from "react";
import {
  Text,
  StyleSheet,
  View,
  Image,
  Linking,
  TouchableOpacity
} from "react-native";
import R from "@app/assets/R";
import theme from "@app/constants/Theme";
import reactotron from "reactotron-react-native";
import RateStar from "@app/components/RateStar";
import NumberFormat from "react-number-format";
import {
  PAYMENT_METHOD,
  ORDER_STATUS,
  SCREEN_ROUTER_WASHER
} from "@app/constants/Constants";
import Button from "@app/components/Button";
import { connect } from "react-redux";
import NavigationUtil from "@app/navigation/NavigationUtil";
import { distance } from "@app/utils/Location";
import { renStatus } from "@app/constants/Functions";
import { Avatar } from "react-native-elements";
import FastImage from "react-native-fast-image";
import LoadableImage from "@app/components/LoadableImage";

class InfoCustomer extends Component {
  render() {
    const { item, locationState, isShowStatus = true } = this.props;
    const distanceLocation = distance(
      item.lati,
      item.longi,
      locationState.lati,
      locationState.longi
    );
    return (
      <View style={styles.container}>
        <View style={{ flexDirection: "row" }}>
          <Text style={styles.txtGray}>{R.strings().invoice}</Text>
          <Text
            style={[styles.txtBold, { color: "black", marginLeft: 4, flex: 1 }]}
          >
            {item.code}
          </Text>
          {isShowStatus && (
            <Text
              style={{
                fontFamily: R.fonts.quicksand_semi_bold,
                fontSize: 14,
                color:
                  item.status == ORDER_STATUS.REJECTED
                    ? theme.colors.red
                    : theme.colors.primary
              }}
            >
              {renStatus(item.status)}
            </Text>
          )}
        </View>
        <View style={styles.lines} />

        <View
          style={{ flexDirection: "row", marginTop: 10, alignItems: "center" }}
        >
          <LoadableImage
            style={styles.imgAvatar}
            source={
              item.customerAvatar
                ? { uri: item.customerAvatar }
                : R.images.ic_symbol
            }
            overlayContainerStyle={{ backgroundColor: "white" }}
          />
          <View style={{ marginHorizontal: 15 }}>
            <Text style={styles.txtGray}>{item.customerName}</Text>
            <Text style={styles.txtGray}>{item.customerPhone}</Text>
            <Text style={[styles.txtGray, { marginRight: width * 0.1 + 2 }]}>
              {item.customerAddress}
            </Text>
          </View>
        </View>
        <View style={styles.lines} />
        <View
          style={{ flexDirection: "row", marginTop: 10, alignItems: "center" }}
        >
          <LoadableImage style={styles.icTime} source={R.images.ic_clock2} />
          <Text style={[styles.txtGray, { marginLeft: 10, marginBottom: 2 }]}>
            {item.bookDateStr || item.bookingDateStr}
          </Text>
        </View>

        <View
          style={{
            flexDirection: "row",
            marginTop: 10,
            alignItems: "center",
            justifyContent: "space-between"
          }}
        >
          <View style={{ flexDirection: "row" }}>
            <LoadableImage
              style={[styles.icTime, { tintColor: "black" }]}
              source={R.images.ic_range}
            />
            <Text style={[styles.txtGray, { marginLeft: 10, marginBottom: 2 }]}>
              {!!Object.keys(locationState).length &&
              locationState.longi &&
              locationState.lati &&
              item.lati &&
              item.longi
                ? distanceLocation < 100
                  ? distanceLocation
                  : "> 100"
                : item.distance}{" "}
              km
            </Text>
          </View>

          <TouchableOpacity
            onPress={() =>
              NavigationUtil.navigate(SCREEN_ROUTER_WASHER.MAP, { item: item })
            }
          >
            <Text style={styles.view_location}>
              {R.strings().view_location}
            </Text>
          </TouchableOpacity>
        </View>
        {!!item.reasonNote && (
          <Text
            style={{
              fontSize: 14,
              fontFamily: R.fonts.quicksand_light,
              color: theme.colors.primaryDark,
              marginLeft: 26,
              fontStyle: "italic"
            }}
          >
            {item.reasonNote}
          </Text>
        )}
        {!!item.note && (
          <Text
            style={{
              fontSize: 14,
              fontFamily: R.fonts.quicksand_light,
              color: theme.colors.primaryDark,
              marginLeft: 26,
              fontStyle: "italic"
            }}
          >
            {item.note}
          </Text>
        )}
        {item.status == ORDER_STATUS.REJECTED && !!item.reasonCancel && (
          <Text
            style={{
              fontSize: 14,
              fontFamily: R.fonts.quicksand_light,
              color: theme.colors.red,
              marginLeft: 26,
              fontStyle: "bold"
            }}
          >
            {item.reasonCancel}
          </Text>
        )}
      </View>
    );
  }
}

class InfoContact extends Component {
  render() {
    const { item } = this.props;
    return (
      <View
        style={{
          flexDirection: "row",
          paddingHorizontal: 10
        }}
      >
        <Button
          action={() => Linking.openURL(`tel:${item.customerPhone}`)}
          title={R.strings().call}
          backgroundColor={theme.colors.white}
          borderColor={theme.colors.grayBorder}
          colorText={theme.colors.gray}
          buttonStyle={{ flex: 1, marginRight: 10 }}
          icon={
            <LoadableImage
              source={R.images.ic_call}
              style={{ width: 26, height: 26 }}
            />
          }
        />
        <Button
          title={R.strings().chat_via_zalo}
          action={() =>
            Linking.openURL(`https://zalo.me/${item.customerPhone}`)
          }
          backgroundColor={theme.colors.backgroundZalo}
          borderColor={theme.colors.grayBorder}
          colorText={theme.colors.white}
          buttonStyle={{ flex: 1, marginLeft: 10 }}
          icon={
            <LoadableImage
              source={R.images.ic_zalo}
              style={{ width: 33, height: 33 }}
            />
          }
        />
      </View>
    );
  }
}

class InfoWasher extends Component {
  render() {
    const { item } = this.props;
    return (
      <View>
        <Text style={[styles.txtBold, styles.txtTitle]}>
          {R.strings().washer_info}
        </Text>

        <View style={[styles.container, { backgroundColor: "white" }]}>
          <View
            style={{
              flexDirection: "row",
              alignItems: "center",
              justifyContent: "space-between"
            }}
          >
            <View style={{ flexDirection: "row", alignItems: "center" }}>
              <LoadableImage
                style={styles.imgAvatar}
                source={
                  item.agentAvatar
                    ? { uri: item.agentAvatar }
                    : R.images.ic_symbol
                }
                overlayContainerStyle={{ backgroundColor: "white" }}
              />
              <View style={{ marginLeft: 15 }}>
                <Text style={styles.txtGray}>{item.agentName}</Text>
                <Text style={styles.txtGray}>{item.agentCode}</Text>
              </View>
            </View>

            {item.status == ORDER_STATUS.COMPLETED && (
              <>
                {item.agentRating ? (
                  <View style={{ alignItems: "center" }}>
                    <RateStar
                      readonly={true}
                      numberStar={item.agentRating}
                      isShowNumber
                      color={theme.colors.gray}
                      size={16}
                    />
                    {!!item.noteRate && (
                      <Text
                        style={[
                          styles.txtGray,
                          { fontSize: 11, marginRight: 10 }
                        ]}
                        children={item.noteRate}
                      />
                    )}
                  </View>
                ) : (
                  <Text style={[styles.txtGray, { marginRight: 10 }]}>
                    {R.strings().no_review}
                  </Text>
                )}
              </>
            )}
          </View>
        </View>
      </View>
    );
  }
}

class InfoCar extends Component {
  render() {
    const { item } = this.props;
    const sourceCarImg = !!item.car.listImage.length
      ? { uri: item.car.listImage[0].url }
      : R.images.carImg;
    return (
      <View style={[styles.container, { marginTop: 10 }]}>
        <View style={{ flexDirection: "row" }}>
          <LoadableImage style={styles.imgCar} source={sourceCarImg} />
          <View style={{ marginLeft: 15 }}>
            <Text style={styles.txtCarBrand}>{item.car.carBrand}</Text>
            <Text style={styles.txtLicensePlates}>
              {item.car.licensePlates}
            </Text>
          </View>
        </View>
      </View>
    );
  }
}

class InfoService extends Component {
  listServiceCombo = [
    {
      serviceName: "Vàng",
      servicePrice: 150000,
      serviceType: 1
    },
    {
      serviceName: "Đánh bóng Hybrid",
      servicePrice: 200000,
      serviceType: 2
    },
    {
      serviceName: "Bạc",
      servicePrice: 90000,
      serviceType: 1
    },
    {
      serviceName: "Bạc",
      servicePrice: 90000,
      serviceType: 1
    },
    {
      serviceName: "Bạc",
      servicePrice: 90000,
      serviceType: 1
    }
  ];
  renderRow(label, money, nameService, end) {
    return (
      <View>
        <View
          style={{
            flexDirection: "row",
            justifyContent: "space-between"
          }}
        >
          <Text
            style={[
              styles.txtGray,
              { color: theme.colors.darkBlue, marginTop: !nameService ? 0 : 4 }
            ]}
          >
            {label}
          </Text>
          <View>
            <NumberFormat
              value={money}
              displayType="text"
              thousandSeparator
              suffix="đ"
              renderText={value => (
                <Text
                  style={[styles.txtGray, { color: theme.colors.darkBlue }]}
                  children={value}
                />
              )}
            />
            {nameService && (
              <Text style={[styles.txtMainService]}>{`(${nameService})`}</Text>
            )}
          </View>
        </View>
        {!end && <View style={[styles.lines, { marginBottom: 6 }]} />}
      </View>
    );
  }

  getService = id => {
    const { item } = this.props;
    return item.listService.find(item => item.serviceID == id);
  };

  render() {
    const { item, combo } = this.props;
    const listService = [...item.listService];
    if (!combo) listService.splice(0, 1);
    return (
      <>
        {!combo ? (
          <View
            style={[styles.container, { marginTop: 10, paddingVertical: 8 }]}
          >
            {this.renderRow(
              R.strings().package,
              this.getService(item.mainService)?.price,
              this.getService(item.mainService)?.name,
              listService.length == 0
            )}
            {listService.map((item, index) =>
              this.renderRow(
                item.name,
                item.price,
                null,
                index == listService.length - 1
              )
            )}
          </View>
        ) : (
          <>
            <Text
              style={[styles.txtBold, styles.txtTitle, { flex: 1 }]}
              children="Gói tháng"
            />

            <View style={[styles.container, { paddingVertical: 8 }]}>
              {listService.map((item, index) =>
                this.renderRow(
                  item.serviceName,
                  item.servicePrice,
                  null,
                  index == this.listServiceCombo.length - 1
                )
              )}
            </View>
          </>
        )}
      </>
    );
  }
}

class InfoPayment extends Component {
  renderRow(label, value, end, styleValue) {
    return (
      <View>
        <View style={{ flexDirection: "row", justifyContent: "space-between" }}>
          <Text style={[styles.txtGray, { color: theme.colors.darkBlue }]}>
            {label}
          </Text>
          <NumberFormat
            value={value}
            displayType="text"
            thousandSeparator
            suffix="đ"
            renderText={value => (
              <Text
                style={[
                  styles.txtGray,
                  { color: theme.colors.darkBlue },
                  styleValue
                ]}
                children={value}
              />
            )}
          />
        </View>
        {!end && <View style={[styles.lines, { marginBottom: 6 }]} />}
      </View>
    );
  }
  render() {
    const { item } = this.props;
    return (
      <View style={{ marginTop: 4 }}>
        <View style={{ flexDirection: "row", alignItems: "center" }}>
          <Text style={[styles.txtBold, styles.txtTitle, { flex: 1 }]}>
            {R.strings().payment_info}
          </Text>
          <Text style={styles.typePayment}>
            {item.paymentType == PAYMENT_METHOD.CASH
              ? R.strings().cash
              : "VNPAY"}
          </Text>
        </View>
        <View style={styles.container}>
          {this.renderRow(R.strings().promotion, item.couponPoint, false, {
            color: theme.colors.greenLight
          })}
          {this.renderRow(R.strings().use_point, item.usePoint, false, {
            color: theme.colors.greenLight
          })}
          {this.renderRow(R.strings().commission, item.commission, false, {
            color: theme.colors.greenLight
          })}
          {this.renderRow(
            R.strings().amount_of_service,
            item.totalPrice,
            true,
            {
              color: theme.colors.primary
            }
          )}
          {/* {this.renderRow(R.strings().total_price, item.totalPrice, true, {
            color: theme.colors.primary
          })} */}
        </View>
      </View>
    );
  }
}

const styles = StyleSheet.create({
  container: {
    width: "100%",
    backgroundColor: "white",
    paddingHorizontal: 9,
    paddingVertical: 12
  },
  txtGray: {
    fontSize: 14,
    fontFamily: R.fonts.quicksand_semi_bold,
    color: theme.colors.nameText
  },
  txtBold: {
    fontSize: 14,
    fontFamily: R.fonts.quicksand_bold
  },
  lines: {
    height: 0.5,
    width: width,
    backgroundColor: theme.colors.grayBorder,
    marginTop: 6
  },
  imgAvatar: {
    width: width / 6.5,
    height: width / 6.5,
    borderRadius: width / 10,
    overflow: "hidden",
    borderWidth: 0.5,
    borderColor: theme.colors.black
  },
  imgCar: {
    width: width / 6.5,
    height: width / 6.5,
    borderRadius: 7,
    marginLeft: 1
  },
  icTime: {
    width: 18,
    height: 18,
    resizeMode: "contain",
    tintColor: theme.colors.grayPlus
  },
  txtBold: {
    fontSize: 14,
    fontFamily: R.fonts.quicksand_bold
  },
  txtCarBrand: {
    fontSize: 20,
    fontFamily: R.fonts.quicksand_bold,
    color: theme.colors.primary
  },
  txtLicensePlates: {
    fontSize: 20,
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.primary
  },
  txtMainService: {
    fontSize: 11,
    fontFamily: R.fonts.quicksand_medium,
    color: theme.colors.green,
    alignSelf: "center"
  },
  txtTitle: {
    color: theme.colors.darkBlue,
    marginLeft: 9,
    marginTop: 10,
    marginBottom: 8
  },
  typePayment: {
    borderWidth: 1,
    borderRadius: 5,
    borderColor: "#FF9A11",
    paddingHorizontal: 14,
    paddingVertical: 4,
    marginRight: 8,
    color: "#FF9A11"
  },
  view_location: {
    fontSize: 14,
    fontFamily: R.fonts.quicksand_semi_bold,
    color: theme.colors.green,
    textDecorationLine: "underline"
  }
});
const mapStateToProps = state => ({
  locationState: state.locationReducer
});

const mapDispatchToProps = {};
module.exports = {
  InfoCustomer: connect(
    mapStateToProps,
    mapDispatchToProps
  )(InfoCustomer),
  InfoContact: InfoContact,
  InfoWasher: InfoWasher,
  InfoCar: InfoCar,
  InfoService: InfoService,
  InfoPayment: InfoPayment
};
